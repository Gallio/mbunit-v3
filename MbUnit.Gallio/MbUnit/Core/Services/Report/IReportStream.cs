using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;
using MbUnit.Core.Services.Report.Attachments;

namespace MbUnit.Core.Services.Report
{
    /// <summary>
    /// A report stream provides methods for including text, images, XML, HTML,
    /// serialized objects, binary blobs, and other content in a report as a stream of
    /// attachments with corresponding mime type.
    /// </summary>
    /// <remarks>
    /// The operations on this interface are thread-safe.
    /// </remarks>
    public interface IReportStream
    {
        /// <summary>
        /// Gets the report within which the stream is contained.
        /// </summary>
        IReport Report { get; }

        /// <summary>
        /// Gets the unique name of the report stream.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a <see cref="TextWriter" /> adapter for the report stream.
        /// </summary>
        /// <returns>The text writer</returns>
        TextWriter TextWriter { get; }

        /// <summary>
        /// Begins a report section with the specified heading.
        /// Sections may be nested.
        /// </summary>
        /// <param name="sectionHeading">The text to appear at the beginning of the section</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionHeading"/> is null</exception>
        void BeginSection(string sectionHeading);

        /// <summary>
        /// Ends the current report section.
        /// </summary>
        void EndSection();

        /// <summary>
        /// Writes a character to the report stream.
        /// </summary>
        /// <param name="value">The character value</param>
        void Write(char value);

        /// <summary>
        /// Writes a string to the report stream.
        /// </summary>
        /// <param name="value">The string value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        void Write(string value);

        /// <summary>
        /// Writes an array of characters to the report stream.
        /// </summary>
        /// <param name="buffer">The character buffer</param>
        /// <param name="index">The index of the first character in the buffer to write</param>
        /// <param name="count">The number of characters from the buffer to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> or <paramref name="count"/> are out of range</exception>
        void Write(char[] buffer, int index, int count);

        /// <summary>
        /// Writes a formatted string to the report stream.
        /// <seealso cref="String.Format"/>
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">The format string arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null</exception>
        void Write(string format, params object[] args);

        /// <summary>
        /// Writes a line delimiter to the report stream.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes a string to the report stream followed by a line delimiter.
        /// </summary>
        /// <param name="value">The string to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        void WriteLine(string value);

        /// <summary>
        /// Writes a formatted string to the report stream followed by a line delimiter.
        /// <seealso cref="String.Format"/>
        /// </summary>
        /// <param name="format">The format string</param>
        /// <param name="args">The format string arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null</exception>
        void WriteLine(string format, params object[] args);

        /// <summary>
        /// Embeds an attachment into the report stream.
        /// The same attachment may be embedded at multiple places in a report and in multiple report streams.
        /// </summary>
        /// <param name="attachment">The attachment to embed</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        Attachment Embed(Attachment attachment);

        /// <summary>
        /// Embeds an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        TextAttachment EmbedPlainText(string text);

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        TextAttachment EmbedHtml(string html);

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        XmlAttachment EmbedXHtml(string xhtml);

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        XmlAttachment EmbedXml(string xml);

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        BinaryAttachment EmbedImage(Image image);

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified serializer.
        /// <seealso cref="XmlSerializer"/>
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default based on the object's type</param>
        /// <returns>The attachment</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer);
    }
}
