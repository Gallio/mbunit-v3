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

using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;

namespace Gallio.Icarus
{
    internal partial class TestResults : DockWindow
    {
        private readonly ITestResultsController testResultsController;

        public TestResults(ITestResultsController testResultsController, IOptionsController optionsController, 
            ITestTreeModel testTreeModel, ITestStatistics testStatistics)
        {
            this.testResultsController = testResultsController;

            InitializeComponent();

            testProgressStatusBar.DataBindings.Add("Mode", optionsController, "TestStatusBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", optionsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", optionsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", optionsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", optionsController, "SkippedColor");

            testStatistics.Passed.PropertyChanged += (s, e) => 
                testProgressStatusBar.Passed = testStatistics.Passed.Value;
            testStatistics.Failed.PropertyChanged += (s, e) =>
                testProgressStatusBar.Failed = testStatistics.Failed.Value;
            testStatistics.Skipped.PropertyChanged += (s, e) =>
                testProgressStatusBar.Skipped = testStatistics.Skipped.Value;
            testStatistics.Inconclusive.PropertyChanged += (s, e) =>
                testProgressStatusBar.Inconclusive = testStatistics.Inconclusive.Value;

            testTreeModel.TestCount.PropertyChanged += (s, e) => testProgressStatusBar.Total = testTreeModel.TestCount;
            
            testProgressStatusBar.DataBindings.Add("ElapsedTime", testResultsController, "ElapsedTime");
            
            testResultsController.ResultsCount.PropertyChanged += (s, e) => 
                testResultsList.VirtualListSize = testResultsController.ResultsCount;

            testResultsList.RetrieveVirtualItem += testResultsList_RetrieveVirtualItem;
            testResultsList.CacheVirtualItems += testResultsList_CacheVirtualItems;
            testResultsList.ColumnClick += testResultsList_ColumnClick;
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