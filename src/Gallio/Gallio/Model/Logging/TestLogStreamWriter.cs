// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Model.Diagnostics;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// A log stream writer provides methods for writing rich structured text with embedded
    /// attachments, nested sections and hidden semantic markers to a particular stream within
    /// a log writer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The operations on this interface are thread-safe.
    /// </para>
    /// <para>
    /// The object extends <see cref="MarshalByRefObject" /> so instances may be
    /// accessed by remote clients if required.
    /// </para>
    /// <para>
    /// Newlines are always normalized to LFs ('\n') only.  Any CRs ('\r') that are
    /// written are automatically stripped.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestLogWriter"/>
    public class TestLogStreamWriter : TextWriter
    {
        private readonly TestLogWriter container;
        private readonly string streamName;

        /// <summary>
        /// Creates a log stream writer.
        /// </summary>
        /// <param name="container">The containing log writer</param>
        /// <param name="streamName">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="container"/>
        /// or <paramref name="streamName"/> is null</exception>
        public TestLogStreamWriter(TestLogWriter container, string streamName)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (streamName == null)
                throw new ArgumentNullException(@"streamName");

            this.container = container;
            this.streamName = streamName;

            NewLine = "\n";
        }

        /// <summary>
        /// Gets the containing log writer.
        /// </summary>
        public TestLogWriter Container
        {
            get { return container; }
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
            container.StreamFlush(streamName);
        }

        /// <inheritdoc />
        public override void Write(char value)
        {
            container.StreamWrite(streamName, new string(value, 1));
        }

        /// <inheritdoc />
        public sealed override void Write(string value)
        {
            if (value != null)
                container.StreamWrite(streamName, value);
        }

        /// <inheritdoc />
        public sealed override void Write(char[] buffer, int index, int count)
        {
            container.StreamWrite(streamName, new String(buffer, index, count));
        }

        /// <inheritdoc />
        public sealed override void WriteLine(object value)
        {
            Write(value);
            WriteLine();
        }

        /// <inheritdoc />
        public override void Write(object value)
        {
            ITestLogStreamWritable writable = value as ITestLogStreamWritable;
            if (writable != null)
                writable.WriteTo(this);
            else
                base.Write(value);
        }

        /// <summary>
        /// <para>
        /// Writes a test log stream writable object to the stream.
        /// </para>
        /// </summary>
        /// <param name="obj">The object to write, or null if none</param>
        public void Write(ITestLogStreamWritable obj)
        {
            if (obj != null)
                obj.WriteTo(this);
        }

        /// <summary>
        /// <para>
        /// Writes an exception.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The exception will not be terminated by a new line.
        /// </remarks>
        /// <param name="exception">The exception to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(Exception exception)
        {
            WriteException(exception, null);
        }

        /// <summary>
        /// <para>
        /// Writes an exception within its own section.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The exception will not be terminated by a new line.
        /// </remarks>
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
        /// <para>
        /// Writes an exception.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The exception will not be terminated by a new line.
        /// </remarks>
        /// <param name="exception">The exception data to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(ExceptionData exception)
        {
            WriteException(exception, null);
        }

        /// <summary>
        /// <para>
        /// Writes an exception within its own section which provides additional cues for interpretation.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The exception will not be terminated by a new line.
        /// </remarks>
        /// <param name="exception">The exception data to write</param>
        /// <param name="sectionName">The section name, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(ExceptionData exception, string sectionName)
        {
            if (exception == null)
                throw new ArgumentNullException(@"exception");

            using (sectionName != null ? BeginSection(sectionName) : null)
                StackTraceFilter.FilterException(exception).WriteTo(this);
        }

        /// <summary>
        /// <para>
        /// Writes highlighted text.  Highlights can be used to
        /// emphasize important information such differences between similar expected
        /// and actual values.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method that simply encapsulates the highlighted text within a
        /// marked region of type <see cref="Marker.Highlight" />.
        /// </remarks>
        /// <param name="text">The text to write, or null if none</param>
        public void WriteHighlighted(string text)
        {
            using (BeginMarker(Marker.Highlight))
                Write(text);
        }

        /// <summary>
        /// <para>
        /// Writes an ellipsis to indicate where content has been elided for brevity.
        /// An ellipsis may be used, for example, when printing assertion failures to clearly
        /// identify sections where the user is not being presented all of the information
        /// because it was too long and had to be truncated.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This is a convenience method that simply encapsulates "..." within a
        /// marked region of type <see cref="Marker.Ellipsis" />.  However, tools
        /// may reinterpret the special marker to make the "..." less ambiguous.
        /// </remarks>
        public void WriteEllipsis()
        {
            using (BeginMarker(Marker.Ellipsis))
                Write(@"...");
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

            container.StreamBeginSection(streamName, sectionName);

            return new RegionCookie(this);
        }

        /// <summary>
        /// <para>
        /// Begins a marked region.  Maybe be nested.
        /// </para>
        /// <para>
        /// A marker is a hidden tag that labels its contents with a semantic class.
        /// It is roughly equivalent in operation to an HTML "span" tag.  Various tools
        /// may inspect the markers and modify the presentation accordingly.
        /// </para>
        /// </summary>
        /// <example>
        /// <code>
        /// using (Log.BeginMarker(Marker.Monospace))
        /// {
        ///     Log.WriteLine(contents);
        /// }
        /// </code>
        /// </example>
        /// <param name="marker">The marker</param>
        /// <returns>A Disposable object that calls <see cref="End" /> when disposed.  This
        /// is a convenience for use with the C# "using" statement.</returns>
        public IDisposable BeginMarker(Marker marker)
        {
            container.StreamBeginMarker(streamName, marker);

            return new RegionCookie(this);
        }

        /// <summary>
        /// <para>
        /// Ends the region most recently started with one of the Begin* methods.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current nested region</exception>
        public void End()
        {
            container.StreamEnd(streamName);
        }

        /// <summary>
        /// Embeds an attachment.
        /// </summary>
        /// <remarks>
        /// An attachment instance can be embedded multiple times efficiently since each
        /// embedded copy is typically represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to embed</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="TestLogWriter.Attach"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public Attachment Embed(Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            container.Attach(attachment);
            container.StreamEmbed(streamName, attachment.Name);
            return attachment;
        }

        /// <summary>
        /// Embeds another copy of an existing attachment.  This method can be used to
        /// repeatedly embed an existing attachment at multiple points in multiple
        /// streams without needing to keep the <see cref="Attachment" /> instance
        /// itself around.  This can help to reduce memory footprint since the
        /// original <see cref="Attachment" /> instance can be garbage collected shortly
        /// after it is first attached.
        /// </summary>
        /// <remarks>
        /// An attachment instance can be embedded multiple times efficiently since each
        /// embedded copy is typically represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachmentName">The name of the existing attachment to embed</param>
        /// <seealso cref="TestLogWriter.Attach"/>
        /// <seealso cref="Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been previously attached</exception>
        public void EmbedExisting(string attachmentName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");

            container.StreamEmbed(streamName, attachmentName);
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
