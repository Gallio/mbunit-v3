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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Castle.Core;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Reporting
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
    public abstract class XsltReportFormatter : IReportFormatter, IInitializable
    {
        private XslCompiledTransform transform;

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string PreferredExtension { get; }

        /// <inheritdoc />
        public virtual void Initialize()
        {
            transform = LoadTransform();
        }

        /// <inheritdoc />
        public void Format(Report report, string reportPath, NameValueCollection options,
            IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            bool saveAttachmentContents;
            if (!bool.TryParse(options.Get("SaveAttachmentContents"), out saveAttachmentContents))
                saveAttachmentContents = true;

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Formatting report to " + Name + ".", 10);

                ApplyTransform(report, reportPath, options, filesWritten);
                progressMonitor.Worked(5);

                if (saveAttachmentContents)
                {
                    ReportUtils.SaveReportAttachments(report,
                        ReportUtils.GetContentDirectoryPath(reportPath), filesWritten,
                        new SubProgressMonitor(progressMonitor, 5));
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
        /// Loads the XSL transform.
        /// </summary>
        /// <returns>The loaded transform</returns>
        protected virtual XslCompiledTransform LoadTransform()
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(GetStylesheetReader());
            return transform;
        }

        /// <summary>
        /// Applies the transform to produce a report.
        /// </summary>
        protected virtual void ApplyTransform(Report report, string filename,
            NameValueCollection options, IList<string> filesWritten)
        {
            XsltArgumentList arguments = new XsltArgumentList();
            PopulateArguments(arguments, report, filename, options);

            IXPathNavigable reportDoc = ReportUtils.SerializeReportToXPathNavigable(report);
            using (StreamWriter writer = new StreamWriter(filename))
            {
                transform.Transform(reportDoc, arguments, writer);
            }

            if (filesWritten != null)
                filesWritten.Add(filename);
        }

        /// <summary>
        /// Populates the arguments for the XSL template processing.
        /// </summary>
        protected virtual void PopulateArguments(XsltArgumentList arguments,
            Report report, string filename, NameValueCollection options)
        {
        }

        /// <summary>
        /// Gets an XmlReader for the XSL transform definition.
        /// </summary>
        /// <returns>The reader</returns>
        protected abstract XmlReader GetStylesheetReader();
    }
}