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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runner.Reports;
using System.Xml.Serialization;
using Gallio.Model.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using Gallio.Model;
using Gallio.Utilities;

namespace Gallio.Icarus.Core.Reports
{
    public static class TestStepReportWriter
    {
        private static string reportFolder;

        public static Stream OutputReport(TestStepRun testStepRun, TestModelData testModelData, string reportFolder)
        {
            TestStepReportWriter.reportFolder = reportFolder;
            MemoryStream memoryStream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteRaw("<html xml:lang=\"en\" lang=\"en\" dir=\"ltr\"><head>");
            xmlTextWriter.WriteRaw(String.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\\ExecutionLog.css\" />", reportFolder));
            xmlTextWriter.WriteRaw("<style type=\"text/css\">html{overflow:auto;}</style></head><body class=\"gallio-report\">");
            xmlTextWriter.WriteRaw("<div id=\"Content\" class=\"content\"><div id=\"Details\" class=\"section\"><div class=\"section-content\"><ul>");
            RenderTestStepRun(xmlTextWriter, testStepRun, testModelData);
            xmlTextWriter.WriteRaw("</ul></div></div></div></body></html>");
            xmlTextWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static void RenderTestStepRun(XmlTextWriter xmlTextWriter, TestStepRun testStepRun, TestModelData testModelData)
        {
            TestData testData = testModelData.Tests[testStepRun.Step.TestId];
            Statistics statistics = CalculateStatistics(testStepRun);
            
            xmlTextWriter.WriteRaw(String.Format("<li><span class=\"testStepRunHeading\"><b>{1}</b> Passed: {2} Failed: {3} Ignored: {4}</span>", 
                testStepRun.Step.Id, testStepRun.Step.Name, statistics.passedCount, statistics.failedCount, 
                (statistics.inconclusiveCount + statistics.skippedCount)));
            
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
                xmlTextWriter.WriteRaw(String.Format("{0}s", statistics.duration.ToString()));
                xmlTextWriter.WriteRaw(String.Format("</td></tr><tr class=\"alternate-row\"><td class=\"statistics-label-cell\">Assertions:</td><td>{0}", 
                    statistics.assertCount));
                xmlTextWriter.WriteRaw("</td></tr></table>");
            }
            else
                xmlTextWriter.WriteRaw(String.Format("Duration: {0}s, Assertions: {1}.", statistics.duration.ToString(), statistics.assertCount));
            
            // metadata
            xmlTextWriter.WriteRaw("<ul class=\"metadata\">");
            if (testStepRun.Step.IsPrimary)
                RenderMetadata(xmlTextWriter, testStepRun, testData);
            else
                RenderMetadata(xmlTextWriter, testStepRun);
            xmlTextWriter.WriteRaw("</ul><div class=\"testStepRun\">");

            // execution logs
            if (testStepRun.ExecutionLog.Streams.Count > 0)
                RenderExecutionLogStreams(xmlTextWriter, testStepRun);
            xmlTextWriter.WriteRaw("</div>");

            // child steps
            if (testStepRun.Children.Count > 0)
            {
                xmlTextWriter.WriteRaw("<ul class=\"testStepRunContainer\">");
                foreach (TestStepRun tsr in testStepRun.Children)
                    RenderTestStepRun(xmlTextWriter, tsr, testModelData);
                xmlTextWriter.WriteRaw("</ul>");
            }
            xmlTextWriter.WriteRaw("</div></li>");
        }

        private static void RenderMetadata(XmlTextWriter xmlTextWriter, TestStepRun testStepRun, TestData testData)
        {
            foreach (string key in testData.Metadata.Keys)
                xmlTextWriter.WriteRaw(String.Format("<li>{0}: {1}</li>", key, testData.Metadata[key]));
            RenderMetadata(xmlTextWriter, testStepRun);
        }

        private static void RenderMetadata(XmlTextWriter xmlTextWriter, TestStepRun testStepRun)
        {
            foreach (string key in testStepRun.Step.Metadata.Keys)
                xmlTextWriter.WriteRaw(String.Format("<li>{0}: {1}</li>", key, testStepRun.Step.Metadata[key]));
        }

