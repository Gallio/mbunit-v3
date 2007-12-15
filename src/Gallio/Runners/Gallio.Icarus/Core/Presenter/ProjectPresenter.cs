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
using Gallio.Runner;

namespace Gallio.Icarus.Core.Presenter
{
    public class ProjectPresenter : IProjectPresenter
    {
        #region Variables

        private readonly IProjectAdapter _View;
        private readonly ITestRunnerModel _TestRunnerModel;
        private readonly ITestRunner testRunner;

        #endregion

        #region Properties

        public string StatusText
        {
            set { _View.StatusText = value; }
        }

        public int CompletedWorkUnits
        {
            set { _View.CompletedWorkUnits = value; }
        }

        public int TotalWorkUnits
        {
            set { _View.TotalWorkUnits = value; }
        }

        public ITestRunner TestRunner
        {
            get { return testRunner; }
        }

        #endregion

        #region Constructor

        public ProjectPresenter(IProjectAdapter view, ITestRunnerModel testrunnermodel)
        {
            _View = view;
            _TestRunnerModel = testrunnermodel;
            _TestRunnerModel.ProjectPresenter = this;

            testRunner = TestRunnerFactory.CreateIsolatedTestRunner();
            
            // wire up events
            _View.GetTestTree += GetTestTree;
            _View.RunTests += RunTests;
            _View.StopTests += StopTests;
            _View.SetFilter += SetFilter;
            _View.GetLogStream += GetLogStream;
            _View.GenerateReport += GenerateReport;
        }

        #endregion

        public void GetTestTree(object sender, ProjectEventArgs e)
        {
            _TestRunnerModel.LoadPackage(e.LocalTestPackageConfig);
            _View.TestModelData = _TestRunnerModel.BuildTests();
            _View.DataBind();
        }

        public void RunTests(object sender, EventArgs e)
        {
            _TestRunnerModel.RunTests();
        }

        public void StopTests(object sender, EventArgs e)
        {
            _TestRunnerModel.StopTests();
        }

        public void SetFilter(object sender, SetFilterEventArgs e)
        {
            testRunner.TestExecutionOptions.Filter = e.Filter;
        }

        public void GetLogStream(object sender, SingleStringEventArgs e)
        {
            _View.LogBody = _TestRunnerModel.GetLogStream(e.String);
        }

        public void GenerateReport(object sender, SingleStringEventArgs e)
        {
            _TestRunnerModel.GenerateReport(e.String);
        }

        public void Passed(string testId)
        {
            _View.Passed(testId);
        }

        public void Failed(string testId)
        {
            _View.Failed(testId);
        }

        public void Skipped(string testId)
        {
            _View.Skipped(testId);
        }

        public void Ignored(string testId)
        {
            _View.Ignored(testId);
        }
    }
}
