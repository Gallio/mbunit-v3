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
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    [TestsOn(typeof(NodeCheckBox)), Category("Controls")]
    public class NodeCheckBoxTest
    {
        private TestNodeCheckBox nodeCheckBox;

        [SetUp]
        public void SetUp()
        {
            nodeCheckBox = new TestNodeCheckBox();
        }

        [Test]
        public void If_check_state_is_unchecked_next_state_should_be_checked()
        {
            var newState = nodeCheckBox.TestGetNewState(CheckState.Unchecked);

            Assert.AreEqual(CheckState.Checked, newState);
        }

        [Test]
        public void If_check_state_is_checked_next_state_should_be_unchecked()
        {
            var newState = nodeCheckBox.TestGetNewState(CheckState.Checked);

            Assert.AreEqual(CheckState.Unchecked, newState);
        }

        [Test]
        public void If_check_state_is_indeterminate_next_state_should_be_unchecked()
        {
            var newState = nodeCheckBox.TestGetNewState(CheckState.Indeterminate);

            Assert.AreEqual(CheckState.Unchecked, newState);
        }

        private class TestNodeCheckBox : NodeCheckBox
        {
            public CheckState TestGetNewState(CheckState checkState)
            {
                return base.GetNewState(checkState);
            }
        }
    }
}
