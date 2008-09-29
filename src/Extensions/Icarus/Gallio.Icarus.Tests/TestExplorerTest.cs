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
