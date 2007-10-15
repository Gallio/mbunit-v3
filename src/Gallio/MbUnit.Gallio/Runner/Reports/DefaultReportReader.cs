// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Logging;

namespace MbUnit.Runner.Reports
{
    /// <summary>
    /// <para>
    /// Default implementation of a report reader.
    /// </para>
    /// </summary>
    public class DefaultReportReader : IReportReader
    {
        private readonly IReportContainer reportContainer;

        /// <summary>
        /// Creates a report reader.
        /// </summary>
        /// <param name="reportContainer">The report container</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportContainer"/> is null</exception>
        public DefaultReportReader(IReportContainer reportContainer)
        {
            if (reportContainer == null)
                throw new ArgumentNullException(@"reportContainer");

            this.reportContainer = reportContainer;
        }

        /// <inheritdoc />
        public IReportContainer ReportContainer
        {
            get { return reportContainer; }
        }

        /// <inheritdoc />
        public Report LoadReport(bool loadAttachmentContents, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading report.", 10);

                string reportPath = reportContainer.ReportName + @".xml";

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                XmlSerializer serializer = new XmlSerializer(typeof(Report));

                Report report;
                using (Stream stream = reportContainer.OpenReportFile(reportPath, FileMode.Open, FileAccess.Read))
                {
                    progressMonitor.ThrowIfCanceled();
                    report = (Report)serializer.Deserialize(stream);
                }
                progressMonitor.Worked(1);
                progressMonitor.SetStatus(@"");

                if (loadAttachmentContents)
                {
                    progressMonitor.ThrowIfCanceled();
                    LoadReportAttachments(report, new SubProgressMonitor(progressMonitor, 9));
                }

                return report;
            }
        }

        /// <inheritdoc />
        public void LoadReportAttachments(Report report, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                if (report.PackageRun == null)
                    return;

                List<ExecutionLogAttachment> attachmentsToLoad = new List<ExecutionLogAttachment>();
                foreach (StepRun stepRun in report.PackageRun.StepRuns)
                {
                    foreach (ExecutionLogAttachment attachment in stepRun.ExecutionLog.Attachments)
                    {
                        if (attachment.ContentPath != null)
                            attachmentsToLoad.Add(attachment);
                    }
                }

                if (attachmentsToLoad.Count == 0)
                    return;

                progressMonitor.BeginTask("Loading report attachments.", attachmentsToLoad.Count);

                foreach (ExecutionLogAttachment attachment in attachmentsToLoad)
                {
                    progressMonitor.ThrowIfCanceled();

                    if (attachment.ContentDisposition != ExecutionLogAttachmentContentDisposition.Link
                        || attachment.ContentPath == null)
                        continue;

                    string attachmentPath = attachment.ContentPath;

                    progressMonitor.SetStatus(attachmentPath);
                    LoadAttachmentContents(attachment, attachmentPath);
                }
            }
        }

        private void LoadAttachmentContents(ExecutionLogAttachment attachment, string attachmentPath)
        {
            using (Stream attachmentStream = reportContainer.OpenReportFile(attachmentPath, FileMode.Open, FileAccess.Read))
            {
                // TODO: How should we handle missing attachments?  Currently we just throw an exception.
                try
                {
                    switch (attachment.Encoding)
                    {
                        case ExecutionLogAttachmentEncoding.Text:
                            {
                                string text = ReadAllText(attachmentStream);
                                attachment.Contents = new TextAttachment(attachment.Name, attachment.ContentType, text);
                            }
                            break;

                        case ExecutionLogAttachmentEncoding.Xml:
                            {
                                string text = ReadAllText(attachmentStream);
                                attachment.Contents = new XmlAttachment(attachment.Name, attachment.ContentType, text);
                            }
                            break;

                        case ExecutionLogAttachmentEncoding.Base64:
                            {
                                byte[] data = ReadAllBytes(attachmentStream);
                                attachment.Contents = new BinaryAttachment(attachment.Name, attachment.ContentType, data);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException(String.Format(CultureInfo.CurrentCulture,
                        "Unable to load report attachment from file: '{0}'.", attachmentPath), ex);
                }
            }
        }

        private static string ReadAllText(Stream stream)
        {
            return new StreamReader(stream).ReadToEnd();
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            if (stream.Read(bytes, 0, (int)stream.Length) != stream.Length)
                throw new IOException("Did not read entire stream.");
            return bytes;
        }
    }
}
