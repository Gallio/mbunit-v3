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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Platform;
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.Common.Caching;
using Gallio.Model.Schema;

namespace Gallio.UI.Reports
{
    /// <summary>
    /// <para>
    /// An optimized HTML renderer for individual test step runs and their descendants.
    /// </para>
    /// </summary>
    internal class HtmlTestStepRunFormatter : IDisposable
    {
        private const string ReportName = "Report";

        private readonly HashSet<string> attachmentPaths;
        private readonly TemporaryDiskCache cache;
        private readonly IDiskCacheGroup cacheGroup;

        private readonly Uri resourcesUrl;
        private readonly Uri cssUrl;
        private readonly Uri imgUrl;
        private readonly string jsDir;
 
        public HtmlTestStepRunFormatter()
        {
            IRuntime runtime = RuntimeAccessor.Instance;
            string resourcesPath = runtime.ResourceLocator.ResolveResourcePath(new Uri("plugin://Gallio.Reports/Resources/"));
            resourcesUrl = new Uri(resourcesPath);
            cssUrl = new Uri(resourcesUrl, "css");
            imgUrl = new Uri(resourcesUrl, "img");
            jsDir = Path.Combine(resourcesPath, "js");

            cache = new TemporaryDiskCache("Gallio.UI");
            cacheGroup = cache.Groups[Guid.NewGuid().ToString()];
            attachmentPaths = new HashSet<string>();
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            if (cacheGroup != null)
            {
                cacheGroup.Delete();
                attachmentPaths.Clear();
            }
        }

        public FileInfo Format(ICollection<TestStepRun> stepRuns, TestModelData modelData)
        {
            cacheGroup.Create();

            FileInfo htmlFile = cacheGroup.GetFileInfo(ReportName + "." + Hash64.CreateUniqueHash() + ".html");

            using (StreamWriter htmlFileWriter = new StreamWriter(htmlFile.Open(FileMode.Create,
                FileAccess.Write, FileShare.ReadWrite | FileShare.Delete), new UTF8Encoding(false)))
            {
                Format(htmlFileWriter, stepRuns, modelData);
            }

            return htmlFile;
        }

        private void Format(TextWriter writer, IEnumerable<TestStepRun> stepRuns, TestModelData modelData)
        {
            TestStepReportWriter reportWriter = new TestStepReportWriter(this, writer, modelData);
            reportWriter.RenderReport(stepRuns);
        }

        private void SaveAttachments(TestStepRun stepRun)
        {
            foreach (AttachmentData attachmentData in stepRun.TestLog.Attachments)
                SaveAttachment(stepRun.Step.Id, attachmentData);
        }

