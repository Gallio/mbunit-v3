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

using Gallio.Icarus.Controls;
using MbUnit.Framework;
using NodeCheckBox = Gallio.Icarus.Controls.NodeCheckBox;

namespace Gallio.Icarus.Tests.Controls
{
    public class TestTreeViewTest
    {
        private TestTreeView testTreeView;

        [SetUp]
        public void SetUp()
        {
            testTreeView = new TestTreeView();
        }

        [Test]
        public void TestTreeView_should_have_four_node_controls()
        {
            Assert.AreEqual(4, testTreeView.NodeControls.Count);
        }

        [Test]
        public void Node_check_box_test()
        {
            var nodeCheckBox = (NodeCheckBox)testTreeView.NodeControls[0];
            Assert.AreEqual("CheckState", nodeCheckBox.DataPropertyName);
            Assert.IsTrue(nodeCheckBox.ThreeState);
        }

        [Test]
        public void Setting_EditEnabled_to_false_should_disable_checkbox()
        {
            var nodeCheckBox = (NodeCheckBox)testTreeView.NodeControls[0];
            Assert.IsTrue(nodeCheckBox.EditEnabled);
            
            testTreeView.SetEditEnabled(false);
            
            Assert.IsFalse(nodeCheckBox.EditEnabled);
        }
    }
}