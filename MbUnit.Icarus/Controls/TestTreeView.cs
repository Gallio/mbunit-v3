using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using MbUnit.GUI.Controls.Enums;

namespace MbUnit.GUI.Controls
{
    public class TestTreeView : TreeView
    {
        private ImageList testStateImages;
        private bool useTriStateCheckBoxes = true;

        public TestTreeView()
        {
            this.testStateImages = new ImageList();
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        #region Overrides

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            this.BeginUpdate();

            base.OnAfterCheck(e);

            if (this.UseTriStateCheckBoxes)
            {
                switch (e.Action)
                {
                    case TreeViewAction.ByKeyboard:
                    case TreeViewAction.ByMouse:
                        {
                            if (e.Node is TestTreeNode)
                            {
                                // Toggle to the next state.
                                TestTreeNode tn = e.Node as TestTreeNode;
                                tn.Toggle();
                            }

                            break;
                        }
                    case TreeViewAction.Collapse:
                    case TreeViewAction.Expand:
                    case TreeViewAction.Unknown:
                    default:
                        {
                            // Do nothing.
                            break;

                        }
                }
            }

            this.EndUpdate();
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {

            if (e.Node.IsVisible && e.Node is TestTreeNode)
            {
                TestTreeNode node = e.Node as TestTreeNode;

                // Clear the nodes text display area so we have a blank canvas to work from.
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), e.Bounds.X, e.Bounds.Y, (this.Width - e.Bounds.X), e.Bounds.Height);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TestTreeView)this).Font;

                // Setup the text colour, default to WindowText on any non leaf node.
                Brush nodeColor = SystemBrushes.WindowText;
                if ((e.State & TreeNodeStates.Focused) != 0)
                    nodeColor = SystemBrushes.HighlightText;

                else if (node.TestState == TestState.Failure)
                    nodeColor = new SolidBrush(Color.Red);

                else if (node.TestState == TestState.Success)
                    nodeColor = new SolidBrush(Color.Green);

                // If we do not want to hide the selection, paint it in again.
                if (this.SelectedNode == e.Node && !this.HideSelection)
                {
                    nodeColor = SystemBrushes.HighlightText;

                    if (e.Node.Nodes.Count == 0 && node.TestState != TestState.Undefined)
                        e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X + 17, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    else
                        e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                }

                // If the test icons have been set and we are at a leaf node, draw the extra image.
                if (this.testStateImages.Images.Count == 2 && e.Node.Nodes.Count == 0)
                {
                    // Set the node image to the failed icon.
                    int index = 0;
                    if (node.TestState == TestState.Failure) 
                        index = 1;

                    // Draw the highlighted background if the node has been selected.
                    if ((e.State & TreeNodeStates.Focused) != 0)
                        e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X + 17, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

                    e.Graphics.DrawImageUnscaled(this.testStateImages.Images[index], e.Bounds.X, e.Bounds.Y);
                    e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X + 16, e.Bounds.Y + 1);
                }
                else
                {
                    // Draw a regular node, we do not need to extend the highlight area or add any image.
                    if ((e.State & TreeNodeStates.Focused) != 0)
                        e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

                    e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X, e.Bounds.Y + 1);
                }
            }
        }


        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            // Extend the selectable region of the control by 16px to include the image.
            // Only leaf level nodes can have the extra icon so only apply the new size to them.
            if ((e.Node.Nodes.Count == 0) && 
                (e.Node.Bounds.Right + 16 >= e.Location.X) && 
                (e.Node.Bounds.Left + 16 <= e.Location.X) && 
                (e.Button == MouseButtons.Left))


                this.SelectedNode = e.Node;
        }

        #endregion

        #region Public Properties

        [Browsable(true),
        Category("Test States")]
        public ImageList TestStateImageList
        {
            get { return this.testStateImages; }
            set { this.testStateImages = value; }
        }

        [Category("Appearance"),
        Description("If enabled the parent checkboxes will indicate the state of children."),
        DefaultValue(true),
        TypeConverter(typeof(bool)),
        Editor("System.Boolean", typeof(bool))]
        public bool UseTriStateCheckBoxes
        {
            get { return this.useTriStateCheckBoxes; }
            set { this.useTriStateCheckBoxes = value; }
        }

        #endregion
    }
}
