// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Markup;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runner.Reports;

namespace Gallio.Reports
{
    /// <summary>
    /// Formats reports as Xml.
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public class XmlReportFormatter : BaseReportFormatter
    {
        /// <summary>
        /// Creates an Xml report formatter.
        /// </summary>
        public XmlReportFormatter()
        {
        }

        /// <inheritdoc />
        public override void Format(IReportWriter reportWriter, ReportFormatterOptions options, IProgressMonitor progressMonitor)
        {
            AttachmentContentDisposition attachmentContentDisposition = GetAttachmentContentDisposition(options);

            reportWriter.SaveReport(attachmentContentDisposition, progressMonitor);
        }
    }
}
  