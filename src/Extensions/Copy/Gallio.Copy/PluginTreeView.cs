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
