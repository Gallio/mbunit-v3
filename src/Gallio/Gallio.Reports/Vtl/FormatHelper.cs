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
    internal class FormatHelper
    {
        private readonly IDictionary<string, TestStepRunTreeStatistics> map = new Dictionary<string, TestStepRunTreeStatistics>();

        public string NormalizeEndOfLinesText(string text)
        {
            return text.Replace("\n", "\r\n");
        }

        public string NormalizeEndOfLinesHtml(string text)
        {
            return text
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");
        }

        public string BreakWord(string text)
        {
            var output = new StringBuilder();

            foreach (char c in text)
            {
                switch (c)
                {
                    // Natural word breaks. Always replace spaces by non-breaking spaces followed by word-breaks to ensure that
                    // text can reflow without actually consuming the space.  Without this detail it can happen that spaces that 
                    // are supposed to be highligted (perhaps as part of a marker for a diff) will instead vanish when the text 
                    // reflow occurs, giving a false impression of the content.
                    case ' ':
                        output.Append("&nbsp;<wbr/>");
                        break;

                    // Characters to break before.
                    case '_':
                    case '/':
                    case ';':
                    case ':':
                    case '.':
                    case '\\':
                    case '(':
                    case '{':
                    case '[':
                        output.Append("<wbr/>");
                        output.Append(c);
                        break;

                    // Characters to break after.
                    case '>':
                    case ')':
                    case ']':
                    case '}':
                        output.Append(c);
                        output.Append("<wbr/>");
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
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

        public string GetAttributeValue(MarkerTag markerTag, string name)
        {
            int index = markerTag.Attributes.FindIndex(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return (index < 0) ? String.Empty : markerTag.Attributes[index].Value;
        }

        public AttachmentData FindAttachment(TestStepRun run, string attachmentName)
        {
            return run.TestLog.Attachments.Find(x => x.Name == attachmentName);
        }

        public string PathToUri(string path)
        { 
            return path
                .Replace('\\', '/')
                .Replace("%", "%25")
                .Replace(" ", "%20");
        }

        public string GenerateId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
