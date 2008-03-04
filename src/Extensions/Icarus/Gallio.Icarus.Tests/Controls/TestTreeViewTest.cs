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

using System.Windows.Forms;

using Gallio.Icarus.Controls;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture]
    public class TestTreeViewTest
    {
        private TestTreeView testTreeView;
        private TestTreeNode node1;
        private TestTreeNode node2;
        private TestTreeNode node3;
        private TestTreeNode node4;
        private TestTreeNode node5;
        private TestTreeNode node6;

        [SetUp]
        public void SetUp()
        {
            testTreeView = new TestTreeView();
            node1 = new TestTreeNode("node1", "node1", 0, true);
            testTreeView.Nodes.Add(node1);
            node2 = new TestTreeNode("node2", "node2", 0, true);
            node1.Nodes.Add(node2);
            node3 = new TestTreeNode("node3", "node3", 0, true);
            node3.IsTest = true;
            node4 = new TestTreeNode("node4", "node4", 0, true);
            node4.IsTest = true;
            node5 = new TestTreeNode("node5", "node5", 0, true);
            node5.IsTest = true;
            node6 = new TestTreeNode("node6", "node6", 0, true);
            node6.IsTest = true;
            node2.Nodes.AddRange(new TreeNode[] { node3, node4, node5, node6 });
        }

        [Test]
        public void UpdateTestState_Test()
        {
            Assert.AreEqual(TestStates.Undefined, node1.TestState);
            testTreeView.UpdateTestState("node4", TestStates.Success);
            Assert.AreEqual(TestStates.Success, node1.TestState);
        }

        [Test]
        public void TestStateImageList_Test()
        {
            ImageList imageList = new ImageList();
            testTreeView.TestStateImageList = imageList;
            Assert.AreEqual(imageList, testTreeView.TestStateImageList);
        }

        [Test]
        public void UseTriStateCheckBoxes_Test()
        {
            Assert.IsTrue(testTreeView.UseTriStateCheckBoxes);
            testTreeView.UseTriStateCheckBoxes = false;
            Assert.IsFalse(testTreeView.UseTriStateCheckBoxes);
        }

        [Test]
        public void Reset_Test()
        {
            Assert.AreEqual(TestStates.Undefined, node1.TestState);
            node4.TestState = TestStates.Success;
            Assert.AreEqual(TestStates.Success, node1.TestState);
            testTreeView.Reset();
            Assert.AreEqual(TestStates.Undefined, node1.TestState);
        }

        [Test]
        public void CountTests_Test()
        {
            Assert.AreEqual(4, testTreeView.CountTests());
            node4.Checked = false;
            Assert.AreEqual(3, testTreeView.CountTests());
        }

        [Test]
        public void Filter_Test()
        {
            node3.TestState = TestStates.Success;
            node4.TestState = TestStates.Success;
            node5.TestState = TestStates.Inconclusive;
            node6.TestState = TestStates.Failed;
            testTreeView.FilterPassed = true;
            TreeNode[] filterNodes = testTreeView.Nodes.Find(TestStates.Success.ToString(), true);
            Assert.AreEqual(1, filterNodes.Length);
            TestTreeNode node = (TestTreeNode)filterNodes[0];
            Assert.AreEqual(2, node.Nodes.Count);
            testTreeView.FilterFailed = true;
            filterNodes = testTreeView.Nodes.Find(TestStates.Failed.ToString(), true);
            Assert.AreEqual(1, filterNodes.Length);
            node = (TestTreeNode)filterNodes[0];
            Assert.AreEqual(1, node.Nodes.Count);
            testTreeView.FilterInconclusive = true;
            filterNodes = testTreeView.Nodes.Find(TestStates.Inconclusive.ToString(), true);
            Assert.AreEqual(1, filterNodes.Length);
            node = (TestTreeNode)filterNodes[0];
            Assert.AreEqual(1, node.Nodes.Count);
            testTreeView.FilterPassed = false;
            filterNodes = testTreeView.Nodes.Find(TestStates.Success.ToString(), true);
            Assert.AreEqual(0, filterNodes.Length);
            testTreeView.FilterFailed = false;
            filterNodes = testTreeView.Nodes.Find(TestStates.Failed.ToString(), true);
            Assert.AreEqual(0, filterNodes.Length);
            testTreeView.FilterInconclusive = false;
            filterNodes = testTreeView.Nodes.Find(TestStates.Inconclusive.ToString(), true);
            Assert.AreEqual(0, filterNodes.Length);
        }
    }
}
