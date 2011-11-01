// Copyright 2011 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.TestResults
{
    public class TestResultsPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly ITestResultsController testResultsController;
        private readonly IOptionsController optionsController;
        private readonly ITestTreeModel testTreeModel;
        private readonly ITestStatistics testStatistics;

        public const string WindowId = "Gallio.Icarus.TestResults";

        public TestResultsPackage(IWindowManager windowManager, ITestResultsController testResultsController, IOptionsController optionsController,
            ITestTreeModel testTreeModel, ITestStatistics testStatistics)
        {
            this.windowManager = windowManager;
            this.testResultsController = testResultsController;
            this.optionsController = optionsController;
            this.testTreeModel = testTreeModel;
            this.testStatistics = testStatistics;
        }

        public void Load()
        {
            RegisterWindow();
            AddMenuItem();
        }

        private void RegisterWindow()
        {
            windowManager.Register(WindowId, () =>
            {
                var testResults = new TestResults(testResultsController, optionsController, testTreeModel, testStatistics);
                windowManager.Add(WindowId, testResults, TestResultsResources.Test_Results);
            }, Location.Centre);
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = TestResultsResources.Test_Results,
            });
        }

        public void Dispose() { }         
    }
}