        private void SaveAttachment(string stepId, AttachmentData attachmentData)
        {
            string attachmentPath = GetAttachmentPath(stepId, attachmentData);
            if (attachmentPaths.Contains(attachmentPath))
                return;

            attachmentPaths.Add(attachmentPath);

            using (Stream fs = cacheGroup.OpenFile(attachmentPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                attachmentData.SaveContents(fs, Encoding.Default);
        }

        private FileInfo GetAttachmentFileInfo(string stepId, AttachmentData attachment)
        {
            return cacheGroup.GetFileInfo(GetAttachmentPath(stepId, attachment));
        }

        private static string GetAttachmentPath(string stepId, AttachmentData attachment)
        {
            string fileName = FileUtils.EncodeFileName(attachment.Name);
            string extension = MimeTypes.GetExtensionByMimeType(attachment.ContentType);
            if (extension != null)
                fileName += extension;

            return Path.Combine(ReportName, Path.Combine(FileUtils.EncodeFileName(stepId), fileName));
        }

        public static void WriteHtmlEncoded(TextWriter writer, string text)
        {
            WriteHtmlEncodedImpl(writer, text, false);
        }

        public static void WriteHtmlEncodedWithBreaks(TextWriter writer, string text)
        {
            WriteHtmlEncodedImpl(writer, text, true);
        }

        private static void WriteHtmlEncodedImpl(TextWriter writer, string text, bool withBreaks)
        {
            foreach (char c in text)
            {
                switch (c)
                {
                    case '&':
                        writer.Write("&amp;");
                        break;

                    case '<':
                        writer.Write("&lt;");
                        break;

                    case '>':
                        writer.Write("&gt;");
                        break;

                    case '"':
                        writer.Write("&quot;");
                        break;

                    case '\r':
                        break;

                    case '\n':
                        if (withBreaks)
                            writer.Write("<br/>");
                        break;

                    case ' ':
                        if (withBreaks)
                            writer.Write("&nbsp;<wbr/>"); // do not allow spaces to be collapsed
                        else
                            writer.Write(' ');
                        break;

                    default:
                        writer.Write(c);
                        break;
                }
            }
        }

        private static void WriteCodeLocationLink(TextWriter writer, CodeLocation location, Action contents)
        {
            if (location.Path != null)
            {
                writer.Write("<a class=\"crossref\" href=\"gallio:navigateTo?path=");
                WriteHtmlEncoded(writer, location.Path);

                if (location.Line != 0)
                {
                    writer.Write("&amp;line=");
                    writer.Write(location.Line.ToString(CultureInfo.InvariantCulture));

                    if (location.Column != 0)
                    {
                        writer.Write("&amp;column=");
                        writer.Write(location.Column.ToString(CultureInfo.InvariantCulture));
                    }
                }

                writer.Write("\">");
                contents();
                writer.Write("</a>");
            }
            else
            {
                contents();
            }
        }

        private sealed class TestStepReportWriter
        {
            private readonly HtmlTestStepRunFormatter formatter;
            private readonly TextWriter writer;
            readonly TestModelData testModelData;

            public TestStepReportWriter(HtmlTestStepRunFormatter formatter, TextWriter writer, TestModelData testModelData)
            {
                this.formatter = formatter;
                this.writer = writer;
                this.testModelData = testModelData;
            }

            public void RenderReport(IEnumerable<TestStepRun> rootRuns)
            {
                bool flashEnabled = ShouldUseFlash(rootRuns);

                writer.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\">\r\n");
                writer.Write("<!-- saved from url=(0014)about:internet -->\r\n");

                writer.Write("<html xml:lang=\"en\" lang=\"en\" dir=\"ltr\"><head>");
                writer.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                writer.Write("<title>Gallio Test Report</title>");
                writer.Write("<link rel=\"stylesheet\" type=\"text/css\" href=\"");
                WriteHtmlEncoded(writer, formatter.cssUrl.ToString());
                writer.Write("/Gallio-Report.css\" />");
                writer.Write("<link rel=\"stylesheet\" type=\"text/css\" href=\"");
                WriteHtmlEncoded(writer, formatter.cssUrl.ToString());
                writer.Write("/Gallio-Report.Generated.css\" />");
                writer.Write("<script type=\"text/javascript\"><!--\n");
                writer.Write(File.ReadAllText(Path.Combine(formatter.jsDir, "Gallio-Report.js")));
                if (flashEnabled)
                    writer.Write(File.ReadAllText(Path.Combine(formatter.jsDir, "swfobject.js")));
                writer.Write("\n--></script>");
                writer.Write("</head><body class=\"gallio-report\" style=\"overflow: auto;\">");

                writer.Write("<div id=\"Header\" class=\"header\"><div class=\"header-image\"></div></div>");

                writer.Write("<div id=\"Content\" class=\"content\">");

                Statistics statistics = new Statistics();
                foreach (TestStepRun rootRun in rootRuns)
                    AddStatistics(statistics, rootRun, false);

                RenderNavigator(statistics, rootRuns);

                writer.Write("<div id=\"Details\" class=\"section\"><div class=\"section-content\"><ul class=\"testStepRunContainer\">");

                foreach (TestStepRun testStepRun in rootRuns)
                    RenderTestStepRun(testStepRun, 1, flashEnabled);

                writer.Write("</ul></div></div></div><script type=\"text/javascript\">reportLoaded();</script></body></html>");
                writer.Flush();
            }

            /// <summary>
            /// Attempting to load flash content in a 64bit process on Windows is doomed to failure
            /// because Adobe does not ship a 64bit version of the Flash plug-in at this time.
            /// Unfortunately instead of failing gracefully, what happens is that Windows pops up a
            /// warning dialog (when the browser is disposed) complaining about a missing flash.ocx.
            /// </summary>
            private static bool ShouldUseFlash(IEnumerable<TestStepRun> rootRuns)
            {
                if (ProcessSupport.Is64BitProcess)
                    return false;

                foreach (var rootRun in rootRuns)
                {
                    foreach (var run in rootRun.AllTestStepRuns)
                    {
                        foreach (var attachment in run.TestLog.Attachments)
                        {
                            if (attachment.ContentType.StartsWith(MimeTypes.FlashVideo))
                                return true;
                        }
                    }
                }

                return false;
            }

            private static IEnumerable<TestStepRun> GetAllRuns(IEnumerable<TestStepRun> rootRuns)
            {
                foreach (TestStepRun run in rootRuns)
                    foreach (TestStepRun item in run.AllTestStepRuns)
                        yield return item;
            }

            private void RenderNavigator(Statistics statistics, IEnumerable<TestStepRun> rootRuns)
            {
                writer.Write("<div id=\"Navigator\" class=\"navigator\">");
                writer.Write("<a href=\"#Details\" title=\"");
                WriteHtmlEncoded(writer, statistics.FormatTestCaseResultSummary());
                writer.Write("\" class=\"navigator-box ");
                writer.Write(StatusCssClassFromStatistics(statistics));
                writer.Write("\"></a>");
                writer.Write("<div class=\"navigator-stripes\">");

                int count = 0;
                foreach (TestStepRun testStepRun in GetAllRuns(rootRuns))
                    count += 1;

                int i = 0;
                foreach (TestStepRun testStepRun in GetAllRuns(rootRuns))
                {
                    float position = i * 98 / count + 1;
                    i++;

                    if (testStepRun.Result.Outcome.Status == TestStatus.Passed ||
                        (!testStepRun.Step.IsTestCase && testStepRun.Children.Count != 0))
                        continue;

                    writer.Write("<a href=\"#testStepRun-");
                    WriteHtmlEncoded(writer, testStepRun.Step.Id);
                    writer.Write("\" style=\"top: ");
                    writer.Write(position.ToString(CultureInfo.InvariantCulture));
                    writer.Write("%\"");

                    string status = Enum.GetName(typeof(TestStatus), testStepRun.Result.Outcome.Status).ToLower();
                    writer.Write(" class=\"status-");
                    writer.Write(status);
                    writer.Write("\" title=\"");
                    WriteHtmlEncoded(writer, testStepRun.Step.Name);
                    writer.Write(" ");
                    writer.Write(status);
                    writer.Write("\"></a>");
                }

                writer.Write("</div></div>");
            }

            private static string StatusCssClassFromStatistics(Statistics statistics)
            {
                if (statistics.FailedCount > 0)
                    return "status-failed";
                if (statistics.InconclusiveCount > 0)
                    return "status-inconclusive";
                return statistics.PassedCount > 0 ? "status-passed" : "status-skipped";
            }

            private void RenderTestStepRun(TestStepRun testStepRun, int nestingLevel, bool flashEnabled)
            {
                formatter.SaveAttachments(testStepRun);

                Statistics statistics = new Statistics();
                AddStatistics(statistics, testStepRun, false);

                writer.Write("<li id=\"testStepRun-");
                WriteHtmlEncoded(writer, testStepRun.Step.Id);
                writer.Write("\">");

                writer.Write("<span class=\"testStepRunHeading testStepRunHeading-Level");
                writer.Write(nestingLevel.ToString(CultureInfo.InvariantCulture));
                writer.Write("\">");

                string testKind = testStepRun.Step.Metadata.GetValue(MetadataKeys.TestKind);
                writer.Write("<span class=\"testKind");
                if (testKind != null)
                {
                    writer.Write(" testKind-");
                    writer.Write(NormalizeTestKindName(testKind));
                }
                writer.Write("\"></span>");

                WriteCodeLocationLink(writer, testStepRun.Step.CodeLocation, () => WriteHtmlEncoded(writer, testStepRun.Step.Name));

                RenderOutcomeBar(testStepRun.Result.Outcome, statistics, (testStepRun.Children.Count == 0));
                writer.Write("</span>");

                // stat panel
                writer.Write("<div id=\"detailPanel-");
                WriteHtmlEncoded(writer, testStepRun.Step.Id);
                writer.Write("\" class=\"panel\">");

                if (nestingLevel == 1)
                {
                    writer.Write("<table class=\"statistics-table\"><tr class=\"alternate-row\">");
                    writer.Write("<td class=\"statistics-label-cell\">Results:</td><td>");
                    writer.Write(FormatStatistics(statistics));
                    writer.Write("</td></tr><tr><td class=\"statistics-label-cell\">Duration:</td><td>");
                    writer.Write(statistics.Duration.ToString("0.000"));
                    writer.Write("s</td></tr><tr class=\"alternate-row\"><td class=\"statistics-label-cell\">Assertions:</td><td>");
                    writer.Write(statistics.AssertCount);
                    writer.Write("</td></tr></table>");
                }
                else
                {
                    writer.Write(String.Format("Duration: {0:0.000}s, Assertions: {1}.", statistics.Duration,
                        statistics.AssertCount));
                }

                // metadata
                RenderMetadata(testStepRun);

                // execution logs
                writer.Write("<div class=\"testStepRun\">");
                if (testStepRun.TestLog.Streams.Count > 0)
                    RenderExecutionLogStreams(testStepRun, flashEnabled);
                writer.Write("</div>");

                // child steps
                if (testStepRun.Children.Count > 0)
                {
                    writer.Write("<ul class=\"testStepRunContainer\">");
                    foreach (TestStepRun tsr in testStepRun.Children)
                        RenderTestStepRun(tsr, nestingLevel + 1, flashEnabled);
                    writer.Write("</ul>");
                }
                writer.Write("</div></li>");
            }

            private static string NormalizeTestKindName(string kind)
            {
                return kind.Replace(" ", "").Replace(".", "");
            }

            private static string FormatStatistics(Statistics statistics)
            {
                return statistics.FormatTestCaseResultSummary();
            }

            private void RenderOutcomeBar(TestOutcome testOutcome, Statistics statistics, bool small)
            {
                writer.Write("<table class=\"outcome-bar\"><tr><td>");
                string status = Enum.GetName(typeof(TestStatus), testOutcome.Status).ToLower();
                writer.Write("<div class=\"outcome-bar status-");
                writer.Write(status);
                if (small)
                    writer.Write(" condensed");
                string title = testOutcome.Category ?? status;
                writer.Write("\" title=\"");
                WriteHtmlEncoded(writer, title);
                writer.Write("\" /></td></tr></table>");

                if (small)
                    return;

                writer.Write("<span class=\"outcome-icons\">");

                writer.Write("<img src=\"");
                WriteHtmlEncoded(writer, formatter.imgUrl.ToString());
                writer.Write("/Passed.gif\" alt=\"Passed\" />");
                writer.Write(statistics.PassedCount);

                writer.Write("<img src=\"");
                WriteHtmlEncoded(writer, formatter.imgUrl.ToString());
                writer.Write("/Failed.gif\" alt=\"Failed\" />");
                writer.Write(statistics.FailedCount);

                writer.Write("<img src=\"");
                WriteHtmlEncoded(writer, formatter.imgUrl.ToString());
                writer.Write("/Ignored.gif\" alt=\"Inconclusive or Skipped\" />");
                writer.Write(statistics.InconclusiveCount + statistics.SkippedCount);

                writer.Write("</span>");
            }

            private void RenderMetadata(TestStepRun testStepRun)
            {
                PropertyBag visibleEntries = testStepRun.Step.Metadata.Copy();
                visibleEntries.Remove(MetadataKeys.TestKind);

                if (visibleEntries.Keys.Count > 0)
                {
                    writer.Write("<ul class=\"metadata\">");
                    foreach (string key in visibleEntries.Keys)
                        RenderMetadataValues(key, visibleEntries[key]);
                    writer.Write("</ul>");
                }
            }

            private void RenderMetadataValues(string key, IList<string> values)
            {
                writer.Write("<li>");
                WriteHtmlEncodedWithBreaks(writer, key);
                writer.Write(": ");

                for (int i = 0; i < values.Count; i++)
                {
                    WriteHtmlEncodedWithBreaks(writer, values[i]);

                    if (i < (values.Count - 1))
                        writer.Write(", ");
                }
                writer.Write("</li>");
            }

            private void RenderExecutionLogStreams(TestStepRun testStepRun, bool flashEnabled)
            {
                writer.Write("<div id=\"log-");
                WriteHtmlEncoded(writer, testStepRun.Step.Id);
                writer.Write("\" class=\"log\">");

                foreach (StructuredStream executionLogStream in testStepRun.TestLog.Streams)
                {
                    writer.Write("<div class=\"logStream logStream-");
                    WriteHtmlEncoded(writer, executionLogStream.Name);
                    writer.Write("\">");
                    writer.Write("<span class=\"logStreamHeading\"><xsl:value-of select=\"");
                    WriteHtmlEncoded(writer, executionLogStream.Name);
                    writer.Write("\" /></span>");
                    writer.Write("<div class=\"logStreamBody\">");

                    executionLogStream.Body.Accept(new RenderTagVisitor(formatter, writer, testStepRun, flashEnabled));

                    writer.Write("</div></div>");
                }

                if (testStepRun.TestLog.Attachments.Count > 0)
                    RenderExecutionLogAttachmentList(testStepRun);

                writer.Write("</div>");
            }

            private void RenderExecutionLogAttachmentList(TestStepRun testStepRun)
            {
                writer.Write("<div class=\"logAttachmentList\">Attachments: ");
                for (int i = 0; i < testStepRun.TestLog.Attachments.Count; i++)
                {
                    AttachmentData attachmentData = testStepRun.TestLog.Attachments[i];
                    string src = formatter.GetAttachmentFileInfo(testStepRun.Step.Id, attachmentData).FullName;
                    writer.Write("<a href=\"");
                    WriteHtmlEncoded(writer, src);
                    writer.Write("\" class=\"attachmentLink\">");
                    WriteHtmlEncoded(writer, attachmentData.Name);
                    writer.Write("</a>");
                    if (i < (testStepRun.TestLog.Attachments.Count - 1))
                        writer.Write(", ");
                }
                writer.Write("</div>");
            }

            private static void AddStatistics(Statistics statistics, TestStepRun testStepRun, bool child)
            {
                if (!child)
                {
                    statistics.AssertCount += testStepRun.Result.AssertCount;
                    statistics.Duration += testStepRun.Result.DurationInSeconds;
                }

                if (testStepRun.Step.IsTestCase)
                {
                    switch (testStepRun.Result.Outcome.Status)
                    {
                        case TestStatus.Failed:
                            statistics.FailedCount++;
                            statistics.RunCount++;
                            break;
                        case TestStatus.Inconclusive:
                            statistics.InconclusiveCount++;
                            statistics.RunCount++;
                            break;
                        case TestStatus.Passed:
                            statistics.PassedCount++;
                            statistics.RunCount++;
                            break;
                        case TestStatus.Skipped:
                            statistics.SkippedCount++;
                            break;
                    }
                }

                foreach (TestStepRun childRun in testStepRun.Children)
                    AddStatistics(statistics, childRun, true);
            }
        }

        private sealed class RenderTagVisitor : ITagVisitor
        {
            private readonly HtmlTestStepRunFormatter formatter;
            private readonly TextWriter writer;
            private readonly TestStepRun testStepRun;
            private readonly bool flashEnabled;

            public RenderTagVisitor(HtmlTestStepRunFormatter formatter, TextWriter writer, TestStepRun testStepRun, bool flashEnabled)
            {
                this.formatter = formatter;
                this.writer = writer;
                this.testStepRun = testStepRun;
                this.flashEnabled = flashEnabled;
            }

            public void VisitBodyTag(BodyTag tag)
            {
                tag.AcceptContents(this);
            }

            public void VisitSectionTag(SectionTag tag)
            {
                writer.Write("<div class=\"logStreamSection\"><span class=\"logStreamSectionHeading\">");
                WriteHtmlEncoded(writer, tag.Name);
                writer.Write("</span><div>");

                tag.AcceptContents(this);

                writer.Write("</div></div>");
            }

            public void VisitMarkerTag(MarkerTag tag)
            {
                writer.Write("<span class=\"logStreamMarker-");
                WriteHtmlEncoded(writer, tag.Class);
                writer.Write("\">");

                switch (tag.Class)
                {
                    case Marker.CodeLocationClass:
                        VisitCodeLocationMarkerTag(tag);
                        break;

                    case Marker.LinkClass:
                        VisitLinkMarkerTag(tag);
                        break;

                    default:
                        tag.AcceptContents(this);
                        break;
                }

                writer.Write("</span>");
            }

            private void VisitCodeLocationMarkerTag(MarkerTag tag)
            {
                Marker marker = tag.Marker;
                string path, line, column;
                marker.Attributes.TryGetValue(Marker.CodeLocationPathAttrib, out path);
                marker.Attributes.TryGetValue(Marker.CodeLocationLineNumberAttrib, out line);
                marker.Attributes.TryGetValue(Marker.CodeLocationColumnNumberAttrib, out column);

                CodeLocation location = new CodeLocation(path,
                    line != null ? int.Parse(line, CultureInfo.InvariantCulture) : 0,
                    column != null ? int.Parse(column, CultureInfo.InvariantCulture) : 0);

                WriteCodeLocationLink(writer, location, () => tag.AcceptContents(this));
            }

            private void VisitLinkMarkerTag(MarkerTag tag)
            {
                Marker marker = tag.Marker;
                string url;
                marker.Attributes.TryGetValue(Marker.LinkUrlAttrib, out url);

                if (url != null)
                {
                    writer.Write("<a class=\"crossref\" href=\"");
                    WriteHtmlEncoded(writer, url);
                    writer.Write("\">");
                    tag.AcceptContents(this);
                    writer.Write("</a>");
                }
                else
                {
                    tag.AcceptContents(this);
                }
            }

            public void VisitEmbedTag(EmbedTag tag)
            {
                AttachmentData attachment = testStepRun.TestLog.GetAttachment(tag.AttachmentName);
                if (attachment == null)
                    return;

                string src = formatter.GetAttachmentFileInfo(testStepRun.Step.Id, attachment).FullName;

                writer.Write("<div class=\"logStreamEmbed\">");
                if (attachment.ContentType.StartsWith("image/"))
                {
                    writer.Write("<a href=\"");
                    writer.Write(src);
                    writer.Write("\" class=\"attachmentLink\">");
                    writer.Write("<img class=\"embeddedImage\" src=\"");
                    WriteHtmlEncoded(writer, src);
                    writer.Write("\" alt=\"Attachment: ");
                    WriteHtmlEncoded(writer, attachment.Name);
                    writer.Write("\" /></a>");
                }
                else if ((attachment.ContentType.StartsWith("text/html") || attachment.ContentType.StartsWith("text/xhtml"))
                    && attachment.IsText)
                {
                    writer.Write(attachment.GetText());
                }
                else if (attachment.ContentType.StartsWith("text/")
                    && attachment.IsText)
                {
                    writer.Write("<pre>");
                    WriteHtmlEncodedWithBreaks(writer, attachment.GetText());
                    writer.Write("</pre>");
                }
                else if (flashEnabled && attachment.ContentType.StartsWith(MimeTypes.FlashVideo))
                {
                    string placeholderId = "video-" + Hash64.CreateUniqueHash();
                    writer.Write("<div id=\"");
                    writer.Write(placeholderId);
                    writer.Write("\">");

                    writer.Write("<script type=\"text/javascript\">");
                    writer.Write("swfobject.embedSWF('");
                    WriteHtmlEncoded(writer, new Uri(Path.Combine(formatter.jsDir, "player.swf")).ToString());
                    writer.Write("', '");
                    writer.Write(placeholderId);
                    writer.Write("', '400', '300', '9.0.98', '");
                    WriteHtmlEncoded(writer, new Uri(Path.Combine(formatter.jsDir, "expressInstall.swf")).ToString());
                    writer.Write("', {file: '");
                    WriteHtmlEncoded(writer, new Uri(src).ToString());
                    writer.Write("'}, {allowfullscreen: 'true', allowscriptaccess: 'always'}, {id: '");
                    writer.Write(placeholderId);
                    writer.Write("'})");
                    writer.Write("</script>");

                    writer.Write("</div>");
                }
                else
                {
                    writer.Write("Attachment: <a href=\"");
                    WriteHtmlEncoded(writer, src);
                    writer.Write("\" class=\"attachmentLink\">");
                    WriteHtmlEncoded(writer, attachment.Name);
                    writer.Write("</a>");
                }
                writer.Write("</div>");
            }

            public void VisitTextTag(TextTag tag)
            {
                WriteHtmlEncodedWithBreaks(writer, tag.Text);
            }
        }
    }
}