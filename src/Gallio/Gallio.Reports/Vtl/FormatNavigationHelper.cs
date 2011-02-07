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
using System.Text.RegularExpressions;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Collections;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Markup;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Provides helper methods to navigate into the test step run tree and find visible items.
    /// </summary>
    internal class FormatNavigationHelper
    {
        private readonly IDictionary<string, TestStepRunTreeStatistics> map = new Dictionary<string, TestStepRunTreeStatistics>();

        /// <summary>
        /// Returns statistics for the entire branch identified by the specified root test step run element.
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public TestStepRunTreeStatistics GetStatistics(TestStepRun run)
        {
            TestStepRunTreeStatistics statistics;

            if (!map.TryGetValue(run.Step.Id, out statistics))
            {
                statistics = new TestStepRunTreeStatistics(run);
                map.Add(run.Step.Id, statistics);
            }

            return statistics;
        }

        /// <summary>
        /// Returns the list of the visible metadata entries for the specified test step run.
        /// </summary>
        /// <param name="run">The test step run.</param>
        /// <returns>A sorted list of metadata entries.</returns>
        public IList<string> GetVisibleMetadataEntries(TestStepRun run)
        {
            var list = new List<string>(GetEnumerableVisibleMetadataEntries(run));
            list.Sort();
            return list;
        }

        private IEnumerable<string> GetEnumerableVisibleMetadataEntries(TestStepRun run)
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
        /// Returns the list of the visible children of the specified test step run.
        /// </summary>
        /// <param name="run">The parent test step run.</param>
        /// <param name="condensed">Indicates whether the current report is condensed.</param>
        /// <returns>A list of child test step runs.</returns>
        public IList<TestStepRun> GetVisibleChildren(TestStepRun run, bool condensed)
        {
            return new List<TestStepRun>(GetEnumerableVisibleChildren(run, condensed));
        }

        private IEnumerable<TestStepRun> GetEnumerableVisibleChildren(TestStepRun run, bool condensed)
        {
            foreach (TestStepRun child in run.Children)
            {
                if (!condensed || run.Result.Outcome != TestOutcome.Passed)
                    yield return child;
            }
        }

        /// <summary>
        /// Returns the value of the specified attribute in a marker tag.
        /// </summary>
        /// <param name="markerTag">The marker tag.</param>
        /// <param name="name">The name of the searched attribute.</param>
        /// <returns>The value of the attribute, or an empty string if not found.</returns>
        public string GetMarkerAttributeValue(MarkerTag markerTag, string name)
        {
            int index = markerTag.Attributes.FindIndex(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return (index < 0) ? String.Empty : markerTag.Attributes[index].Value;
        }

        /// <summary>
        /// Returns an attachment in the specified test step run.
        /// </summary>
        /// <param name="run">The current test step run.</param>
        /// <param name="attachmentName">The name of the attachment to find.</param>
        /// <returns>Attachment data.</returns>
        public AttachmentData FindAttachment(TestStepRun run, string attachmentName)
        {
            return run.TestLog.Attachments.Find(x => x.Name == attachmentName);
        }

        /// <summary>
        /// Returns a list of the visible children for the summary section.
        /// </summary>
        /// <param name="parent">The parent test step run.</param>
        /// <param name="condensed">Indicates whether the current report is condensed.</param>
        /// <returns>A list of child test step runs.</returns>
        public IList<TestStepRun> GetVisibleSummaryChildren(TestStepRun parent, bool condensed)
        {
            return new List<TestStepRun>(GetEnumerableVisibleSummaryChildren(parent, condensed));
        }

        private IEnumerable<TestStepRun> GetEnumerableVisibleSummaryChildren(TestStepRun run, bool condensed)
        {
            foreach (TestStepRun child in run.Children)
            {
                if (!child.Step.IsTestCase && (!condensed || child.Result.Outcome != TestOutcome.Passed))
                    yield return child;
            }
        }

        private readonly IDictionary<string, string> parentMap = new Dictionary<string, string>();

        /// <summary>
        /// Builds a map of the parent step id's.
        /// </summary>
        /// <param name="root">The root test step run of the tree.</param>
        public void BuildParentMap(TestStepRun root)
        {
            parentMap.Clear();
            parentMap.Add(root.Step.Id, null);
            BuildParentMapImpl(root);
        }

        private void BuildParentMapImpl(TestStepRun parent)
        {
            foreach (TestStepRun run in parent.Children)
            {
                parentMap.Add(run.Step.Id, parent.Step.Id);
                BuildParentMapImpl(run);
            }
        }

        /// <summary>
        /// Enumerates the id's of the specified step and the ancestors'.
        /// </summary>
        /// <param name="id">The id of child element.</param>
        /// <returns>An enumeration of step id's.</returns>
        public IList<string> GetSelfAndAncestorIds(string id)
        {
            return new List<string>(GetEnumerableSelfAndAncestorIds(id));
        }

        private IEnumerable<string> GetEnumerableSelfAndAncestorIds(string id)
        {
            while (id != null)
            {
                yield return id;
                id = parentMap[id];
            }
        }

        /// <summary>
        /// A wrapper around a test step run which contains its vertical position in the navigaton sidebar.
        /// </summary>
        public class NavigatorTestStepRun
        {
            public TestStepRun Run { get; set; }
            public double TopPosition { get; set; }
        };

        /// <summary>
        /// Returns a list of the visible children for the navigator side bar.
        /// </summary>
        /// <param name="report">The test report.</param>
        /// <returns>A list of child test step runs.</returns>
        public IList<NavigatorTestStepRun> GetVisibleNavigatorTestStepRuns(Report report)
        {
            var all = new List<TestStepRun>(report.TestPackageRun.AllTestStepRuns);
            var count = all.Count;
            return new List<NavigatorTestStepRun>(GetEnumerableVisibleSummaryChildren(all));
        }

        private IEnumerable<NavigatorTestStepRun> GetEnumerableVisibleSummaryChildren(IList<TestStepRun> all)
        {
            int i = 0;

            foreach (TestStepRun run in all)
            {
                if ((run.Result.Outcome != TestOutcome.Passed) && (run.Step.IsTestCase || run.Children.Count == 0))
                    yield return new NavigatorTestStepRun 
                    { 
                        Run = run,
                        TopPosition = i * 98.0 / all.Count,
                    };

                i++;
            }
        }
    }
}
