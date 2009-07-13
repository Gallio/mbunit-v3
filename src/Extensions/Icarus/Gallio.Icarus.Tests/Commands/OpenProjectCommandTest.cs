// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Runner.Projects.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(OpenProjectCommand))]
    internal class OpenProjectCommandTest
    {
        [Test]
        public void Execute_should_reset_test_status()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.ResetTestStatus(progressMonitor));
        }

        [Test]
        public void Execute_should_open_project()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.OpenProject(fileName, progressMonitor));
        }

        [Test]
        public void Execute_should_reload_test_package()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.Stub(pc => pc.TestFilters).Return(new BindingList<FilterInfo>(new List<FilterInfo>()));
            var testPackage = new TestPackage();
            projectController.Stub(pc => pc.TestPackage).Return(testPackage);
            var testRunnerExtensions = new BindingList<string>(new List<string>());
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();

            openProjectCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.SetTestPackage(testPackage));
            testController.AssertWasCalled(tc => tc.Explore(progressMonitor, testRunnerExtensions));
        }

        [Test]
        public void Execute_should_throw_if_canceled()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            const string fileName = "fileName";
            var openProjectCommand = new OpenProjectCommand(testController, projectController, fileName);
            var progressMonitor = MockProgressMonitor.GetMockProgressMonitor();
            progressMonitor.Stub(pm => pm.IsCanceled).Return(true);

            Assert.Throws<OperationCanceledException>(() => openProjectCommand.Execute(progressMonitor));
        }
    }
}
