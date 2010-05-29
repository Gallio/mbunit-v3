// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(LoadPackageCommand))]
    public class LoadPackageCommandTest
    {
        private ITestController testController;
        private IProjectController projectController;
        private LoadPackageCommand loadPackageCommand;
        private IProgressMonitor progressMonitor;

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            loadPackageCommand = new LoadPackageCommand(testController, projectController);
            progressMonitor = MockProgressMonitor.Instance;
        }

        [Test]
        public void Execute_should_reload_the_test_package()
        {
            var testPackage = new TestPackage();
            projectController.Stub(pc => pc.TestPackage).Return(testPackage);

            loadPackageCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackage(testPackage));
        }

        [Test]
        public void Execute_should_explore()
        {
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensionSpecifications).Return(testRunnerExtensions);

            loadPackageCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }
    }
}
