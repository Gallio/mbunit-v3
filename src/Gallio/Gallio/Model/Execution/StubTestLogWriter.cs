using System;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A stub implementation of <see cref="ITestLogWriter" /> that logs output to <see cref="Console.Out" />.
    /// </summary>
    /// <seealso cref="StubTestContextTracker" />
    public class StubTestLogWriter : BaseTestLogWriter
    {
        private bool needNewline;

        /// <inheritdoc />
        protected override void CloseImpl()
        {
        }

        /// <inheritdoc />
        protected override void AttachTextImpl(string attachmentName, string contentType, string text)
        {
            Attach(attachmentName, contentType);
        }

        /// <inheritdoc />
        protected override void AttachBytesImpl(string attachmentName, string contentType, byte[] bytes)
        {
            Attach(attachmentName, contentType);
        }

        private void Attach(string attachmentName, string contentType)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Attachment '{0}': {1}]", attachmentName, contentType);
        }

        /// <inheritdoc />
        protected override void WriteImpl(string streamName, string text)
        {
            Console.Out.Write(text);

            needNewline = ! text.EndsWith("\n");
        }

        /// <inheritdoc />
        protected override void EmbedImpl(string streamName, string attachmentName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Embedded Attachment '{0}']", attachmentName);
        }

        /// <inheritdoc />
        protected override void BeginSectionImpl(string streamName, string sectionName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[Begin Section '{0}']", sectionName);
        }

        /// <inheritdoc />
        protected override void EndSectionImpl(string streamName)
        {
            WriteNewlineIfNeeded();
            Console.Out.WriteLine("[End Section]");
        }

        private void WriteNewlineIfNeeded()
        {
            if (needNewline)
            {
                needNewline = false;
                Console.Out.WriteLine();
            }
        }
    }
}
