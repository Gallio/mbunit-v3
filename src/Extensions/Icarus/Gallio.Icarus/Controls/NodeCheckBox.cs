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

using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;

namespace Gallio.Icarus.Controls
{
    public class NodeCheckBox : Aga.Controls.Tree.NodeControls.NodeCheckBox
    {
        public NodeCheckBox()
        {
            CheckStateChanged += NodeCheckBox_CheckStateChanged;
        }

        private void NodeCheckBox_CheckStateChanged(object sender, TreePathEventArgs e)
        {
            Parent.BeginUpdate();
            ((TestTreeNode) e.Path.LastNode).UpdateStateOfRelatedNodes();
            Parent.EndUpdate();
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
            if (args.Button != MouseButtons.Left || !IsEditEnabled(args.Node))
                return;
            
            var context = new DrawContext { Bounds = args.ControlBounds };
            var rect = GetBounds(args.Node, context);
            
            if (!rect.Contains(args.ViewLocation))
                return;

            CheckState state = GetCheckState(args.Node);
            state = GetNewState(state);
            SetCheckState(args.Node, state);
            args.Handled = true;
        }

        protected override CheckState GetNewState(CheckState state)
        {
            switch (state)
            {
                case CheckState.Unchecked:
                        return CheckState.Checked;
                default:
                        return CheckState.Unchecked;
            }
        }
    }
}
