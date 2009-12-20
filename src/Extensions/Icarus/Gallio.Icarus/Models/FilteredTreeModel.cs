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

                if (specification.Matches(node))
                    yield return child;
            }
            yield break;
        }

        private IEnumerable GetChildrenFromBase(TreePath treePath)
        {
            return base.GetChildren(treePath);
        }

        public void Handle(FilterTreeEvent message)
        {
            specification = message.Specification;
        }
    }
}
