using Gallio.Model.Execution;

namespace Gallio.Framework
{
    internal sealed class InternalLogWriter : LogWriter
    {
        private readonly ITestLogWriter logWriter;

        public InternalLogWriter(ITestLogWriter logWriter)
        {
            this.logWriter = logWriter;
        }

        protected override LogStreamWriter GetLogStreamWriterImpl(string streamName)
        {
            return new InternalLogStreamWriter(logWriter, streamName);
        }

        protected override void AttachImpl(Attachment attachment)
        {
            attachment.Attach(logWriter);
        }
    }
}
