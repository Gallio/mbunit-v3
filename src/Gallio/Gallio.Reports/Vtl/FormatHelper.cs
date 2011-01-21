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

namespace Gallio.Reports.Vtl
{
    internal class FormatHelper
    {
        private readonly IDictionary<string, TestStepRunTreeStatistics> map = new Dictionary<string, TestStepRunTreeStatistics>();

        public string NormalizeEndOfLines(string text)
        {
            return text.Replace("\n", Environment.NewLine);
        }

        public string BreakWord(string text)
        {
            return Regex.Replace(text, @"([\s\\]+)", "$1<wbr/>");
        }

        public string RemoveChars(string text, string chars)
        {
            foreach (char c in chars)
            {
                text = text.Replace(c.ToString(), String.Empty);
            }

            return text;
        }

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

        public IList<TestStepRun> GetVisibleChildren(TestStepRun run, bool condensed)
        {
            return new List<TestStepRun>(GetEnumerableVisibleChildren(run, condensed));
        }

        public IEnumerable<TestStepRun> GetEnumerableVisibleChildren(TestStepRun run, bool condensed)
        {
            foreach (TestStepRun child in run.Children)
            {
                if (!condensed || run.Result.Outcome != TestOutcome.Passed)
                    yield return child;
            }
        }
    }
}
