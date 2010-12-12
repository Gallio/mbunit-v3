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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(NewProjectCommand))]
    internal class NewProjectCommandTest
    {
        private IProjectController projectController;
        private ICommandFactory commandFactory;
        private NewProjectCommand command;
        private ICommand loadPackageCommand;

        [SetUp]
        public void SetUp()
        {
            projectController = MockRepository.GenerateStub<IProjectController>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            loadPackageCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateLoadPackageCommand()).Return(loadPackageCommand);
            command = new NewProjectCommand(projectController, commandFactory);
        }

        [Test]
        public void Execute_should_create_new_project()
        {
            command.Execute(MockProgressMonitor.Instance);

            projectController.AssertWasCalled(pc => pc.NewProject(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_reload_test_package()
        {
            command.Execute(NullProgressMonitor.CreateInstance());

            loadPackageCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }
    }
}
