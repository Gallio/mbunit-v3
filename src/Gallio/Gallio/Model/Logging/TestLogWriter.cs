// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;
using Gallio.Common.Collections;

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
    /// Subclasses may assume that the following validation steps have been performed before
    /// the implementation methods are called:
    /// <list type="bullet">
    /// <item>Checking arguments for null and invalid values</item>
    /// <item>Ensuring that the writer has not been closed</item>
    /// <item>Ensuring that no attachment with the same name exists when adding an attachment</item>
    /// <item>Ensuring that there is an attachment with the specified name exists when embedding an attachment</item>
    /// <item>Ensuring that the nesting level of sections and markers is correct and balanced when End is called</item>
    /// </list>
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
        private HashSet<string> attachmentNames;
        private Dictionary<string, int> streamDepths;
        private bool isClosed;

        #region Log writer stream accessors

        /// <summary>
        /// Gets the stream writer for the built-in log stream where the <see cref="Console.Error" />
        /// stream for the test is recorded.
        /// </summary>
        public TestLogStreamWriter ConsoleError
        {
            get { return this[TestLogStreamNames.ConsoleError]; }
        }

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
        /// Gets the stream writer for the built-in log stream where diagnostic <see cref="Debug" />
        /// and <see cref="Trace" /> information is recorded.
        /// </summary>
        public TestLogStreamWriter DebugTrace
        {
            get { return this[TestLogStreamNames.DebugTrace]; }
        }

        /// <summary>
        /// Gets the stream writer for the built-in log stream.
        /// </summary>
        public TestLogStreamWriter Default
        {
            get { return this[TestLogStreamNames.Default]; }
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

        #endregion

        /// <summary>
        /// Returns true if the log writer is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return isClosed; }
        }

        /// <summary>
        /// Gets the log stream with the specified name.  If the stream
        /// does not exist, it is created on demand.
        /// </summary>
        /// <remarks>
        /// This property may return different instances of <see cref="Model.Logging.TestLogStreamWriter" />
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

                return GetStreamImpl(streamName);
            }
        }

        /// <summary>
        /// Closes the log writer.
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (!isClosed)
                {
                    CloseImpl();
                    isClosed = true;
                    attachmentNames = null;
                    streamDepths = null;
                }
            }
        }

        /// <summary>
        /// Flushes the log writer.
        /// </summary>
        public void Flush()
        {
            lock (this)
            {
                ThrowIfClosed();
                FlushImpl();
            }
        }

        /// <summary>
        /// Attaches an attachment to the log.
        /// </summary>
        /// <remarks>
        /// An attachment instance can be embedded multiple times efficiently since each
        /// embedded copy is typically represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to include</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public Attachment Attach(Attachment attachment)
        {
            lock (this)
            {
                ThrowIfClosed();
                PrepareToRegisterAttachment(attachment.Name);
                AttachImpl(attachment);
                RegisterAttachment(attachment.Name);
            }

            return attachment;
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachHtml(string attachmentName, string html)
        {
            return (TextAttachment) Attach(Attachment.CreateHtmlAttachment(attachmentName, html));
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public BinaryAttachment AttachImage(string attachmentName, Image image)
        {
            return (BinaryAttachment) Attach(Attachment.CreateImageAttachment(attachmentName, image));
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
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedObjectAsXml(string, object)"/>
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
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return (TextAttachment) Attach(Attachment.CreateObjectAsXmlAttachment(attachmentName, obj, xmlSerializer));
        }

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachPlainText(string attachmentName, string text)
        {
            return (TextAttachment) Attach(Attachment.CreatePlainTextAttachment(attachmentName, text));
        }

        /// <summary>
        /// Attaches an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachXHtml(string attachmentName, string xhtml)
        {
            return (TextAttachment) Attach(Attachment.CreateXHtmlAttachment(attachmentName, xhtml));
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach</param>
        /// <returns>The attachment</returns>
        /// <seealso cref="Model.Logging.TestLogStreamWriter.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name</exception>
        public TextAttachment AttachXml(string attachmentName, string xml)
        {
            return (TextAttachment) Attach(Attachment.CreateXmlAttachment(attachmentName, xml));
        }

        #region Implementation template methods

        /// <summary>
        /// Gets a writer for the stream with the specified name.
        /// </summary>
        /// <param name="streamName">The stream name, never null</param>
        /// <returns>The log stream writer</returns>
        protected virtual TestLogStreamWriter GetStreamImpl(string streamName)
        {
            return new TestLogStreamWriter(this, streamName);
        }

        /// <summary>
        /// Closes the log.
        /// </summary>
        /// <remarks>
        /// The implementation may prevent the log from being closed by throwing an
        /// exception.  When this happens, the log's <see cref="IsClosed" /> property
        /// will remain false.
        /// </remarks>
        protected virtual void CloseImpl()
        {
        }

        /// <summary>
        /// Flushes the log.
        /// </summary>
        /// <remarks>
        /// The implementation should flush all of the individual log streams.
        /// </remarks>
        protected virtual void FlushImpl()
        {
        }

        /// <summary>
        /// Adds an attachment to the log.
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

        /// <summary>
        /// Writes a text string to a log stream.
        /// </summary>
        /// <remarks>
        /// The implementation can assume that newlines are normalized to LFs ('\n') only
        /// and that CRs ('\r') have been stripped.
        /// </remarks>
        /// <param name="streamName">The log stream name</param>
        /// <param name="text">The text to write, never null</param>
        protected abstract void StreamWriteImpl(string streamName, string text);

        /// <summary>
        /// Embeds an attachment into a log stream.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimize this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="streamName">The log stream name</param>
        /// <param name="attachmentName">The name of the attachment to write, never null</param>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been previously attached</exception>
        protected abstract void StreamEmbedImpl(string streamName, string attachmentName);

        /// <summary>
        /// Begins a section in a log stream.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="sectionName">The name of the section to begin, never null</param>
        protected abstract void StreamBeginSectionImpl(string streamName, string sectionName);

        /// <summary>
        /// Begins a marked region in a log stream.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="marker">The marker</param>
        protected abstract void StreamBeginMarkerImpl(string streamName, Marker marker);

        /// <summary>
        /// Ends the current region started with one of the Begin* methods in a log stream.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no current nested region</exception>
        protected abstract void StreamEndImpl(string streamName);

        /// <summary>
        /// Flushes a log stream.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        protected virtual void StreamFlushImpl(string streamName)
        {
        }
        #endregion

        internal void StreamWrite(string streamName, string text)
        {
            lock (this)
            {
                ThrowIfClosed();

                text = NormalizeLineEndings(text);
                if (text.Length != 0)
                    StreamWriteImpl(streamName, text);
            }
        }

        internal void StreamEmbed(string streamName, string attachmentName)
        {
            lock (this)
            {
                ThrowIfClosed();
                EnsureAttachmentExists(attachmentName);
                StreamEmbedImpl(streamName, attachmentName);
            }
        }

        internal void StreamBeginSection(string streamName, string sectionName)
        {
            lock (this)
            {
                ThrowIfClosed();
                StreamBeginSectionImpl(streamName, sectionName);
                IncrementStreamDepth(streamName);
            }
        }

        internal void StreamBeginMarker(string streamName, Marker marker)
        {
            lock (this)
            {
                ThrowIfClosed();
                StreamBeginMarkerImpl(streamName, marker);
                IncrementStreamDepth(streamName);
            }
        }

        internal void StreamEnd(string streamName)
        {
            lock (this)
            {
                ThrowIfClosed();
                PrepareToDecrementStreamDepth(streamName);
                StreamEndImpl(streamName);
                DecrementStreamDepth(streamName);
            }
        }

        internal void StreamFlush(string streamName)
        {
            lock (this)
            {
                ThrowIfClosed();
                StreamFlushImpl(streamName);
            }
        }

        private void ThrowIfClosed()
        {
            if (isClosed)
                throw new InvalidOperationException("The log writer has been closed.");
        }

        private void PrepareToRegisterAttachment(string attachmentName)
        {
            if (attachmentNames != null && attachmentNames.Contains(attachmentName))
                throw new InvalidOperationException(String.Format("There is already an attachment named '{0}'.", attachmentName));
        }

        private void RegisterAttachment(string attachmentName)
        {
            if (attachmentNames == null)
                attachmentNames = new HashSet<string>();
            attachmentNames.Add(attachmentName);
        }

        private void EnsureAttachmentExists(string attachmentName)
        {
            if (attachmentNames == null || !attachmentNames.Contains(attachmentName))
                throw new InvalidOperationException(String.Format("There is no attachment named '{0}'.", attachmentName));
        }

        private void IncrementStreamDepth(string streamName)
        {
            if (streamDepths == null)
                streamDepths = new Dictionary<string, int>();

            int depth;
            streamDepths.TryGetValue(streamName, out depth);
            streamDepths[streamName] = depth + 1;
        }

        private void PrepareToDecrementStreamDepth(string streamName)
        {
            if (streamDepths == null || !streamDepths.ContainsKey(streamName))
                throw new InvalidOperationException(String.Format("Stream '{0}' does not currently have any open sections.", streamName));
        }

        private void DecrementStreamDepth(string streamName)
        {
            int depth = streamDepths[streamName];
            if (depth == 1)
                streamDepths.Remove(streamName);
            else
                streamDepths[streamName] = depth - 1;
        }

        private static string NormalizeLineEndings(string text)
        {
            return text.Replace("\r", "");
        }
    }
}