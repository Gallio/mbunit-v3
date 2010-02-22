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
using Aga.Controls.Tree;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.Models
{
    public class TestTreeNode : ThreeStateNode, IComparable<TestTreeNode>
    {
        private TestStatus? testStatus;
        private readonly List<TestStepRun> testStepRuns = new List<TestStepRun>();

        public virtual string Id { get; private set; }

        public TestStatus? TestStatus
        {
            get
            {
                return testStatus;
            }
            set
            {
                testStatus = value;
                NotifyModel();
                UpdateParentTestStatus();
            }
        }

        public virtual string TestKind { get; set; }

        private TestStatus? GetSiblingTestStatus()
        {
            if (NodeHasNoSiblings())
                return TestStatus;

            TestStatus? siblingTestStatus = null;
            
            foreach (var node in Parent.Nodes)
            {
                siblingTestStatus = EvaluateTestStatus(node, siblingTestStatus);
            }
            return siblingTestStatus;
        }

        private static TestStatus? EvaluateTestStatus(Node node, TestStatus? siblingTestStatus)
        {
            var child = node as TestTreeNode;
            
            if (child == null || child.TestStatus == null)
                return siblingTestStatus;

            if (child.TestStatus > siblingTestStatus || siblingTestStatus == null)
                siblingTestStatus = child.TestStatus.Value;

            return siblingTestStatus;
        }

        private bool NodeHasNoSiblings()
        {
            return Parent == null || Parent.Nodes.Count == 1;
        }

        public List<TestStepRun> TestStepRuns
        {
            get { return testStepRuns; }
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

        private void UpdateParentTestStatus()
        {
            var parent = Parent as TestTreeNode;
            if (parent != null)
                parent.TestStatus = GetSiblingTestStatus();
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
            TestStatus = null;

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
