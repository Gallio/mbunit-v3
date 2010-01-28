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

using System.Linq;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Specifications;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Models
{
    [TestsOn(typeof(FilteredTreeModel))]
    public class FilteredTreeModelTest
    {
        private FilteredTreeModel filteredTreeModel;
        private ITestTreeModel testTreeModel;

        [SetUp]
        public void Establish_context()
        {
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            filteredTreeModel = new FilteredTreeModel(testTreeModel);
        }

        [Test]
        public void Only_children_matching_the_specification_should_be_returned()
        {
            testTreeModel.Stub(ttm => ttm.GetChildren(Arg<TreePath>.Is.Anything))
                .Return(new[]
                {
                    new TestTreeNode("in", "in"),
                    new TestTreeNode("out", "out")
                });
            filteredTreeModel.Handle(new FilterTreeEvent(new TestSpec()));
            
            var children = filteredTreeModel.GetChildren(new TreePath());

            var nodes = children.Cast<TestTreeNode>().ToList();

            Assert.AreEqual(1, nodes.Count);
            Assert.AreEqual("in", nodes[0].Id);
        }

        [Test]
        public void By_default_all_children_should_be_returned()
        {
            var treePath = new TreePath();
            testTreeModel.Stub(ttm => ttm.GetChildren(treePath)).Return(new[]
            {
                new TestTreeNode("in", "in"),
                new TestTreeNode("out", "out")
            });

            var children = filteredTreeModel.GetChildren(treePath);

            var nodes = children.Cast<TestTreeNode>().ToList();

            Assert.AreEqual(2, nodes.Count);
            Assert.AreEqual("in", nodes[0].Id);
            Assert.AreEqual("out", nodes[1].Id);
        }

        private class TestSpec : ISpecification<TestTreeNode>
        {
            public bool Matches(TestTreeNode item)
            {
                return item.Id == "in";
            }
        }
    }
}
