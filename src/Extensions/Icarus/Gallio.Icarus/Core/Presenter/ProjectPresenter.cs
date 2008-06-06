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
using System.Collections.Generic;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;

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

        public ProjectPresenter(IProjectAdapter projectAdapter, ITestRunnerModel testRunnerModel)
        {
            this.projectAdapter = projectAdapter;
            this.testRunnerModel = testRunnerModel;

            // wire up events
            projectAdapter.GetTestTree += projectAdapter_GetTestTree;
            projectAdapter.RunTests += projectAdapter_RunTests;
            projectAdapter.GenerateReport += projectAdapter_GenerateReport;
            projectAdapter.CancelOperation += projectAdapter_CancelOperation;
            projectAdapter.SetFilter += projectAdapter_SetFilter;
            projectAdapter.GetReportTypes += projectAdapter_GetReportTypes;
            projectAdapter.SaveReportAs += projectAdapter_SaveReportAs;
            projectAdapter.GetTestFrameworks += projectAdapter_GetTestFrameworks;
            projectAdapter.GetExecutionLog += projectAdapter_GetExecutionLog;
            projectAdapter.UnloadTestPackage += projectAdapter_UnloadTestPackage;

            testRunnerModel.ProgressUpdate += new EventHandler<ProgressUpdateEventArgs>(testRunnerModel_ProgressUpdate);
            testRunnerModel.TestStepFinished += new EventHandler<TestStepFinishedEventArgs>(testRunnerModel_TestStepFinished);
        }

        public void testRunnerModel_TestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            projectAdapter.Update(e.Test, e.TestStepRun);
        }

        public void testRunnerModel_ProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            projectAdapter.TaskName = e.TaskName;
            projectAdapter.SubTaskName = e.SubTaskName;
            projectAdapter.CompletedWorkUnits = e.CompletedWorkUnits;
            projectAdapter.TotalWorkUnits = e.TotalWorkUnits;
        }

        public void projectAdapter_GetTestTree(object sender, GetTestTreeEventArgs e)
        {
            Unload();

            testPackageConfig = e.TestPackageConfig;
            shadowCopyEnabled = e.ShadowCopyEnabled;

            projectAdapter.TestModelData = Explore();
            projectAdapter.DataBind();

            if (!shadowCopyEnabled)
                Unload();
        }

        public void projectAdapter_RunTests(object sender, EventArgs e)
        {
            if (Explore() != null)
            {
                testRunnerModel.Run();
            }

            if (!shadowCopyEnabled)
                Unload();
        }

        public void projectAdapter_GenerateReport(object sender, EventArgs e)
        {
            projectAdapter.ReportPath = testRunnerModel.GenerateReport();
        }

        public void projectAdapter_CancelOperation(object sender, EventArgs e)
        {
            testRunnerModel.CancelOperation();
        }

        public void projectAdapter_SetFilter(object sender, SingleEventArgs<Filter<ITest>> e)
        {
            testRunnerModel.SetFilter(e.Arg);
        }

        public void projectAdapter_GetReportTypes(object sender, EventArgs e)
        {
            projectAdapter.ReportTypes = testRunnerModel.GetReportTypes();
        }

        public void projectAdapter_SaveReportAs(object sender, SaveReportAsEventArgs e)
        {
            testRunnerModel.SaveReportAs(e.FileName, e.Format);
        }

        public void projectAdapter_GetTestFrameworks(object sender, EventArgs e)
        {
            projectAdapter.TestFrameworks = testRunnerModel.GetTestFrameworks();
        }

        public void projectAdapter_GetExecutionLog(object sender, SingleEventArgs<IList<string>> e)
        {
            projectAdapter.ExecutionLog = testRunnerModel.GetExecutionLog(e.Arg, projectAdapter.TestModelData);
        }

        public void projectAdapter_UnloadTestPackage(object sender, EventArgs e)
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
