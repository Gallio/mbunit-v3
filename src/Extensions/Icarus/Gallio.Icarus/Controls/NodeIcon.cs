using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Common;

namespace Gallio.Icarus.Controls
{
    public class NodeIcon<T> : NodeIcon where T : Node
    {
        private readonly Func<T, object> dataPropertyName;

        public NodeIcon(Func<T, object> dataPropertyName)
        {
            this.dataPropertyName = dataPropertyName;
        }

        public override object GetValue(TreeNodeAdv node)
        {
            if (!(node.Tag is T))
                return null;

            return dataPropertyName((T)node.Tag);
        }
    }
}
