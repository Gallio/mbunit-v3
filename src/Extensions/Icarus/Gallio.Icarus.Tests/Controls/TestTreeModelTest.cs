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

using Gallio.Icarus.Controls;

using MbUnit.Framework;
using Gallio.Model;

namespace Gallio.Icarus.Controls.Tests
{
    [TestFixture]
    public class TestTreeModelTest
    {
        private TestTreeModel testTreeModel;

        [SetUp]
        public void SetUp()
        {
            testTreeModel = new TestTreeModel();
        }

        [Test]
        public void TestCount_Test()
        {
            TestTreeNode node1 = new TestTreeNode("node1", "node1", "node1");
            testTreeModel.Nodes.Add(node1);
            TestTreeNode node2 = new TestTreeNode("node2", "node2", "node2");
            node2.IsChecked = true;
            node2.IsTest = true;
            node1.Nodes.Add(node2);
            Assert.AreEqual(1, testTreeModel.TestCount);
        }
    }
}
