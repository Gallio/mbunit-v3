using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using System;
using Gallio.Icarus.Utilities;
using Gallio.Runner.Reports;
using Aga.Controls.Tree;
using Gallio.Model;

namespace Gallio.Icarus.Controllers
{
    internal class TestResultsController : NotifyController, ITestResultsController
    {
        private readonly ITestController testController;
        private readonly IOptionsController optionsController;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        private int resultsCount;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly Stopwatch perf = new Stopwatch();
        private readonly List<ListViewItem> listViewItems = new List<ListViewItem>();
        private int firstItem;
        private int lastItem;
        private int index;

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

        public int Passed
        {
            get { return testController.Passed; }
        }

        public int Failed
        {
            get { return testController.Failed; }
        }

        public int Skipped
        {
            get { return testController.Skipped; }
        }

        public int Inconclusive
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

        internal TestResultsController(ITestController testController, IOptionsController optionsController, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.testController = testController;

            testController.TestStepFinished += delegate
            {
                CountResults();
            };
            testController.SelectedTests.ListChanged += delegate
            {
                CountResults();
                OnPropertyChanged(new PropertyChangedEventArgs("TestCount"));
            };
            testController.ExploreStarted += delegate
            {
                Reset();
            };
            testController.ExploreFinished += delegate
            {
                OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));
            };
            testController.RunStarted += delegate
            {
                Reset();
                stopwatch.Start();
            };
            testController.RunFinished += delegate
            {
                stopwatch.Stop();
                MessageBox.Show(perf.ElapsedMilliseconds.ToString());
            };
            testController.PropertyChanged += ((sender, e) => OnPropertyChanged(e));

            this.optionsController = optionsController;
            optionsController.PropertyChanged += ((sender, e) => OnPropertyChanged(e));

            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
        }

        private void CountResults()
        {
            perf.Start();

            // invalidate cache
            ResultsCount = 0;

            int count = 0;

            if (testController.SelectedTests.Count == 0)
                count = CountResults(testController.Model.Root);
            else
            {
                // need to do this because poxy namespace nodes don't really exist!
                foreach (TestTreeNode node in testController.SelectedTests)
                    count += CountResults(node);
            }

            ResultsCount = count;

            OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));

            perf.Stop();
        }

        private static int CountResults(TestTreeNode node)
        {
            int count = 0;
            count += node.TestStepRuns.Count;

            foreach (Node n in node.Nodes)
                count += CountResults((TestTreeNode)n);

            return count;
        }

        private void Reset()
        {
            ResultsCount = 0;
          
            stopwatch.Reset();
            OnPropertyChanged(new PropertyChangedEventArgs("ElapsedTime"));

            perf.Reset();
        }

        public void CacheVirtualItems(int startIndex, int endIndex)
        {
            perf.Start();

            if (startIndex >= firstItem && endIndex <= firstItem + listViewItems.Count)
            {
                // If the newly requested cache is a subset of the old cache, 
                // no need to rebuild everything, so do nothing.
                return;
            }

            firstItem = startIndex;
            lastItem = endIndex;

            UpdateTestResults();

            perf.Stop();
        }

        public ListViewItem RetrieveVirtualItem(int itemIndex)
        {
            perf.Start();

            try
            {
                // If we don't have the item cached, then update the list
                if (itemIndex <= firstItem)
                {
                    firstItem = itemIndex;
                    UpdateTestResults();
                }
                else if (itemIndex > firstItem + (listViewItems.Count - 1))
                {
                    lastItem = itemIndex;
                    UpdateTestResults();
                }

                return listViewItems[itemIndex - firstItem];
            }
            catch (Exception)
            {
                return CreateListViewItem(string.Empty, -1, string.Empty, string.Empty, string.Empty, 
                    string.Empty, string.Empty, 0);
            }
            finally
            {
                perf.Stop();
            }
        }

        private void UpdateTestResults()
        {
            listViewItems.Clear();
            index = 0;

            if (testController.SelectedTests.Count == 0)
                UpdateTestResults(testController.Model.Root, 0);
            else
            {
                // need to do this because poxy namespace nodes don't really exist!
                foreach (TestTreeNode node in testController.SelectedTests)
                    UpdateTestResults(node, 0);
            }
        }

        private void UpdateTestResults(TestTreeNode node, int indentCount)
        {
            // performance optimization, no need to worry about items outside the viewport
            if (index > lastItem)
                return;

            foreach (TestStepRun tsr in node.TestStepRuns)
                AddTestStepRun(node.NodeType, tsr, indentCount);

            foreach (Node n in node.Nodes)
            {
                if (n is TestTreeNode)
                    UpdateTestResults((TestTreeNode)n, indentCount + 1);
            }
        }

        private void AddTestStepRun(string testKind, TestStepRun testStepRun, int indentCount)
        {
            // performance optimization, no need to worry about items outside the viewport
            if (index < firstItem)
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
    }
}
