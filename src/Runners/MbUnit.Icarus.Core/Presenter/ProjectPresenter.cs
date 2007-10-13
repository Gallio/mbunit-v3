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
using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Core.RuntimeSupport;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;

namespace MbUnit.Icarus.Core.Presenter
{
    public class ProjectPresenter : IProjectPresenter
    {
        #region Variables

        private readonly IProjectAdapter _View;
        private readonly ITestRunnerModel _TestRunnerModel;
        private readonly StandaloneRunner runner;

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

        public StandaloneRunner Runner
        {
            get { return runner; }
        }

        #endregion

        #region Constructor

        public ProjectPresenter(IProjectAdapter view, ITestRunnerModel testrunnermodel)
        {
            _View = view;
            _TestRunnerModel = testrunnermodel;

            RuntimeSetup runtimeSetup = new RuntimeSetup();
            runner = StandaloneRunner.CreateRunner(runtimeSetup);
            
            // wire up events
            _View.GetTestTree += GetTestTree;
            _View.RunTests += RunTests;
        }

        #endregion

        public void GetTestTree(object sender, ProjectEventArgs e)
        {
            _View.TestModel = _TestRunnerModel.LoadUpAssembly(this, e.LocalTestPackage);
            _View.DataBind();
        }

        public void RunTests(object sender, EventArgs e)
        {
            _TestRunnerModel.RunTests(this);
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
