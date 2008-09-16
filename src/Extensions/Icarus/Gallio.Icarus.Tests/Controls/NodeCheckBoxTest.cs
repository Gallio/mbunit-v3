using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    class NodeCheckBoxTest
    {
        [Test]
        public void MouseDown_RightClick_Test()
        {
            NodeCheckBox nodeCheckBox = new NodeCheckBox();
            nodeCheckBox.MouseDown(new TreeNodeAdvMouseEventArgs(new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0)));
        }
    }
}
