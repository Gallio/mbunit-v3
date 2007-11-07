// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Monitors;

namespace Gallio.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        #region Variables

        private ReportMonitor reportMonitor = null; 
        private IProjectPresenter projectPresenter = null;
        private StatusStripProgressMonitor statusStripProgressMonitor = null;
        private TestRunnerMonitor testRunnerMonitor = null;

        #endregion

        #region Properties

        public IProjectPresenter ProjectPresenter
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"ProjectPresenter");

                projectPresenter = value;                
            }
        }

        #endregion

        #region Methods

        public void LoadPackage(TestPackage testpackage)
        {
            // attach report monitor to test runner
            reportMonitor = new ReportMonitor();
            reportMonitor.Attach(projectPresenter.TestRunner);

            projectPresenter.TestRunner.LoadPackage(testpackage, new StatusStripProgressMonitor(projectPresenter));
        }

        public void BuildTemplates()
        {
            projectPresenter.TestRunner.BuildTemplates(new StatusStripProgressMonitor(projectPresenter));
        }

        public TestModel BuildTests()
        {
            projectPresenter.TestRunner.BuildTests(new StatusStripProgressMonitor(projectPresenter));
            return projectPresenter.TestRunner.TestModel;
        }

        public void RunTests()
        {
            testRunnerMonitor = new TestRunnerMonitor(projectPresenter, reportMonitor);
            testRunnerMonitor.Attach(projectPresenter.TestRunner);
            statusStripProgressMonitor = new StatusStripProgressMonitor(projectPresenter);
            projectPresenter.TestRunner.Run(statusStripProgressMonitor);
            statusStripProgressMonitor.Done();
            testRunnerMonitor.Detach();
        }

        public void StopTests()
        {
            if (statusStripProgressMonitor != null)
            {
                statusStripProgressMonitor.Cancel();
            }
        }

        public void SetFilter(Filter<ITest> filter)
        {
            projectPresenter.TestRunner.TestExecutionOptions.Filter = filter;
        }

        public string GetLogStream(string log)
        {
            return testRunnerMonitor.GetLogStream(log);
        }

        #endregion
    }
}