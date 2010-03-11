// Copyright 2005-2010 Gallio Project - http://www.gallio.org/

// Portions Copyright 2000-2004 Jonathan de Halleux

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
using System.Windows.Forms;
using Aga.Controls.Tree;

namespace Gallio.UI.Tree.Nodes
{
    ///<summary>
    /// A three-state node.
    ///</summary>
    [CLSCompliant(false)]
    public class ThreeStateNode : Node
    {
        ///<summary>
        /// The check state of the node
        ///</summary>
        public override CheckState CheckState
        {
            get
            {
                return base.CheckState;
            }
            set
            {
                if (base.CheckState == value)
                    return;

                SetCheckState(value);
            }
        }

        private void SetCheckState(CheckState checkState) {
            base.CheckState = checkState;
            UpdateStateOfRelatedNodes();
        }

        ///<summary>
        /// Default ctor.
        ///</summary>
        public ThreeStateNode() { }

        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="text">The text to display.</param>
        public ThreeStateNode(string text)
            : base(text)
        { }

        /// <summary>
        /// Returns the 'combined' state for all siblings of a node.
        /// </summary>
        private CheckState GetSiblingsState()
        {
            if (HasNoSiblings())
            {
                return CheckState;
            }

            foreach (var node in Parent.Nodes)
            {
                var child = node as ThreeStateNode;

                if (child != null && CheckState != child.CheckState)
                {
                    return CheckState.Indeterminate;
                }
            }
            return CheckState;
        }

        private bool HasNoSiblings()
        {
            return Parent == null || Parent.Nodes.Count == 1;
        }

        /// <summary>
        /// Manages updating related child and parent nodes of this instance.
        /// </summary>
        private void UpdateStateOfRelatedNodes()
        {
            if (CheckState != CheckState.Indeterminate)
            {
                UpdateChildNodeState();
            }
            UpdateParentNodeState();
        }

        /// <summary>
        /// Recursively update child node's state based on the state of this node.
        /// </summary>
        private void UpdateChildNodeState()
        {
            var checkState = CheckState;

            foreach (var node in Nodes)
            {
                var child = node as ThreeStateNode;

                if (child != null && ChildShouldBeChecked(child, checkState))
                {
                    child.CheckState = checkState;
                }
            }
        }

        /// <summary>
        /// Allows inheriting classes to block cascading check state.
        /// </summary>
        /// <param name="child">The child to evaluate.</param>
        /// <param name="checkState">The proposed check state.</param>
        /// <returns>True if the check state can be cascaded, otherwise false.</returns>
        protected virtual bool ChildShouldBeChecked(ThreeStateNode child, 
            CheckState checkState)
        {
            return true;
        }

        /// <summary>
        /// Recursively update parent node state based on the current state of this node.
        /// </summary>
        private void UpdateParentNodeState()
        {
            var parent = Parent as ThreeStateNode;

            if (parent == null)
                return;

            var newState = CheckState == CheckState.Indeterminate ? CheckState.Indeterminate 
                               : GetSiblingsState();

            if (parent.CheckState != newState)
                Parent.CheckState = newState;
        }
    }
}
