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
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.UI.Progress;

namespace Gallio.Icarus.Controllers
{
    internal class TestResultsController : NotifyController, ITestResultsController
    {
        private readonly ITestController testController;
        private readonly IOptionsController optionsController;
        private readonly ITaskManager taskManager;

        private int resultsCount;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly List<ListViewItem> listViewItems = new List<ListViewItem>();
        private int firstItem;
        private int lastItem;
        private int index;
        private int sortColumn = -1;
        private SortOrder sortOrder;

        public int ResultsCount
        {
            get 
            { 
                return resultsCount; 
            }
            private set
            {
                resultsCount = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ResultsCount"));
            }
        }

        public string TestStatusBarStyle
        {
            get { return optionsController.TestStatusBarStyle; }
        }

        public Color PassedColor
        {
            get { return optionsController.PassedColor; }
        }

        public Color FailedColor
        {
            get { return optionsController.FailedColor; }
        }

        public Color InconclusiveColor
        {
            get { return optionsController.InconclusiveColor; }
        }

        public Color SkippedColor
        {
            get { return optionsController.SkippedColor; }
        }

        public int PassedTestCount
        {
            get { return testController.Passed; }
        }

        public int FailedTestCount
        {
            get { return testController.Failed; }
        }

        public int SkippedTestCount
        {
            get { return testController.Skipped; }
        }

        public int InconclusiveTestCount
        {
            get { return testController.Inconclusive; }
        }

        public TimeSpan ElapsedTime
        {
            get { return stopwatch.Elapsed; }
        }

        public int TestCount
        {
            get { return testController.TestCount; }
        }

        public TestResultsController(ITestController testController, IOptionsController optionsController, 
            ITaskManager taskManager)
        {
            if (testController == null) 
                throw new ArgumentNullException("testController");
            
            if (optionsController == null) 
                throw new ArgumentNullException("optionsController");
            
            if (taskManager == null) 
                throw new ArgumentNullException("taskManager");

            this.testController = testController;

            testController.TestStepFinished += (sender, e) => CountResults();
            testController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "SelectedTests")
                    return;

                CountResults();
            };
            testController.ExploreStarted += (sender, e) => Reset();
            testController.ExploreFinished += (sender, e) => 
            {
                OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
            };
            testController.RunStarted += (sender, e) =>
            {
                Reset();
                stopwatch.Start();
            };
            testController.RunFinished += (sender, e) => stopwatch.Stop();
            testController.PropertyChanged += ((sender, e) => OnPropertyChanged(e));

            this.optionsController = optionsController;
            optionsController.PropertyChanged += ((sender, e) => OnPropertyChanged(e));

            this.taskManager = taskManager;
        }

        private void CountResults()
        {
            taskManager.BackgroundTask(() =>
            {
                int count = 0;

                if (testController.Model.Root != null)
                {
                    if (testController.SelectedTests.Count == 0)
                    {
                        CountResults(testController.Model.Root, ref count);
                    }
                    else
                    {
                        // need to do this because poxy namespace nodes don't really exist!
                        foreach (TestTreeNode node in testController.SelectedTests)
                            CountResults(node, ref count);
                    }
                }

                // update cache
                UpdateTestResults();

                // notify that list has changed
                ResultsCount = count;
                OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
            });
        }

        private void CountResults(TestTreeNode node, ref int count)
        {
            count += node.TestStepRuns.Count;

            foreach (Node n in node.Nodes)
                CountResults((TestTreeNode) n, ref count);
        }

        private void Reset()
        {
            ResultsCount = 0;
          
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
            else if (itemIndex > (firstItem + (listViewItems.Count - 1)))
            {
                lastItem = itemIndex;
                UpdateTestResults();
            }

            if ((itemIndex - firstItem) > (listViewItems.Count - 1))
            {
                UpdateTestResults();
            }

            return listViewItems[itemIndex - firstItem];
        }

        private void UpdateTestResults()
        {
            listViewItems.Clear();
            index = 0;

            if (testController.SelectedTests.Count == 0)
            {
                UpdateTestResults(testController.Model.Root, 0);
            }
            else
            {
                // need to do this because poxy namespace nodes don't really exist!
                foreach (TestTreeNode node in testController.SelectedTests)
                    UpdateTestResults(node, 0);
            }

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
            ListViewItem[] relevantItems = new ListViewItem[lastItem - firstItem];
            listViewItems.CopyTo(firstItem, relevantItems, 0, lastItem - firstItem);
            listViewItems.Clear();
            listViewItems.AddRange(relevantItems);
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
            string duration = testStepRun.Result.Duration.ToString("0.000");
            string assertCount = testStepRun.Result.AssertCount.ToString();
            string codeReference = testStepRun.Step.CodeReference.TypeName ?? string.Empty;
            string assemblyName = testStepRun.Step.CodeReference.AssemblyName ?? string.Empty;
            ListViewItem listViewItem = CreateListViewItem(testStepRun.Step.Name, imgIndex, testKind, duration, assertCount,
                codeReference, assemblyName, indentCount);
            listViewItems.Add(listViewItem);
        }

        private static ListViewItem CreateListViewItem(string name, int imgIndex, string testKind, string duration, string assertCount,
            string codeReference, string assemblyName, int indentCount)
        {
            // http://blogs.msdn.com/cumgranosalis/archive/2006/03/18/ListViewVirtualModeBugs.aspx
            if (name.Length == 260)
                name += " ";

            ListViewItem lvi = new ListViewItem(name, imgIndex);
            lvi.SubItems.AddRange(new[] { testKind, duration, assertCount, codeReference, assemblyName });
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
