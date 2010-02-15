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
using Gallio.Icarus.TestResults;
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
        private const int NO_IMAGE = -1;
        private IList<TestTreeNode> selectedTests = new List<TestTreeNode>();
        private readonly TestStepComparer testStepComparer = new TestStepComparer();

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

            var count = 0;
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
            var listViewItem = new ListViewItem("", NO_IMAGE);
            listViewItem.SubItems.AddRange(new[] { "", "", "", "", "" });
            return listViewItem;
        }

        private void UpdateTestResults()
        {
            listViewItems.Clear();
            index = 0;

            WalkTree(ttn => UpdateTestResults(ttn, 0));

            if (testStepComparer.SortColumn != TestStepComparer.NO_SORT)
            {
                listViewItems.Sort(testStepComparer);
            }
        }

        private void UpdateTestResults(TestTreeNode node, int indentCount)
        {
            if (node == null)
                return;

            // performance optimization, no need to worry about items outside the viewport
            // (only works when unsorted)
            if (index > lastItem && testStepComparer.SortColumn == TestStepComparer.NO_SORT)
                return;

            foreach (var tsr in node.TestStepRuns)
                AddTestStepRun(node.TestKind, tsr, indentCount);

            if (!(node is NamespaceNode) && testStepComparer.SortColumn == TestStepComparer.NO_SORT)
                indentCount++;

            foreach (var n in node.Nodes)
                UpdateTestResults((TestTreeNode) n, indentCount);
        }

        private void AddTestStepRun(string testKind, TestStepRun testStepRun, int indentCount)
        {
            // performance optimization, no need to worry about items outside the viewport
            // (only works when unsorted)
            if (index < firstItem && testStepComparer.SortColumn == TestStepComparer.NO_SORT)
            {
                index++;
                return;
            }
            index++;

            // get the appropriate icon based on outcome
            var imgIndex = GetImageIndex(testStepRun.Result.Outcome.Status);
            var listViewItem = CreateListViewItem(testStepRun, imgIndex, testKind, indentCount);
            listViewItems.Add(listViewItem);
        }

        private static int GetImageIndex(TestStatus testStatus)
        {
            var imgIndex = NO_IMAGE;
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
            var testStepName = testStepRun.Step.Name;
            var duration = testStepRun.Result.DurationInSeconds.ToString("0.000");
            var assertCount = testStepRun.Result.AssertCount.ToString();
            var codeReference = testStepRun.Step.CodeReference.TypeName ?? string.Empty;
            var fileName = testStepRun.Step.Metadata.GetValue(MetadataKeys.File) ?? string.Empty;

            // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
            if (testStepName.Length == 260)
                testStepName += " ";

            var listViewItem = new ListViewItem(testStepName, imgIndex);
            listViewItem.SubItems.AddRange(new[] { testKind, duration, assertCount, codeReference, fileName });
            listViewItem.IndentCount = indentCount;
            return listViewItem;
        }

        public void SetSortColumn(int column)
        {
            if (testStepComparer.SortColumn == column)
            {
                switch (testStepComparer.SortOrder)
                {
                    case SortOrder.Ascending:
                        testStepComparer.SortOrder = SortOrder.Descending;
                        break;
                    case SortOrder.Descending:
                        testStepComparer.SortColumn = TestStepComparer.NO_SORT;
                        break;
                }
            }
            else
            {
                testStepComparer.SortColumn = column;
                testStepComparer.SortOrder = SortOrder.Ascending;
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
