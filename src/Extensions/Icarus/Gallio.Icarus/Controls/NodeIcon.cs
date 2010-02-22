using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Common;

namespace Gallio.Icarus.Controls
{
    public class NodeIcon<T> : NodeIcon where T : Node
    {
        private readonly Func<T, object> getValue;

        public NodeIcon(Func<T, object> getValue)
        {
            this.getValue = getValue;
        }

        public override object GetValue(TreeNodeAdv node)
        {
            var tag = node.Tag as T;

            if (tag == null)
                return null;

            return getValue(tag);
        }
    }
}
