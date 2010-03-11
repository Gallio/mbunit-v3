using System.Windows.Forms;
using Aga.Controls.Tree;

namespace Gallio.Copy.Controls.Tree.NodeControls
{
    public class NodeCheckBox : UI.Tree.NodeControls.NodeCheckBox
    {
        protected override void SetCheckState(TreeNodeAdv node, CheckState value)
        {
            if (node.Tag is FileNode)
                return;

            base.SetCheckState(node, value);
        }
    }
}