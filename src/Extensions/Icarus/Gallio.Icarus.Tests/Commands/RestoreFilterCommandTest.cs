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
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.DataBinding;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RestoreFilterCommand))]
    public class RestoreFilterCommandTest
    {
        private ICommandFactory commandFactory;
        private IProjectController projectController;
        private RestoreFilterCommand command;

        [SetUp]
        public void SetUp()
        {
            projectController = MockRepository.GenerateStub<IProjectController>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            command = new RestoreFilterCommand(projectController, commandFactory);
        }

        [Test]
        public void Execute_should_restore_test_filter()
        {
            const string filterName = "filter";
            StubTestFilters(new List<FilterInfo>
            {
                new FilterInfo("SomeFilter", "*"), 
                new FilterInfo(filterName, "*")
            });
            command.FilterName = filterName;
            var applyFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(fs => fs.CreateApplyFilterCommand(Arg<FilterSet<ITestDescriptor>>.Is.Anything))
                .Return(applyFilterCommand);

            command.Execute(MockProgressMonitor.Instance);

            applyFilterCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        private void StubTestFilters(IList<FilterInfo> filterInfos)
        {
            var testFilters = new Observable<IList<FilterInfo>>(filterInfos);
            projectController.Stub(pc => pc.TestFilters).Return(testFilters);
        }
    }
}
