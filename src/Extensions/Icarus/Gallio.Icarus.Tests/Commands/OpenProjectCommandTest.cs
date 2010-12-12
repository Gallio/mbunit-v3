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

using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(OpenProjectCommand))]
    internal class OpenProjectCommandTest
    {
        private IProjectController projectController;
        private IEventAggregator eventAggregator;
        private OpenProjectCommand openProjectCommand;
        private ICommand loadPackageCommand;
        private ICommand restoreFilterCommand;
        private const string FileName = "fileName";

        [SetUp]
        public void SetUp()
        {
            projectController = MockRepository.GenerateStub<IProjectController>();
            var testFilters = new Observable<IList<FilterInfo>>(new List<FilterInfo>());
            projectController.Stub(pc => pc.TestFilters).Return(testFilters);
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            var commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            loadPackageCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateLoadPackageCommand()).Return(loadPackageCommand);
            restoreFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateRestoreFilterCommand(Arg<string>.Is.Anything))
                .Return(restoreFilterCommand);

            openProjectCommand = new OpenProjectCommand(projectController, eventAggregator, commandFactory)
            {
                ProjectLocation = FileName
            };
        }

        [Test]
        public void Execute_should_reset_test_status()
        {
            openProjectCommand.Execute(MockProgressMonitor.Instance);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg.Is(openProjectCommand), Arg<TestsReset>.Is.Anything));
        }

        [Test]
        public void Execute_should_open_project()
        {
            openProjectCommand.Execute(MockProgressMonitor.Instance);

            projectController.AssertWasCalled(pc => pc.OpenProject(Arg<IProgressMonitor>.Is.Anything, Arg.Is(FileName)));
        }

        [Test]
        public void Execute_should_load_package()
        {
            openProjectCommand.Execute(MockProgressMonitor.Instance);

            loadPackageCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_restore_test_filter()
        {
            openProjectCommand.Execute(MockProgressMonitor.Instance);

            restoreFilterCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }
    }
}
