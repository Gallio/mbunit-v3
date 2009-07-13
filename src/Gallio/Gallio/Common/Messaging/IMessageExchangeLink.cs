using System;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// An interface implemented by the server and registered on the server remoting channel
    /// to allow message exchange with the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is interim API to be used until the new message broker is ready.
    /// </para>
    /// </remarks>
    public interface IMessageExchangeLink
    {
        /// <summary>
        /// Gets the next message.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for a message.</param>
        /// <returns>The next message, or null if a timeout occurred.</returns>
        Message Receive(TimeSpan timeout);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(Message message);
    }
}