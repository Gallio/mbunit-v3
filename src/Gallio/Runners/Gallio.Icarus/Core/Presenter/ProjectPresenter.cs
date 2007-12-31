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
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.Presenter
{
    public class ProjectPresenter : IProjectPresenter
    {
        private readonly IProjectAdapter projectAdapter;
        private readonly ITestRunnerModel testRunnerModel;
        private readonly ITestRunner testRunner;

        public string StatusText
        {
            set { projectAdapter.StatusText = value; }
        }

        public int CompletedWorkUnits
        {
            set { projectAdapter.CompletedWorkUnits = value; }
        }

        public int TotalWorkUnits
        {
            set { projectAdapter.TotalWorkUnits = value; }
        }

        public string ReportPath
        {
            set { projectAdapter.ReportPath = value; }
        }

        public ITestRunner TestRunner
        {
            get { return testRunner; }
        }

        public ProjectPresenter(IProjectAdapter view, ITestRunnerModel testrunnermodel)
        {
            projectAdapter = view;
            testRunnerModel = testrunnermodel;
            testRunnerModel.ProjectPresenter = this;

            testRunner = TestRunnerFactory.CreateIsolatedTestRunner();
            
            // wire up events
            projectAdapter.GetTestTree += GetTestTree;
            projectAdapter.RunTests += RunTests;
            projectAdapter.StopTests += StopTests;
            projectAdapter.SetFilter += SetFilter;
            projectAdapter.GetLogStream += GetLogStream;
            projectAdapter.GetReportTypes += GetReportTypes;
            projectAdapter.SaveReportAs += SaveReportAs;
            projectAdapter.GetAvailableLogStreams += GetAvailableLogStreams;
        }

        public void GetTestTree(object sender, ProjectEventArgs e)
        {
            testRunnerModel.LoadPackage(e.LocalTestPackageConfig);
            projectAdapter.TestModelData = testRunnerModel.BuildTests();
            projectAdapter.DataBind();
        }

        public void RunTests(object sender, EventArgs e)
        {
            testRunnerModel.RunTests();
            testRunnerModel.GenerateReport();
        }

        public void StopTests(object sender, EventArgs e)
        {
            testRunnerModel.StopTests();
        }

        public void SetFilter(object sender, SetFilterEventArgs e)
        {
            testRunner.TestExecutionOptions.Filter = e.Filter;
        }

        private void GetLogStream(object sender, GetLogStreamEventArgs e)
        {
            projectAdapter.LogBody = testRunnerModel.GetLogStream(e.LogStream, e.TestId);
        }

        public void GetReportTypes(object sender, EventArgs e)
        {
            projectAdapter.ReportTypes = testRunnerModel.GetReportTypes();
        }

        public void SaveReportAs(object sender, SaveReportAsEventArgs e)
        {
            testRunnerModel.SaveReportAs(e.FileName, e.Format);
        }

        public void GetAvailableLogStreams(object sender, SingleStringEventArgs e)
        {
            projectAdapter.AvailableLogStreams = testRunnerModel.GetAvailableLogStreams(e.String);
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            projectAdapter.Update(testData, testStepRun);
        }
    }
}
