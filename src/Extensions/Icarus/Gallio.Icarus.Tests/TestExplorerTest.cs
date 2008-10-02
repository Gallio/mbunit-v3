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
using Gallio.Icarus.Models;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [MbUnit.Framework.Category("Views")]
    class TestExplorerTest : MockTest
    {
        [Test]
        public void Constructor_Test()
        {
            const string treeViewCategory = "test";
            IProjectController projectController = mocks.CreateMock<IProjectController>();
            ITestController testController = mocks.CreateMock<ITestController>();
            IOptionsController optionsController = mocks.CreateMock<IOptionsController>();
            Expect.Call(optionsController.SelectedTreeViewCategories).Return(
                new BindingList<string>(new List<string>(new[] {treeViewCategory})));
            Expect.Call(testController.Model).Return(new TestTreeModel()).Repeat.AtLeastOnce();
            testController.LoadStarted += null;
            LastCall.IgnoreArguments();
            testController.LoadFinished += null;
            LastCall.IgnoreArguments();
            testController.RunStarted += null;
            LastCall.IgnoreArguments();
            testController.RunFinished += null;
            LastCall.IgnoreArguments();
            testController.TreeViewCategory = treeViewCategory;
            testController.Reload();
            mocks.ReplayAll();
            TestExplorer testExplorer = new TestExplorer(projectController, testController, optionsController);
        }
    }
}
