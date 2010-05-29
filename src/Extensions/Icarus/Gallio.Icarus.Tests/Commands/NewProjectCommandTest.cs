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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Model.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(NewProjectCommand))]
    internal class NewProjectCommandTest
    {
        [Test]
        public void Execute_should_create_new_project()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;

            newProjectCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.NewProject(progressMonitor));
        }

        [Test]
        public void Execute_should_reload_test_package()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testPackage = new TestPackage();
            projectController.Stub(pc => pc.TestPackage).Return(testPackage);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            projectController.Stub(pc => pc.TestRunnerExtensionSpecifications).Return(testRunnerExtensions);
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;

            newProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackage(testPackage));
            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }

        [Test]
        public void Execute_should_throw_if_canceled()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testController = MockRepository.GenerateStub<ITestController>();
            var newProjectCommand = new NewProjectCommand(projectController, testController);
            var progressMonitor = MockProgressMonitor.Instance;
            progressMonitor.Stub(pm => pm.IsCanceled).Return(true);

            Assert.Throws<OperationCanceledException>(() => newProjectCommand.Execute(progressMonitor));
        }
    }
}
