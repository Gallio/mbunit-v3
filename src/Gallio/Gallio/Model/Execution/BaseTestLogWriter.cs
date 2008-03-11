using System;
using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// <para>
    /// Base implementation of a log writer that assists in the implementation
    /// of log writers by serializing requests and performing basic argument
    /// and state validation.
    /// </para>
    /// <para>
    /// The validation includes:
    /// <list type="bullet">
    /// <item>Checking arguments for null</item>
    /// <item>Ensuring that the writer has not been closed</item>
    /// <item>Ensuring that no attachment with the same name exists when adding an attachment</item>
    /// <item>Ensuring that there is an attachment with the specified name exists when embedding an attachment</item>
    /// <item>Ensuring that the nesting level of stream sections is correct such that all <see cref="EndSection" />
    /// calls are balanced by <see cref="BeginSection" /></item>
    /// </list>
    /// </para>
    /// </summary>
    public abstract class BaseTestLogWriter : ITestLogWriter
    {
        private HashSet<string> attachmentNames;
        private Dictionary<string, int> streamDepths;
        private bool isClosed;

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void EndSection(string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");

            lock (this)
            {
                ThrowIfClosed();
                PrepareToDecrementStreamDepth(streamName);
                EndSectionImpl(streamName);
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
        /// Ends a section in a particular stream of the execution log.
        /// </summary>
        /// <param name="streamName">The log stream name, not null</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running</exception>
        protected abstract void EndSectionImpl(string streamName);

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
    }
}
