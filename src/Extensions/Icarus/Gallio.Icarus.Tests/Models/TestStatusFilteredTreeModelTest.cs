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

using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Models
{
    [TestsOn(typeof(TestStatusFilteredTreeModel))]
    public class TestStatusFilteredTreeModelTest
    {
        private IFilteredTreeModel innerTreeModel;
        private TestStatusFilteredTreeModel treeModel;

        [SetUp]
        public void Establish_context()
        {
            innerTreeModel = MockRepository.GenerateStub<IFilteredTreeModel>();
            treeModel = new TestStatusFilteredTreeModel(innerTreeModel);
        }

        [Test]
        public void Test_node_should_not_be_returned_if_status_is_filtered()
        {
            var node = new TestTreeNode("name", "text")
            {
                TestKind = TestKinds.Test,
                TestStatus = TestStatus.Passed
            };
            innerTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new [] { node });
            treeModel.Handle(new FilterTestStatusEvent(TestStatus.Passed));

            var children = treeModel.GetChildren(new TreePath());

            foreach (var child in children)
            {
                var testTreeNode = child as TestTreeNode;

                if (testTreeNode == null)
                    continue;

                if (testTreeNode == node)
                    Assert.Fail("Node should have been filtered.");
            }
        }

        [Test]
        public void Test_node_should_be_returned_if_status_is_not_filtered()
        {
            var node = new TestTreeNode("name", "text")
            {
                TestKind = TestKinds.Test,
                TestStatus = TestStatus.Failed
            };
            innerTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new[] { node });
            treeModel.Handle(new FilterTestStatusEvent(TestStatus.Passed));

            var children = treeModel.GetChildren(new TreePath());

            bool foundNode = false;
            foreach (var child in children)
            {
                var testTreeNode = child as TestTreeNode;

                if (testTreeNode == null)
                    continue;

                if (testTreeNode != node) 
                    continue;
                
                foundNode = true;
                break;
            }
            Assert.IsTrue(foundNode);
        }

        [Test]
        public void Node_should_be_returned_if_status_is_filtered_but_not_a_test()
        {
            var node = new TestTreeNode("name", "text")
            {
                TestKind = TestKinds.Fixture,
                TestStatus = TestStatus.Passed
            };
            innerTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new[] { node });
            treeModel.Handle(new FilterTestStatusEvent(TestStatus.Passed));

            var children = treeModel.GetChildren(new TreePath());

            bool foundNode = false;
            foreach (var child in children)
            {
                var testTreeNode = child as TestTreeNode;

                if (testTreeNode == null)
                    continue;

                if (testTreeNode != node)
                    continue;

                foundNode = true;
                break;
            }
            Assert.IsTrue(foundNode);
        }
    }
}
