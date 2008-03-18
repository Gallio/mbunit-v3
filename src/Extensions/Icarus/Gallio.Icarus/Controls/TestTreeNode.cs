// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.Drawing;
using System.Windows.Forms;

using Aga.Controls.Tree;

using Gallio.Model;
using System.Collections.Generic;

namespace Gallio.Icarus.Controls
{
    public class TestTreeNode : Node
    {
        private TestStatus testStatus = TestStatus.Inconclusive;
        private bool sourceCodeAvailable, isTest;
        private string name;
        protected Image nodeTypeIcon, testStatusIcon;

        public string Name
        {
            get { return name; }
        }

        public TestStatus TestStatus
        {
            get { return testStatus; }
            set
            {
                testStatus = value;
                UpdateParentTestStatus();
            }
        }

        public bool SourceCodeAvailable
        {
            get { return sourceCodeAvailable; }
            set { sourceCodeAvailable = value; }
        }

        public bool IsTest
        {
            get { return isTest; }
            set { isTest = value; }
        }

        /// <summary>
        /// Returns the 'combined' state for all siblings of a node.
        /// </summary>
        private CheckState SiblingsState
        {
            get
            {
                // If parent is null, cannot have any siblings or if the parent
                // has only one child (i.e. this node) then return the state of this 
                // instance as the state.
                if (Parent == null || Parent.Nodes.Count == 1)
                    return CheckState;

                // The parent has more than one child.  Walk through parent's child
                // nodes to determine the state of all this node's siblings,
                // including this node.
                foreach (Node node in Parent.Nodes)
                {
                    TestTreeNode child = node as TestTreeNode;
                    if (child != null && CheckState != child.CheckState)
                        return CheckState.Indeterminate;
                }
                return CheckState;
            }
        }

        private TestStatus SiblingTestStatus
        {
            get
            {
                if (Parent == null || Parent.Nodes.Count == 1)
                    return TestStatus;

                TestStatus ts = TestStatus.Inconclusive;
                foreach (Node node in Parent.Nodes)
                {
                    TestTreeNode child = node as TestTreeNode;
                    if (child != null)
                    {
                        if (child.TestStatus == TestStatus.Failed)
                            return TestStatus.Failed;
                        if (child.TestStatus == TestStatus.Skipped)
                            ts = TestStatus.Skipped;
                        if (child.TestStatus == TestStatus.Passed && ts != TestStatus.Skipped)
                            ts = TestStatus.Passed;
                    }
                }
                return ts;
            }
        }

        public Image NodeTypeIcon
        {
            get { return nodeTypeIcon; }
            set
            {
                nodeTypeIcon = value;
                NotifyModel();
            }
        }

        public Image TestStatusIcon
        {
            get { return testStatusIcon; }
            set
            {
                testStatusIcon = value;
                NotifyModel();
            }
        }

        public TestTreeNode(string text, string name)
            : base(text)
        {
            this.name = name;
            CheckState = CheckState.Checked;
        }

        public List<TestTreeNode> Find(string key, bool searchChildren)
        {
            List<TestTreeNode> nodes = new List<TestTreeNode>();

            // always search one level deep...
            foreach (Node n in Nodes)
                nodes.AddRange(Find(key, searchChildren, n));

            return nodes;
        }

        private List<TestTreeNode> Find(string key, bool searchChildren, Node node)
        {
            List<TestTreeNode> nodes = new List<TestTreeNode>();
            if (node is TestTreeNode)
            {
                TestTreeNode ttnode = (TestTreeNode)node;
                if (ttnode.Name == key)
                    nodes.Add(ttnode);

                // continue down the tree if necessary
                if (searchChildren)
                {
                    foreach (Node n in node.Nodes)
                        nodes.AddRange(Find(key, searchChildren, n));
                }
            }
            return nodes;
        }

        /// <summary>
        /// Manages updating related child and parent nodes of this instance.
        /// </summary>
        public void UpdateStateOfRelatedNodes()
        {

            // If want to cascade checkbox state changes to child nodes of this node and
            // if the current state is not intermediate, update the state of child nodes.
            if (CheckState != CheckState.Indeterminate)
                UpdateChildNodeState();

            UpdateParentNodeState(true);
        }

        /// <summary>
        /// Recursively update child node's state based on the state of this node.
        /// </summary>
        private void UpdateChildNodeState()
        {
            foreach (Node node in Nodes)
            {
                // It is possible node is not a ThreeStateTreeNode, so check first.
                TestTreeNode child = node as TestTreeNode;
                if (child != null)
                {
                    child.CheckState = CheckState;
                    child.UpdateChildNodeState();
                }
            }
        }

        /// <summary>
        /// Recursively update parent node state based on the current state of this node.
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
            TestTreeNode parent = Parent as TestTreeNode;
            if (parent != null)
            {
                CheckState state;

                // Determine the new state
                if (!isStartingPoint && (CheckState == CheckState.Indeterminate))
                    state = CheckState.Indeterminate;
                else
                    state = SiblingsState;

                // Update parent state if not the same.
                if (parent.CheckState != state)
                {
                    parent.CheckState = state;
                    parent.UpdateParentNodeState(false);
                }
            }
        }

        private void UpdateParentTestStatus()
        {
            TestTreeNode parent = Parent as TestTreeNode;
            if (parent != null)
                parent.TestStatus = SiblingTestStatus;
        }
    }
}
