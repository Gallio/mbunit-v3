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

using System.Collections.Specialized;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Core.Reporting;

namespace MbUnit.Plugin.Reports
{
    /// <summary>
    /// <para>
    /// Formats reports as Xml.
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
    /// <item>
    /// <term>EmbedAttachmentContents</term>
    /// <description>If <c>"true"</c> and <c>SaveAttachmentContents</c> is also <c>"true"</c>,
    /// embeds attachment contents within the Xml file.
    /// If "false", writes attachments to separate files in a subdirectory with the
    /// same name as the report file.
    /// Defaults to <c>"false"</c>.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    public class XmlReportFormatter : IReportFormatter
    {
        /// <summary>
        /// Gets the name of this formatter.
        /// </summary>
        public const string FormatterName = @"Xml";

        /// <summary>
        /// Gets the name of the option that controls whether attachments are saved.
        /// </summary>
        public const string SaveAttachmentContentsOption = @"SaveAttachmentContents";

        /// <summary>
        /// Gets the name of the option that controls whether attachment contents
        /// are embedded within the report Xml or saved to individual files.
        /// </summary>
        public const string EmbedAttachmentContentsOption = @"EmbedAttachmentContents";

        /// <inheritdoc />
        public string Name
        {
            get { return FormatterName; }
        }

        /// <inheritdoc />
        public string PreferredExtension
        {
            get { return @"xml"; }
        }

        /// <inheritdoc />
        public void Format(Report report, ReportContext reportContext, NameValueCollection options,
            IProgressMonitor progressMonitor)
        {
            bool saveAttachmentContents;
            if (!bool.TryParse(options.Get(SaveAttachmentContentsOption), out saveAttachmentContents))
                saveAttachmentContents = true;

            bool embedAttachmentContents;
            if (!bool.TryParse(options.Get(EmbedAttachmentContentsOption), out embedAttachmentContents))
                embedAttachmentContents = false;

            reportContext.SaveReport(report, saveAttachmentContents, embedAttachmentContents, progressMonitor);
        }
    }
}
