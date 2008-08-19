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
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using Gallio.Model.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Reports
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

            using (progressMonitor.BeginTask("Loading report.", 10))
            {
                string reportPath = reportContainer.ReportName + @".xml";

                progressMonitor.ThrowIfCanceled();
                progressMonitor.SetStatus(reportPath);

                XmlSerializer serializer = new XmlSerializer(typeof(Report));

                Report report;
                using (Stream stream = reportContainer.OpenRead(reportPath))
                {
                    progressMonitor.ThrowIfCanceled();
                    report = (Report)serializer.Deserialize(stream);
                }

                FixImplicitIds(report);

                progressMonitor.Worked(1);
                progressMonitor.SetStatus(@"");

                if (loadAttachmentContents)
                {
                    progressMonitor.ThrowIfCanceled();
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(9))
                        LoadReportAttachments(report, subProgressMonitor);
                }

                return report;
            }
        }

        private static void FixImplicitIds(Report report)
        {
            if (report.TestPackageRun != null && report.TestPackageRun.RootTestStepRun != null)
                FixImplicitIds(report.TestPackageRun.RootTestStepRun, null);
        }

        private static void FixImplicitIds(TestStepRun testStepRun, string parentId)
        {
            testStepRun.Step.ParentId = parentId;

            string id = testStepRun.Step.Id;
            foreach (TestStepRun child in testStepRun.Children)
                FixImplicitIds(child, id);
        }

        /// <inheritdoc />
        public void LoadReportAttachments(Report report, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            if (report.TestPackageRun == null)
                return;

            List<AttachmentData> attachmentsToLoad = new List<AttachmentData>();
            foreach (TestStepRun testStepRun in report.TestPackageRun.AllTestStepRuns)
            {
                foreach (AttachmentData attachment in testStepRun.TestLog.Attachments)
                {
                    if (attachment.ContentPath != null)
                        attachmentsToLoad.Add(attachment);
                }
            }

            if (attachmentsToLoad.Count == 0)
                return;

            using (progressMonitor.BeginTask("Loading report attachments.", attachmentsToLoad.Count))
            {
                foreach (AttachmentData attachment in attachmentsToLoad)
                {
                    progressMonitor.ThrowIfCanceled();

                    if (attachment.ContentDisposition != AttachmentContentDisposition.Link
                        || attachment.ContentPath == null)
                        continue;

                    string attachmentPath = attachment.ContentPath;

                    progressMonitor.SetStatus(attachmentPath);
                    LoadAttachmentContents(attachment, attachmentPath);
                }
            }
        }

        private void LoadAttachmentContents(AttachmentData attachmentData, string attachmentPath)
        {
            using (Stream attachmentStream = reportContainer.OpenRead(attachmentPath))
            {
                // TODO: How should we handle missing attachments?  Currently we just throw an exception.
                try
                {
                    attachmentData.LoadContents(attachmentStream);
                }
                catch (Exception ex)
                {
                    throw new IOException(String.Format(CultureInfo.CurrentCulture,
                        "Unable to load report attachment from file: '{0}'.", attachmentPath), ex);
                }
            }
        }
    }
}
