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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Runner.Reports;
using MbUnit.Hosting;

namespace MbUnit.Plugin.Reports
{
    /// <summary>
    /// <para>
    /// Abstract base class for formatters implemented using XSLT.
    /// </para>
    /// <para>
    /// Recognizes the following options:
    /// <list type="bullet">
    /// <listheader>
    /// <term>Option</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>SaveAttachmentContents</term>
    /// <description>If <c>"true"</c>, saves the attachment contents.
    /// If <c>"false"</c>, discards the attachment altogether.
    /// Defaults to <c>"true"</c>.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class XsltReportFormatter : IReportFormatter
    {
        private readonly string name;
        private readonly string extension;
        private readonly string contentLocalPath;
        private readonly string xsltPath;
        private readonly string[] resourcePaths;

        private XslCompiledTransform transform;

        /// <summary>
        /// Gets the name of the option that controls whether attachments are saved.
        /// </summary>
        public const string SaveAttachmentContentsOption = @"SaveAttachmentContents";

        /// <summary>
        /// Creates an XSLT report formatter.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="name">The name of the formatter</param>
        /// <param name="extension">The preferred extension without a '.'</param>
        /// <param name="contentUri">The Uri of the content directory</param>
        /// <param name="xsltPath">The path of the XSLT relative to the content directory</param>
        /// <param name="resourcePaths">The paths of the resources (such as images or CSS) to copy
        /// to the report directory relative to the content directory</param>
        /// <exception cref="ArgumentNullException">Thrown if any arguments are null</exception>
        public XsltReportFormatter(IRuntime runtime, string name, string extension, string contentUri, string xsltPath, string[] resourcePaths)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");
            if (name == null)
                throw new ArgumentNullException(@"name");
            if (extension == null)
                throw new ArgumentNullException(@"extension");
            if (contentUri == null)
                throw new ArgumentNullException(@"contentUri");
            if (xsltPath == null)
                throw new ArgumentNullException(@"xsltPath");
            if (resourcePaths == null || Array.IndexOf(resourcePaths, null) >= 0)
                throw new ArgumentNullException(@"resourcePaths");

            this.name = name;
            this.extension = extension;

            contentLocalPath = runtime.MapUriToLocalPath(new Uri(contentUri));
            this.xsltPath = xsltPath;
            this.resourcePaths = resourcePaths;

            LoadTransform();
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public string PreferredExtension
        {
            get { return extension; }
        }

        /// <inheritdoc />
        public void Format(Report report, ReportContext reportContext, NameValueCollection options,
            IProgressMonitor progressMonitor)
        {
            bool saveAttachmentContents;
            if (!bool.TryParse(options.Get(SaveAttachmentContentsOption), out saveAttachmentContents))
                saveAttachmentContents = true;

            using (progressMonitor)
            {
                progressMonitor.BeginTask(String.Format("Formatting report as {0}.", name), 10);

                progressMonitor.SetStatus("Applying XSL transform.");
                ApplyTransform(report, reportContext, options);
                progressMonitor.Worked(3);

                progressMonitor.SetStatus("Copying resources.");
                CopyResources(reportContext);
                progressMonitor.Worked(2);

                progressMonitor.SetStatus(@"");

                if (saveAttachmentContents)
                {
                    reportContext.SaveReportAttachments(report, new SubProgressMonitor(progressMonitor, 5));
                }
            }
        }

        /// <summary>
        /// Gets the XSL transform.
        /// </summary>
        protected XslCompiledTransform Transform
        {
            get { return transform; }
        }

        /// <summary>
        /// Applies the transform to produce a report.
        /// </summary>
        protected virtual void ApplyTransform(Report report, ReportContext reportContext,
            NameValueCollection options)
        {
            XsltArgumentList arguments = new XsltArgumentList();
            PopulateArguments(arguments, report, reportContext, options);

            IXPathNavigable reportDoc = reportContext.SerializeReportToXPathNavigable(report);

            using (StreamWriter writer = new StreamWriter(reportContext.OpenReport(FileMode.Create, FileAccess.Write), Encoding.UTF8))
                transform.Transform(reportDoc, arguments, writer);
        }

        /// <summary>
        /// Copies additional resources to the content path within the report.
        /// </summary>
        protected virtual void CopyResources(ReportContext reportContext)
        {
            foreach (string resourcePath in resourcePaths)
            {
                reportContext.FileSystem.Copy(Path.Combine(contentLocalPath, resourcePath),
                    Path.Combine(reportContext.ContentPath, resourcePath), true);
            }
        }

        /// <summary>
        /// Populates the arguments for the XSL template processing.
        /// </summary>
        protected virtual void PopulateArguments(XsltArgumentList arguments,
            Report report, ReportContext reportContext, NameValueCollection options)
        {
            arguments.AddParam(@"contentRoot", @"", reportContext.RelativeContentPath);
            arguments.AddParam(@"resourceRoot", @"", reportContext.RelativeContentPath);
        }

        private void LoadTransform()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.None;
            settings.XmlResolver = GetContentResolver();

            transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(Path.Combine(contentLocalPath, xsltPath), settings))
                transform.Load(reader);
        }

        private static XmlResolver GetContentResolver()
        {
            return new XmlUrlResolver();
        }
    }
}