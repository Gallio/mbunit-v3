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
using MbUnit.Core.IO;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Logging;
using MbUnit.Model.Serialization;

namespace MbUnit.Runner.Reports
{
    /// <summary>
    /// <para>
    /// Describes the context in which a report is being loaded or saved including the
    /// path of the primary report file.
    /// </para>
    /// <para>
    /// By convention the <see cref="ReportPath"/> always refers to the
    /// primary file written by the report formatter including its extension.
    /// Auxiliary resources may be written to the same directory in files with the same
    /// base name as <see cref="ReportPath"/> but with different extensions.
    /// If there are many auxiliary resources (such as linked images and execution log attachments)
    /// then they should be created in a directory with the same base name as
    /// <see cref="ReportPath"/>.
    /// </para>
    /// <para>
    /// Example layout:
    /// <list type="bullet">
    /// <item>Reports\IntegrationTests.html (the value of <see cref="ReportPath"/></item>
    /// <item>Reports\IntegrationTests.css (an associated file, could also be put in the IntegrationTests folder)</item>
    /// <item>Reports\IntegrationTests\Step123\AttachedLog.txt (an execution log attachment)</item>
    /// <item>Reports\IntegrationTests\img\TestIcon.png (an image used in the report)</item>
    /// <item>Reports\IntegrationTests\img\FixtureIcon.png (another image used in the report)</item>
    /// <item>Reports\IntegrationTests\img\Screenshot.jpg (an execution log attachment)</item>
    /// </list>
    /// </para>
    /// </summary>
    public class ReportContext
    {
        private readonly IFileSystem fileSystem;
        private readonly string reportPath;
        private readonly string contentPath;

        /// <summary>
        /// Creates a new report context.
        /// </summary>
        /// <param name="reportPath">The report path in the native file system</param>
        public ReportContext(string reportPath)
            : this(reportPath, NativeFileSystem.Instance)
        {
        }

        /// <summary>
        /// Creates a new report context.
        /// </summary>
        /// <param name="reportPath">The report path</param>
        /// <param name="fileSystem">The filesystem</param>
        /// <exception cref="ArgumentNullException">Throw if <paramref name="reportPath"/>
        /// or <paramref name="fileSystem"/> is null</exception>
        public ReportContext(string reportPath, IFileSystem fileSystem)
        {
            if (reportPath == null)
                throw new ArgumentNullException(@"reportPath");
            if (fileSystem == null)
                throw new ArgumentNullException(@"fileSystem");

            this.reportPath = Path.GetFullPath(reportPath);
            this.fileSystem = fileSystem;

            string contentDirectoryName = Path.GetFileNameWithoutExtension(reportPath);
            if (contentDirectoryName == reportPath)
                contentDirectoryName += @".content";

            string reportDirectory = Path.GetDirectoryName(reportPath);
            if (reportDirectory.Length == 0)
                contentPath = contentDirectoryName;
            else 
                contentPath = Path.Combine(reportDirectory, contentDirectoryName);
        }

        /// <summary>
        /// Gets the report path.
        /// </summary>
        public string ReportPath
        {
            get { return reportPath; }
        }

        /// <summary>
        /// Gets the associated filesystem interface.
        /// </summary>
        public IFileSystem FileSystem
        {
            get { return fileSystem; }
        }

        /// <summary>
        /// Gets the path of a directory in which to store content such
        /// as attachments that are associated with the report.
        /// </summary>
        public string ContentPath
        {
            get { return contentPath; }
        }

        /// <summary>
        /// Gets the path of <see cref="ContentPath" /> relative to the directory that
        /// contains the <see cref="ReportPath" />.
        /// </summary>
        public string RelativeContentPath
        {
            get { return Path.GetFileName(contentPath); }
        }

        /// <summary>
        /// Opens the file indicated by <see cref="ReportPath" />.
        /// </summary>
        /// <param name="mode">The file open/create mode</param>
        /// <param name="access">The file access mode</param>
        /// <returns>The stream</returns>
        public Stream OpenReport(FileMode mode, FileAccess access)
        {
            return fileSystem.OpenFile(reportPath, mode, access);
        }

