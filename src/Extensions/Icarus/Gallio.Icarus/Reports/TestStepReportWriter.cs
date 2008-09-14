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
using System.Xml;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Utilities;

namespace Gallio.Icarus.Core.Reports
{
    public class TestStepReportWriter
    {
        private readonly XmlTextWriter xmlTextWriter;
        private readonly string reportFolder;
        private readonly TestModelData testModelData;

        public TestStepReportWriter(XmlTextWriter xmlTextWriter, string reportFolder, TestModelData testModelData)
        {
            this.xmlTextWriter = xmlTextWriter;
            this.reportFolder = reportFolder;
            this.testModelData = testModelData;
        }

        internal void RenderReport(IList<string> testIds, IEnumerable<TestStepRun> testStepRuns)
        {
            RenderHeader();
            xmlTextWriter.WriteRaw(
                "<div id=\"Content\" class=\"content\"><div id=\"Details\" class=\"section\"><div class=\"section-content\"><ul>");
            foreach (TestStepRun testStepRun in testStepRuns)
            {
                if (testIds.Count == 0 || testIds.Contains(testStepRun.Step.TestId))
                    RenderTestStepRun(testStepRun);
            }
            xmlTextWriter.WriteRaw("</ul></div></div></div></body></html>");
            xmlTextWriter.Flush();
        }

        private void RenderHeader()
        {
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteRaw("<html xml:lang=\"en\" lang=\"en\" dir=\"ltr\"><head>");
            xmlTextWriter.WriteRaw(String.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\\ExecutionLog.css\" />", reportFolder));
            xmlTextWriter.WriteRaw("<style type=\"text/css\">html{overflow:auto;}</style></head><body class=\"gallio-report\">");
        }

        private void RenderTestStepRun(TestStepRun testStepRun)
        {
            TestData testData = testModelData.GetTestById(testStepRun.Step.TestId);
            if (testData == null)
                throw new InvalidOperationException("The step referenced an unknown test.");

            Statistics statistics = CalculateStatistics(testStepRun);
            
            xmlTextWriter.WriteRaw(String.Format("<li><span class=\"testStepRunHeading\"><b>{0}</b> Passed: {1} Failed: {2} Inconclusive: {3} Skipped: {4}</span>", testStepRun.Step.Name, statistics.passedCount, statistics.failedCount, statistics.inconclusiveCount, statistics.skippedCount));
            
            // stat panel
            xmlTextWriter.WriteRaw("<div class=\"panel\">");
            string testKind = testData.Metadata.GetValue(MetadataKeys.TestKind);
            if (testKind == TestKinds.Assembly || testKind == TestKinds.Framework)
            {
                xmlTextWriter.WriteRaw("<table class=\"statistics-table\"><tr class=\"alternate-row\">");
                xmlTextWriter.WriteRaw("<td class=\"statistics-label-cell\">Results:</td><td>");
                xmlTextWriter.WriteRaw(String.Format("{0} run, {1} passed, {2} failed, {3} inconclusive, {4} skipped", statistics.runCount,
                    statistics.passedCount, statistics.failedCount, statistics.inconclusiveCount, statistics.skippedCount));
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
                    RenderTestStepRun(tsr);
                xmlTextWriter.WriteRaw("</ul>");
            }
            xmlTextWriter.WriteRaw("</div></li>");
        }

        private void RenderMetadata(TestStepRun testStepRun, TestComponentData testData)
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

        private void RenderMetadataValues(string key, IList<string> values)
        {
            xmlTextWriter.WriteRaw(String.Format("<li>{0}: ", key));
            for (int i = 0; i < values.Count; i++)
            {
                xmlTextWriter.WriteRaw(values[i]);
                if (i < (values.Count - 1))
                    xmlTextWriter.WriteRaw(",");
            }
            xmlTextWriter.WriteRaw("</li>");
        }

        private void RenderExecutionLogStreams(TestStepRun testStepRun)
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

        private void RenderExecutionLogAttachmentList(TestStepRun testStepRun)
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

        private static Statistics CalculateStatistics(TestStepRun testStepRun)
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

        private struct Statistics
        {
            public int skippedCount;
            public int passedCount;
            public int failedCount;
            public int inconclusiveCount;
            public int assertCount;
            public double duration;
            public int runCount;
        }

        private sealed class RenderTagVisitor : ITagVisitor
        {
            private readonly XmlTextWriter xmlTextWriter;
            private readonly string reportFolder;
            private readonly TestStepRun testStepRun;

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
                xmlTextWriter.WriteRaw(String.Format("<span>{0}</span>", tag.Text));
            }
        }
    }
}