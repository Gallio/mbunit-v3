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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Schema;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Default implementation of a report writer.
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
        /// <param name="report">The report.</param>
        /// <param name="reportContainer">The report container.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> or <paramref name="reportContainer"/> is null.</exception>
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
        public void SerializeReport(XmlWriter xmlWriter, AttachmentContentDisposition attachmentContentDisposition)
        {
            if (xmlWriter == null)
                throw new ArgumentNullException(@"xmlWriter");

            var originalAttachmentData = new Dictionary<AttachmentData, KeyValuePair<AttachmentContentDisposition, string>>();

            try
            {
                var ignoreAttributes = new XmlAttributes();
                ignoreAttributes.XmlIgnore = true;
                var overrides = new XmlAttributeOverrides();

                // Prune unnecessary ids that can be determined implicitly from the report structure.
                overrides.Add(typeof(TestStepData), @"ParentId", ignoreAttributes);

                // Only include content path when linking.
                if (attachmentContentDisposition != AttachmentContentDisposition.Link)
                {
                    overrides.Add(typeof(AttachmentData), @"ContentPath", ignoreAttributes);
                }

                // Only include content data when inline.
                if (attachmentContentDisposition != AttachmentContentDisposition.Inline)
                {
                    overrides.Add(typeof(AttachmentData), @"SerializedContents", ignoreAttributes);
                    overrides.Add(typeof(AttachmentData), @"Encoding", ignoreAttributes);
                    //overrides.Add(typeof(AttachmentData), @"InnerText", ignoreAttributes);	
                    //overrides.Add(typeof(AttachmentData), @"InnerXml", ignoreAttributes);
                }

                // Munge the content paths and content disposition.
                if (report.TestPackageRun != null)
                {
                    foreach (TestStepRun testStepRun in report.TestPackageRun.AllTestStepRuns)
                    {
                        foreach (AttachmentData attachment in testStepRun.TestLog.Attachments)
                        {
                            originalAttachmentData.Add(attachment, new KeyValuePair<AttachmentContentDisposition, string>(
                                attachment.ContentDisposition, attachment.ContentPath));

                            string attachmentPath = GetAttachmentPath(testStepRun.Step.Id, attachment.Name, attachment.ContentType);
                            attachment.ContentDisposition = attachmentContentDisposition;
                            attachment.ContentPath = attachmentPath;
                        }
                    }
                }

                // Serialize the report.
                var serializer = new XmlSerializer(typeof(Report), overrides);
                serializer.Serialize(xmlWriter, report);
            }
            finally
            {
                // Restore content disposition and path in the XML document to the original values.
                foreach (KeyValuePair<AttachmentData,
                    KeyValuePair<AttachmentContentDisposition, string>> pair in originalAttachmentData)
                {
                    pair.Key.ContentDisposition = pair.Value.Key;
                    pair.Key.ContentPath = pair.Value.Value;
                }
            }
        }

        /// <inheritdoc />
        public void SaveReport(AttachmentContentDisposition attachmentContentDisposition,
            IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            if (reportSaved)
                return;

            int attachmentCount = CountAttachments(report);
            using (progressMonitor.BeginTask("Saving report.", attachmentCount + 1))
            {
                var encoding = new UTF8Encoding(false);
                var settings = new XmlWriterSettings();
                settings.CheckCharacters = false;
                settings.Indent = true;
                settings.Encoding = encoding;
                settings.CloseOutput = true;

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

                if (attachmentContentDisposition == AttachmentContentDisposition.Link && attachmentCount != 0)
                {
                    progressMonitor.ThrowIfCanceled();
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(attachmentCount))
                        SaveReportAttachments(subProgressMonitor);
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

            if (reportAttachmentsSaved)
                return;

            int attachmentCount = CountAttachments(report);
            if (attachmentCount == 0)
                return;

            using (progressMonitor.BeginTask("Saving report attachments.", attachmentCount))
            {
                foreach (TestStepRun testStepRun in report.TestPackageRun.AllTestStepRuns)
                {
                    foreach (AttachmentData attachment in testStepRun.TestLog.Attachments)
                    {
                        string attachmentPath = GetAttachmentPath(testStepRun.Step.Id, attachment.Name, attachment.ContentType);

                        progressMonitor.ThrowIfCanceled();
                        progressMonitor.SetStatus(attachmentPath);

                        SaveAttachmentContents(attachment, attachmentPath);

                        progressMonitor.Worked(1);
                    }
                }

                reportAttachmentsSaved = true;
            }
        }

        private void SaveAttachmentContents(AttachmentData attachmentData, string attachmentPath)
        {
            var encoding = new UTF8Encoding(false);
            using (Stream attachmentStream = reportContainer.OpenWrite(attachmentPath, attachmentData.ContentType, encoding))
                attachmentData.SaveContents(attachmentStream, encoding);
        }

        private string GetAttachmentPath(string testStepId, string attachmentName, string mimeType)
        {
            string path = Path.Combine(Path.Combine(
                reportContainer.ReportName,
                reportContainer.EncodeFileName(testStepId)),
                reportContainer.EncodeFileName(attachmentName));

            string extension = MimeTypes.GetExtensionByMimeType(mimeType);
            if (extension != null)
                path += extension;

            return path;
        }

        private static int CountAttachments(Report report)
        {
            int count = 0;

            if (report.TestPackageRun != null)
            {
                foreach (TestStepRun testStepRun in report.TestPackageRun.AllTestStepRuns)
                {
                    count += testStepRun.TestLog.Attachments.Count;
                }
            }

            return count;
        }
    }
}
