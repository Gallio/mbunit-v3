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

using System.Collections;
using Aga.Controls.Tree;
using Gallio.Icarus.Events;
using Gallio.Icarus.Specifications;

namespace Gallio.Icarus.Models
{
    public class FilteredTreeModel : TreeModelDecorator, IFilteredTreeModel, Handles<FilterTreeEvent>
    {
        private ISpecification<TestTreeNode> specification;

        public FilteredTreeModel(ITestTreeModel innerTreeModel) 
            : base(innerTreeModel)
        {
            specification = new AnySpecification<TestTreeNode>();
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            foreach (var child in GetChildrenFromBase(treePath))
            {
                var node = child as TestTreeNode;
                
                if (node == null)
                    continue;

                if (Matches(node))
                    yield return child;
            }
            yield break;
        }

        private bool Matches(TestTreeNode node)
        {
            if (specification.Matches(node))
                return true;

            foreach (var child in node.Nodes)
            {
                var testTreeNode = child as TestTreeNode;

                if (testTreeNode == null)
                    continue;

                if (Matches(testTreeNode))
                    return true;
            }

            return false;
        }

        private IEnumerable GetChildrenFromBase(TreePath treePath)
        {
            return base.GetChildren(treePath);
        }

        public void Handle(FilterTreeEvent @event)
        {
            specification = @event.Specification;
        }
    }
}
