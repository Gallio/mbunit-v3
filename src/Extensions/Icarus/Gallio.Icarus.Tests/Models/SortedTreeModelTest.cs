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
using SortedTreeModel=Gallio.Icarus.Models.SortedTreeModel;
using SortOrder=Gallio.Icarus.Models.SortOrder;

namespace Gallio.Icarus.Tests.Models
{
    [TestsOn(typeof(SortedTreeModel))]
    public class SortedTreeModelTest
    {
        private IFilteredTreeModel filteredTreeModel;
        private SortedTreeModel sortedTreeModel;

        [SetUp]
        public void Establish_context()
        {
            filteredTreeModel = MockRepository.GenerateStub<IFilteredTreeModel>();
            sortedTreeModel = new SortedTreeModel(filteredTreeModel);            
        }

        [Test]
        public void Children_should_be_sorted_ascending_by_default()
        {
            var treePath = new TreePath();
            filteredTreeModel.Stub(ttm => ttm.GetChildren(treePath)).Return(new[]
            {
                new Node("a"), new Node("c"), new Node("d"), new Node("b"), new Node("a")
            });

            var children = sortedTreeModel.GetChildren(treePath);

            Node prev = null;
            foreach (var child in children)
            {
                var node = (Node)child;
                if (prev != null)
                {
                    Assert.GreaterThanOrEqualTo(node.Text, prev.Text);
                }
                prev = node;
            }
        }

        [Test]
        public void Children_should_be_sorted_descending_if_set()
        {
            filteredTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything)).Return(new[]
            {
                new Node("a"), new Node("c"), new Node("d"), new Node("b"), new Node("a")
            });
            sortedTreeModel.Handle(new SortTreeEvent(SortOrder.Descending));

            var children = sortedTreeModel.GetChildren(new TreePath());

            Node prev = null;
            foreach (var child in children)
            {
                var node = (Node)child;
                if (prev != null)
                {
                    Assert.GreaterThanOrEqualTo(prev.Text, node.Text);
                }
                prev = node;
            }
        }

        [Test]
        public void Tree_view_should_be_notified_if_sort_order_is_changed()
        {
            var flag = false;
            sortedTreeModel.StructureChanged += (s, e) => { flag = true; };
            filteredTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new Node[0]);

            sortedTreeModel.Handle(new SortTreeEvent(SortOrder.Descending));

            Assert.IsTrue(flag);
        }

        [Test]
        public void GetChildren_should_return_null_if_inner_model_does()
        {
            Assert.IsNull(sortedTreeModel.GetChildren(new TreePath()));
        }
    }
}
