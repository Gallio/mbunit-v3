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
using Gallio.UI.DataBinding;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(RestoreFilterCommand))]
    public class RestoreFilterCommandTest
    {
        [Test]
        public void Execute_should_restore_test_filter()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var testFilters = new List<FilterInfo>
            {
                new FilterInfo("SomeFilter", "*"), new FilterInfo("AutoSave", "*")
            };
            projectController.Stub(pc => pc.TestFilters).Return(new Observable<IList<FilterInfo>>(testFilters));
            var restoreFilterCommand = new RestoreFilterCommand(testController, projectController);

            restoreFilterCommand.Execute(MockProgressMonitor.Instance);

            testController.AssertWasCalled(tc => tc.ApplyFilterSet(Arg<FilterSet<ITestDescriptor>>.Is.Anything));
        }
    }
}
