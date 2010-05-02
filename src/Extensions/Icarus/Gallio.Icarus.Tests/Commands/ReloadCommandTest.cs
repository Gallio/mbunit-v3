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
using System.ComponentModel;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.Services;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.Events;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [MbUnit.Framework.Category("Commands"), TestsOn(typeof(ReloadCommand))]
    public class ReloadCommandTest
    {
        private ReloadCommand reloadCommand;
        private ITestController testController;
        private IProjectController projectController;
        private IEventAggregator eventAggregator;

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            var filterService = MockRepository.GenerateStub<IFilterService>();
            reloadCommand = new ReloadCommand(testController, projectController, eventAggregator, filterService);
        }

        [Test]
        public void Execute_should_explore_with_test_runner_extensions()
        {
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>(new List<FilterInfo>()));

            reloadCommand.Execute(MockProgressMonitor.Instance);

            testController.AssertWasCalled(tc => tc.Explore(Arg<IProgressMonitor>.Is.Anything, 
                Arg.Is(testRunnerExtensions)));
        }

        [Test]
        public void Execute_should_send_an_event()
        {
            var testRunnerExtensions = new BindingList<string>();
            projectController.Stub(pc => pc.TestRunnerExtensions).Return(testRunnerExtensions);
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>(new List<FilterInfo>()));

            reloadCommand.Execute(MockProgressMonitor.Instance);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<Reloading>.Is.Anything));
        }
    }
}
