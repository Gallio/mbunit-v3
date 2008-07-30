using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using Gallio.Framework;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A log records the output of a test during its execution including any text
    /// that was written to console output streams, exceptions that occurred, and 
    /// anything else the test writer might want to save.
    /// </para>
    /// <para>
    /// A log consists of zero or more log streams that are opened automatically
    /// on demand to capture independent sequences of log output.  Each stream can
    /// further be broken down into possibly nested sections to classify output
    /// during different phases of test execution (useful for drilling into complex tests).
    /// In addition to text, a log can contain attachments that are either attached
    /// at the top level of the log or embedded into log streams.  Attachments are
    /// typed by mime-type and can contain Text, Xml, Images, Blobs, or any other content.
    /// Certain test frameworks may automatically create attachments to gather all manner
    /// of diagnostic information over the course of the test.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// All operations on this interface are thread-safe.
    /// </para>
    /// <para>
    /// The object extends <see cref="MarshalByRefObject" /> so instances may be
    /// accessed by remote clients if required.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestLogStreamWriter"/>
    [Serializable]
    public abstract class TestLogWriter : MarshalByRefObject
    {
        #region Log writer stream accessors
        /// <summary>
        /// Gets the stream writer for the built-in log stream where the <see cref="Console.In" />
        /// stream for the test is recorded.
        /// </summary>
        public TestLogStreamWriter ConsoleInput
        {
            get { return this[TestLogStreamNames.ConsoleInput]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where the <see cref="Console.Out" />
        /// stream for the test is recorded.
        /// </summary>
        public TestLogStreamWriter ConsoleOutput
        {
            get { return this[TestLogStreamNames.ConsoleOutput]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where the <see cref="Console.Error" />
        /// stream for the test is recorded.
        /// </summary>
        public TestLogStreamWriter ConsoleError
        {
            get { return this[TestLogStreamNames.ConsoleError]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where diagnostic <see cref="Debug" />
        /// and <see cref="Trace" /> information is recorded.
        /// </summary>
        public TestLogStreamWriter DebugTrace
        {
            get { return this[TestLogStreamNames.DebugTrace]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public TestLogStreamWriter Failures
        {
            get { return this[TestLogStreamNames.Failures]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where warnings are recorded.
        /// </summary>
        public TestLogStreamWriter Warnings
        {
            get { return this[TestLogStreamNames.Warnings]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream where the output from the convenience methods
        /// of the <see cref="Log" /> class is recorded.
        /// </summary>
        public TestLogStreamWriter Default
        {
            get { return this[TestLogStreamNames.Default]; }
        }
        #endregion

        /// <summary>
        /// Gets the log stream with the specified name.  If the stream
        /// does not exist, it is created on demand.
        /// </summary>
        /// <remarks>
        /// This property may return different instances of <see cref="TestLogStreamWriter" />
        /// each time it is called but they always represent the same stream just the same.
        /// </remarks>
        /// <param name="streamName">The name of the log stream</param>
        /// <returns>The log stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        public TestLogStreamWriter this[string streamName]
        {
            get
            {
                if (streamName == null)
                    throw new ArgumentNullException(@"streamName");

                return GetLogStreamWriterImpl(streamName);
            }
        }

        /// <summary>
        /// Attaches an attachment to the execution log.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="TestLogWriter.Attach" /> or <see cref="TestLogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public Attachment Attach(Attachment attachment)
        {
            AttachImpl(attachment);
            return attachment;
        }

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachPlainText(string attachmentName, string text)
        {
            return (TextAttachment)Attach(Attachment.CreatePlainTextAttachment(attachmentName, text));
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachHtml(string attachmentName, string html)
        {
            return (TextAttachment)Attach(Attachment.CreateHtmlAttachment(attachmentName, html));
        }

        /// <summary>
        /// Attaches an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachXHtml(string attachmentName, string xhtml)
        {
            return (TextAttachment)Attach(Attachment.CreateXHtmlAttachment(attachmentName, xhtml));
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachXml(string attachmentName, string xml)
        {
            return (TextAttachment)Attach(Attachment.CreateXmlAttachment(attachmentName, xml));
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public BinaryAttachment AttachImage(string attachmentName, Image image)
        {
            return (BinaryAttachment)Attach(Attachment.CreateImageAttachment(attachmentName, image));
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachObjectAsXml(string attachmentName, object obj)
        {
            return AttachObjectAsXml(attachmentName, obj, null);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogStreamWriter.EmbedObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return (TextAttachment)Attach(Attachment.CreateObjectAsXmlAttachment(attachmentName, obj, xmlSerializer));
        }

        #region Implementation template methods
        /// <summary>
        /// Gets a writer for the stream with the specified name.
        /// </summary>
        /// <param name="streamName">The stream name, never null</param>
        /// <returns>The log stream writer</returns>
        protected abstract TestLogStreamWriter GetLogStreamWriterImpl(string streamName);

        /// <summary>
        /// Adds an attachment to the execution log.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimize this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="attachment">The attachment to write, never null</param>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        protected abstract void AttachImpl(Attachment attachment);
        #endregion
    }
}