// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Icarus.Controls
{
    public class ProjectTreeView : TreeViewAdv
    {
        public ProjectTreeView()
        {
            NodeIcon nodeIcon = new NodeIcon();
            nodeIcon.DataPropertyName = "Image";
            nodeIcon.LeftMargin = 1;
            nodeIcon.ParentColumn = null;
            NodeControls.Add(nodeIcon);

            NodeTextBox nodeTextBox = new NodeTextBox();
            nodeTextBox.DataPropertyName = "Text";
            nodeTextBox.IncrementalSearchEnabled = true;
            nodeTextBox.EditEnabled = false;
            nodeTextBox.LeftMargin = 3;
            nodeTextBox.ParentColumn = null;
            NodeControls.Add(nodeTextBox);
        }
    }
}
