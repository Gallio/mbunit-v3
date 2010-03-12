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
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestNodeTextBox : NodeTextBox<TestTreeNode>
    {
        public Color PassedColor { get; set; }
        public Color FailedColor { get; set; }
        public Color SkippedColor { get; set; }
        public Color InconclusiveColor { get; set; }

        public TestNodeTextBox() 
            : base(ttn => ttn.Text)
        { }

        protected override void OnDrawText(DrawEventArgs args)
        {
            base.OnDrawText(args);

            var node = args.Node.Tag as TestTreeNode;
            if (node == null)
                return;

            switch (node.TestStatus)
            {
                case TestStatus.Passed:
                    args.TextColor = PassedColor;
                    break;
                case TestStatus.Failed:
                    args.TextColor = FailedColor;
                    break;
                case TestStatus.Inconclusive:
                    args.TextColor = InconclusiveColor;
                    break;
                default:
                    args.TextColor = SkippedColor;
                    break;
            }
        }

        protected override bool DrawTextMustBeFired(TreeNodeAdv node)
        {
            return true;
        }
    }
}
