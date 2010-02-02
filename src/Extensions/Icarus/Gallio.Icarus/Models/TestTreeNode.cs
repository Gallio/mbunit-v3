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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Properties;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.Models
{
    public class TestTreeNode : Node, IComparable<TestTreeNode>
    {
        public virtual string Id { get; private set; }

        private TestStatus testStatus = TestStatus.Skipped;
        private readonly List<TestStepRun> testStepRuns = new List<TestStepRun>();

        public virtual string FileName { get; protected set; }

        public TestStatus TestStatus
        {
            get { return testStatus; }
            set
            {
                testStatus = value;
                TestStatusIcon = GetTestStatusIcon(value);
                NotifyModel();
                UpdateParentTestStatus();
            }
        }

        public virtual bool SourceCodeAvailable { get; protected set; }

        public virtual bool IsTest { get; protected set; }

        public virtual string TestKind { get; set; }

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
                foreach (var node in Parent.Nodes)
                {
                    var child = node as TestTreeNode;
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

                var ts = TestStatus.Skipped;
                foreach (var node in Parent.Nodes)
                {
                    var child = node as TestTreeNode;
                    if (child == null)
                        continue;

                    if (child.TestStatus == TestStatus.Failed)
                        return TestStatus.Failed;
                    if (child.TestStatus == TestStatus.Inconclusive)
                        ts = TestStatus.Inconclusive;
                    if (child.TestStatus == TestStatus.Passed && ts != TestStatus.Inconclusive)
                        ts = TestStatus.Passed;
                }
                return ts;
            }
        }

        public Image TestStatusIcon { get; private set; }

        public List<TestStepRun> TestStepRuns
        {
            get { return testStepRuns; }
        }

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

                base.CheckState = value;
                UpdateStateOfRelatedNodes();
            }
        }

        public TestTreeNode(string id, string text)
            : base(text)
        {
            Id = id;
        }

        public List<TestTreeNode> Find(string key, bool searchChildren)
        {
            var nodes = new List<TestTreeNode>();

            if (Id == key)
                nodes.Add(this);

            // always search one level deep...
            foreach (var n in Nodes)
                nodes.AddRange(Find(key, searchChildren, n));

            return nodes;
        }

        private static IEnumerable<TestTreeNode> Find(string key, bool searchChildren, Node node)
        {
            var nodes = new List<TestTreeNode>();
            if (node is TestTreeNode)
            {
                var ttnode = (TestTreeNode)node;

                if (ttnode.Id == key)
                    nodes.Add(ttnode);

                // continue down the tree if necessary
                if (searchChildren)
                    foreach (var n in node.Nodes)
                        nodes.AddRange(Find(key, true, n));
            }
            return nodes;
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
                var child = node as TestTreeNode;

                if (child != null)
                {
                    child.CheckState = checkState;
                }
            }
        }

        /// <summary>
        /// Recursively update parent node state based on the current state of this node.
        /// </summary>
        private void UpdateParentNodeState()
        {
            var parent = Parent as TestTreeNode;

            if (parent == null)
                return;

            var newState = CheckState == CheckState.Indeterminate ? CheckState.Indeterminate 
                : SiblingsState;

            if (parent.CheckState != newState)
                Parent.CheckState = newState;
        }

        private void UpdateParentTestStatus()
        {
            var parent = Parent as TestTreeNode;
            if (parent != null)
                parent.TestStatus = SiblingTestStatus;
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

        public void AddTestStepRun(TestStepRun testStepRun)
        {
            testStepRuns.Add(testStepRun);
            
            // combine test status
            if (testStepRun.Result.Outcome.Status > TestStatus || testStepRun.Step.IsPrimary)
                TestStatus = testStepRun.Result.Outcome.Status;
        }

        public void Reset()
        {
            testStatus = TestStatus.Skipped;
            TestStatusIcon = GetTestStatusIcon(TestStatus.Skipped);

            testStepRuns.Clear();

            foreach (var n in Nodes)
                ((TestTreeNode)n).Reset();
        }

        public int CompareTo(TestTreeNode other)
        {
            return Text.CompareTo(other.Text);
        }
    }
}
