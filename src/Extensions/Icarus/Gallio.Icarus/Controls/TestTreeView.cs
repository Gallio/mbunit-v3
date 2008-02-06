// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gallio.Icarus.Controls.Enums;

namespace Gallio.Icarus.Controls
{
    public class TestTreeView : TreeView
    {
        private ImageList testStateImages;
        private bool useTriStateCheckBoxes = true;
        private bool filterPassed, filterFailed, filterInconclusive;

        [Browsable(true), Category("Test States")]
        public ImageList TestStateImageList
        {
            get { return testStateImages; }
            set { testStateImages = value; }
        }

        [Category("Appearance"), Description("If enabled the parent checkboxes will indicate the state of children.")]
        [DefaultValue(true), TypeConverter(typeof(bool)), Editor("System.Boolean", typeof(bool))]
        public bool UseTriStateCheckBoxes
        {
            get { return useTriStateCheckBoxes; }
            set { useTriStateCheckBoxes = value; }
        }

        public bool FilterPassed
        {
            set
            {
                filterPassed = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStates.Success);
            }
        }

        public bool FilterFailed
        {
            set
            {
                filterFailed = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStates.Failed);
            }
        }

        public bool FilterInconclusive
        {
            set
            {
                filterInconclusive = value;
                if (value)
                    FilterTree();
                else
                    ClearFilter(TestStates.Inconclusive);
            }
        }

        public TestTreeView()
        {
            testStateImages = new ImageList();
            DrawMode = TreeViewDrawMode.OwnerDrawText;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            BeginUpdate();

            if (UseTriStateCheckBoxes)
            {
                switch (e.Action)
                {
                    case TreeViewAction.ByKeyboard:
                    case TreeViewAction.ByMouse:
                        {
                            // Toggle node
                            TestTreeNode tn = e.Node as TestTreeNode;
                            if (tn != null)
                            {
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

            base.OnAfterCheck(e);

            EndUpdate();
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (e.Node.IsVisible && e.Node is TestTreeNode)
            {
                TestTreeNode node = e.Node as TestTreeNode;

                // Clear the nodes text display area so we have a blank canvas to work from.
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds.X, e.Bounds.Y, (Width - e.Bounds.X),
                                         e.Bounds.Height);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = Font;

                // Setup the text colour, default to WindowText on any non leaf node.
                Brush nodeColor = SystemBrushes.WindowText;
                if ((e.State & TreeNodeStates.Focused) != 0)
                    nodeColor = SystemBrushes.HighlightText;

                else if (node != null)
                {
                    switch (node.TestState)
                    {
                        case TestStates.Failed:
                            nodeColor = new SolidBrush(Color.Red);
                            break;

                        case TestStates.Success:
                            nodeColor = new SolidBrush(Color.Green);
                            break;

                        case TestStates.Inconclusive:
                            nodeColor = new SolidBrush(Color.Yellow);
                            break;

                        case TestStates.Undefined:
                            nodeColor = new SolidBrush(Color.SlateGray);
                            break;
                    }
                }
                
                SizeF textSize = e.Graphics.MeasureString(e.Node.Text, nodeFont);

                // If we do not want to hide the selection, paint it in again.
                if (SelectedNode == e.Node && !HideSelection)
                {
                    nodeColor = SystemBrushes.HighlightText;

                    if (node != null)
                        if (node.TestState != TestStates.Undefined)
                            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X + 17, e.Bounds.Y,
                                                     textSize.Width, e.Bounds.Height);
                        else
                            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X, e.Bounds.Y,
                                                     textSize.Width, e.Bounds.Height);
                }

                // If the test icons have been set and we are at a leaf node, draw the extra image.
                if (node != null)
                    if (testStateImages.Images.Count > 0 && node.TestState != TestStates.Undefined)
                    {
                        // Draw the highlighted background if the node has been selected.
                        if ((e.State & TreeNodeStates.Focused) != 0)
                            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X + 17, e.Bounds.Y,
                                                     textSize.Width, e.Bounds.Height);

                        e.Graphics.DrawImageUnscaled(testStateImages.Images[(int) node.TestState - 1], e.Bounds.X,
                                                     e.Bounds.Y);
                        e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X + 16, e.Bounds.Y + 1);
                    }
                    else
                    {
                        // Draw a regular node, we do not need to extend the highlight area or add any image.
                        if ((e.State & TreeNodeStates.Focused) != 0)
                            e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds.X, e.Bounds.Y,
                                                     textSize.Width, e.Bounds.Height);

                        e.Graphics.DrawString(e.Node.Text, nodeFont, nodeColor, e.Bounds.X, e.Bounds.Y + 1);
                    }
            }
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            TestTreeNode node = e.Node as TestTreeNode;

            // Extend the selectable region of the control by 16px to include the image.
            if ((node != null) &&
                (node.TestState != TestStates.Undefined) &&
                (e.Node.Bounds.Right + 16 >= e.Location.X) &&
                (e.Node.Bounds.Left + 16 <= e.Location.X) &&
                (e.Button == MouseButtons.Left))


                SelectedNode = e.Node;
        }

        protected override void OnNotifyMessage(Message m)
        {
            // filter WM_ERASEBKGND
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SelectedNode = GetNodeAt(e.X, e.Y);
            base.OnMouseDown(e);
        }

        public void Reset(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                TestTreeNode ttnode = node as TestTreeNode;
                if (ttnode != null)
                {
                    ttnode.TestState = TestStates.Undefined;
                    Reset(ttnode.Nodes);
                }
            }
        }

        public void UpdateTestState(string testId, TestStates testState)
        {
            TreeNode[] nodes = Nodes.Find(testId, true);
            foreach (TreeNode n in nodes)
            {
                TestTreeNode node = n as TestTreeNode;
                node.TestState = testState;
                Filter(node);
            }
        }

        public int CountTests(TreeNodeCollection nodes, List<string> dupes)
        {
            int count = 0;
            foreach (TreeNode node in nodes)
            {
                TestTreeNode ttnode = node as TestTreeNode;
                if (ttnode != null)
                {
                    if (ttnode.SelectedImageIndex == 4 && ttnode.Checked && !dupes.Contains(ttnode.Name))
                    {
                        if (Nodes.Find(ttnode.Name, true).Length > 1)
                        {
                            dupes.Add(ttnode.Name);
                        }
                        count++;
                    }
                    count += CountTests(ttnode.Nodes, dupes);
                }
            }
            return count;
        }

        public void FilterTree()
        {
            foreach (TreeNode node in Nodes)
                Filter(node);
        }

        private bool Filter(TreeNode treeNode)
        {
            if (treeNode.Nodes.Count == 0)
            {
                TestTreeNode node = (TestTreeNode)treeNode;
                switch (node.TestState)
                {
                    case TestStates.Success:
                        if (filterPassed)
                        {
                            FilterNode((TestTreeNode)node, "Passed", TestStates.Success, 5);
                            return false;
                        }
                        break;
                    case TestStates.Inconclusive:
                        if (filterInconclusive)
                        {
                            FilterNode((TestTreeNode)node, "Inconclusive", TestStates.Inconclusive, 6);
                            return false;
                        }
                        break;
                    case TestStates.Failed:
                        if (filterFailed)
                        {
                            FilterNode((TestTreeNode)node, "Failed", TestStates.Failed, 7);
                            return false;
                        }
                        break;
                }
            }
            else if (treeNode.Name != TestStates.Success.ToString() && treeNode.Name != TestStates.Failed.ToString() 
                && treeNode.Name != TestStates.Inconclusive.ToString())
            {
                int i = 0;
                while (i < treeNode.Nodes.Count)
                {
                    if (Filter(treeNode.Nodes[i]))
                        i++;
                }
            }
            return true;
        }

        private void FilterNode(TestTreeNode node, string text, TestStates testState, int imgIndex)
        {
            string key = testState.ToString();
            TestTreeNode filterNode;
            if (node.Parent.Nodes.ContainsKey(key))
                filterNode = (TestTreeNode)node.Parent.Nodes.Find(key, false)[0];
            else
            {
                filterNode = new TestTreeNode(text, key, imgIndex, true);
                filterNode.TestState = testState;
                node.Parent.Nodes.Add(filterNode);
            }
            node.Parent.Nodes.Remove(node);
            filterNode.Nodes.Add(node);
        }

        private void ClearFilter(TestStates testState)
        {
            foreach (TreeNode filterNode in Nodes.Find(testState.ToString(), true))
            {
                foreach (TreeNode tn in filterNode.Nodes)
                {
                    filterNode.Nodes.Remove(tn);
                    filterNode.Parent.Nodes.Add(tn);
                }
                filterNode.Parent.Nodes.Remove(filterNode);
            }
        }
    }
}
