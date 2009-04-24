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

using System.Windows.Forms;
using Gallio.Icarus.Controllers.Interfaces;

namespace Gallio.Icarus
{
    internal partial class TestResults : DockWindow
    {
        private readonly ITestResultsController testResultsController;

        public TestResults(ITestResultsController testResultsController)
        {
            this.testResultsController = testResultsController;

            InitializeComponent();

            testProgressStatusBar.DataBindings.Add("Mode", testResultsController, "TestStatusBarStyle");
            testProgressStatusBar.DataBindings.Add("PassedColor", testResultsController, "PassedColor");
            testProgressStatusBar.DataBindings.Add("FailedColor", testResultsController, "FailedColor");
            testProgressStatusBar.DataBindings.Add("InconclusiveColor", testResultsController, "InconclusiveColor");
            testProgressStatusBar.DataBindings.Add("SkippedColor", testResultsController, "SkippedColor");

            testProgressStatusBar.DataBindings.Add("Passed", testResultsController, "PassedTestCount");
            testProgressStatusBar.DataBindings.Add("Failed", testResultsController, "FailedTestCount");
            testProgressStatusBar.DataBindings.Add("Skipped", testResultsController, "SkippedTestCount");
            testProgressStatusBar.DataBindings.Add("Inconclusive", testResultsController, "InconclusiveTestCount");
            testProgressStatusBar.DataBindings.Add("ElapsedTime", testResultsController, "ElapsedTime");
            testProgressStatusBar.DataBindings.Add("Total", testResultsController, "TestCount");

            testResultsList.DataBindings.Add("VirtualListSize", testResultsController, "ResultsCount");

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