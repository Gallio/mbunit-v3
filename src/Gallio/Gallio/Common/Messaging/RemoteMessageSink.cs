using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Remoting;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// Wraps a <see cref="IMessageSink"/> so that messages can be sent remotely.
    /// </summary>
    [Serializable]
    public class RemoteMessageSink : IMessageSink
    {
        private readonly Forwarder forwarder;

        /// <summary>
        /// Creates a remote message sink.
        /// </summary>
        /// <param name="messageSink">The message sink to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="messageSink"/> is null.</exception>
        public RemoteMessageSink(IMessageSink messageSink)
        {
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");

            forwarder = new Forwarder(messageSink);
        }

        /// <inheritdoc />
        public void Publish(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            forwarder.Publish(message);
        }

        private sealed class Forwarder : LongLivedMarshalByRefObject
        {
            private readonly IMessageSink messageSink;

            public Forwarder(IMessageSink messageSink)
            {
                this.messageSink = messageSink;
            }

            public void Publish(Message message)
            {
                messageSink.Publish(message);
            }
        }
    }
}
