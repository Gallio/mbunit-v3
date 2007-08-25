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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Kernel.ExecutionLogs;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Reporting utilities.
    /// </summary>
    public static class ReportUtils
    {
        /// <summary>
        /// Gets the path of a directory in which to store content that
        /// is associated with a report with the specified path.
        /// </summary>
        /// <param name="reportPath">The path of the report file</param>
        /// <returns>The directory in which to store additional associated
        /// report content</returns>
        public static string GetContentDirectoryPath(string reportPath)
        {
            string contentDirectoryName = Path.GetFileNameWithoutExtension(reportPath);
            if (contentDirectoryName == reportPath)
                contentDirectoryName += ".content";

            string reportDirectory = Path.GetDirectoryName(reportPath);
            if (reportDirectory.Length == 0)
                return contentDirectoryName;

            return Path.Combine(reportDirectory, contentDirectoryName);
        }

        /// <summary>
        /// Saves the contents of an attachment to a file.
        /// Creates the directory containing the file if it does not exist yet.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="contentPath">The path of the content file to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentPath"/>
        /// or <paramref name="attachment"/> is null</exception>
        public static void SaveAttachmentContents(ExecutionLogAttachment attachment, string contentPath)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");
            if (contentPath == null)
                throw new ArgumentNullException("contentPath");

            string directory = Path.GetDirectoryName(contentPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            attachment.Contents.Accept(new SaveAttachmentVisitor(contentPath));
        }

        /// <summary>
        /// Loads the contents of an attachment from a file.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="contentPath">The path of the content file to read</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentPath"/>
        /// or <paramref name="attachment"/> is null</exception>
        public static void LoadAttachmentContents(ExecutionLogAttachment attachment, string contentPath)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");
            if (contentPath == null)
                throw new ArgumentNullException("contentPath");

            // TODO: How should we handle missing attachments?
            //       Currently we just throw an exception.
            try
            {
                switch (attachment.Encoding)
                {
                    case ExecutionLogAttachmentEncoding.Text:
                        using (StreamReader reader = new StreamReader(contentPath))
                        {
                            string text = reader.ReadToEnd();
                            attachment.Contents = new TextAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Xml:
                        using (StreamReader reader = new StreamReader(contentPath))
                        {
                            string text = reader.ReadToEnd();
                            attachment.Contents = new XmlAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Base64:
                        {
                            byte[] data = File.ReadAllBytes(contentPath);
                            attachment.Contents = new BinaryAttachment(attachment.Name, attachment.ContentType, data);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new IOException(String.Format(CultureInfo.CurrentCulture, "Unable to load report attachment from file: {0}.", contentPath), ex);
            }
        }

        /// <summary>
        /// Given the name of an attachment, returns a probably unique file name
        /// to contain it.  The name is encoded as needed to make it valid and
        /// to prevent the file name from containing invalid characters that
        /// could cause i/o errors or security violations.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <returns>The file name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
        public static string GetAttachmentFileName(string attachmentName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");

            return FileUtils.EncodeFileName(attachmentName);
        }

        /// <summary>
        /// Gets the name of a directory in which to store results associated
        /// with a run of the specified step id.
        /// </summary>
        /// <param name="stepId">The step id</param>
        /// <returns>The directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> is null</exception>
        public static string GetStepRunDirectoryName(string stepId)
        {
            if (stepId == null)
                throw new ArgumentNullException("stepId");

            return FileUtils.EncodeFileName(stepId);
        }

        /// <summary>
        /// Serializes a report to Xml.
        /// </summary>
        /// <param name="writer">The Xml writer</param>
        /// <param name="report">The report to serialize</param>
        /// <param name="embedAttachmentContents">True if the attachments should be
        /// embedded in the XML directory, or false to omit the contents but
        /// include <see cref="ExecutionLogAttachment.ContentPath" /> references to
        /// attachment files assuming the standard directory layout is used</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/>
        /// or <paramref name="report"/> is null</exception>
        public static void SerializeReportToXml(Report report, XmlWriter writer, bool embedAttachmentContents)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (report == null)
                throw new ArgumentNullException("report");

            Dictionary<ExecutionLogAttachment, string> oldContentPaths = new Dictionary<ExecutionLogAttachment, string>();
            try
            {
                XmlAttributes ignoreAttributes = new XmlAttributes();
                ignoreAttributes.XmlIgnore = true;
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();

                if (embedAttachmentContents)
                {
                    overrides.Add(typeof(ExecutionLogAttachment), "ContentPath", ignoreAttributes);
                }
                else
                {
                    overrides.Add(typeof(ExecutionLogAttachment), "InnerText", ignoreAttributes);
                    overrides.Add(typeof(ExecutionLogAttachment), "InnerXml", ignoreAttributes);

                    if (report.PackageRun != null)
                    {
                        foreach (StepRun stepRun in report.PackageRun.StepRuns)
                        {
                            string stepDirectory = GetStepRunDirectoryName(stepRun.StepId);

                            foreach (ExecutionLogAttachment attachment in stepRun.ExecutionLog.Attachments)
                            {
                                oldContentPaths.Add(attachment, attachment.ContentPath);

                                string attachmentPath = Path.Combine(stepDirectory, GetAttachmentFileName(attachment.Name));
                                attachment.ContentPath = attachmentPath;
                            }
                        }
                    }
                }

                XmlSerializer serializer = new XmlSerializer(typeof(Report), overrides);
                serializer.Serialize(writer, report);
            }
            finally
            {
                foreach (KeyValuePair<ExecutionLogAttachment, string> pair in oldContentPaths)
                    pair.Key.ContentPath = pair.Value;
            }
        }

        /// <summary>
        /// Serializes a report to an XPath navigable document.
        /// </summary>
        /// <param name="report">The report</param>
        /// <returns>The XPath navigable document</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> is null</exception>
        public static IXPathNavigable SerializeReportToXPathNavigable(Report report)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            // Surely there must be a better way that avoids the extra parsing step...
            StringBuilder reportXml = new StringBuilder();
            SerializeReportToXml(report, XmlWriter.Create(reportXml), false);

            return new XPathDocument(XmlReader.Create(new StringReader(reportXml.ToString())));
        }

        /// <summary>
        /// Saves the report as XML to the specified file pretty printed and
        /// with embedded attachments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="reportPath">The path of the report file to save</param>
        /// <param name="saveAttachmentContents">If true, saves the attachment contents</param>
        /// <param name="embedAttachmentContents">If true and <paramref name="saveAttachmentContents"/> is
        /// true, the contents are embedded in the report's Xml</param>
        /// <param name="filesWritten">If not null, the files written during the operation are appended to this list</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="reportPath"/>,
        /// or <paramref name="progressMonitor" /> is null</exception>
        public static void SaveReport(Report report, string reportPath, bool saveAttachmentContents,
            bool embedAttachmentContents, IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (reportPath == null)
                throw new ArgumentNullException("reportPath");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                int attachmentCount = CountAttachments(report);
                progressMonitor.BeginTask("Saving report.", attachmentCount + 1);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                if (filesWritten != null)
                    filesWritten.Add(reportPath);

                using (XmlWriter writer = XmlWriter.Create(reportPath, settings))
                {
                    progressMonitor.ThrowIfCanceled();
                    SerializeReportToXml(report, writer, saveAttachmentContents && embedAttachmentContents);
                }

                progressMonitor.Worked(1);

                if (saveAttachmentContents && !embedAttachmentContents && attachmentCount != 0)
                {
                    progressMonitor.ThrowIfCanceled();
                    SaveReportAttachments(report, GetContentDirectoryPath(reportPath),
                        filesWritten, new SubProgressMonitor(progressMonitor, attachmentCount));
                }
            }
        }

        /// <summary>
        /// Saves all attachments associated with test runs of a report to disk.
        /// Creates the directory if it does not exist yet.
        /// </summary>
        /// <param name="contentDirectoryPath">The path of the content directory</param>
        /// <param name="report">The report</param>
        /// <param name="filesWritten">If not null, the files written during the operation are appended to this list</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentDirectoryPath"/>
        /// or <paramref name="report"/> or <paramref name="progressMonitor"/> is null</exception>
        public static void SaveReportAttachments(Report report, string contentDirectoryPath, IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            if (contentDirectoryPath == null)
                throw new ArgumentNullException("contentDirectoryPath");
            if (report == null)
                throw new ArgumentNullException("report");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                int attachmentCount = CountAttachments(report);
                if (attachmentCount == 0)
                    return;

                progressMonitor.BeginTask("Saving report attachments.", attachmentCount);

                foreach (StepRun stepRun in report.PackageRun.StepRuns)
                {
                    string stepDirectory = Path.Combine(contentDirectoryPath, GetStepRunDirectoryName(stepRun.StepId));

                    foreach (ExecutionLogAttachment attachment in stepRun.ExecutionLog.Attachments)
                    {
                        string attachmentPath = Path.Combine(stepDirectory, GetAttachmentFileName(attachment.Name));

                        progressMonitor.ThrowIfCanceled();
                        progressMonitor.SetStatus(attachmentPath);

                        if (filesWritten != null)
                            filesWritten.Add(attachmentPath);
                        SaveAttachmentContents(attachment, attachmentPath);

                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the report from XML from the specified file.
        /// </summary>
        /// <param name="reportPath">The path of the report file to load</param>
        /// <param name="loadAttachmentContents">If true, loads attachment
        /// contents in referenced content files if they were not embedded</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportPath"/> or <paramref name="progressMonitor "/> is null</exception>
        public static Report LoadReport(string reportPath, bool loadAttachmentContents, IProgressMonitor progressMonitor)
        {
            if (reportPath == null)
                throw new ArgumentNullException("reportPath");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading report.", 10);

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);
                Report report = SerializationUtils.LoadFromXml<Report>(reportPath);
                progressMonitor.Worked(1);

                if (loadAttachmentContents)
                {
                    progressMonitor.ThrowIfCanceled();
                    LoadReportAttachments(report, Path.GetDirectoryName(reportPath),
                        new SubProgressMonitor(progressMonitor, 9));
                }

                return report;
            }
        }

        /// <summary>
        /// Loads referenced report attachments from the specified directory.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="contentDirectoryPath">The path of the directory that contains the attachments</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report" />, <paramref name="contentDirectoryPath"/> or <paramref name="progressMonitor "/> is null</exception>
        public static void LoadReportAttachments(Report report, string contentDirectoryPath, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (contentDirectoryPath == null)
                throw new ArgumentNullException("contentDirectoryPath");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

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

                    if (attachment.ContentPath == null)
                        continue;

                    string attachmentPath = Path.Combine(contentDirectoryPath, attachment.ContentPath);
                    progressMonitor.SetStatus(attachmentPath);
                    LoadAttachmentContents(attachment, attachmentPath);
                }
            }
        }

        private static int CountAttachments(Report report)
        {
            int count = 0;

            if (report.PackageRun != null)
            {
                foreach (StepRun stepRun in report.PackageRun.StepRuns)
                {
                    count += stepRun.ExecutionLog.Attachments.Count;
                }
            }

            return count;
        }

        private class SaveAttachmentVisitor : IAttachmentVisitor
        {
            private readonly string path;

            public SaveAttachmentVisitor(string path)
            {
                this.path = path;
            }

            public void VisitTextAttachment(TextAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(path))
                    writer.Write(attachment.Text);
            }

            public void VisitXmlAttachment(XmlAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(path))
                    writer.Write(attachment.XmlString);
            }

            public void VisitBinaryAttachment(BinaryAttachment attachment)
            {
                byte[] bytes = attachment.Data;
                using (Stream stream = File.OpenWrite(path))
                    stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
