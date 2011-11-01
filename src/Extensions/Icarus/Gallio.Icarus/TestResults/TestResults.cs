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
using System.ComponentModel;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.TestResults
{
    internal partial class TestResults : UserControl
    {
        private readonly ITestResultsController testResultsController;
        private readonly IOptionsController optionsController;
        private readonly ITestTreeModel testTreeModel;
        private readonly ITestStatistics testStatistics;

        public TestResults(ITestResultsController testResultsController, IOptionsController optionsController, 
            ITestTreeModel testTreeModel, ITestStatistics testStatistics)
        {
            this.testResultsController = testResultsController;
            this.optionsController = optionsController;
            this.testTreeModel = testTreeModel;
            this.testStatistics = testStatistics;

            InitializeComponent();            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            BindProgressBar();
            BindTestStatistics();
            BindTreeModel();
            BindTestResults();
        }

        private void BindProgressBar()
        {
            testProgressStatusBar.DataBindings.Add("Mode", optionsController, "TestStatusBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", optionsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", optionsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", optionsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", optionsController, "SkippedColor");
        }

        private void BindTestStatistics()
        {
            PropertyChangedEventHandler passedOnPropertyChanged = (s, e) => testProgressStatusBar.Passed = testStatistics.Passed.Value;
            PropertyChangedEventHandler failedOnPropertyChanged = (s, e) => testProgressStatusBar.Failed = testStatistics.Failed.Value;
            PropertyChangedEventHandler skippedOnPropertyChanged = (s, e) => testProgressStatusBar.Skipped = testStatistics.Skipped.Value;
            PropertyChangedEventHandler inconclusiveOnPropertyChanged = (s, e) => testProgressStatusBar.Inconclusive = testStatistics.Inconclusive.Value;

            testStatistics.Passed.PropertyChanged += passedOnPropertyChanged;
            testStatistics.Failed.PropertyChanged += failedOnPropertyChanged;
            testStatistics.Skipped.PropertyChanged += skippedOnPropertyChanged;
            testStatistics.Inconclusive.PropertyChanged += inconclusiveOnPropertyChanged;

            Disposed += (s, e) =>
            {
                testStatistics.Passed.PropertyChanged -= passedOnPropertyChanged;
                testStatistics.Failed.PropertyChanged -= failedOnPropertyChanged;
                testStatistics.Skipped.PropertyChanged -= skippedOnPropertyChanged;
                testStatistics.Inconclusive.PropertyChanged -= inconclusiveOnPropertyChanged;
            };
        }

        private void BindTreeModel()
        {
            PropertyChangedEventHandler testCountOnPropertyChanged = (s, e) => testProgressStatusBar.Total = testTreeModel.TestCount;
            testTreeModel.TestCount.PropertyChanged += testCountOnPropertyChanged;
            Disposed += (s, e) => testTreeModel.TestCount.PropertyChanged -= testCountOnPropertyChanged;
        }

        private void BindTestResults()
        {
            PropertyChangedEventHandler elapsedTimeOnPropertyChanged = (s, e) => testProgressStatusBar.ElapsedTime = testResultsController.ElapsedTime;
            PropertyChangedEventHandler resultsCountOnPropertyChanged = (s, e) => testResultsList.VirtualListSize = testResultsController.ResultsCount;
            testResultsController.ElapsedTime.PropertyChanged += elapsedTimeOnPropertyChanged;
            testResultsController.ResultsCount.PropertyChanged += resultsCountOnPropertyChanged;

            testResultsList.RetrieveVirtualItem += testResultsList_RetrieveVirtualItem;
            testResultsList.CacheVirtualItems += testResultsList_CacheVirtualItems;
            testResultsList.ColumnClick += testResultsList_ColumnClick;

            Disposed += (s, e) =>
            {
                testResultsController.ElapsedTime.PropertyChanged -= elapsedTimeOnPropertyChanged;
                testResultsController.ResultsCount.PropertyChanged -= resultsCountOnPropertyChanged;

                testResultsList.RetrieveVirtualItem -= testResultsList_RetrieveVirtualItem;
                testResultsList.CacheVirtualItems -= testResultsList_CacheVirtualItems;
                testResultsList.ColumnClick -= testResultsList_ColumnClick;
            };
        }

        private void testResultsList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            testResultsController.SetSortColumn(e.Column);
            testResultsList.Invalidate();
        }

        private void testResultsList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            testResultsController.CacheVirtualItems(e.StartIndex, e.EndIndex);
        }

        private void testResultsList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = testResultsController.RetrieveVirtualItem(e.ItemIndex);
        }
    }
}