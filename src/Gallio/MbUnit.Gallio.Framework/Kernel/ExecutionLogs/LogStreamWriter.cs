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

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.ExecutionLogs;

namespace MbUnit.Framework.Kernel.ExecutionLogs
{
    /// <summary>
    /// A log stream writer provides methods for writing text and embedded attachments to
    /// a named stream within a log.  Each log may contain many streams.  Each stream
    /// may be further subdivided into sections.
    /// </summary>
    /// <remarks>
    /// The operations on this interface are thread-safe.
    /// </remarks>
    /// <seealso cref="LogWriter"/>
    public abstract class LogStreamWriter : TextWriter
    {
        private readonly string streamName;

        /// <summary>
        /// Creates a log stream writer.
        /// </summary>
        /// <param name="streamName">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        protected LogStreamWriter(string streamName)
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
        /// Writes an exception to the log within its own section with the name "Exception".
        /// </summary>
        /// <remarks>
        /// If the exception is a <see cref="ClientException" /> then only its message and
        /// inner exception are written.
        /// </remarks>
        /// <param name="exception">The exception to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null</exception>
        public void WriteException(Exception exception)
        {
            WriteException(exception, "Exception");
        }

        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <remarks>
        /// If the exception is a <see cref="ClientException" /> then only its message and
        /// inner exception are written.
        /// </remarks>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionName">The section name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>,
        /// or <paramref name="sectionName"/> is null</exception>
        public void WriteException(Exception exception, string sectionName)
        {
            if (exception == null)
                throw new ArgumentNullException(@"exception");
            if (sectionName == null)
                throw new ArgumentNullException(@"sectionName");

            BeginSectionImpl(sectionName);

            ClientException clientException = exception as ClientException;
            if (clientException != null)
            {
                WriteLine(clientException.Message);

                if (clientException.InnerException != null)
                    WriteImpl(clientException.InnerException.ToString());
            }
            else
            {
                WriteImpl(exception.ToString());
            }

            EndSectionImpl();
        }

        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <remarks>
        /// If the exception is a <see cref="ClientException" /> then only its message and
        /// inner exception are written.
        /// </remarks>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionNameFormat">The section name format string</param>
        /// <param name="sectionNameArgs">The section name arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>,
        /// <paramref name="sectionNameFormat"/> or <paramref name="sectionNameArgs"/> is null</exception>
        public void WriteException(Exception exception, string sectionNameFormat, params object[] sectionNameArgs)
        {
            WriteException(exception, String.Format(sectionNameFormat, sectionNameArgs));
        }

        /// <summary>
        /// Begins a section with the specified name.
        /// Execution log sections may be nested.
        /// </summary>
        /// <example>
        /// <code>
        /// using (Log.BeginSection("Doing something interesting"))
        /// {
        ///     Log.WriteLine("Ah ha!");
        /// }
        /// </code>
        /// </example>
        /// <param name="sectionName">The name of the section</param>
        /// <returns>A Disposable object that calls <see cref="EndSection" /> when disposed.  This
        /// is a convenience for using the C# "using" statement to contain log stream sections.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionName"/> is null</exception>
        public IDisposable BeginSection(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException(@"sectionName");

            BeginSectionImpl(sectionName);

            return new SectionCookie(this);
        }

        /// <summary>
        /// Ends the current section.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section</exception>
        public void EndSection()
        {
            EndSectionImpl();
        }

        /// <summary>
        /// Embeds an attachment into the stream.
        /// </summary>
        /// <remarks>
        /// Only one copy of an attachment instance is saved with an execution log even if
        /// <see cref="LogWriter.Attach" /> or <see cref="LogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to embed</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.Attach"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already attached or embedded</exception>
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
        /// <see cref="LogWriter.Attach" /> or <see cref="LogStreamWriter.Embed" /> are
        /// called multiple times with the same instance.  However, an attachment instance
        /// can be embedded multiple times into multiple execution log streams since each
        /// embedded copy is represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachmentName">The name of the existing attachment to embed</param>
        /// <seealso cref="LogWriter.Attach"/>
        /// <seealso cref="LogStreamWriter.Embed"/>
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
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        public TextAttachment EmbedPlainText(string text)
        {
            return (TextAttachment)Embed(AttachmentUtils.CreatePlainTextAttachment(null, text));
        }

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        public TextAttachment EmbedHtml(string html)
        {
            return (TextAttachment)Embed(AttachmentUtils.CreateHtmlAttachment(null, html));
        }

        /// <summary>
        /// Embeds an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        public XmlAttachment EmbedXHtml(string xhtml)
        {
            return (XmlAttachment)Embed(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
        }

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        public XmlAttachment EmbedXml(string xml)
        {
            return (XmlAttachment)Embed(AttachmentUtils.CreateXmlAttachment(null, xml));
        }

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        public BinaryAttachment EmbedImage(Image image)
        {
            return (BinaryAttachment)Embed(AttachmentUtils.CreateImageAttachment(null, image));
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public XmlAttachment EmbedObjectAsXml(object obj)
        {
            return EmbedObjectAsXml(obj, null);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="obj">The object to serialize and embed, must not be null</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="LogWriter.AttachObjectAsXml(object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return (XmlAttachment)Embed(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
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
        /// Ends the current section.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section</exception>
        protected abstract void EndSectionImpl();

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

        private sealed class SectionCookie : IDisposable
        {
            private readonly LogStreamWriter writer;

            public SectionCookie(LogStreamWriter writer)
            {
                this.writer = writer;
            }

            public void Dispose()
            {
                writer.EndSection();
            }
        }
    }
}
