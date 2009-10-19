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

using Gallio.Icarus.Controls;
using MbUnit.Framework;
using Aga.Controls.Tree.NodeControls;
using NodeCheckBox=Gallio.Icarus.Controls.NodeCheckBox;

namespace Gallio.Icarus.Tests.Controls
{
    public class TestTreeViewTest
    {
        [Test]
        public void Constructor_Test()
        {
            TestTreeView testTreeView = new TestTreeView();
            Assert.AreEqual(4, testTreeView.NodeControls.Count);

            NodeCheckBox nodeCheckBox = (NodeCheckBox)testTreeView.NodeControls[0];
            Assert.AreEqual("CheckState", nodeCheckBox.DataPropertyName);
            Assert.AreEqual(0, nodeCheckBox.LeftMargin);
            Assert.IsNull(nodeCheckBox.ParentColumn);
            Assert.IsTrue(nodeCheckBox.ThreeState);

            NodeIcon nodeTypeIcon = (NodeIcon) testTreeView.NodeControls[1];
            Assert.AreEqual("NodeTypeIcon", nodeTypeIcon.DataPropertyName);
            Assert.AreEqual(1, nodeTypeIcon.LeftMargin);
            Assert.IsNull(nodeTypeIcon.ParentColumn);

            NodeIcon testStateIcon = (NodeIcon) testTreeView.NodeControls[2];
            Assert.AreEqual("TestStatusIcon", testStateIcon.DataPropertyName);
            Assert.AreEqual(1, testStateIcon.LeftMargin);
            Assert.IsNull(testStateIcon.ParentColumn);

            NodeTextBox nodeTextBox = (NodeTextBox) testTreeView.NodeControls[3];
            Assert.AreEqual("Text", nodeTextBox.DataPropertyName);
            Assert.IsTrue(nodeTextBox.IncrementalSearchEnabled);
            Assert.IsFalse(nodeTextBox.EditEnabled);
            Assert.AreEqual(3, nodeTextBox.LeftMargin);
            Assert.IsNull(nodeTextBox.ParentColumn);
        }

        [Test]
        public void EditEnabled_Test()
        {
            TestTreeView testTreeView = new TestTreeView();
            Assert.IsFalse(testTreeView.EditEnabled);
            testTreeView.EditEnabled = true;
            Assert.IsTrue(testTreeView.EditEnabled);
        }
    }
}