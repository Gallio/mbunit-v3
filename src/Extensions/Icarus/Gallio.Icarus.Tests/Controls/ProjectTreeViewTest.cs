using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    class ProjectTreeViewTest
    {
        [Test]
        public void Constructor_Test()
        {
            ProjectTreeView projectTreeView = new ProjectTreeView();
            Assert.AreEqual(2, projectTreeView.NodeControls.Count);
            
            NodeIcon nodeIcon = (NodeIcon)projectTreeView.NodeControls[0];
            Assert.AreEqual("Image", nodeIcon.DataPropertyName);
            Assert.AreEqual(1, nodeIcon.LeftMargin);
            Assert.IsNull(nodeIcon.ParentColumn);

            NodeTextBox nodeTextBox = (NodeTextBox) projectTreeView.NodeControls[1];
            Assert.AreEqual("Text", nodeTextBox.DataPropertyName);
            Assert.IsTrue(nodeTextBox.IncrementalSearchEnabled);
            Assert.IsFalse(nodeTextBox.EditEnabled);
            Assert.AreEqual(3, nodeTextBox.LeftMargin);
            Assert.IsNull(nodeTextBox.ParentColumn);
        }
    }
}
