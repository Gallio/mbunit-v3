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

using Gallio.Icarus.Models;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models
{
    public class TestTreeNodeTest
    {
        [Test]
        public void When_a_nodes_test_status_is_updated_its_parent_should_be_too()
        {
            var parent = new TestTreeNode("parent", "parent");
            var child1 = new TestTreeNode("child1", "child1");
            var child2 = new TestTreeNode("child2", "child2");
            parent.Nodes.Add(child1);
            parent.Nodes.Add(child2);

            child1.TestStatus = TestStatus.Passed;

            Assert.AreEqual(TestStatus.Passed, parent.TestStatus);
        }
    }
}
