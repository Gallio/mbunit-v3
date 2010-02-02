// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.UI.DataBinding;
using SortOrder=Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.Controllers
{
    public class TestResultsController : ITestResultsController, Handles<RunStarted>, 
        Handles<TestSelectionChanged>, Handles<RunFinished>, Handles<ExploreStarted>, 
        Handles<TestStepFinished>
    {
        private readonly ITestTreeModel testTreeModel;

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly List<ListViewItem> listViewItems = new List<ListViewItem>();
        private int firstItem;
        private int lastItem;
        private int index;
        private int sortColumn = -1;
        private SortOrder sortOrder;
        private IList<TestTreeNode> selectedTests = new List<TestTreeNode>();

        public Observable<int> ResultsCount { get; private set; }

        public Observable<TimeSpan> ElapsedTime { get; private set; }

        public TestResultsController(ITestTreeModel testTreeModel)
        {
            this.testTreeModel = testTreeModel;

            ResultsCount = new Observable<int>();
            ElapsedTime = new Observable<TimeSpan>();
        }

        private void CountResults()
        {
            // invalidate cache
            listViewItems.Clear();
            ResultsCount.Value = 0;

            int count = 0;
            WalkTree(ttn => count += CountResults(ttn));
            
            // notify that list has changed
            ResultsCount.Value = count;
            ElapsedTime.Value = stopwatch.Elapsed;
        }

        private void WalkTree(Action<TestTreeNode> action)
        {
            if (selectedTests.Count == 0 && testTreeModel.Root != null)
            {
                action(testTreeModel.Root);
            }
            else
            {
                // need to do this because poxy namespace nodes don't really exist!
                foreach (var node in selectedTests)
                {
                    action(node);
                }
            }
        }

        private static int CountResults(TestTreeNode node)
        {
            int count = 0;
            count += node.TestStepRuns.Count;

            foreach (Node n in node.Nodes)
                count += CountResults((TestTreeNode) n);

            return count;
        }

        private void Reset()
        {
            ResultsCount.Value = 0;
            listViewItems.Clear();
          
            stopwatch.Reset();
            ElapsedTime.Value = stopwatch.Elapsed;
        }

        public void CacheVirtualItems(int startIndex, int endIndex)
        {
            if (startIndex >= firstItem && endIndex <= firstItem + listViewItems.Count)
            {
                // If the newly requested cache is a subset of the old cache, 
                // no need to rebuild everything, so do nothing.
                return;
            }

            firstItem = startIndex;
            lastItem = endIndex;

            UpdateTestResults();
        }

        public ListViewItem RetrieveVirtualItem(int itemIndex)
        {
            try
            {
                // update cache, if necessary
                if (itemIndex < firstItem)
                {
                    firstItem = itemIndex;
                    UpdateTestResults();
                }
                else if (itemIndex >= firstItem + listViewItems.Count)
                {
                    lastItem = itemIndex;
                    UpdateTestResults();
                }

                if ((itemIndex - firstItem) >= listViewItems.Count)
                {
                    UpdateTestResults();
                }

                var listViewItem = listViewItems[itemIndex - firstItem];

                if (listViewItem == null)
                    throw new Exception();

                return listViewItem;
            }
            catch
            {
                // this is bad, and should never happen :(
                return EmptyListViewItem();
            }
        }

        private static ListViewItem EmptyListViewItem()
        {
            var listViewItem = new ListViewItem("", -1);
            listViewItem.SubItems.AddRange(new[] { "", "", "", "", "" });
            return listViewItem;
        }

        private void UpdateTestResults()
        {
            listViewItems.Clear();
            index = 0;

            WalkTree(ttn => UpdateTestResults(ttn, 0));

            if (sortColumn != -1)
                SortAndTrimListViewItems();
        }

        private void SortAndTrimListViewItems()
        {
            // sort by relevant column
            listViewItems.Sort(delegate(ListViewItem left, ListViewItem right)
            {
                int result;
                switch (sortColumn)
                {
                    case 2:
                        // duration (double)
                        double dl = Convert.ToDouble(left.SubItems[sortColumn].Text);
                        double dr = Convert.ToDouble(right.SubItems[sortColumn].Text);
                        result = dl.CompareTo(dr);
                        break;

                    case 3:
                        // assert count (int)
                        double il = Convert.ToInt32(left.SubItems[sortColumn].Text);
                        double ir = Convert.ToInt32(right.SubItems[sortColumn].Text);
                        result = il.CompareTo(ir);
                        break;

                    default:
                        // string comparison
                        string sl = left.SubItems[sortColumn].Text;
                        string sr = right.SubItems[sortColumn].Text;
                        result = sl.CompareTo(sr);
                        break;
                }
                return sortOrder == SortOrder.Ascending ? result : -result;
            });

            // trim list to viewport
            //var relevantItems = new ListViewItem[lastItem - firstItem];
            //listViewItems.CopyTo(firstItem, relevantItems, 0, lastItem - firstItem);
            //listViewItems.Clear();
            //listViewItems.AddRange(relevantItems);
        }

        private void UpdateTestResults(TestTreeNode node, int indentCount)
        {
            if (node == null)
                return;

            // performance optimization, no need to worry about items outside the viewport
            // (only works when unsorted)
            if (index > lastItem && sortColumn == -1)
                return;

            foreach (TestStepRun tsr in node.TestStepRuns)
                AddTestStepRun(node.TestKind, tsr, indentCount);

            if (!(node is NamespaceNode) && sortColumn == -1)
                indentCount++;

            foreach (Node n in node.Nodes)
                UpdateTestResults((TestTreeNode) n, indentCount);
        }

        private void AddTestStepRun(string testKind, TestStepRun testStepRun, int indentCount)
        {
            // performance optimization, no need to worry about items outside the viewport
            // (only works when unsorted)
            if (index < firstItem && sortColumn == -1)
            {
                index++;
                return;
            }
            index++;

            // get the appropriate icon based on outcome
            int imgIndex = GetImageIndex(testStepRun.Result.Outcome.Status);
            var listViewItem = CreateListViewItem(testStepRun, imgIndex, testKind, indentCount);
            listViewItems.Add(listViewItem);
        }

        private static int GetImageIndex(TestStatus testStatus)
        {
            int imgIndex = -1;
            switch (testStatus)
            {
                case TestStatus.Failed:
                    imgIndex = 1;
                    break;
                case TestStatus.Inconclusive:
                    imgIndex = 2;
                    break;
                case TestStatus.Passed:
                    imgIndex = 0;
                    break;
            }
            return imgIndex;
        }

        private static ListViewItem CreateListViewItem(TestStepRun testStepRun, int imgIndex, 
            string testKind, int indentCount)
        {
            string name = testStepRun.Step.Name;
            string duration = testStepRun.Result.DurationInSeconds.ToString("0.000");
            string assertCount = testStepRun.Result.AssertCount.ToString();
            string codeReference = testStepRun.Step.CodeReference.TypeName ?? string.Empty;
            string fileName = testStepRun.Step.Metadata.GetValue(MetadataKeys.File) ?? string.Empty;

            // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
            if (name.Length == 260)
                name += " ";

            var listViewItem = new ListViewItem(name, imgIndex);
            listViewItem.SubItems.AddRange(new[] { testKind, duration, assertCount, codeReference, fileName });
            listViewItem.IndentCount = indentCount;
            return listViewItem;
        }

        public void SetSortColumn(int column)
        {
            // if sorting by the same column, reverse sort order
            if (sortColumn == column)
                sortOrder = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                // default to sorting ascending
                sortColumn = column;
                sortOrder = SortOrder.Ascending;
            }

            UpdateTestResults();
        }

        public void Handle(RunStarted @event)
        {
            Reset();
            stopwatch.Start();
        }

        public void Handle(TestSelectionChanged @event)
        {
            selectedTests = new List<TestTreeNode>(@event.Nodes);
            CountResults();
        }

        public void Handle(RunFinished @event)
        {
            stopwatch.Stop();
            ElapsedTime.Value = stopwatch.Elapsed;
        }

        public void Handle(ExploreStarted @event)
        {
            Reset();
        }

        public void Handle(TestStepFinished @event)
        {
            CountResults();
        }
    }
}
