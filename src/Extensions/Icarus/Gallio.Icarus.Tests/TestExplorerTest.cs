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

using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;

using MbUnit.Framework;

using Rhino.Mocks;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class TestExplorerTest : MockTest
    {
        private IProjectAdapterView projectAdapterView;
        private TestExplorer testExplorer;

        [SetUp]
        public void SetUp()
        {
            projectAdapterView = mocks.CreateMock<IProjectAdapterView>();
            projectAdapterView.ReloadTree();
        }

        [Test]
        public void TreeFilter_Test()
        {
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            string treeFilter = string.Empty;
            Thread thread = new Thread(delegate()
                {
                    treeFilter = testExplorer.TreeFilter;
                });
            thread.Start();
            thread.Join();
            Assert.AreEqual("Namespaces", treeFilter);
        }

        //[Test]
        //public void UpdateTestState_Test()
        //{
        //    mocks.ReplayAll();
        //    testExplorer = new TestExplorer(projectAdapterView);
        //    TestTreeNode node = new TestTreeNode("test", "test", 0);
        //    testExplorer.DataBind(new TreeNode[] { node });
        //    testExplorer.UpdateTestState("test", TestStates.Success);
        //    Assert.AreEqual(TestStates.Success, node.TestState);
        //}
   }
}