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
using NodeCheckBox = Gallio.Copy.Controls.Tree.NodeControls.NodeCheckBox;
using NodeTextBox = Gallio.Copy.Controls.Tree.NodeControls.NodeTextBox;

namespace Gallio.Copy.Controls.Tree
{
    public class PluginTreeView : TreeViewAdv
    {
        public PluginTreeView()
        {
            var nodeCheckBox = new NodeCheckBox();
            NodeControls.Add(nodeCheckBox);

            var nodeIcon = new NodeIcon
            {
                DataPropertyName = "Image",
            };
            NodeControls.Add(nodeIcon);

            NodeControls.Add(new NodeTextBox());
        }

        public void ExpandPluginList()
        {
            foreach (var node in AllNodes)
            {
                node.Expand();
                return;
            }
        }
    }
}
