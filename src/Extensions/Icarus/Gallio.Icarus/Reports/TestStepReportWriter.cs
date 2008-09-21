// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus.Reports
{
    public class TestStepReportWriter
    {
        readonly XmlTextWriter xmlTextWriter;
        readonly string reportFolder, cssDir, imgDir;
        readonly TestModelData testModelData;

        public TestStepReportWriter(XmlTextWriter xmlTextWriter, string reportFolder, TestModelData testModelData)
        {
            this.xmlTextWriter = xmlTextWriter;
            this.reportFolder = reportFolder;
            this.testModelData = testModelData;

            cssDir = Path.Combine(reportFolder, "css");
            imgDir = Path.Combine(reportFolder, "img");
        }

        internal void RenderReport(IList<string> testIds, TestPackageRun testPackageRun)
        {
            RenderHeader();
            xmlTextWriter.WriteRaw("<div id=\"Content\" class=\"content\">");
            RenderNavigator(testPackageRun.Statistics, testPackageRun.AllTestStepRuns);
            xmlTextWriter.WriteRaw("<div id=\"Details\" class=\"section\"><div class=\"section-content\"><ul class=\"testStepRunContainer\">");
            foreach (TestStepRun testStepRun in testPackageRun.AllTestStepRuns)
            {
                if (testIds.Contains(testStepRun.Step.TestId))
                    RenderTestStepRun(testStepRun, 1);
            }
            xmlTextWriter.WriteRaw("</ul></div></div></div></body></html>");
            xmlTextWriter.Flush();
        }

        static int CountTestStepRuns(IEnumerable<TestStepRun> testStepRuns)
        {
            int i = 0;
            foreach (TestStepRun testStepRun in testStepRuns)
            {
                i++;
                i += CountTestStepRuns(testStepRun.Children);
            }
            return i;
        }

        void RenderHeader()
        {
            xmlTextWriter.WriteStartDocument();
            //xmlTextWriter.WriteRaw("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" ");
            //xmlTextWriter.WriteRaw("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            xmlTextWriter.WriteRaw("<html xml:lang=\"en\" lang=\"en\" dir=\"ltr\"><head>");
            xmlTextWriter.WriteRaw("<title>Execution Log</title>");
            xmlTextWriter.WriteRaw(String.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\\Gallio-Report.css\" />", cssDir));
            xmlTextWriter.WriteRaw("</head><body class=\"gallio-report\" style=\"overflow: auto;\">");
        }

        void RenderNavigator(Runner.Reports.Statistics statistics, IEnumerable<TestStepRun> testStepRuns)
        {
            xmlTextWriter.WriteRaw("<div id=\"Navigator\" class=\"navigator\">");
            string summary = string.Format("{0} run, {1} passed, {2} failed, {3} inconclusive, {4} skipped", statistics.RunCount,
                FormatStatisticsCategoryCounts(statistics, TestStatus.Passed), FormatStatisticsCategoryCounts(statistics, TestStatus.Failed), 
                FormatStatisticsCategoryCounts(statistics, TestStatus.Inconclusive), FormatStatisticsCategoryCounts(statistics, TestStatus.Skipped));
            xmlTextWriter.WriteRaw(string.Format("<a href=\"#\" title=\"{0}\" class=\"navigator-box {1}\"></a>", summary, StatusFromStatistics(statistics)));
            xmlTextWriter.WriteRaw("<div class=\"navigator-stripes\">");
            int i = 0;
            int count = CountTestStepRuns(testStepRuns);
            foreach (TestStepRun testStepRun in testStepRuns)
            {
                float position = i * 98 / count + 1;
                i++;

                if (testStepRun.Result.Outcome.Status == TestStatus.Passed ||
                    (!testStepRun.Step.IsTestCase && testStepRun.Children.Count != 0))
                    continue;
                
                xmlTextWriter.WriteRaw(string.Format("<a href=\"#testStepRun-{0}\" style=\"top: {1}%\"", testStepRun.Step.Id, position));
                string status = Enum.GetName(typeof (TestStatus), testStepRun.Result.Outcome.Status).ToLower();
                xmlTextWriter.WriteRaw(string.Format(" class=\"status-{0}\" title=\"{1} {0}\"></a>", status, testStepRun.Step.Name));
            }
            xmlTextWriter.WriteRaw("</div></div>");
        }

        static string FormatStatisticsCategoryCounts(Runner.Reports.Statistics statistics, TestStatus status)
        {
            List<TestOutcomeSummary> testOutcomeSummaries = new List<TestOutcomeSummary>();
            foreach (TestOutcomeSummary tos in statistics.OutcomeSummaries)
            {
                if (tos.Outcome.Status == status && tos.Outcome.Category != null)
                    testOutcomeSummaries.Add(tos);
            }
            
            if (testOutcomeSummaries.Count == 0)
                return string.Empty;

            testOutcomeSummaries.Sort(delegate(TestOutcomeSummary left, TestOutcomeSummary right)
            {
                return left.Outcome.Category.CompareTo(right.Outcome.Category);
            });
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" (");
            foreach (TestOutcomeSummary testOutcomeSummary in testOutcomeSummaries)
            {
                stringBuilder.Append(testOutcomeSummary.Count);
                stringBuilder.Append(" ");
                stringBuilder.Append(testOutcomeSummary.Outcome.Category);
                stringBuilder.Append(",");
            }
            
            // remove trailing comma (if necessary)
            if (stringBuilder.Length > 0)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(")");
            return stringBuilder.ToString();
        }

        static string StatusFromStatistics(Runner.Reports.Statistics statistics)
        {
            if (statistics.FailedCount > 0)
                return "status-failed";
            if (statistics.InconclusiveCount > 0)
                return "status-inconclusive";
            return statistics.PassedCount > 0 ? "status-passed" : "status-skipped";
        }

        void RenderTestStepRun(TestStepRun testStepRun, int nestingLevel)
        {
            TestData testData = testModelData.GetTestById(testStepRun.Step.TestId);
            if (testData == null)
                throw new InvalidOperationException("The step referenced an unknown test.");

            Statistics statistics = CalculateStatistics(testStepRun);

            xmlTextWriter.WriteRaw(string.Format("<li id=\"testStepRun-{0}\">", testStepRun.Step.Id));
            xmlTextWriter.WriteRaw(string.Format("<span class=\"testStepRunHeading testStepRunHeading-Level{0}\"><b>{1}</b>", 
                nestingLevel, PrintTextWithBreaks(testStepRun.Step.Name)));
            RenderOutcomeBar(testStepRun.Result.Outcome, statistics, (testStepRun.Children.Count == 0));
            xmlTextWriter.WriteRaw("</span>");
            
            // stat panel
            xmlTextWriter.WriteRaw(string.Format("<div id=\"detailPanel-{0}\" class=\"panel\">", testStepRun.Step.Id));
            string testKind = testData.Metadata.GetValue(MetadataKeys.TestKind);
            if (testKind == TestKinds.Assembly || testKind == TestKinds.Framework)
            {
                xmlTextWriter.WriteRaw("<table class=\"statistics-table\"><tr class=\"alternate-row\">");
                xmlTextWriter.WriteRaw("<td class=\"statistics-label-cell\">Results:</td><td>");
                xmlTextWriter.WriteRaw(FormatStatistics(statistics));
                xmlTextWriter.WriteRaw("</td></tr><tr><td class=\"statistics-label-cell\">Duration:</td><td>");
                xmlTextWriter.WriteRaw(String.Format("{0}s", statistics.duration));
                xmlTextWriter.WriteRaw(String.Format("</td></tr><tr class=\"alternate-row\"><td class=\"statistics-label-cell\">Assertions:</td><td>{0}", 
                    statistics.assertCount));
                xmlTextWriter.WriteRaw("</td></tr></table>");
            }
            else
                xmlTextWriter.WriteRaw(String.Format("Duration: {0}s, Assertions: {1}.", statistics.duration, statistics.assertCount));
            
            // metadata
            RenderMetadata(testStepRun, testData);

            // execution logs
            xmlTextWriter.WriteRaw("<div class=\"testStepRun\">");
            if (testStepRun.TestLog.Streams.Count > 0)
                RenderExecutionLogStreams(testStepRun);
            xmlTextWriter.WriteRaw("</div>");

            // child steps
            if (testStepRun.Children.Count > 0)
            {
                xmlTextWriter.WriteRaw("<ul class=\"testStepRunContainer\">");
                foreach (TestStepRun tsr in testStepRun.Children)
                    RenderTestStepRun(tsr, (nestingLevel + 1));
                xmlTextWriter.WriteRaw("</ul>");
            }
            xmlTextWriter.WriteRaw("</div></li>");
        }

        static string FormatStatistics(Statistics statistics)
        {
            return string.Format("{0} run, {1} passed, {2} failed, {3} inconclusive, {4} skipped", statistics.runCount,
                statistics.passedCount, statistics.failedCount, statistics.inconclusiveCount, statistics.skippedCount);
        }

        static string PrintTextWithBreaks(string text)
        {
            return text.Replace(Environment.NewLine, "<br />");
        }

        void RenderOutcomeBar(TestOutcome testOutcome, Statistics statistics, bool small)
        {
            xmlTextWriter.WriteRaw("<table class=\"outcome-bar\"><tr><td>");
            string status = Enum.GetName(typeof(TestStatus), testOutcome.Status).ToLower();
            xmlTextWriter.WriteRaw(string.Format("<div class=\"outcome-bar status-{0}", status));
            if (small)
                xmlTextWriter.WriteRaw(" condensed");
            string title = testOutcome.Category ?? status;
            xmlTextWriter.WriteRaw(string.Format("\" title=\"{0}\" /></td></tr></table>", title));

            if (small)
                return;

            xmlTextWriter.WriteRaw("<span class=\"outcome-icons\">");
            xmlTextWriter.WriteRaw(string.Format("<img src=\"{0}\\Passed.gif\" alt=\"Passed\" />{1}", imgDir, statistics.passedCount));
            xmlTextWriter.WriteRaw(string.Format("<img src=\"{0}\\Failed.gif\" alt=\"Failed\" />{1}", imgDir, statistics.failedCount));
            xmlTextWriter.WriteRaw(string.Format("<img src=\"{0}\\Ignored.gif\" alt=\"Inconclusive or Skipped\" />{1}", imgDir, 
                (statistics.inconclusiveCount + statistics.skippedCount)));
            xmlTextWriter.WriteRaw("</span>");
        }

        void RenderMetadata(TestStepRun testStepRun, TestComponentData testData)
        {
            MetadataMap visibleEntries = testStepRun.Step.Metadata.Copy();
            visibleEntries.Remove(MetadataKeys.TestKind);

            if (testStepRun.Step.IsPrimary)
            {
                visibleEntries.AddAll(testData.Metadata);
                visibleEntries.Remove(MetadataKeys.TestKind);
            }

            if (visibleEntries.Keys.Count <= 0)
                return;

            xmlTextWriter.WriteRaw("<ul class=\"metadata\">");
            foreach (string key in visibleEntries.Keys)
                RenderMetadataValues(key, visibleEntries[key]);
            xmlTextWriter.WriteRaw("</ul>");
        }

        void RenderMetadataValues(string key, IList<string> values)
        {
            xmlTextWriter.WriteRaw(String.Format("<li>{0}: ", PrintTextWithBreaks(key)));
            for (int i = 0; i < values.Count; i++)
            {
                xmlTextWriter.WriteRaw(values[i]);
                if (i < (values.Count - 1))
                    xmlTextWriter.WriteRaw(",");
            }
            xmlTextWriter.WriteRaw("</li>");
        }

        void RenderExecutionLogStreams(TestStepRun testStepRun)
        {
            xmlTextWriter.WriteRaw(String.Format("<div id=\"log-{0}\" class=\"log\">", testStepRun.Step.Id));

            foreach (StructuredTestLogStream executionLogStream in testStepRun.TestLog.Streams)
            {
                xmlTextWriter.WriteRaw(String.Format("<div class=\"logStream logStream-{0}\">", executionLogStream.Name));
                xmlTextWriter.WriteRaw(String.Format("<span class=\"logStreamHeading\"><xsl:value-of select=\"{0}\" /></span>", 
                    executionLogStream.Name));
                xmlTextWriter.WriteRaw("<div class=\"logStreamBody\">");

                executionLogStream.Body.Accept(new RenderTagVisitor(xmlTextWriter, reportFolder, testStepRun));

                xmlTextWriter.WriteRaw("</div></div>");
            }

            if (testStepRun.TestLog.Attachments.Count > 0)
                RenderExecutionLogAttachmentList(testStepRun);

            xmlTextWriter.WriteRaw("</div>");
        }

        void RenderExecutionLogAttachmentList(TestStepRun testStepRun)
        {
            xmlTextWriter.WriteRaw("<div class=\"logAttachmentList\">Attachments: ");
            for (int i = 0; i < testStepRun.TestLog.Attachments.Count; i++)
            {
                AttachmentData attachmentData = testStepRun.TestLog.Attachments[i];
                string src = Path.Combine(Path.Combine(reportFolder, FileUtils.EncodeFileName(testStepRun.Step.Id)), attachmentData.Name);
                xmlTextWriter.WriteRaw(String.Format("<a href=\"{0}\">{1}</a>", src, attachmentData.Name));
                if (i < (testStepRun.TestLog.Attachments.Count - 1))
                    xmlTextWriter.WriteRaw(", ");
            }
            xmlTextWriter.WriteRaw("</div>");
        }

        static Statistics CalculateStatistics(TestStepRun testStepRun)
        {
            Statistics statistics = new Statistics();
            statistics.assertCount = testStepRun.Result.AssertCount;
            statistics.duration = testStepRun.Result.Duration;
            if (testStepRun.Step.IsTestCase)
            {
                switch (testStepRun.Result.Outcome.Status)
                {
                    case TestStatus.Failed:
                        statistics.failedCount++;
                        statistics.runCount++;
                        break;
                    case TestStatus.Inconclusive:
                        statistics.inconclusiveCount++;
                        statistics.runCount++;
                        break;
                    case TestStatus.Passed:
                        statistics.passedCount++;
                        statistics.runCount++;
                        break;
                    case TestStatus.Skipped:
                        statistics.skippedCount++;
                        break;
                }
            }
            foreach (TestStepRun tsr in testStepRun.Children)
            {
                Statistics childStats = CalculateStatistics(tsr);
                statistics.failedCount += childStats.failedCount;
                statistics.inconclusiveCount += childStats.inconclusiveCount;
                statistics.passedCount += childStats.passedCount;
                statistics.skippedCount += childStats.skippedCount;
                statistics.runCount += childStats.runCount;
            }
            return statistics;
        }

        struct Statistics
        {
            public int skippedCount;
            public int passedCount;
            public int failedCount;
            public int inconclusiveCount;
            public int assertCount;
            public double duration;
            public int runCount;
        }

        sealed class RenderTagVisitor : ITagVisitor
        {
            readonly XmlTextWriter xmlTextWriter;
            readonly string reportFolder;
            readonly TestStepRun testStepRun;

            public RenderTagVisitor(XmlTextWriter xmlTextWriter, string reportFolder, TestStepRun testStepRun)
            {
                this.xmlTextWriter = xmlTextWriter;
                this.reportFolder = reportFolder;
                this.testStepRun = testStepRun;
            }

            public void VisitBodyTag(BodyTag tag)
            {
                tag.AcceptContents(this);
            }

            public void VisitSectionTag(SectionTag tag)
            {
                xmlTextWriter.WriteRaw(String.Format("<div class=\"logStreamSection\"><span class=\"logStreamSectionHeading\">{0}</span><div>", tag.Name));

                tag.AcceptContents(this);

                xmlTextWriter.WriteRaw("</div></div>");
            }

            public void VisitMarkerTag(MarkerTag tag)
            {
                tag.AcceptContents(this);
            }

            public void VisitEmbedTag(EmbedTag tag)
            {
                AttachmentData attachment = testStepRun.TestLog.GetAttachment(tag.AttachmentName);
                if (attachment == null)
                    return;

                string src = Path.Combine(Path.Combine(reportFolder, FileUtils.EncodeFileName(testStepRun.Step.Id)), attachment.Name);

                if (attachment.ContentType.StartsWith("image/"))
                {
                    xmlTextWriter.WriteRaw(
                        String.Format(
                            "<div class=\"logAttachmentEmbedding\"><img src=\"{0}\" alt=\"Attachment: {1}\" /></div>",
                            src, attachment.Name));
                }
                else
                {
                    xmlTextWriter.WriteRaw(String.Format("Attachment: <a href=\"{0}\">{1}</a>", src, attachment.Name));
                }
            }

            public void VisitTextTag(TextTag tag)
            {
                xmlTextWriter.WriteRaw(String.Format("<span>{0}</span>", PrintTextWithBreaks(tag.Text)));
            }
        }
    }
}