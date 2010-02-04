using System.Drawing;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Properties;
using Gallio.Model;

namespace Gallio.Icarus.Controls
{
    public class TestStatusNodeIcon : NodeIcon<TestTreeNode>
    {
        public TestStatusNodeIcon()
            : base(ttn => ttn.TestStatus)
        {
            DataPropertyName = "TestStatus";
        }

        protected override Image GetIcon(TreeNodeAdv node)
        {
            var value = GetValue(node);

            if (value == null)
                return null;
             
            return GetTestStatusIcon((TestStatus)value);
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
