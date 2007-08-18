using System.Xml;
using Castle.Core;
using MbUnit.Core.Reporting.Resources;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// <para>
    /// Formats reports as plain text.
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
    [Singleton]
    public class TextReportFormatter : XsltReportFormatter
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return "Text"; }
        }

        /// <inheritdoc />
        public override string PreferredExtension
        {
            get { return "txt"; }
        }

        /// <inheritdoc />
        protected override XmlReader GetStylesheetReader()
        {
            return XmlReader.Create(ReportingResources.GetResource(ReportingResources.TextTemplate));
        }
    }
}