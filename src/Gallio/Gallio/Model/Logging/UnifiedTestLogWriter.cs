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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A implementation of a log writer that assists in the implementation
    /// of log writers by serializing requests, performing basic argument
    /// and state validation, and reifying messages from stream writers in one place.
    /// </para>
    /// <para>
    /// The validation includes:
    /// <list type="bullet">
    /// <item>Checking arguments for null and valid values</item>
    /// <item>Ensuring that the writer has not been closed</item>
    /// <item>Ensuring that no attachment with the same name exists when adding an attachment</item>
    /// <item>Ensuring that there is an attachment with the specified name exists when embedding an attachment</item>
    /// <item>Ensuring that the nesting level of sections and markers is correct and balanced</item>
    /// </list>
    /// </para>
    /// </summary>
    [Serializable]
    public abstract class UnifiedTestLogWriter : TestLogWriter
    {
        private HashSet<string> attachmentNames;
        private Dictionary<string, int> streamDepths;
        private bool isClosed;

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
        /// Attaches an text attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="text">The text contents</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="text"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment with the same name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        public void AttachText(string attachmentName, string contentType, string text)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");
            if (contentType == null)
                throw new ArgumentNullException("contentType");
            if (text == null)
                throw new ArgumentNullException("text");

            lock (this)
            {
                ThrowIfClosed();
                PrepareToRegisterAttachment(attachmentName);
                AttachTextImpl(attachmentName, contentType, text);
                RegisterAttachment(attachmentName);
            }
        }

        /// <summary>
        /// Attaches an binary attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="bytes">The binary contents</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/>,
        /// <paramref name="contentType"/> or <paramref name="bytes"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment with the same name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        public void AttachBytes(string attachmentName, string contentType, byte[] bytes)
        {
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");
            if (contentType == null)
                throw new ArgumentNullException("contentType");
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            lock (this)
            {
                ThrowIfClosed();
                PrepareToRegisterAttachment(attachmentName);
                AttachBytesImpl(attachmentName, contentType, bytes);
                RegisterAttachment(attachmentName);
            }
        }

        /// <summary>
        /// Writes text to a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="text">The text to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or <paramref name="text"/>
        /// is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        public void Write(string streamName, string text)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (text == null)
                throw new ArgumentNullException("text");

            lock (this)
            {
                ThrowIfClosed();
                WriteImpl(streamName, text);
            }
        }

        /// <summary>
        /// Embeds an attachment into a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="attachmentName">The attachment name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/>
        /// or <paramref name="attachmentName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no attachment with the specified name</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        public void Embed(string streamName, string attachmentName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (attachmentName == null)
                throw new ArgumentNullException("attachmentName");

            lock (this)
            {
                ThrowIfClosed();
                EnsureAttachmentExists(attachmentName);
                EmbedImpl(streamName, attachmentName);
            }
        }

        /// <summary>
        /// Begins a section in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="sectionName">The name of the section</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="sectionName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        /// <seealso cref="End"/>
        public void BeginSection(string streamName, string sectionName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            lock (this)
            {
                ThrowIfClosed();
                BeginSectionImpl(streamName, sectionName);
                IncrementStreamDepth(streamName);
            }
        }

        /// <summary>
        /// Begins a marked region in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <param name="class">The marker class identifier that describes its semantics</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="class"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="MarkerClasses.Validate"/></exception>
        /// <seealso cref="End"/>
        public void BeginMarker(string streamName, string @class)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            MarkerClasses.Validate(@class);

            lock (this)
            {
                ThrowIfClosed();
                BeginMarkerImpl(streamName, @class);
                IncrementStreamDepth(streamName);
            }
        }

        /// <summary>
        /// Ends a region started with one of the Begin* methods in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no current section or region in the stream</exception>
        /// <exception cref="InvalidOperationException">Thrown if the log writer has been closed</exception>
        public void End(string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");

            lock (this)
            {
                ThrowIfClosed();
                PrepareToDecrementStreamDepth(streamName);
                EndImpl(streamName);
                DecrementStreamDepth(streamName);
            }
        }

        /// <summary>
        /// Closes the log writer.
        /// </summary>
        protected abstract void CloseImpl();

        /// <summary>
        /// Attaches an text attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name, unique, not null</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="text">The text contents, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void AttachTextImpl(string attachmentName, string contentType, string text);

        /// <summary>
        /// Attaches an binary attachment to the execution log.
        /// </summary>
        /// <param name="attachmentName">The attachment name, unique, not null</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="bytes">The binary contents, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void AttachBytesImpl(string attachmentName, string contentType, byte[] bytes);

        /// <summary>
        /// Writes text to a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, not null</param>
        /// <param name="text">The text to write, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void WriteImpl(string streamName, string text);

        /// <summary>
        /// Embeds an attachment into a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, known to exist, not null</param>
        /// <param name="attachmentName">The attachment name, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void EmbedImpl(string streamName, string attachmentName);

        /// <summary>
        /// Begins a section in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, not null</param>
        /// <param name="sectionName">The name of the section, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void BeginSectionImpl(string streamName, string sectionName);

        /// <summary>
        /// Begins a marked region in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, not null</param>
        /// <param name="class">The marker class identifier that describes its semantics, already validated</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void BeginMarkerImpl(string streamName, string @class);

        /// <summary>
        /// Ends a region started with one of the Begin* methods in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void EndImpl(string streamName);

        /// <inheritdoc />
        protected sealed override TestLogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            return new UnifiedTestLogStreamWriter(streamName, this);
        }

        /// <inheritdoc />
        protected sealed override void AttachImpl(Attachment attachment)
        {
            if (attachment is BinaryAttachment)
            {
                BinaryAttachment binaryAttachment = (BinaryAttachment)attachment;
                AttachBytes(binaryAttachment.Name, binaryAttachment.ContentType, binaryAttachment.Data);
            }
            else
            {
                TextAttachment textAttachment = (TextAttachment)attachment;
                AttachText(textAttachment.Name, textAttachment.ContentType, textAttachment.Text);
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
                streamDepths = new Dictionary<string,int>();

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

        private sealed class UnifiedTestLogStreamWriter : TestLogStreamWriter
        {
            private readonly UnifiedTestLogWriter logWriter;

            public UnifiedTestLogStreamWriter(string streamName, UnifiedTestLogWriter logWriter)
                : base(streamName)
            {
                this.logWriter = logWriter;
            }

            /// <inheritdoc />
            protected override void WriteImpl(string text)
            {
                logWriter.Write(StreamName, text);
            }

            /// <inheritdoc />
            protected override void EmbedImpl(Attachment attachment)
            {
                logWriter.Attach(attachment);
                EmbedExistingImpl(attachment.Name);
            }

            /// <inheritdoc />
            protected override void EmbedExistingImpl(string attachmentName)
            {
                logWriter.Embed(StreamName, attachmentName);
            }

            /// <inheritdoc />
            protected override void BeginSectionImpl(string sectionName)
            {
                logWriter.BeginSection(StreamName, sectionName);
            }

            /// <inheritdoc />
            protected override void BeginMarkerImpl(string @class)
            {
                logWriter.BeginMarker(StreamName, @class);
            }

            /// <inheritdoc />
            protected override void EndImpl()
            {
                logWriter.End(StreamName);
            }
        }
    }
}