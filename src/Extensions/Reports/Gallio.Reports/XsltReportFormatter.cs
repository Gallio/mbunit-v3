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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Runner.Reports;

namespace Gallio.Reports
{
    /// <summary>
    /// <para>
    /// Generic XSLT report formatter.
    /// </para>
    /// <para>
    /// Recognizes the following options:
    /// <list type="bullet">
    /// <listheader>
    /// <term>Option</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>AttachmentContentDisposition</term>
    /// <description>Overrides the default attachment content disposition for the format.
    /// The content disposition may be "Absent" to exclude attachments, "Link" to
    /// include attachments by reference to external files, or "Inline" to include attachments as
    /// inline content within the formatted document.  Different formats use different
    /// default content dispositions.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class XsltReportFormatter : BaseReportFormatter
    {
        private readonly string extension;
        private readonly string contentType;
        private readonly string contentLocalPath;
        private readonly string xsltPath;
        private readonly string[] resourcePaths;

        private XslCompiledTransform transform;

        /// <summary>
        /// Creates an XSLT report formatter.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="name">The formatter name</param>
        /// <param name="description">The formatter description</param>
        /// <param name="extension">The preferred extension without a '.'</param>
        /// <param name="contentType">The content type of the main report document</param>
        /// <param name="contentUri">The Uri of the content directory</param>
        /// <param name="xsltPath">The path of the XSLT relative to the content directory</param>
        /// <param name="resourcePaths">The paths of the resources (such as images or CSS) to copy
        /// to the report directory relative to the content directory</param>
        /// <exception cref="ArgumentNullException">Thrown if any arguments are null</exception>
        public XsltReportFormatter(IRuntime runtime, string name, string description, string extension, string contentType, string contentUri, string xsltPath, string[] resourcePaths)
            : base(name, description)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");
            if (extension == null)
                throw new ArgumentNullException(@"extension");
            if (contentType == null)
                throw new ArgumentNullException(@"contentType");
            if (contentUri == null)
                throw new ArgumentNullException(@"contentUri");
            if (xsltPath == null)
                throw new ArgumentNullException(@"xsltPath");
            if (resourcePaths == null || Array.IndexOf(resourcePaths, null) >= 0)
                throw new ArgumentNullException(@"resourcePaths");

            this.extension = extension;
            this.contentType = contentType;

            contentLocalPath = runtime.MapUriToLocalPath(new Uri(contentUri));
            this.xsltPath = xsltPath;
            this.resourcePaths = resourcePaths;
        }

        /// <inheritdoc />
        public override void Format(IReportWriter reportWriter, NameValueCollection options, IProgressMonitor progressMonitor)
        {
            ExecutionLogAttachmentContentDisposition attachmentContentDisposition = GetAttachmentContentDisposition(options);

            using (progressMonitor)
            {
                progressMonitor.BeginTask(String.Format("Formatting report as {0}.", Name), 10);

                progressMonitor.SetStatus("Applying XSL transform.");
                ApplyTransform(reportWriter, attachmentContentDisposition, options);
                progressMonitor.Worked(3);

                progressMonitor.SetStatus("Copying resources.");
                CopyResources(reportWriter);
                progressMonitor.Worked(2);

                progressMonitor.SetStatus(@"");

                if (attachmentContentDisposition == ExecutionLogAttachmentContentDisposition.Link)
                {
                    reportWriter.SaveReportAttachments(progressMonitor.CreateSubProgressMonitor(5));
                }
            }
        }

        /// <summary>
        /// Gets the XSL transform.
        /// </summary>
        protected XslCompiledTransform Transform
        {
            get
            {
                if (transform == null)
                    transform = LoadTransform(Path.Combine(contentLocalPath, xsltPath));

                return transform;
            }
        }

        /// <summary>
        /// Applies the transform to produce a report.
        /// </summary>
        protected virtual void ApplyTransform(IReportWriter reportWriter, ExecutionLogAttachmentContentDisposition attachmentContentDisposition,
            NameValueCollection options)
        {
            XsltArgumentList arguments = new XsltArgumentList();
            PopulateArguments(arguments, options, reportWriter.ReportContainer.ReportName);

            XPathDocument document = SerializeReportToXPathDocument(reportWriter, attachmentContentDisposition);

            string reportPath = reportWriter.ReportContainer.ReportName + @"." + extension;

            XslCompiledTransform transform = Transform;
            XmlWriterSettings settings = transform.OutputSettings.Clone();
            settings.CheckCharacters = false;
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(reportWriter.ReportContainer.OpenWrite(reportPath, contentType, Encoding.UTF8), settings))
                transform.Transform(document, arguments, writer);

            reportWriter.AddReportDocumentPath(reportPath);
        }

        /// <summary>
        /// Copies additional resources to the content path within the report.
        /// </summary>
        protected virtual void CopyResources(IReportWriter reportWriter)
        {
            foreach (string resourcePath in resourcePaths)
            {
                string sourceContentPath = Path.Combine(contentLocalPath, resourcePath);
                string destContentPath = Path.Combine(reportWriter.ReportContainer.ReportName, resourcePath);

                ReportContainerUtils.CopyToReport(reportWriter.ReportContainer, sourceContentPath, destContentPath);
            }
        }

        /// <summary>
        /// Populates the arguments for the XSL template processing.
        /// </summary>
        protected virtual void PopulateArguments(XsltArgumentList arguments, NameValueCollection options, string reportName)
        {
            arguments.AddParam(@"resourceRoot", @"", reportName);
        }

        /// <summary>
        /// Loads the XSL transform.
        /// </summary>
        /// <param name="resolvedXsltPath">The full path of the XSLT</param>
        /// <returns>The transform</returns>
        protected virtual XslCompiledTransform LoadTransform(string resolvedXsltPath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = false;
            settings.ValidationType = ValidationType.None;
            settings.XmlResolver = GetContentResolver();

            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(resolvedXsltPath, settings))
                transform.Load(reader);

            return transform;
        }

        private static XPathDocument SerializeReportToXPathDocument(IReportWriter reportWriter,
            ExecutionLogAttachmentContentDisposition attachmentContentDisposition)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.CheckCharacters = false;
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = false;
            xmlWriterSettings.CloseOutput = false;

            MemoryStream stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings);
            reportWriter.SerializeReport(xmlWriter, attachmentContentDisposition);

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.CheckCharacters = false;
            xmlReaderSettings.ValidationType = ValidationType.None;

            stream.Position = 0;
            XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings);

            XPathDocument document = new XPathDocument(xmlReader);
            return document;
        }

        private static XmlResolver GetContentResolver()
        {
            return new XmlUrlResolver();
        }
    }
}