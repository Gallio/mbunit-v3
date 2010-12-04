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

using System.Drawing;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Copy.Model;

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
