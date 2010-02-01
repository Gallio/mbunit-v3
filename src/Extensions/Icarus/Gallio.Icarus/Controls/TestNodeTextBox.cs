using System.Drawing;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestNodeTextBox : NodeTextBox
    {
        public Color PassedColor { get; set; }
        public Color FailedColor { get; set; }
        public Color SkippedColor { get; set; }
        public Color InconclusiveColor { get; set; }

        public TestNodeTextBox()
        {
            DataPropertyName = "Text";
        }

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
                case TestStatus.Skipped:
                    args.TextColor = SkippedColor;
                    break;
                case TestStatus.Inconclusive:
                    args.TextColor = InconclusiveColor;
                    break;
            }
        }

        protected override bool DrawTextMustBeFired(TreeNodeAdv node)
        {
            return true;
        }
    }
}
