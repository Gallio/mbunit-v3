// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Core.Presenter
{
    public class ProjectPresenter : IProjectPresenter
    {
        private readonly IProjectAdapter projectAdapter;
        private readonly ITestRunnerModel testRunnerModel;
        private TestPackageConfig testPackageConfig;

        private bool shadowCopyEnabled;
        private bool testPackageLoaded;
        private TestModelData testModelData;

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

        public bool TestPackageLoaded
        {
            get { return testPackageLoaded; }
        }

        public ProjectPresenter(IProjectAdapter view, ITestRunnerModel testrunnermodel)
        {
            projectAdapter = view;
            testRunnerModel = testrunnermodel;
            testRunnerModel.ProjectPresenter = this;

            // wire up events
            projectAdapter.GetTestTree += GetTestTree;
            projectAdapter.RunTests += RunTests;
            projectAdapter.GenerateReport += OnGenerateReport;
            projectAdapter.StopTests += StopTests;
            projectAdapter.SetFilter += SetFilter;
            projectAdapter.GetReportTypes += GetReportTypes;
            projectAdapter.SaveReportAs += SaveReportAs;
            projectAdapter.GetTestFrameworks += OnGetTestFrameworks;
            projectAdapter.GetExecutionLog += OnGetExecutionLog;
            projectAdapter.UnloadTestPackage += OnUnload;
        }

        public void GetTestTree(object sender, GetTestTreeEventArgs e)
        {
            Unload();

            testPackageConfig = e.TestPackageConfig;
            shadowCopyEnabled = e.ShadowCopyEnabled;

            projectAdapter.TestModelData = Explore();
            projectAdapter.DataBind();

            if (!shadowCopyEnabled)
                Unload();
        }

        public void RunTests(object sender, EventArgs e)
        {
            if (Explore() != null)
            {
                testRunnerModel.Run();
            }

            if (!shadowCopyEnabled)
                Unload();
        }

        public void OnGenerateReport(object sender, EventArgs e)
        {
            projectAdapter.ReportPath = testRunnerModel.GenerateReport();
        }

        public void StopTests(object sender, EventArgs e)
        {
            testRunnerModel.StopTests();
        }

        public void SetFilter(object sender, SetFilterEventArgs e)
        {
            testRunnerModel.SetFilter(e.Filter);
        }

        public void GetReportTypes(object sender, EventArgs e)
        {
            projectAdapter.ReportTypes = testRunnerModel.GetReportTypes();
        }

        public void SaveReportAs(object sender, SaveReportAsEventArgs e)
        {
            testRunnerModel.SaveReportAs(e.FileName, e.Format);
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            projectAdapter.Update(testData, testStepRun);
        }

        public void OnGetTestFrameworks(object sender, EventArgs e)
        {
            projectAdapter.TestFrameworks = testRunnerModel.GetTestFrameworks();
        }

        public void OnGetExecutionLog(object sender, SingleEventArgs<string> e)
        {
            projectAdapter.ExecutionLog = testRunnerModel.GetExecutionLog(e.Arg, projectAdapter.TestModelData);
        }

        public void OnUnload(object sender, EventArgs e)
        {
            Unload();
        }

        private void Load()
        {
            if (!testPackageLoaded)
            {
                testRunnerModel.Load(testPackageConfig);

                testPackageLoaded = true;
                testModelData = null;
            }
        }

        private void Unload()
        {
            if (testPackageLoaded)
            {
                testRunnerModel.Unload();

                testPackageLoaded = false;
                testModelData = null;
            }
        }

        private TestModelData Explore()
        {
            Load();

            if (testModelData == null)
                testModelData = testRunnerModel.Explore();

            return testModelData;
        }
    }
}
