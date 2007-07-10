using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Collections;
using MbUnit.Core.Services.Report.Attachments;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// A report is produced for each test to contain its output and describe its output.
    /// A test or test framework may include additional information in a report beyond what
    /// is normally captured by creating new report streams and by attaching or embedding
    /// Text, Xml, Images, Blobs and other content.  This mechanism allows for the production
    /// of rich reports containaing all manner of diagnostic information.
    /// </summary>
    /// <remarks>
    /// The operations on this interface are thread-safe.
    /// </remarks>
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
        /// Gets the collection of attachments in the report.
        /// </summary>
        IAttachmentCollection Attachments { get; } 

        /// <summary>
        /// Attaches an attachment to the report.
        /// If the attachment has already been attached to the report, does nothing.
        /// </summary>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        Attachment Attach(Attachment attachment);

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        TextAttachment AttachPlainText(string text);

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        TextAttachment AttachHtml(string html);

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        XmlAttachment AttachXHtml(string xhtml);

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        XmlAttachment AttachXml(string xml);

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        BinaryAttachment AttachImage(Image image);

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified serializer.
        /// <seealso cref="XmlSerializer"/>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default based on the object's type</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer);
    }
}
