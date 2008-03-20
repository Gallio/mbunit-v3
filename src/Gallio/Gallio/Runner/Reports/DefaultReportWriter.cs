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
using System.Xml.Serialization;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model.Serialization;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// <para>
    /// Default implementation of a report writer.
    /// </para>
    /// </summary>
    public class DefaultReportWriter : IReportWriter
    {
        private readonly Report report;
        private readonly IReportContainer reportContainer;
        private readonly List<string> reportDocumentPaths;

        private bool reportSaved;
        private bool reportAttachmentsSaved;

        /// <summary>
        /// Creates a report writer for the specified report.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="reportContainer">The report container</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> or <paramref name="reportContainer"/> is null</exception>
        public DefaultReportWriter(Report report, IReportContainer reportContainer)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");
            if (reportContainer == null)
                throw new ArgumentNullException(@"reportContainer");

            this.report = report;
            this.reportContainer = reportContainer;

            reportDocumentPaths = new List<string>();
        }

        /// <inheritdoc />
        public Report Report
        {
            get { return report; }
        }

        /// <inheritdoc />
        public IReportContainer ReportContainer
        {
            get { return reportContainer; }
        }

        /// <inheritdoc />
        public IList<string> ReportDocumentPaths
        {
            get { return reportDocumentPaths.ToArray(); }
        }

        /// <inheritdoc />
        public void AddReportDocumentPath(string path)
        {
            reportDocumentPaths.Add(path);
        }

        /// <inheritdoc />
        public void SerializeReport(XmlWriter xmlWriter, ExecutionLogAttachmentContentDisposition attachmentContentDisposition)
        {
            if (xmlWriter == null)
                throw new ArgumentNullException(@"xmlWriter");

            Dictionary<ExecutionLogAttachment, KeyValuePair<ExecutionLogAttachmentContentDisposition, string>>
                originalAttachmentData = new Dictionary<ExecutionLogAttachment, KeyValuePair<ExecutionLogAttachmentContentDisposition, string>>();
            try
            {
                XmlAttributes ignoreAttributes = new XmlAttributes();
                ignoreAttributes.XmlIgnore = true;
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();

                // Prune unnecessary ids that can be determined implicitly from the report structure.
                overrides.Add(typeof(TestInstanceData), @"ParentId", ignoreAttributes);
                overrides.Add(typeof(TestStepData), @"ParentId", ignoreAttributes);
                overrides.Add(typeof(TestStepData), @"TestInstanceId", ignoreAttributes);

                // Only include content path when linking.
                if (attachmentContentDisposition != ExecutionLogAttachmentContentDisposition.Link)
                {
                    overrides.Add(typeof(ExecutionLogAttachment), @"ContentPath", ignoreAttributes);
                }

                // Only include content data when inline.
                if (attachmentContentDisposition != ExecutionLogAttachmentContentDisposition.Inline)
                {
                    overrides.Add(typeof(ExecutionLogAttachment), @"InnerText", ignoreAttributes);
                    overrides.Add(typeof(ExecutionLogAttachment), @"InnerXml", ignoreAttributes);
                    overrides.Add(typeof(ExecutionLogAttachment), @"Encoding", ignoreAttributes);
                }

                // Munge the content paths and content disposition.
                if (report.PackageRun != null)
                {
                    foreach (TestInstanceRun testInstanceRun in report.PackageRun.TestInstanceRuns)
                    {
                        foreach (TestStepRun testStepRun in testInstanceRun.TestStepRuns)
                        {
                            foreach (ExecutionLogAttachment attachment in testStepRun.ExecutionLog.Attachments)
                            {
                                originalAttachmentData.Add(attachment, new KeyValuePair<ExecutionLogAttachmentContentDisposition, string>(
                                    attachment.ContentDisposition, attachment.ContentPath));

                                string attachmentPath = GetAttachmentPath(testStepRun.Step.Id, attachment.Name);
                                attachment.ContentDisposition = attachmentContentDisposition;
                                attachment.ContentPath = attachmentPath;
                            }
                        }
                    }
                }

                // Serialize the report.
                XmlSerializer serializer = new XmlSerializer(typeof(Report), overrides);
                serializer.Serialize(xmlWriter, report);
            }
            finally
            {
                // Restore content disposition and path in the XML document to the original values.
                foreach (KeyValuePair<ExecutionLogAttachment,
                    KeyValuePair<ExecutionLogAttachmentContentDisposition, string>> pair in originalAttachmentData)
                {
                    pair.Key.ContentDisposition = pair.Value.Key;
                    pair.Key.ContentPath = pair.Value.Value;
                }
            }
        }

        /// <inheritdoc />
        public void SaveReport(ExecutionLogAttachmentContentDisposition attachmentContentDisposition,
            IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                if (reportSaved)
                    return;

                int attachmentCount = CountAttachments(report);
                progressMonitor.BeginTask("Saving report.", attachmentCount + 1);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                string reportPath = reportContainer.ReportName + @".xml";

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                using (XmlWriter writer = XmlWriter.Create(reportContainer.OpenWrite(reportPath, MimeTypes.Xml, settings.Encoding), settings))
                {
                    progressMonitor.ThrowIfCanceled();
                    SerializeReport(writer, attachmentContentDisposition);
                }

                progressMonitor.Worked(1);
                progressMonitor.SetStatus(@"");

                if (attachmentContentDisposition == ExecutionLogAttachmentContentDisposition.Link && attachmentCount != 0)
                {
                    progressMonitor.ThrowIfCanceled();
                    SaveReportAttachments(progressMonitor.CreateSubProgressMonitor(attachmentCount));
                }

                AddReportDocumentPath(reportPath);
                reportSaved = true;
            }
        }

        /// <inheritdoc />
        public void SaveReportAttachments(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                if (reportAttachmentsSaved)
                    return;

                int attachmentCount = CountAttachments(report);
                if (attachmentCount == 0)
                    return;

                progressMonitor.BeginTask("Saving report attachments.", attachmentCount);

                foreach (TestInstanceRun testInstanceRun in report.PackageRun.TestInstanceRuns)
                {
                    foreach (TestStepRun testStepRun in testInstanceRun.TestStepRuns)
                    {
                        foreach (ExecutionLogAttachment attachment in testStepRun.ExecutionLog.Attachments)
                        {
                            string attachmentPath = GetAttachmentPath(testStepRun.Step.Id, attachment.Name);

                            progressMonitor.ThrowIfCanceled();
                            progressMonitor.SetStatus(attachmentPath);

                            SaveAttachmentContents(attachment, attachmentPath);

                            progressMonitor.Worked(1);
                        }
                    }
                }

                reportAttachmentsSaved = true;
            }
        }

        private void SaveAttachmentContents(ExecutionLogAttachment attachment, string attachmentPath)
        {
            using (Stream attachmentStream = reportContainer.OpenWrite(attachmentPath, attachment.ContentType, Encoding.UTF8))
                attachment.SaveContents(attachmentStream, Encoding.UTF8);
        }

        private string GetAttachmentPath(string testStepId, string attachmentName)
        {
            return Path.Combine(Path.Combine(
                reportContainer.ReportName,
                reportContainer.EncodeFileName(testStepId)),
                reportContainer.EncodeFileName(attachmentName));
        }

        private static int CountAttachments(Report report)
        {
            int count = 0;

            if (report.PackageRun != null)
            {
                foreach (TestInstanceRun testInstanceRun in report.PackageRun.TestInstanceRuns)
                {
                    foreach (TestStepRun testStepRun in testInstanceRun.TestStepRuns)
                    {
                        count += testStepRun.ExecutionLog.Attachments.Count;
                    }
                }
            }

            return count;
        }
    }
}
