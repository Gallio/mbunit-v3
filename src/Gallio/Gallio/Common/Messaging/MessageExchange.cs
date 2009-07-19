// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.Threading;
using Gallio.Common.Remoting;
using Gallio.Common.Validation;

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
        /// <exception cref="ValidationException">Thrown if <paramref name="message"/> is not valid.</exception>
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
