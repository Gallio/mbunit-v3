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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ReloadCommand))]
    public class ReloadCommandTest
    {
        private ReloadCommand command;
        private IEventAggregator eventAggregator;
        private IOptionsController optionsController;
        private ICommandFactory commandFactory;
        private ICommand restoreFilterCommand;
        private ICommand saveFilterCommand;
        private ICommand loadPackageCommand;

        [SetUp]
        public void SetUp()
        {
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            optionsController = MockRepository.GenerateStub<IOptionsController>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            StubCommands();

            command = new ReloadCommand(commandFactory, eventAggregator, optionsController);
        }

        private void StubCommands()
        {
            saveFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateSaveFilterCommand(Arg<string>.Is.Anything))
                .Return(saveFilterCommand);

            loadPackageCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateLoadPackageCommand())
                .Return(loadPackageCommand);

            restoreFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRestoreFilterCommand(Arg<string>.Is.Anything))
                .Return(restoreFilterCommand);
        }

        [Test]
        public void Execute_should_send_a_reloading_event()
        {
            command.Execute(MockProgressMonitor.Instance);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(command), Arg<Reloading>.Is.Anything));
        }

        [Test]
        public void Execute_should_save_current_test_filter()
        {
            command.Execute(MockProgressMonitor.Instance);

            commandFactory.AssertWasCalled(cf => cf.CreateSaveFilterCommand("AutoSave"));
            saveFilterCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_load_package()
        {
            command.Execute(MockProgressMonitor.Instance);

            loadPackageCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_restore_test_filter()
        {
            command.Execute(MockProgressMonitor.Instance);

            restoreFilterCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_restore_test_tree_state()
        {
            command.Execute(MockProgressMonitor.Instance);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(command), 
                Arg<UserOptionsLoaded>.Is.Anything));
        }

        [Test]
        public void Tests_should_not_be_run_if_option_is_not_set()
        {
            optionsController.RunTestsAfterReload = false;

            command.Execute(MockProgressMonitor.Instance);

            commandFactory.AssertWasNotCalled(cf => cf.CreateRunTestsCommand(Arg<bool>.Is.Anything));
        }

        [Test]
        public void Tests_should_be_run_if_option_is_set()
        {
            optionsController.RunTestsAfterReload = true;
            var runTestsCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRunTestsCommand(Arg<bool>.Is.Anything))
                .Return(runTestsCommand);

            command.Execute(MockProgressMonitor.Instance);

            runTestsCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }
    }
}
