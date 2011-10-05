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
using System.Text;
using NVelocity;
using NVelocity.App;
using Gallio.Runner.Reports;
using NVelocity.Runtime;
using Gallio.Runtime;
using Gallio.Model;
using System.IO;
using Gallio.Runner.Reports.Schema;
using Gallio.Common;
using Commons.Collections;
using Gallio.Model.Schema;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// A wrapper around a test step run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The wrapper holds additional information about the test step run such as
    /// the zero-based index of the test step run in the sequential representation of the tree,
    /// a reference to the parent node, and statistics about the entire child tree.
    /// </para>
    /// </remarks>
    public class TestStepRunNode
    {
        private readonly TestStepRun run;
        private readonly TestStepRunNode parent;
        private readonly List<TestStepRunNode> children = new List<TestStepRunNode>();
        private readonly int index;
        private TestStepRunTreeStatistics statistics;
        private Memoizer<int> memoizerCount;

        /// <summary>
        /// Returns the inner test step run.
        /// </summary>
        public TestStepRun Run
        {
            get
            {
                return run;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public TestStepRunNode Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Gets the zero-based index of the test step run in the sequential representation of the tree.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Gets the statistics about the entire child tree.
        /// </summary>
        public TestStepRunTreeStatistics Statistics
        {
            get
            {
                if (statistics == null)
                    statistics = new TestStepRunTreeStatistics(run);

                return statistics;
            }
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        public List<TestStepRunNode> Children
        {
            get 
            {
                return children;
            }
        }

        /// <summary>
        /// Gets the total number of nodes in the tree.
        /// </summary>
        public int Count
        {
            get
            {
                return memoizerCount.Memoize(() =>
                {
                    int n = 1;

                    for (int i = 0; i < children.Count; i++)
                        n += children[i].Count;

                    return n;
                });
            }
        }

        /// <summary>
        /// Calculates the top position of the node in the navigation sidebar.
        /// </summary>
        public double GetTopPosition(int count)
        {
            return index * 98.0 / count;
        }

        /// <summary>
        /// Returns the child nodes shown in the "Summary" section of the report.
        /// </summary>
        /// <param name="condensed">Indicates whether the report is condensed or not.</param>
        /// <returns>An enumeration of nodes.</returns>
        public IEnumerable<TestStepRunNode> GetSummaryChildren(bool condensed)
        {
            foreach (TestStepRunNode child in children)
            {
                if (!child.Run.Step.IsTestCase && (!condensed || child.Run.Result.Outcome != TestOutcome.Passed))
                    yield return child;
            }
        }

        /// <summary>
        /// Returns the child nodes shown in the "Details" section of the report.
        /// </summary>
        /// <param name="condensed">Indicates whether the report is condensed or not.</param>
        /// <returns>An enumeration of nodes.</returns>
        public IEnumerable<TestStepRunNode> GetDetailsChildren(bool condensed)
        {
            foreach (TestStepRunNode child in children)
            {
                if (!condensed || child.Run.Result.Outcome != TestOutcome.Passed)
                    yield return child;
            }
        }

        /// <summary>
        /// Returns all the nodes visible in the navigator side bar.
        /// </summary>
        /// <returns>An enumeration of nodes.</returns>
        public IEnumerable<TestStepRunNode> GetNavigatorChildren()
        { 
            foreach (TestStepRunNode child in children)
            {
                if ((child.Run.Result.Outcome != TestOutcome.Passed) &&
                    (child.Run.Step.IsTestCase || child.Run.Children.Count == 0))
                    yield return child;

                foreach (TestStepRunNode innerChild in child.GetNavigatorChildren())
                    yield return innerChild;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="run">The test step run.</param>
        /// <param name="parent">The parent node.</param>
        /// <param name="index">The zero-based index of the test step run in the sequential representation of the tree.</param>
        public TestStepRunNode(TestStepRun run, TestStepRunNode parent, int index)
        {
            if (run == null)
                throw new ArgumentNullException("run");

            this.run = run;
            this.parent = parent;
            this.index = index;
        }

        /// <summary>
        /// Builds the tree under the specified root test step run.
        /// </summary>
        /// <param name="root">The root test step run.</param>
        /// <returns></returns>
        public static TestStepRunNode BuildTreeFromRoot(TestStepRun root)
        {
            if (root == null)
            {
                var step = new TestStepData(String.Empty, String.Empty, String.Empty, String.Empty);
                root = new TestStepRun(step);
                root.Result = new TestResult();
            }

            int index = 0;
            var node = new TestStepRunNode(root, null, index++);
            node.Children.AddRange(GetChildren(node, ref index));
            return node;
        }

        private static IList<TestStepRunNode> GetChildren(TestStepRunNode parent, ref int index)
        {
            var list = new List<TestStepRunNode>();

            foreach (TestStepRun run in parent.Run.Children)
            {
                var node = new TestStepRunNode(run, parent, index);

                if (node.Run.Children.Count == 0)
                    index++;

                node.Children.AddRange(GetChildren(node, ref index));
                list.Add(node);
            }

            return list;
        }

        /// <summary>
        /// Enumerates the id's of the current test step and the ancestors'.
        /// </summary>
        /// <returns>An enumeration of step id's.</returns>
        public IEnumerable<string> GetSelfAndAncestorIds()
        {
            yield return run.Step.TestId;
            var p = parent;

            while (p != null)
            {
                yield return p.Run.Step.TestId;
                p = p.Parent;
            }
        }

        /// <summary>
        /// Returns the list of the visible metadata entries for the current test step run.
        /// </summary>
        /// <returns>A sorted list of metadata entries.</returns>
        public IList<string> GetVisibleMetadataEntries()
        {
            var list = new List<string>(GetEnumerableVisibleMetadataEntries());
            list.Sort();
            return list;
        }

        private IEnumerable<string> GetEnumerableVisibleMetadataEntries()
        {
            foreach (var entry in run.Step.Metadata)
            {
                if (entry.Key != MetadataKeys.TestKind)
                {
                    foreach (var value in entry.Value)
                        yield return entry.Key + ": " + value;
                }
            }
        }

        /// <summary>
        /// Returns an attachment in the specified test step run.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to find.</param>
        /// <returns>Attachment data.</returns>
        public AttachmentData FindAttachment(string attachmentName)
        {
            return run.TestLog.Attachments.Find(x => x.Name == attachmentName);
        }

        /// <summary>
        /// Determines whether the test step at the current index is visible on the specified page.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <returns>True if the test step is visible; otherwise, false.</returns>
        public bool IsVisibleInPage(int pageIndex, int pageSize)
        {
            if (index == 0)
                return false;
            if (pageIndex == 0)
                return true;

            int start = 1 + (pageIndex - 1) * pageSize;
            return (index >= start) && (index < start + pageSize);
        }
    }
}
