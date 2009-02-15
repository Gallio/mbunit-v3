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

using System.Collections.Generic;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Models;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [Category("Views")]
    public class TestResultsTest
    {
        [Test]
        public void Constructor_Test()
        {
            var mediator = MockRepository.GenerateStub<IMediator>();
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(x => x.SelectedTests).Return(
                new System.ComponentModel.BindingList<TestTreeNode>(new List<TestTreeNode>()));
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            optionsController.Stub(x => x.UpdateDelay).Return(1);
            mediator.TestController = testController;
            mediator.OptionsController = optionsController;
            var testResults = new TestResults(mediator);
        }
    }
}
