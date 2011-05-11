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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Gallio.Common.Xml;
using Gallio.Common.Markup;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Runner.Reports;
using Path = System.IO.Path;
using NVelocity;
using NVelocity.App;
using System.Collections;
using System.Collections.Generic;
using NVelocity.Runtime;
using Gallio.Runner.Reports.Schema;
using Gallio.Common;
using System.Text.RegularExpressions;
using Gallio.Model;
using Gallio.Reports.Vtl;
using Gallio.Runner.Reports.Preferences;

namespace Gallio.Reports
{
    /// <summary>
    /// Generic report formatter based on the Castle NVelocity template engine.
    /// </summary>
    public class VtlReportFormatter : BaseReportFormatter
    {
        private readonly ReportPreferenceManager preferenceManager;
        private readonly string extension;
        private readonly string contentType;
        private readonly DirectoryInfo resourceDirectory;
        private readonly string templatePath;
        private readonly string[] resourcePaths;
        private readonly string templateFilePath;
        private readonly bool supportSplit;
        private IVelocityEngineFactory velocityEngineFactory;

        /// <summary>
        /// Creates a VTL report formatter.
        /// </summary>
        /// <param name="preferenceManager">The user preference manager</param>
        /// <param name="extension">The preferred extension without a '.'</param>
        /// <param name="contentType">The content type of the main report document.</param>
        /// <param name="resourceDirectory">The resource directory.</param>
        /// <param name="templatePath">The path of the NVelocity template relative to the resource directory.</param>
        /// <param name="resourcePaths">The paths of the resources (such as images or CSS) to copy to the report directory relative to the resource directory.</param>
        /// <param name="supportSplit">Indicates whether the format supports file splitting.</param>
        /// <exception cref="ArgumentNullException">Thrown if any arguments are null.</exception>
        public VtlReportFormatter(ReportPreferenceManager preferenceManager, string extension, string contentType, DirectoryInfo resourceDirectory, string templatePath, string[] resourcePaths, bool supportSplit)
        {
            if (preferenceManager == null)
                throw new ArgumentNullException(@"preferenceManager");
            if (extension == null)
                throw new ArgumentNullException(@"extension");
            if (contentType == null)
                throw new ArgumentNullException(@"contentType");
            if (resourceDirectory == null)
                throw new ArgumentNullException(@"resourceDirectory");
            if (templatePath == null)
                throw new ArgumentNullException(@"templatePath");
            if (resourcePaths == null || Array.IndexOf(resourcePaths, null) >= 0)
                throw new ArgumentNullException(@"resourcePaths");

            this.preferenceManager = preferenceManager;
            this.extension = extension;
            this.contentType = contentType;
            this.resourceDirectory = resourceDirectory;
            this.templatePath = templatePath;
            this.resourcePaths = resourcePaths;
            this.supportSplit = supportSplit;
            templateFilePath = Path.Combine(resourceDirectory.FullName, templatePath);
        }

        /// <inheritdoc />
        public override void Format(IReportWriter reportWriter, ReportFormatterOptions options, IProgressMonitor progressMonitor)
        {
            AttachmentContentDisposition attachmentContentDisposition = GetAttachmentContentDisposition(options);

            using (progressMonitor.BeginTask("Formatting report.", 10))
            {
                progressMonitor.SetStatus("Applying template.");
                ApplyTemplate(reportWriter, attachmentContentDisposition, options);
                progressMonitor.Worked(3);

                progressMonitor.SetStatus("Copying resources.");
                CopyResources(reportWriter);
                progressMonitor.Worked(2);

                progressMonitor.SetStatus(@"");

                if (attachmentContentDisposition == AttachmentContentDisposition.Link)
                {
                    using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(5))
                        reportWriter.SaveReportAttachments(subProgressMonitor);
                }
            }
        }

        internal IVelocityEngineFactory VelocityEngineFactory
        {
            get
            { 
                if (velocityEngineFactory == null)
                    velocityEngineFactory = new DefaultVelocityEngineFactory(templateFilePath);

                return velocityEngineFactory;
            }
        
            set
            {
                velocityEngineFactory = value;
            }
        }

        /// <summary>
        /// Applies the template to produce a report.
        /// </summary>
        protected virtual void ApplyTemplate(IReportWriter reportWriter, AttachmentContentDisposition attachmentContentDisposition, ReportFormatterOptions options)
        {
            VelocityEngine velocityEngine = VelocityEngineFactory.CreateVelocityEngine();
            var helper = new FormatHelper();
            VelocityContext velocityContext = VelocityEngineFactory.CreateVelocityContext(reportWriter, helper);
            var writer = GetReportWriter(velocityEngine, velocityContext, reportWriter, helper, options);
            writer.Run();
        }

        private VtlReportWriter GetReportWriter(VelocityEngine velocityEngine, VelocityContext velocityContext, IReportWriter reportWriter, FormatHelper helper, ReportFormatterOptions options)
        {
            int pageSize = GetReportPageSize(options);
            int testCount = (reportWriter.Report.TestPackageRun == null) ? 0 : reportWriter.Report.TestPackageRun.Statistics.TestCount;

            if (pageSize < 0)
            {
                HtmlReportSplitSettings settings = preferenceManager.HtmlReportSplitSettings;
                pageSize = settings.Enabled ? settings.PageSize : 0;
            }

            if (supportSplit && pageSize > 0 && testCount > pageSize)
                return new MultipleFilesVtlReportWriter(velocityEngine, velocityContext, reportWriter, templatePath, contentType, extension, helper, pageSize);

            return new SingleFileVtlReportWriter(velocityEngine, velocityContext, reportWriter, templatePath, contentType, extension, helper);
        }

        /// <summary>
        /// Copies additional resources to the content path within the report.
        /// </summary>
        protected virtual void CopyResources(IReportWriter reportWriter)
        {
            foreach (string resourcePath in resourcePaths)
            {
                if (resourcePath.Length > 0)
                {
                    string sourceContentPath = Path.Combine(resourceDirectory.FullName, resourcePath);
                    string destContentPath = Path.Combine(reportWriter.ReportContainer.ReportName, resourcePath);
                    ReportContainerUtils.CopyToReport(reportWriter.ReportContainer, sourceContentPath, destContentPath);
                }
            }
        }
    }
}