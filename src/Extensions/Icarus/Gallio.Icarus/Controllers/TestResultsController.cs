// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.Controllers
{
    internal class TestResultsController : NotifyController, ITestResultsController
    {
        private readonly ITestController testController;
        private readonly ITestTreeModel testTreeModel;

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly List<ListViewItem> listViewItems = new List<ListViewItem>();
        private int firstItem;
        private int lastItem;
        private int index;
        private int sortColumn = -1;
        private SortOrder sortOrder;

        public Observable<int> ResultsCount
        {
            get; private set;
        }

        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
        }

        public TestResultsController(ITestController testController, ITestTreeModel testTreeModel)
        {
            this.testController = testController;
            this.testTreeModel = testTreeModel;

            testController.TestStepFinished += (sender, e) => CountResults();
            testController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "SelectedTests")
                    return;

                CountResults();

                OnPropertyChanged(e);
            };
            testController.ExploreStarted += (sender, e) => Reset();
            testController.ExploreFinished += (sender, e) => OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
            testController.RunStarted += (sender, e) =>
            {
                Reset();
                stopwatch.Start();
            };
            testController.RunFinished += (sender, e) => stopwatch.Stop();

            ResultsCount = new Observable<int>();
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
            OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
        }

        private void WalkTree(Action<TestTreeNode> action)
        {
            var selectedTests = new TestTreeNode[0];
            testController.SelectedTests.Read(sts =>
            {
                selectedTests = new TestTreeNode[sts.Count];
                sts.CopyTo(selectedTests, 0);
            });

            if (selectedTests.Length == 0 && testTreeModel.Root != null)
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
            OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
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

            return listViewItems[itemIndex - firstItem];
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
            int imgIndex = -1;
            switch (testStepRun.Result.Outcome.Status)
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
            // convert the test step run information to a format for display as a list view item
            string duration = testStepRun.Result.DurationInSeconds.ToString("0.000");
            string assertCount = testStepRun.Result.AssertCount.ToString();
            string codeReference = testStepRun.Step.CodeReference.TypeName ?? string.Empty;
            string fileName = testStepRun.Step.Metadata.GetValue(MetadataKeys.File) ?? string.Empty;
            ListViewItem listViewItem = CreateListViewItem(testStepRun.Step.Name, imgIndex, testKind, duration, assertCount,
                codeReference, fileName, indentCount);
            listViewItems.Add(listViewItem);
        }

        private static ListViewItem CreateListViewItem(string name, int imgIndex, string testKind, string duration, string assertCount,
            string codeReference, string fileName, int indentCount)
        {
            // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
            if (name.Length == 260)
                name += " ";

            ListViewItem lvi = new ListViewItem(name, imgIndex);
            lvi.SubItems.AddRange(new[] { testKind, duration, assertCount, codeReference, fileName });
            lvi.IndentCount = indentCount;
            return lvi;
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
    }
}
