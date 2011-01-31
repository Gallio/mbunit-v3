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
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(AddFilesCommand))]
    internal class AddFilesCommandTest
    {
        private AddFilesCommand command;
        private IProjectController projectController;
        private ICommandFactory commandFactory;
        private ICommand loadPackageCommand;

        [SetUp]
        public void SetUp()
        {
            projectController = MockRepository.GenerateStub<IProjectController>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            loadPackageCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateLoadPackageCommand())
                .Return(loadPackageCommand);

            command = new AddFilesCommand(projectController, commandFactory);
        }

        [Test]
        public void Execute_should_throw_if_files_are_not_set()
        {
            var exception = Assert.Throws<Exception>(() => command.Execute(MockProgressMonitor.Instance));

            Assert.AreEqual("No files to add", exception.Message);
        }

        [Test]
        public void Execute_should_throw_if_file_list_is_empty()
        {
            command.Files = new List<string>();

            var exception = Assert.Throws<Exception>(() => command.Execute(MockProgressMonitor.Instance));

			Assert.AreEqual("No files to add", exception.Message);
        }

        [Test]
        public void Execute_should_add_files_to_project()
        {
            var files = new List<string> { "a" };
            command.Files = files;

            command.Execute(MockProgressMonitor.Instance);

            projectController.AssertWasCalled(pc => pc.AddFiles(Arg<IProgressMonitor>.Is.Anything, 
                Arg.Is(files)));
        }

        [Test]
        public void Execute_should_load_the_package()
        {
            command.Files = new List<string> { "a" };

            command.Execute(MockProgressMonitor.Instance);

            loadPackageCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }
    }
}
