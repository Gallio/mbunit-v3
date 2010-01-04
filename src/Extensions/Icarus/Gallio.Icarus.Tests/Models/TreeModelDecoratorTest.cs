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

using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Models
{
    [TestsOn(typeof(TreeModelDecorator))]
    public class TreeModelDecoratorTest
    {
        private ITreeModel treeModel;
        private TestTreeModelDecorator decorator;

        [SetUp]
        public void Establish_context()
        {
            treeModel = MockRepository.GenerateStub<ITreeModel>();
            decorator = new TestTreeModelDecorator(treeModel);            
        }

        [Test]
        public void NodesChanged_event_should_bubble_up()
        {
            var flag = false;
            decorator.NodesChanged += (s, e) => { flag = true; };
            var eventArgs = new TreeModelEventArgs(new TreePath(), new object[0]);

            treeModel.Raise(tm => tm.NodesChanged += null, null, eventArgs);

            Assert.IsTrue(flag);
        }

        [Test]
        public void NodesInserted_event_should_bubble_up()
        {
            var flag = false;
            decorator.NodesInserted += (s, e) => { flag = true; };
            var eventArgs = new TreeModelEventArgs(new TreePath(), new object[0]);

            treeModel.Raise(tm => tm.NodesInserted += null, null, eventArgs);

            Assert.IsTrue(flag);
        }

        [Test]
        public void NodesRemoved_event_should_bubble_up()
        {
            var flag = false;
            decorator.NodesRemoved += (s, e) => { flag = true; };
            var eventArgs = new TreeModelEventArgs(new TreePath(), new object[0]);

            treeModel.Raise(tm => tm.NodesRemoved += null, null, eventArgs);

            Assert.IsTrue(flag);
        }

        [Test]
        public void StructureChanged_event_should_bubble_up()
        {
            var flag = false;
            decorator.StructureChanged += (s, e) => { flag = true; };
            var eventArgs = new TreeModelEventArgs(new TreePath(), new object[0]);

            treeModel.Raise(tm => tm.StructureChanged += null, null, eventArgs);

            Assert.IsTrue(flag);
        }

        [Test]
        public void GetChildren_should_delegate_to_inner_model()
        {
            var treePath = new TreePath();

            decorator.GetChildren(treePath);

            treeModel.AssertWasCalled(tm => tm.GetChildren(treePath));
        }

        [Test]
        public void IsLeaf_should_delegate_to_inner_model()
        {
            var treePath = new TreePath();

            decorator.IsLeaf(treePath);

            treeModel.AssertWasCalled(tm => tm.IsLeaf(treePath));
        }

        [Test]
        public void GetRoot_should_return_first_node_from_inner_model()
        {
            var root = new Node();
            treeModel.Stub(tm => tm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new[] { root });

            var node = decorator.GetRoot();

            Assert.AreEqual(root, node);
        }

        private class TestTreeModelDecorator : TreeModelDecorator
        {
            public TestTreeModelDecorator(ITreeModel innerTreeModel) 
                : base(innerTreeModel)
            { }

            public new Node GetRoot()
            {
                return base.GetRoot();
            }
        }
    }
}
