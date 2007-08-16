using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Castle.Core;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Reporting
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
    [Singleton]
    public class XmlReportFormatter : IReportFormatter
    {
        /// <inheritdoc />
        public string Name
        {
            get { return "XML"; }
        }

        /// <inheritdoc />
        public string PreferredExtension
        {
            get { return "xml"; }
        }

        /// <inheritdoc />
        public void Format(Report report, string filename, NameValueCollection options,
            IList<string> filesWritten, IProgressMonitor progressMonitor)
        {
            bool saveAttachmentContents;
            if (!bool.TryParse(options.Get("SaveAttachmentContents"), out saveAttachmentContents))
                saveAttachmentContents = true;

            bool embedAttachmentContents;
            if (!bool.TryParse(options.Get("EmbedAttachmentContents"), out embedAttachmentContents))
                embedAttachmentContents = false;

            ReportUtils.SaveReport(report, filename, saveAttachmentContents, embedAttachmentContents,
                filesWritten, progressMonitor);
        }
    }
}
