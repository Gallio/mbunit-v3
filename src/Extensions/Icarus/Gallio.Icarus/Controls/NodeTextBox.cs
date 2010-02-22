using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Common;

namespace Gallio.Icarus.Controls
{
    public class NodeTextBox<T> : NodeTextBox where T : Node
    {
        private readonly Func<T, object> getValue;

        public NodeTextBox(Func<T, object> getValue)
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
