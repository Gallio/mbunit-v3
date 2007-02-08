using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using MbUnit.GUI.Controls.Enums;

namespace MbUnit.GUI.Controls
{
    public class TestTreeNode : TreeNode
    {
        private CheckBoxState checkState = CheckBoxState.Unchecked;
        private TestState testState = TestState.Undefined;

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
        DefaultValue(CheckBoxState.Unchecked),
        TypeConverter(typeof(CheckBoxState)),
        Editor("MbUnit.GUI.Controls.Enums.CheckBoxState", typeof(CheckBoxState))]
        public CheckBoxState CheckState
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
                        this.Checked = (this.checkState == CheckBoxState.Checked);
                }
            }
        }

        public TestState TestState
        {
            get { return this.testState; }
            set { this.testState = value; }
        }

        /// <summary>
        /// Returns the 'combined' state for all siblings of a node.
        /// </summary>
        private CheckBoxState SiblingsState
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
                CheckBoxState state = 0;
                foreach (TreeNode node in this.Parent.Nodes)
                {
                    TestTreeNode child = node as TestTreeNode;
                    if (child != null)
                        state |= child.CheckState;

                    // If the state is now indeterminate then know there
                    // is a combination of checked and unchecked nodes
                    // and no longer need to continue evaluating the rest
                    // of the sibling nodes.
                    if (state == CheckBoxState.Indeterminate)
                        break;
                }

                return (state == 0) ? CheckBoxState.Unchecked : state;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Manages state changes from one state to the next.
        /// </summary>
        /// <param name="fromState">The state upon which to base the state change.</param>
        public void Toggle(CheckBoxState fromState)
        {
            switch (fromState)
            {
                case CheckBoxState.Unchecked:
                    {
                        this.CheckState = CheckBoxState.Checked;
                        break;
                    }
                case CheckBoxState.Checked:
                case CheckBoxState.Indeterminate:
                default:
                    {
                        this.CheckState = CheckBoxState.Unchecked;
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
                if (this.CheckState != CheckBoxState.Indeterminate)
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
                    child.Checked = (this.CheckState != CheckBoxState.Unchecked);
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
                CheckBoxState state = CheckBoxState.Unchecked;

                // Determine the new state
                if (!isStartingPoint && (this.CheckState == CheckBoxState.Indeterminate))
                    state = CheckBoxState.Indeterminate;
                else
                    state = this.SiblingsState;

                // Update parent state if not the same.
                if (parent.CheckState != state)
                {
                    parent.CheckState = state;
                    parent.Checked = (state != CheckBoxState.Unchecked);
                    parent.UpdateParentNodeState(false);
                }
            }
        }

        #endregion
    }
}
