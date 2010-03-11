using System.Drawing;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Gallio.Copy.Controls.Tree.NodeControls
{
    public class NodeTextBox : Aga.Controls.Tree.NodeControls.NodeTextBox
    {
        public NodeTextBox()
        {
            DataPropertyName = "Text";
            IncrementalSearchEnabled = true;
            EditEnabled = false;
            LeftMargin = 3;
        }

        protected override void OnDrawText(DrawEventArgs args)
        {
            base.OnDrawText(args);

            var node = args.Node.Tag as FileNode;
            if (node == null || node.Exists)
                return;

            args.TextColor = Color.Red;
        }

        protected override bool DrawTextMustBeFired(TreeNodeAdv node)
        {
            return true;
        }
    }
}