// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using MbUnit.Icarus.Controls.Enums;

namespace MbUnit.Icarus.Controls
{
    public class TestTreeNode : TreeNode
    {
        private CheckBoxStates checkState = CheckBoxStates.Unchecked;
        private TestStates testState = TestStates.Undefined;

        #region Constructors

        public TestTreeNode()
            : base()
        {

        }

        public TestTreeNode(string text)
            : base(text)
        {

        }

        public TestTreeNode(string text, TestTreeNode[] children)
            : base(text, children)
        {

        }

        public TestTreeNode(string text, int imageIndex, int selectedImageIndex)
            : base(text, imageIndex, selectedImageIndex)
        {

        }

        public TestTreeNode(string text, int imageIndex, int selectedImageIndex, TestTreeNode[] children)
            : base(text, imageIndex, selectedImageIndex, children)
        {

        }

        #endregion

        #region Properties

        [Category("Behaviour"),
        Description("The current state of the node's checkbox, Unchecked, Checked, or Indeterminate"),
        DefaultValue(CheckBoxStates.Unchecked),
        TypeConverter(typeof(CheckBoxStates)),
        Editor("MbUnit.Icarus.Controls.Enums.CheckBoxState", typeof(CheckBoxStates))]
        public CheckBoxStates CheckState
        {
            get { return this.checkState; }
            set
            {
                if (this.checkState != value)
                {
                    this.checkState = value;

                    // Ensure if checkboxes are used to make the checkbox checked or unchecked.
                    // When go to a fully drawn control, this will be managed in the drawing code.
                    // Setting the Checked property in code will cause the OnAfterCheck to be called
                    // and the action will be 'Unknown'; do not handle that case.
                    if ((this.TreeView != null) && (this.TreeView.CheckBoxes))
                        this.Checked = (this.checkState == CheckBoxStates.Checked);
                }
            }
        }

        public TestStates TestState
        {
            get { return this.testState; }
            set { 
                this.testState = value;
                UpdateParentTestState();
            }
        }

        /// <summary>
        /// Returns the 'combined' state for all siblings of a node.
        /// </summary>
        private CheckBoxStates SiblingsState
        {
            get
            {
                // If parent is null, cannot have any siblings or if the parent
                // has only one child (i.e. this node) then return the state of this 
                // instance as the state.
                if ((this.Parent == null) || (this.Parent.Nodes.Count == 1))
                    return this.CheckState;

                // The parent has more than one child.  Walk through parent's child
                // nodes to determine the state of all this node's siblings,
                // including this node.
                CheckBoxStates state = 0;
                foreach (TreeNode node in this.Parent.Nodes)
                {
                    TestTreeNode child = node as TestTreeNode;
                    if (child != null)
                        state |= child.CheckState;

                    // If the state is now indeterminate then know there
                    // is a combination of checked and unchecked nodes
                    // and no longer need to continue evaluating the rest
                    // of the sibling nodes.
                    if (state == CheckBoxStates.Indeterminate)
                        break;
                }

                return (state == 0) ? CheckBoxStates.Unchecked : state;
            }
        }

        private TestStates SiblingTestState
        {
            get
            {
                if ((this.Parent == null) || (this.Parent.Nodes.Count == 1))
                    return this.TestState;

                TestStates testState = TestStates.Undefined;
                foreach (TreeNode node in this.Parent.Nodes)
                {
                    TestTreeNode child = node as TestTreeNode;
                    if (child != null && child.TestState > testState)
                        testState = child.TestState;

                    // Failed is the worst state we can get to, dont bother checking the rest.
                    if (testState == TestStates.Failed)
                        break;
                }

                return testState;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Manages state changes from one state to the next.
        /// </summary>
        /// <param name="fromState">The state upon which to base the state change.</param>
        public void Toggle(CheckBoxStates fromState)
        {
            switch (fromState)
            {
                case CheckBoxStates.Unchecked:
                    {
                        this.CheckState = CheckBoxStates.Checked;
                        break;
                    }
                case CheckBoxStates.Checked:
                case CheckBoxStates.Indeterminate:
                default:
                    {
                        this.CheckState = CheckBoxStates.Unchecked;
                        break;
                    }
            }

            this.UpdateStateOfRelatedNodes();
        }

        /// <summary>
        /// Manages state changes from one state to the next.
        /// </summary>
        public new void Toggle()
        {
            this.Toggle(this.CheckState);
        }

        /// <summary>
        /// Manages updating related child and parent nodes of this instance.
        /// </summary>
        public void UpdateStateOfRelatedNodes()
        {
            TestTreeView tv = this.TreeView as TestTreeView;
            if ((tv != null) && tv.CheckBoxes && tv.UseTriStateCheckBoxes)
            {
                tv.BeginUpdate();

                // If want to cascade checkbox state changes to child nodes of this node and
                // if the current state is not intermediate, update the state of child nodes.
                if (this.CheckState != CheckBoxStates.Indeterminate)
                    this.UpdateChildNodeState();

                this.UpdateParentNodeState(true);

                tv.EndUpdate();
            }
        }

        /// <summary>
        /// Recursiveley update child node's state based on the state of this node.
        /// </summary>
        private void UpdateChildNodeState()
        {
            TestTreeNode child;
            foreach (TreeNode node in this.Nodes)
            {
                // It is possible node is not a ThreeStateTreeNode, so check first.
                if (node is TestTreeNode)
                {
                    child = node as TestTreeNode;
                    child.CheckState = this.CheckState;
                    child.Checked = (this.CheckState != CheckBoxStates.Unchecked);
                    child.UpdateChildNodeState();
                }
            }
        }

        /// <summary>
        /// Recursiveley update parent node state based on the current state of this node.
        /// </summary>
        private void UpdateParentNodeState(bool isStartingPoint)
        {
            // If isStartingPoint is false, then know this is not the initial call
            // to the recursive method as we want to force on the first time
            // this is called to set the instance's parent node state based on
            // the state of all the siblings of this node, including the state
            // of this node.  So, if not the startpoint (!isStartingPoint) and
            // the state of this instance is indeterminate (Enumerations.CheckBoxState.Indeterminate)
            // then know to set all subsequent parents to the indeterminate
            // state.  However, if not in an indeterminate state, then still need
            // to evaluate the state of all the siblings of this node, including the state
            // of this node before setting the state of the parent of this instance.

            TestTreeNode parent = this.Parent as TestTreeNode;
            if (parent != null)
            {
                CheckBoxStates state = CheckBoxStates.Unchecked;

                // Determine the new state
                if (!isStartingPoint && (this.CheckState == CheckBoxStates.Indeterminate))
                    state = CheckBoxStates.Indeterminate;
                else
                    state = this.SiblingsState;

                // Update parent state if not the same.
                if (parent.CheckState != state)
                {
                    parent.CheckState = state;
                    parent.Checked = (state != CheckBoxStates.Unchecked);
                    parent.UpdateParentNodeState(false);
                }
            }
        }

        private void UpdateParentTestState()
        {
            TestTreeView tv = this.TreeView as TestTreeView;
            if (tv != null)
            {
                tv.BeginUpdate();

                TestTreeNode parent = this.Parent as TestTreeNode;
                if (parent != null)
                {
                    TestStates state = this.SiblingTestState;
                    parent.TestState = state;
                }

                tv.EndUpdate();
            }
        }

        #endregion
    }
}
