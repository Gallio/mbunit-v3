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
    [Category("Commands"), TestsOn(typeof(OpenProjectCommand))]
    internal class OpenProjectCommandTest
    {
        private ITestController testController;
        private IProjectController projectController;
        private IEventAggregator eventAggregator;
        private OpenProjectCommand openProjectCommand;
        private IProgressMonitor progressMonitor;
        private const string FileName = "fileName";

        [SetUp]
        public void SetUp()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            projectController = MockRepository.GenerateStub<IProjectController>();
            var testFilters = new Observable<IList<FilterInfo>>(new List<FilterInfo>());
            projectController.Stub(pc => pc.TestFilters).Return(testFilters);
            eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            var filterService = MockRepository.GenerateStub<IFilterService>();
            progressMonitor = MockProgressMonitor.Instance;
            openProjectCommand = new OpenProjectCommand(testController, projectController, 
                eventAggregator, filterService)
            {
                ProjectLocation = FileName
            };
        }

        [Test]
        public void Execute_should_reset_test_status()
        {
            openProjectCommand.Execute(progressMonitor);

            eventAggregator.AssertWasCalled(ea => ea.Send(Arg<TestsReset>.Is.Anything));
        }

        [Test]
        public void Execute_should_open_project()
        {
            openProjectCommand.Execute(progressMonitor);

            projectController.AssertWasCalled(pc => pc.OpenProject(progressMonitor, FileName));
        }
    }
}
