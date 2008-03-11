using Gallio.Model.Execution;

namespace Gallio.Framework
{
    internal sealed class InternalLogStreamWriter : LogStreamWriter
    {
        private readonly ITestLogWriter logWriter;

        public InternalLogStreamWriter(ITestLogWriter logWriter, string streamName) : base(streamName)
        {
            this.logWriter = logWriter;
        }

        protected override void WriteImpl(string text)
        {
            logWriter.Write(StreamName, text);
        }

        protected override void EmbedImpl(Attachment attachment)
        {
            attachment.Attach(logWriter);
            EmbedExistingImpl(attachment.Name);
        }

        protected override void EmbedExistingImpl(string attachmentName)
        {
            logWriter.Embed(StreamName, attachmentName);
        }

        protected override void BeginSectionImpl(string sectionName)
        {
            logWriter.BeginSection(StreamName, sectionName);
        }

        protected override void EndSectionImpl()
        {
            logWriter.EndSection(StreamName);
        }
    }
}
