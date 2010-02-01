using System.Drawing;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Models;
using Gallio.Icarus.Properties;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestStatusNodeIcon : NodeIcon
    {
        public TestStatusNodeIcon()
        {
            DataPropertyName = "TestStatus";
        }

        protected override Image GetIcon(TreeNodeAdv node)
        {
            var treeNode = node.Tag as TestTreeNode;

            if (treeNode == null)
                return null;
             
            return GetTestStatusIcon(treeNode.TestStatus);
        }

        private static Image GetTestStatusIcon(TestStatus status)
        {
            switch (status)
            {
                case TestStatus.Failed:
                    return Resources.cross;
                case TestStatus.Passed:
                    return Resources.tick;
                case TestStatus.Inconclusive:
                    return Resources.error;
            }
            return null;
        }
    }
}
