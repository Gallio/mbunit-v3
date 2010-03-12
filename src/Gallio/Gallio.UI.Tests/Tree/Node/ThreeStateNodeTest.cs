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

using System.Windows.Forms;
using Gallio.UI.Tree.Nodes;
using MbUnit.Framework;

namespace Gallio.UI.Tests.Tree.Node
{
    [Category("Nodes"), TestsOn(typeof(ThreeStateNode))]
    public class ThreeStateNodeTest
    {
        private ThreeStateNode child2;
        private ThreeStateNode parent;
        private ThreeStateNode child1;

        [SetUp]
        public void SetUp()
        {
            parent = new ThreeStateNode();
            child1 = new ThreeStateNode();
            child2 = new ThreeStateNode();
            parent.Nodes.Add(child1);
            parent.Nodes.Add(child2);
        }

        [Test]
        public void If_neither_child_is_checked_then_parent_state_should_be_unchecked()
        {
            child2.CheckState = CheckState.Unchecked;

            Assert.AreEqual(CheckState.Unchecked, parent.CheckState);
        }

        [Test]
        public void If_one_child_is_checked_then_parent_state_should_be_indeterminate()
        {
            child2.CheckState = CheckState.Checked;

            Assert.AreEqual(CheckState.Indeterminate, parent.CheckState);
        }

        [Test]
        public void If_both_children_are_checked_then_parent_state_should_be_checked()
        {
            child1.CheckState = CheckState.Checked;
            child2.CheckState = CheckState.Checked;

            Assert.AreEqual(CheckState.Checked, parent.CheckState);
        }
    }
}


