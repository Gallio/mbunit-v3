// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Models.Interfaces;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class TestExplorerTest
    {
        [Test]
        public void Constructor_Test()
        {
            var mediator = MockRepository.GenerateStub<IMediator>();
            var projectController = MockRepository.GenerateStub<IProjectController>();
            projectController.TreeViewCategory = "Namespace";
            mediator.ProjectController = projectController;
            var testController = MockRepository.GenerateStub<ITestController>();
            mediator.TestController = testController;
            testController.Stub(x => x.Model).Return(new TestTreeModel());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            TestExplorer testExplorer = new TestExplorer(mediator, optionsController);
        }
    }
}
