using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Reporting utilities.
    /// </summary>
    public static class ReportUtils
    {
        /// <summary>
        /// Saves the contents of an attachment to a file.
        /// Creates the directory containing the file if it does not exist yet.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="filename">The filename</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/>
        /// or <paramref name="attachment"/> is null</exception>
        public static void SaveAttachmentContents(ExecutionLogAttachment attachment, string filename)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");
            if (filename == null)
                throw new ArgumentNullException("filename");

            string directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            attachment.Contents.Accept(new SaveAttachmentVisitor(filename));
        }

        /// <summary>
        /// Loads the contents of an attachment from a file.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        /// <param name="filename">The filename</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/>
        /// or <paramref name="attachment"/> is null</exception>
        public static void LoadAttachmentContents(ExecutionLogAttachment attachment, string filename)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");
            if (filename == null)
                throw new ArgumentNullException("filename");

            // TODO: How should we handle missing attachments?
            //       Currently we just throw an exception.
            try
            {
                switch (attachment.Encoding)
                {
                    case ExecutionLogAttachmentEncoding.Text:
                        using (StreamReader reader = new StreamReader(filename))
                        {
                            string text = reader.ReadToEnd();
                            attachment.Contents = new TextAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Xml:
                        using (StreamReader reader = new StreamReader(filename))
                        {
                            string text = reader.ReadToEnd();
                            attachment.Contents = new XmlAttachment(attachment.Name, attachment.ContentType, text);
                        }
                        break;

                    case ExecutionLogAttachmentEncoding.Base64:
                        {
                            byte[] data = File.ReadAllBytes(filename);
                            attachment.Contents = new BinaryAttachment(attachment.Name, attachment.ContentType, data);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new IOException(String.Format(CultureInfo.CurrentCulture, "Unable to load report attachment from file: {0}.", filename), ex);
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
        /// with a run of the specified test id.
        /// </summary>
        /// <param name="testId">The test id</param>
        /// <returns>The directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null</exception>
        public static string GetTestRunDirectoryName(string testId)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            return FileUtils.EncodeFileName(testId);
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
        public static void SerializeReportToXml(XmlWriter writer, Report report,
            bool embedAttachmentContents)
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
                        foreach (TestRun testRun in report.PackageRun.TestRuns)
                        {
                            string testRunDirectory = GetTestRunDirectoryName(testRun.TestId);

                            foreach (ExecutionLogAttachment attachment in testRun.ExecutionLog.Attachments)
                            {
                                oldContentPaths.Add(attachment, attachment.ContentPath);

                                string attachmentPath = Path.Combine(testRunDirectory, GetAttachmentFileName(attachment.Name));
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
        /// Saves the report as XML to the specified file pretty printed and
        /// with embedded attachments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="filename">The name of the file to save</param>
        /// <param name="saveAttachmentContents">If true, saves the attachment contents</param>
        /// <param name="embedAttachmentContents">If true and <paramref name="saveAttachmentContents"/> is
        /// true, the contents are embedded in the report's Xml</param>
        /// <param name="filesWritten">If not null, the files written during the operation are appended to this list</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="filename"/>,
        /// or <param name="progressMonitor" /> is null</exception>
        public static void SaveReport(Report report, string filename, bool saveAttachmentContents,
            bool embedAttachmentContents, IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                int attachmentCount = CountAttachments(report);
                progressMonitor.BeginTask("Saving report.", attachmentCount + 1);

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(filename);

                if (filesWritten != null)
                    filesWritten.Add(filename);
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    progressMonitor.ThrowIfCanceled();
                    SerializeReportToXml(writer, report, saveAttachmentContents && embedAttachmentContents);
                }

                progressMonitor.Worked(1);

                if (saveAttachmentContents && !embedAttachmentContents && attachmentCount != 0)
                {
                    progressMonitor.ThrowIfCanceled();
                    SaveReportAttachments(Path.GetDirectoryName(filename), report,
                        filesWritten, new SubProgressMonitor(progressMonitor, attachmentCount));
                }
            }
        }

        /// <summary>
        /// Saves all attachments associated with test runs of a report to disk.
        /// Creates the directory if it does not exist yet.
        /// </summary>
        /// <param name="directory">The directory</param>
        /// <param name="report">The report</param>
        /// <param name="filesWritten">If not null, the files written during the operation are appended to this list</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="directory"/>
        /// or <paramref name="report"/> or <paramref name="progressMonitor"/> is null</exception>
        public static void SaveReportAttachments(string directory, Report report,
            IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");
            if (report == null)
                throw new ArgumentNullException("report");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                if (report.PackageRun == null)
                    return;

                progressMonitor.BeginTask("Saving report attachments.", CountAttachments(report));

                foreach (TestRun testRun in report.PackageRun.TestRuns)
                {
                    string testRunDirectory = Path.Combine(directory, GetTestRunDirectoryName(testRun.TestId));

                    foreach (ExecutionLogAttachment attachment in testRun.ExecutionLog.Attachments)
                    {
                        string attachmentPath = Path.Combine(testRunDirectory, GetAttachmentFileName(attachment.Name));

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
        /// <param name="filename">The name of the file to load</param>
        /// <param name="loadAttachmentContents">If true, loads attachment
        /// contents in referenced content files if they were not embedded</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/> or <paramref name="progressMonitor "/> is null</exception>
        public static Report LoadReport(string filename, bool loadAttachmentContents, IProgressMonitor progressMonitor)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading report.", 10);

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(filename);
                Report report = SerializationUtils.LoadFromXml<Report>(filename);
                progressMonitor.Worked(1);

                if (loadAttachmentContents)
                {
                    progressMonitor.ThrowIfCanceled();
                    LoadReportAttachments(report, Path.GetDirectoryName(filename),
                        new SubProgressMonitor(progressMonitor, 9));
                }

                return report;
            }
        }

        /// <summary>
        /// Loads referenced report attachments from the specified directory.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="directory">The directory that contains the attachments</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report" />, <paramref name="directory"/> or <paramref name="progressMonitor "/> is null</exception>
        public static void LoadReportAttachments(Report report, string directory, IProgressMonitor progressMonitor)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (directory == null)
                throw new ArgumentNullException("directory");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor)
            {
                if (report.PackageRun == null)
                    return;

                List<ExecutionLogAttachment> attachmentsToLoad = new List<ExecutionLogAttachment>();
                foreach (TestRun testRun in report.PackageRun.TestRuns)
                    foreach (ExecutionLogAttachment attachment in testRun.ExecutionLog.Attachments)
                        if (attachment.ContentPath != null)
                            attachmentsToLoad.Add(attachment);

                if (attachmentsToLoad.Count == 0)
                    return;

                progressMonitor.BeginTask("Loading report attachments.", attachmentsToLoad.Count);

                foreach (ExecutionLogAttachment attachment in attachmentsToLoad)
                {
                    progressMonitor.ThrowIfCanceled();

                    if (attachment.ContentPath == null)
                        continue;

                    string attachmentPath = Path.Combine(directory, attachment.ContentPath);
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
                foreach (TestRun testRun in report.PackageRun.TestRuns)
                    count += testRun.ExecutionLog.Attachments.Count;
            }

            return count;
        }

        private class SaveAttachmentVisitor : IAttachmentVisitor
        {
            private readonly string fileName;

            public SaveAttachmentVisitor(string fileName)
            {
                this.fileName = fileName;
            }

            public void VisitTextAttachment(TextAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                    writer.Write(attachment.Text);
            }

            public void VisitXmlAttachment(XmlAttachment attachment)
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                    writer.Write(attachment.XmlString);
            }

            public void VisitBinaryAttachment(BinaryAttachment attachment)
            {
                byte[] bytes = attachment.Data;
                using (Stream stream = File.OpenWrite(fileName))
                    stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
