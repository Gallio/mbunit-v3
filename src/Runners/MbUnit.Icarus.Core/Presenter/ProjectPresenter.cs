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
    public class ProjectPresenter
    {
        private readonly IProjectAdapter _View;
        private readonly ITestRunnerModel _TestRunnerModel;
        private readonly StandaloneRunner runner;

        public ProjectPresenter(IProjectAdapter view, ITestRunnerModel testrunnermodel)
        {
            _View = view;
            _TestRunnerModel = testrunnermodel;

            RuntimeSetup runtimeSetup = new RuntimeSetup();
            runner = StandaloneRunner.CreateRunner(runtimeSetup);
            _View.GetTestTree += GetTestTree;

        }

        private void GetTestTree(object sender, ProjectEventArgs e)
        {
            //TestPackage testpackage = new TestPackage();
            //testpackage.AssemblyFiles.Add("C:\\Source\\MbUnitGoogle\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll");
            // "C:\\MbUnit\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.Gallio\\bin\\MbUnit.TestResources.Gallio.dll");

            _View.TestCollection = _TestRunnerModel.LoadUpAssembly(runner, e.LocalTestPackage);
            _View.DataBind();
        }
    }
}
