using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Gallio.Copy
{
    internal class PluginTreeView : TreeViewAdv
    {
        public PluginTreeView()
        {
            var nodeCheckBox = new NodeCheckBox
            {
                DataPropertyName = "CheckState",
                LeftMargin = 0,
                ParentColumn = null,
                ThreeState = false
            };
            NodeControls.Add(nodeCheckBox);

            nodeCheckBox.CheckStateChanged += (sender, e) =>
            {
                if (e.Path.LastNode is PluginNode)
                {
                    var node = (PluginNode) e.Path.LastNode;
                    foreach (var child in node.Nodes)
                    {
                        child.CheckState = node.CheckState;
                    }
                }
                else if (e.Path.LastNode is FileNode)
                {
                    var node = (FileNode) e.Path.LastNode;
                    node.CheckState = node.Parent.CheckState;
                }
            };

            var nodeIcon = new NodeIcon
            {
                DataPropertyName = "Image",
                LeftMargin = 1,
                ParentColumn = null
            };
            NodeControls.Add(nodeIcon);

            var nodeTextBox = new NodeTextBox
            {
                DataPropertyName = "Text",
                IncrementalSearchEnabled = true,
                EditEnabled = false,
                LeftMargin = 3,
                ParentColumn = null
            };
            NodeControls.Add(nodeTextBox);
        }
    }
}
