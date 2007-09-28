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


using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model.Serialization;
using MbUnit.Icarus.Core.Interfaces;

namespace MbUnit.Icarus.Core.Model
{
    public class TestRunnerModel : ITestRunnerModel
    {
        #region public methods

        public TestModel LoadUpAssembly(ITestRunner runner, TestPackage testpackage)
        {
            ProgressMonitorShower p1 = new ProgressMonitorShower();
            runner.LoadPackage(testpackage, p1);

            ProgressMonitorShower p2 = new ProgressMonitorShower();
            runner.BuildTemplates(p2);

            ProgressMonitorShower p3 = new ProgressMonitorShower();
            runner.BuildTests(p3);

            return runner.TestModel;
        }

        #endregion
    }
}