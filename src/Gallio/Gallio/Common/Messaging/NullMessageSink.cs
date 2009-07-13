using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// A null message sink discards all messages sent to it.
    /// </summary>
    public class NullMessageSink : IMessageSink
    {
        /// <summary>
        /// Gets the singleton instance of the null message sink.
        /// </summary>
        public static readonly NullMessageSink Instance = new NullMessageSink();

        private NullMessageSink()
        {
        }

        /// <inheritdoc />
        public void Publish(Message message)
        {
        }
    }
}
