using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MbUnit.GUI.Controls
{
    public class TestTreeView : TreeView
    {
        private ImageList _testStateImages;

        public TestTreeView()
        {
            _testStateImages = new ImageList();
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
        }


        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Node.IsVisible)
            {
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TestTreeView)this).Font;

                Brush nodeColor = SystemBrushes.WindowText;
                if ((e.State & TreeNodeStates.Focused) != 0)
                    nodeColor = SystemBrushes.HighlightText;
                else if (e.Node.Tag != null)
                    nodeColor = new SolidBrush((Color)e.Node.Tag);

                if (_testStateImages.Images.Count == 2 && e.Node.Nodes.Count == 0)
                {
                    int index = 0;
                    if ((Color)e.Node.Tag == Color.Red)
                        index = 1;

                    //if ((e.State & TreeNodeStates.Focused) != 0)
                    //{
                    //    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X, e.Bounds.Y, e.Bounds.Width + 16, e.Bounds.Height);
                    //}

                    e.Graphics.DrawImageUnscaled(_testStateImages.Images[index], e.Bounds.X, e.Bounds.Y);
                    e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X + 16, e.Bounds.Y + 1);
                }
                else
                {
                    e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X, e.Bounds.Y + 1);
                }
            }
        }

        [Browsable(true)]
        [Category("Test States")]
        public ImageList TestStateImageList
        {
            get { return _testStateImages; }
            set { _testStateImages = value; }
        }
    }
}
