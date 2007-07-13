using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// <para>
    /// A report records the output of a test during its execution including any text
    /// that was written to console output streams, exceptions that occurred, and 
    /// anything else the test writer might want to record.
    /// </para>
    /// <para>
    /// A report consists of zero or more report streams that are opened automatically
    /// on demand to capture independent sequences of report data.  Each stream can
    /// further be broken down into possibly nested sections to classify output
    /// during different phases of test execution (useful for drilling into complex tests).
    /// In addition to text, a report can contain attachments that are either attached
    /// at the top level of the report or embedded into report streams.  Attachments are
    /// typed by mime-type and can contain Text, Xml, Images, Blobs, or any other content.
    /// Certain test frameworks may automatically create attachments to gather all manner
    /// of diagnostic information over the course of the test.
    /// </para>
    /// </summary>
    /// <remarks>
    /// All operations on this interface are thread-safe.
    /// </remarks>
    /// <seealso cref="IReportStream"/>
    public interface IReport
    {
        /// <summary>
        /// Gets the lazily-populated collection of report streams for the report.
        /// Streams are automatically created on demand if no stream with the specified name
        /// exists at the time of the request.
        /// </summary>
        /// <returns>The report stream collection, never null</returns>
        IReportStreamCollection Streams { get; }

        /// <summary>
        /// Attaches an attachment to the report.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with a report even if
        /// <see cref="IReport.Attach" /> or <see cref="IReportStream.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple report streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already attached or embedded</exception>
        Attachment Attach(Attachment attachment);

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        TextAttachment AttachPlainText(string text);

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        TextAttachment AttachHtml(string html);

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        XmlAttachment AttachXHtml(string xhtml);

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        XmlAttachment AttachXml(string xml);

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        BinaryAttachment AttachImage(Image image);

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified serializer.
        /// <seealso cref="XmlSerializer"/>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default XmlSerializer
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="IReportStream.EmbedObjectAsXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer);
    }
}
