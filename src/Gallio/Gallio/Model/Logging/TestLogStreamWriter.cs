using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// A log stream writer provides methods for writing rich text with embedded attachments,
    /// nested sections and hidden semantic markers to a particular stream within a log writer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The operations on this interface are thread-safe.
    /// </para>
    /// <para>
    /// The object extends <see cref="MarshalByRefObject" /> so instances may be
    /// accessed by remote clients if required.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class TestLogStreamWriter : TextWriter
    {
        private readonly string streamName;

        /// <summary>
        /// Creates a log stream writer.
        /// </summary>
        /// <param name="streamName">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        protected TestLogStreamWriter(string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");

            this.streamName = streamName;
            NewLine = "\n";
        }

        /// <summary>
        /// Gets the name of the log stream being written.
        /// </summary>
        public string StreamName
        {
            get { return streamName; }
        }

        /// <inheritdoc />
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        /// <inheritdoc />
        new public string NewLine
        {
            get { return base.NewLine; }
            private set { base.NewLine = value; }
        }

        /// <inheritdoc />
        public sealed override void Flush()
        {
            FlushImpl();
        }

        /// <inheritdoc />
        public override void Write(char value)
        {
            WriteImpl(new string(value, 1));
        }

        /// <inheritdoc />
        public sealed override void Write(string value)
        {
            if (value != null)
                WriteImpl(value);
        }

        /// <inheritdoc />
        public sealed override void Write(char[] buffer, int index, int count)
        {
            WriteImpl(new String(buffer, index, count));
        }

        /// <summary>
        /// Writes an exception to the log.
        /// </summary>
        /// <param name="exception">The exception to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(Exception exception)
        {
            WriteException(exception, null);
        }

        /// <summary>
        /// Writes an exception to the log within its own section.
        /// </summary>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionName">The section name, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(Exception exception, string sectionName)
        {
            if (exception == null)
                throw new ArgumentNullException(@"exception");

            WriteException(new ExceptionData(exception), sectionName);
        }

        /// <summary>
        /// Writes an exception to the log.
        /// </summary>
        /// <param name="exception">The exception data to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(ExceptionData exception)
        {
            WriteException(exception, null);
        }

        /// <summary>
        /// Writes an exception to the log within its own section.
        /// </summary>
        /// <param name="exception">The exception data to write</param>
        /// <param name="sectionName">The section name, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(ExceptionData exception, string sectionName)
        {
            if (exception == null)
                throw new ArgumentNullException(@"exception");

            if (sectionName != null)
                BeginSectionImpl(sectionName);

            StackTraceFilter.FilterException(exception).WriteTo(this);

            if (sectionName != null)
                EndImpl();
        }

        /// <summary>
        /// <para>
        /// Begins a section with the specified name.  Maybe be nested.
        /// </para>
        /// <para>
        /// A section groups together related content in the test log to make it
        /// easier to distinguish.  The section name is used as a heading.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// using (Log.Default.BeginSection("Doing something interesting"))
        /// {
        ///     Log.Default.WriteLine("Ah ha!");
        /// }
        /// </code>
        /// </example>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>A Disposable object that calls <see cref="End" /> when disposed.  This
        /// is a convenience for use with the C# "using" statement.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionName"/> is null</exception>
        public IDisposable BeginSection(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException(@"sectionName");

            BeginSectionImpl(sectionName);

            return new RegionCookie(this);
        }

        /// <summary>
        /// <para>
        /// Begins a marked region of the specified class.  Maybe be nested.
        /// </para>
        /// <para>
        /// A marker is a hidden tag that labels its contents with a semantic class.
        /// It is roughly equivalent in operation to an HTML "span" tag.  Various tools
        /// may inspect the markers and modify the presentation accordingly.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// using (Log.Default.BeginMarker("exception"))
        /// {
        ///     Log.Default.WriteLine(someException);
        /// }
        /// </code>
        /// </example>
        /// <param name="class">The marker class identifier that describes its semantics</param>
        /// <returns>A Disposable object that calls <see cref="End" /> when disposed.  This
        /// is a convenience for use with the C# "using" statement.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="MarkerClasses.Validate"/></exception>
        public IDisposable BeginMarker(string @class)
        {
            MarkerClasses.Validate(@class);

            BeginMarkerImpl(@class);

            return new RegionCookie(this);
        }

        /// <summary>
        /// <para>
        /// Ends the region most recently started with one of the Begin* methods.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section</exception>
        public void End()
        {
            EndImpl();
        }

        /// <summary>
        /// Embeds an attachment into the stream.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="TestLogWriter.Attach" /> or <see cref="TestLogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to embed</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.Attach"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public Attachment Embed(Attachment attachment)
        {
            EmbedImpl(attachment);
            return attachment;
        }

        /// <summary>
        /// Embeds an existing attachment into the stream.  This method can be used to
        /// repeatedly embed an existing attachment at multiple points in multiple
        /// streams without needing to keep the <see cref="Attachment" /> instance
        /// itself around.  This can help to reduce memory footprint since the
        /// original <see cref="Attachment" /> instance can be garbage collected shortly
        /// after it is first attached.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="TestLogWriter.Attach" /> or <see cref="TestLogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachmentName">The name of the existing attachment to embed</param>
        /// <seealso cref="TestLogWriter.Attach"/>
        /// <seealso cref="TestLogStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been attached to the log</exception>
        public void EmbedExisting(string attachmentName)
        {
            EmbedExistingImpl(attachmentName);
        }

        /// <summary>
        /// Embeds an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedPlainText(string attachmentName, string text)
        {
            return (TextAttachment)Embed(Attachment.CreatePlainTextAttachment(attachmentName, text));
        }

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedHtml(string attachmentName, string html)
        {
            return (TextAttachment)Embed(Attachment.CreateHtmlAttachment(attachmentName, html));
        }

        /// <summary>
        /// Embeds an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedXHtml(string attachmentName, string xhtml)
        {
            return (TextAttachment)Embed(Attachment.CreateXHtmlAttachment(attachmentName, xhtml));
        }

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedXml(string attachmentName, string xml)
        {
            return (TextAttachment)Embed(Attachment.CreateXmlAttachment(attachmentName, xml));
        }

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public BinaryAttachment EmbedImage(string attachmentName, Image image)
        {
            return (BinaryAttachment)Embed(Attachment.CreateImageAttachment(attachmentName, image));
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedObjectAsXml(string attachmentName, object obj)
        {
            return EmbedObjectAsXml(attachmentName, obj, null);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.AttachObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment EmbedObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return (TextAttachment)Embed(Attachment.CreateObjectAsXmlAttachment(attachmentName, obj, xmlSerializer));
        }

        #region Implementation template methods
        /// <summary>
        /// Writes a text string to the execution log.
        /// </summary>
        /// <param name="text">The text to write, never null</param>
        protected abstract void WriteImpl(string text);

        /// <summary>
        /// Adds an attachment to the execution log and embeds it in this stream
        /// at the current location.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimize this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="attachment">The attachment to write, never null</param>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already written</exception>
        protected abstract void EmbedImpl(Attachment attachment);

        /// <summary>
        /// Adds previously attached attachment to the execution log and embeds it in
        /// this stream at the current location.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimize this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to write, never null</param>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been attached to the log</exception>
        protected abstract void EmbedExistingImpl(string attachmentName);

        /// <summary>
        /// Begins a section.
        /// </summary>
        /// <param name="sectionName">The name of the section to begin, never null</param>
        protected abstract void BeginSectionImpl(string sectionName);

        /// <summary>
        /// Begins a marked region.
        /// </summary>
        /// <param name="class">The marker identifier, already validated</param>
        protected abstract void BeginMarkerImpl(string @class);

        /// <summary>
        /// Ends the current region started with one of the Begin* methods.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current nested region</exception>
        protected abstract void EndImpl();

        /// <summary>
        /// Flushes the stream.
        /// </summary>
        protected virtual void FlushImpl()
        {
        }
        #endregion

        #region Overrides to hide irrelevant TextWriter behavior
        /// <summary>
        /// This method does not make sense for a log stream writer because
        /// a stream cannot be closed independently of its containing log.
        /// </summary>
        new private void Close()
        {
        }

        /// <summary>
        /// This method does not make sense for a log stream writer because
        /// a stream cannot be closed independently of its containing log.
        /// </summary>
        new private void Dispose()
        {
        }

        /// <summary>
        /// This method does not make sense for a log stream writer because
        /// a stream cannot be closed independently of its containing log.
        /// </summary>
        protected sealed override void Dispose(bool disposing)
        {
        }
        #endregion

        private sealed class RegionCookie : IDisposable
        {
            private readonly TestLogStreamWriter writer;

            public RegionCookie(TestLogStreamWriter writer)
            {
                this.writer = writer;
            }

            public void Dispose()
            {
                writer.End();
            }
        }
    }
}