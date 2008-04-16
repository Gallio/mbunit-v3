// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System;
using System.Drawing;
using System.Windows.Forms;

using Aga.Controls.Tree;

namespace Gallio.Icarus.Controls
{
    public class NodeCheckBox : Aga.Controls.Tree.NodeControls.NodeCheckBox
    {
        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left && IsEditEnabled(args.Node))
            {
                DrawContext context = new DrawContext();
                context.Bounds = args.ControlBounds;
                Rectangle rect = GetBounds(args.Node, context);
                if (rect.Contains(args.ViewLocation))
                {
                    CheckState state = GetCheckState(args.Node);
                    state = GetNewState(state);
                    Parent.BeginUpdate();
                    SetCheckState(args.Node, state);
                    ((TestTreeNode)args.Node.Tag).UpdateStateOfRelatedNodes();
                    Parent.EndUpdate();
                    ((TestTreeModel)Parent.Model).OnTestCountChanged(EventArgs.Empty);
                    args.Handled = true;
                }
            }
        }
        
        private CheckState GetNewState(CheckState state)
        {
            switch (state)
            {
                case CheckState.Unchecked:
                        return CheckState.Checked;
                case CheckState.Checked:
                case CheckState.Indeterminate:
                default:
                        return CheckState.Unchecked;
            }
        }
    }
}
