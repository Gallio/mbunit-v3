using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Gallio.Common.Remoting;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// A message exchange mediates interactions between a server and a client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is interim API to be used until the new message broker is ready.
    /// </para>
    /// </remarks>
    public class MessageExchange : LongLivedMarshalByRefObject, IMessageExchangeLink, IMessageSink
    {
        private readonly IMessageSink messageSink;
        private readonly Queue<Message> receiveQueue;

        /// <summary>
        /// Creates a message exchange.
        /// </summary>
        /// <param name="messageSink">The message sink to which messages sent by the client should
        /// be directed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="messageSink"/> is null.</exception>
        public MessageExchange(IMessageSink messageSink)
        {
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");

            this.messageSink = messageSink;

            receiveQueue = new Queue<Message>();
        }

        /// <summary>
        /// Gets an interface that a client can use to asynchronously receive and
        /// synchronously send messages.
        /// </summary>
        /// <returns>The client link.</returns>
        public IMessageExchangeLink GetClientLink()
        {
            return this;
        }

        /// <summary>
        /// Sends a message to the client.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        public void Publish(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            message.Validate();

            lock (receiveQueue)
            {
                receiveQueue.Enqueue(message);

                Monitor.PulseAll(receiveQueue);
            }
        }

        /// <summary>
        /// Waits for all published messages to be received by the client.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>True if all messages were received, false if a timeout occurred.</returns>
        public bool WaitForPublishedMessagesToBeReceived(TimeSpan timeout)
        {
            lock (receiveQueue)
            {
                for (; ; )
                {
                    if (receiveQueue.Count == 0)
                        return true;

                    if (timeout.Ticks <= 0)
                        return false;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (!Monitor.Wait(receiveQueue, timeout))
                        return false;
                    timeout -= stopwatch.Elapsed;
                }
            }
        }

        Message IMessageExchangeLink.Receive(TimeSpan timeout)
        {
            lock (receiveQueue)
            {
                for (; ; )
                {
                    if (receiveQueue.Count != 0)
                    {
                        Message message = receiveQueue.Dequeue();
                        Monitor.PulseAll(receiveQueue);
                        return message;
                    }

                    if (timeout.Ticks <= 0)
                        return null;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (! Monitor.Wait(receiveQueue, timeout))
                        return null;
                    timeout -= stopwatch.Elapsed;
                }
            }
        }

        void IMessageExchangeLink.Send(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            message.Validate();

            messageSink.Publish(message);
        }
    }
}