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

namespace Gallio.Icarus.Controls.Tests
{
    [TestFixture]
    public class TestTreeNodeTest
    {
        //private TestTreeNode parent;
        //private TestTreeNode testTreeNode;
        //private TestTreeNode child1;
        //private TestTreeNode child2;
        //private TestTreeNode child3;

        //[SetUp]
        //public void SetUp()
        //{
        //    TestTreeView treeView = new TestTreeView();
        //    treeView.CheckBoxes = true;
        //    parent = new TestTreeNode("parent", "parent", 0);
        //    testTreeNode = new TestTreeNode("test", "test", 0);
        //    parent.Nodes.Add(testTreeNode);
        //    child1 = new TestTreeNode("child1", "child1", 0);
        //    child2 = new TestTreeNode("child2", "child2", 0);
        //    testTreeNode.Nodes.AddRange(new TreeNode[] { child1, child2 });
        //    child3 = new TestTreeNode("child3", "child3", 0);
        //    child1.Nodes.Add(child3);
        //    treeView.Nodes.Add(parent);
        //}

        //[Test]
        //public void Toggle_Test()
        //{
        //    Assert.AreEqual(CheckBoxStates.Checked, testTreeNode.CheckState);
        //    child1.Toggle();
        //    Assert.AreEqual(CheckBoxStates.Indeterminate, testTreeNode.CheckState);
        //    child2.Toggle();
        //    Assert.AreEqual(CheckBoxStates.Unchecked, testTreeNode.CheckState);
        //    parent.Toggle();
        //    Assert.AreEqual(CheckBoxStates.Checked, testTreeNode.CheckState);
        //}

        //[Test]
        //public void TestState_Test()
        //{
        //    child3.TestState = TestStates.Success;
        //    Assert.AreEqual(TestStates.Success, parent.TestState);
        //    child2.TestState = TestStates.Failed;
        //    Assert.AreEqual(TestStates.Failed, parent.TestState);
        //}
    }
}
