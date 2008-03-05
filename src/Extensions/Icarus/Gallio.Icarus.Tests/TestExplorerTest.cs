// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
        public void ApplyFilter_Test()
        {
            projectAdapterView.TotalTests = 0;
            LastCall.Repeat.AtLeastOnce();
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            TestTreeNode node = new TestTreeNode("test", "testtesttesttest", 0);
            testExplorer.DataBind(new TreeNode[] { node });
            Filter<ITest> filter = new OrFilter<ITest>(new Filter<ITest>[] { new NoneFilter<ITest>(), 
                new IdFilter<ITest>(new EqualityFilter<string>("testtesttesttest")) });
            testExplorer.ApplyFilter(filter.ToFilterExpr());
            Assert.IsTrue(node.Checked);
        }

        [Test]
        public void CountTests_Test()
        {
            projectAdapterView.TotalTests = 0;
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            testExplorer.CountTests();
        }

        [Test]
        public void ExpandTree_Test()
        {
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            TestTreeNode node = new TestTreeNode("test", "test", 0);
            TestTreeNode child = new TestTreeNode("child", "child", 0);
            child.TestState = TestStates.Success;
            TestTreeNode child2 = new TestTreeNode("child2", "child2", 0);
            child2.TestState = TestStates.Success;
            child.Nodes.Add(child2);
            node.Nodes.Add(child);
            testExplorer.DataBind(new TreeNode[] { node });
            testExplorer.ExpandTree(TestStates.Success);
        }

        [Test]
        public void Reset_Test()
        {
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            testExplorer.Reset();
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

        [Test]
        public void UpdateTestState_Test()
        {
            mocks.ReplayAll();
            testExplorer = new TestExplorer(projectAdapterView);
            TestTreeNode node = new TestTreeNode("test", "test", 0);
            testExplorer.DataBind(new TreeNode[] { node });
            testExplorer.UpdateTestState("test", TestStates.Success);
            Assert.AreEqual(TestStates.Success, node.TestState);
        }
   }
}