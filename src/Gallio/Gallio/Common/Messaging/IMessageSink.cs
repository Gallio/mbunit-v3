using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// Publishes messages directly to a subscriber.
    /// </summary>
    public interface IMessageSink
    {
        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        void Publish(Message message);
    }
}