        private static void RenderExecutionLogStreams(XmlTextWriter xmlTextWriter, TestStepRun testStepRun)
        {
            xmlTextWriter.WriteRaw(String.Format("<div id=\"log-{0}\" class=\"log\">", testStepRun.Step.Id));
            foreach (ExecutionLogStream executionLogStream in testStepRun.ExecutionLog.Streams)
            {
                xmlTextWriter.WriteRaw(String.Format("<div class=\"logStream logStream-{0}\">", executionLogStream.Name));
                xmlTextWriter.WriteRaw(String.Format("<span class=\"logStreamHeading\"><xsl:value-of select=\"{0}\" /></span>", 
                    executionLogStream.Name));
                xmlTextWriter.WriteRaw("<div class=\"logStreamBody\">");
                foreach (ExecutionLogStreamTag executionLogStreamTag in executionLogStream.Body.Contents)
                    RenderExecutionLogStreamContents(xmlTextWriter, testStepRun, executionLogStreamTag);
                xmlTextWriter.WriteRaw("</div></div>");
            }
            if (testStepRun.ExecutionLog.Attachments.Count > 0)
                RenderExecutionLogAttachmentList(xmlTextWriter, testStepRun);
            xmlTextWriter.WriteRaw("</div>");
        }

        private static void RenderExecutionLogStreamContents(XmlTextWriter xmlTextWriter, TestStepRun testStepRun, ExecutionLogStreamTag executionLogStreamTag)
        {
            if (executionLogStreamTag is ExecutionLogStreamTextTag)
            {
                // write text block
                ExecutionLogStreamTextTag textTag = ((ExecutionLogStreamTextTag)executionLogStreamTag);
                xmlTextWriter.WriteRaw(String.Format("<div>{0}</div>", textTag.Text));
            }
            else if (executionLogStreamTag is ExecutionLogStreamSectionTag)
            {
                ExecutionLogStreamSectionTag sectionTag = (ExecutionLogStreamSectionTag)executionLogStreamTag;
                xmlTextWriter.WriteRaw(String.Format("<div class=\"logStreamSection\"><span class=\"logStreamSectionHeading\">{0}</span><div>", 
                    sectionTag.Name));
                foreach (ExecutionLogStreamTag elst in sectionTag.Contents)
                    RenderExecutionLogStreamContents(xmlTextWriter, testStepRun, elst);
                xmlTextWriter.WriteRaw("</div></div>");
            }
            else if (executionLogStreamTag is ExecutionLogStreamEmbedTag)
            {
                ExecutionLogStreamEmbedTag embedTag = (ExecutionLogStreamEmbedTag)executionLogStreamTag;
                foreach (ExecutionLogAttachment attachment in testStepRun.ExecutionLog.Attachments)
                {
                    if (attachment.Name == embedTag.AttachmentName)
                    {
                        string src = Path.Combine(Path.Combine(reportFolder, FileUtils.EncodeFileName(testStepRun.Step.Id)), attachment.Name);
                        if (attachment.ContentType.StartsWith("image/"))
                            xmlTextWriter.WriteRaw(String.Format("<div class=\"logAttachmentEmbedding\"><img src=\"{0}\" alt=\"Attachment: {1}\" /></div>",
                                src, attachment.Name));
                        else
                            xmlTextWriter.WriteRaw(String.Format("Attachment: <a href=\"{0}\">{1}</a>", src, attachment.Name));
                        break;
                    }
                }
            }
        }

        private static void RenderExecutionLogAttachmentList(XmlTextWriter xmlTextWriter, TestStepRun testStepRun)
        {
            xmlTextWriter.WriteRaw("<div class=\"logAttachmentList\">Attachments: ");
            for (int i = 0; i < testStepRun.ExecutionLog.Attachments.Count; i++)
            {
                ExecutionLogAttachment attachment = testStepRun.ExecutionLog.Attachments[i];
                string src = Path.Combine(Path.Combine(reportFolder, FileUtils.EncodeFileName(testStepRun.Step.Id)), attachment.Name);
                xmlTextWriter.WriteRaw(String.Format("<a href=\"{0}\">{1}</a>", src, attachment.Name));
                if (i < (testStepRun.ExecutionLog.Attachments.Count - 1))
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
                statistics.assertCount += childStats.assertCount;
                statistics.duration += childStats.duration;
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
    }
}
