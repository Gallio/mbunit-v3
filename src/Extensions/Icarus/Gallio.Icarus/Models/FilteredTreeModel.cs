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
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Events;
using Gallio.Icarus.Specifications;

namespace Gallio.Icarus.Models
{
    public class FilteredTreeModel : TreeModelDecorator, IFilteredTreeModel, 
        Handles<FilterTreeEvent>
    {
        private ISpecification<TestTreeNode> specification;

        public FilteredTreeModel(ITestTreeModel innerTreeModel) 
            : base(innerTreeModel)
        {
            specification = new AnySpecification<TestTreeNode>();
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (specification is AnySpecification<TestTreeNode>)
            {
                foreach (var child in GetChildrenFromBase(treePath))
                {
                    yield return child;
                }
            }
            else
            {
                foreach (var child in GetFilteredChildren(treePath))
                {
                    yield return child;
                }
            }
        }

        private IEnumerable GetChildrenFromBase(TreePath treePath)
        {
            return base.GetChildren(treePath);
        }

        private IEnumerable GetFilteredChildren(TreePath treePath)
        {
            foreach (var child in GetChildrenFromBase(treePath))
            {
                var node = child as TestTreeNode;
                
                if (node == null)
                    continue;

                if (Matches(node))
                {
                    yield return node;
                }
                else
                {
                    node.CheckState = CheckState.Unchecked;
                }
            }
        }

        private bool Matches(TestTreeNode node)
        {
            return specification.Matches(node) || MatchesChild(node)
                || MatchesParent(node);
        }

        private bool MatchesParent(Node node)
        {
            var parent = node.Parent as TestTreeNode;

            if (parent == null)
                return false;

            return specification.Matches(parent) || MatchesParent(parent);
        }

        private bool MatchesChild(Node node)
        {
            foreach (var child in node.Nodes)
            {
                var testTreeNode = child as TestTreeNode;

                if (testTreeNode == null)
                    continue;

                if (specification.Matches(testTreeNode))
                    return true;

                if (MatchesChild(testTreeNode))
                    return true;
            }

            return false;
        }

        public void Handle(FilterTreeEvent @event)
        {
            specification = @event.Specification;
            OnStructureChanged();
        }
    }
}
