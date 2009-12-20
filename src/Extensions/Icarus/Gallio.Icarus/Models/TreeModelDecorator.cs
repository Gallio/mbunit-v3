using System.Collections;
using Aga.Controls.Tree;

namespace Gallio.Icarus.Models
{
    public abstract class TreeModelDecorator : TreeModelBase
    {
        protected readonly ITreeModel innerTreeModel;

        protected TreeModelDecorator(ITreeModel innerTreeModel)
        {
            this.innerTreeModel = innerTreeModel;

            innerTreeModel.NodesChanged += (sender, e) => OnNodesChanged(e);
            innerTreeModel.NodesInserted += (sender, e) => OnNodesInserted(e);
            innerTreeModel.NodesRemoved += (sender, e) => OnNodesRemoved(e);
            innerTreeModel.StructureChanged += (sender, e) => OnStructureChanged(e);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            return innerTreeModel.GetChildren(treePath);
        }

        public override bool IsLeaf(TreePath treePath)
        {
            return innerTreeModel.IsLeaf(treePath);
        }

        protected Node GetRoot()
        {
            var children = innerTreeModel.GetChildren(new TreePath());
            foreach (var child in children)
            {
                return (Node)child;
            }
            return null;
        }
    }
}