        /// <summary>
        /// Saves the contents of an attachment to a file.
        /// Creates the directory containing the file if it does not exist yet.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="attachmentPath">The path of the attachment file to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentPath"/>
        /// or <paramref name="attachment"/> is null</exception>
        public void SaveAttachmentContents(ExecutionLogAttachment attachment, string attachmentPath)
        {
            if (attachment == null)
                throw new ArgumentNullException(@"attachment");
            if (attachmentPath == null)
                throw new ArgumentNullException(@"attachmentPath");

            fileSystem.CreateDirectory(Path.GetDirectoryName(attachmentPath));
            attachment.Contents.Accept(new SaveAttachmentVisitor(attachmentPath, fileSystem));
        }

        /// <summary>
        /// Loads the contents of an attachment from a file.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="attachmentPath">The path of the attachment file to read</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentPath"/>
        /// or <paramref name="attachment"/> is null</exception>
        public void LoadAttachmentContents(ExecutionLogAttachment attachment, string attachmentPath)
        {
            if (attachment == null)
                throw new ArgumentNullException(@"attachment");
            if (attachmentPath == null)
                throw new ArgumentNullException(@"attachmentPath");

            // TODO: How should we handle missing attachments?
            //       Currently we just throw an exception.
            try
            {
                switch (attachment.Encoding)
                {
                    case ExecutionLogAttachmentEncoding.Text:
                        {
                            string text = fileSystem.ReadAllText(attachmentPath);
                            attachment.Contents = new TextAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Xml:
                        {
                            string text = fileSystem.ReadAllText(attachmentPath);
                            attachment.Contents = new XmlAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Base64:
                        {
                            byte[] data = fileSystem.ReadAllBytes(attachmentPath);
                            attachment.Contents = new BinaryAttachment(attachment.Name, attachment.ContentType, data);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new IOException(String.Format(CultureInfo.CurrentCulture, "Unable to load report attachment from file: {0}.", attachmentPath), ex);
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
        public string GetAttachmentFileName(string attachmentName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");

            return fileSystem.EncodeFileName(attachmentName);
        }

        /// <summary>
        /// Gets the name of a directory in which to store results associated
        /// with a run of the specified step id.
        /// </summary>
        /// <param name="stepId">The step id</param>
        /// <returns>The directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/> is null</exception>
        public string GetStepRunDirectoryName(string stepId)
        {
            if (stepId == null)
                throw new ArgumentNullException(@"stepId");

            return fileSystem.EncodeFileName(stepId);
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
        public void SerializeReportToXml(Report report, XmlWriter writer, bool embedAttachmentContents)
        {
            if (writer == null)
                throw new ArgumentNullException(@"writer");
            if (report == null)
                throw new ArgumentNullException(@"report");

            Dictionary<ExecutionLogAttachment, string> oldContentPaths = new Dictionary<ExecutionLogAttachment, string>();
            try
            {
                XmlAttributes ignoreAttributes = new XmlAttributes();
                ignoreAttributes.XmlIgnore = true;
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();

                if (embedAttachmentContents)
                {
                    overrides.Add(typeof(ExecutionLogAttachment), @"ContentPath", ignoreAttributes);
                }
                else
                {
                    overrides.Add(typeof(ExecutionLogAttachment), @"InnerText", ignoreAttributes);
                    overrides.Add(typeof(ExecutionLogAttachment), @"InnerXml", ignoreAttributes);

                    if (report.PackageRun != null)
                    {
                        foreach (StepRun stepRun in report.PackageRun.StepRuns)
                        {
                            string stepDirectory = GetStepRunDirectoryName(stepRun.Step.Id);

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
        public IXPathNavigable SerializeReportToXPathNavigable(Report report)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");

            // Surely there must be a better way that avoids the extra parsing step...
            StringBuilder reportXml = new StringBuilder();
            SerializeReportToXml(report, XmlWriter.Create(reportXml), false);

            return new XPathDocument(XmlReader.Create(new StringReader(reportXml.ToString())));
        }

        /// <summary>
        /// Saves the report as XML to <see cref="ReportPath" />.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="saveAttachmentContents">If true, saves the attachment contents</param>
        /// <param name="embedAttachmentContents">If true and <paramref name="saveAttachmentContents"/> is
        /// true, the contents are embedded in the report's Xml</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>,
        /// or <paramref name="progressMonitor" /> is null</exception>
        public void SaveReport(Report report, bool saveAttachmentContents,
            bool embedAttachmentContents, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                int attachmentCount = CountAttachments(report);
                progressMonitor.BeginTask("Saving report.", attachmentCount + 1);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                using (XmlWriter writer = XmlWriter.Create(OpenReport(FileMode.Create, FileAccess.Write), settings))
                {
                    progressMonitor.ThrowIfCanceled();
                    SerializeReportToXml(report, writer, saveAttachmentContents && embedAttachmentContents);
                }

                progressMonitor.Worked(1);

                if (saveAttachmentContents && !embedAttachmentContents && attachmentCount != 0)
                {
                    progressMonitor.ThrowIfCanceled();
                    SaveReportAttachments(report, new SubProgressMonitor(progressMonitor, attachmentCount));
                }
            }
        }

        /// <summary>
        /// Saves all attachments associated with test runs of a report to <see cref="ContentPath" />.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> or <paramref name="progressMonitor"/> is null</exception>
        public void SaveReportAttachments(Report report, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                int attachmentCount = CountAttachments(report);
                if (attachmentCount == 0)
                    return;

                progressMonitor.BeginTask("Saving report attachments.", attachmentCount);

                foreach (StepRun stepRun in report.PackageRun.StepRuns)
                {
                    string stepDirectory = Path.Combine(contentPath, GetStepRunDirectoryName(stepRun.Step.Id));

                    foreach (ExecutionLogAttachment attachment in stepRun.ExecutionLog.Attachments)
                    {
                        string attachmentPath = Path.Combine(stepDirectory, GetAttachmentFileName(attachment.Name));

                        progressMonitor.ThrowIfCanceled();
                        progressMonitor.SetStatus(attachmentPath);

                        SaveAttachmentContents(attachment, attachmentPath);

                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the report from XML from <see cref="ReportPath" />.
        /// </summary>
        /// <param name="loadAttachmentContents">If true, loads attachment
        /// contents in referenced content files if they were not embedded</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor "/> is null</exception>
        public Report LoadReport(bool loadAttachmentContents, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading report.", 10);

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                XmlSerializer serializer = new XmlSerializer(typeof(Report));
                Report report = (Report)serializer.Deserialize(OpenReport(FileMode.Open, FileAccess.Read));
                progressMonitor.Worked(1);

                if (loadAttachmentContents)
                {
                    progressMonitor.ThrowIfCanceled();
                    LoadReportAttachments(report, new SubProgressMonitor(progressMonitor, 9));
                }

                return report;
            }
        }

        /// <summary>
        /// Loads referenced report attachments from <see cref="ContentPath" />.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report" /> or <paramref name="progressMonitor "/> is null</exception>
        public void LoadReportAttachments(Report report, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");
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

                    if (attachment.ContentPath == null)
                        continue;

                    string attachmentPath = Path.Combine(contentPath, attachment.ContentPath);
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
            private readonly IFileSystem fileSystem;

            public SaveAttachmentVisitor(string path, IFileSystem fileSystem)
            {
                this.path = path;
                this.fileSystem = fileSystem;
            }

            public void VisitTextAttachment(TextAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(OpenFile()))
                    writer.Write(attachment.Text);
            }

            public void VisitXmlAttachment(XmlAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(OpenFile()))
                    writer.Write(attachment.XmlString);
            }

            public void VisitBinaryAttachment(BinaryAttachment attachment)
            {
                byte[] bytes = attachment.Data;
                using (Stream stream = OpenFile())
                    stream.Write(bytes, 0, bytes.Length);
            }

            private Stream OpenFile()
            {
                return fileSystem.OpenFile(path, FileMode.Create, FileAccess.Write);
            }
        }
    }
}
