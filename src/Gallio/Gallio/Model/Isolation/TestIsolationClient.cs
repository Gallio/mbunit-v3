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
using System.Diagnostics;
using System.Threading;
using Gallio.Common.Diagnostics;
using Gallio.Model.Isolation.Messages;
using Gallio.Common.Messaging;
using Gallio.Common.Policies;
using Gallio.Common.Remoting;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// Manages the client end of a remote connection that runs isolated tasks provided by the server.
    /// </summary>
    public class TestIsolationClient : IDisposable
    {
        private static readonly TimeSpan PollTimeout = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5);

        private readonly BinaryIpcClientChannel clientChannel;
        private readonly BinaryIpcServerChannel serverChannel;
        private readonly Guid linkId;

        /// <summary>
        /// Creates a test isolation client.
        /// </summary>
        /// <param name="ipcPortName">The IPC port name.</param>
        /// <param name="linkId">The unique id of the client/server pair.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ipcPortName"/> is null.</exception>
        public TestIsolationClient(string ipcPortName, Guid linkId)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");

            this.linkId = linkId;

            clientChannel = new BinaryIpcClientChannel(ipcPortName);
            serverChannel = new BinaryIpcServerChannel(ipcPortName + ".ClientCallback");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the client.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()"/> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                serverChannel.Dispose();
                clientChannel.Dispose();
            }
        }

        /// <summary>
        /// Runs isolated tasks until the server shuts down.
        /// </summary>
        public void Run()
        {
            try
            {
                var link = (IMessageExchangeLink)clientChannel.GetService(typeof(IMessageExchangeLink), TestIsolationServer.GetMessageExchangeLinkServiceName(linkId));
                using (new Timer(dummy => Ping(link), null, TimeSpan.Zero, PingInterval))
                {
                    for (; ; )
                    {
                        Message message = link.Receive(PollTimeout);
                        if (message == null)
                            continue;

                        if (message is ShutdownMessage)
                            break;

                        RunIsolatedTask(link, (RunIsolatedTaskMessage)message);
                    }
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while processing messages.  Assuming the server is no longer available.", ex);
            }
        }

        [DebuggerNonUserCode]
        private static void Ping(IMessageExchangeLink link)
        {
            try
            {
                link.Send(new PingMessage());
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while sending a ping.  Ignoring it.", ex);
            }
        }

        private static void RunIsolatedTask(IMessageExchangeLink link, RunIsolatedTaskMessage message)
        {
            object result;
            try
            {
                var isolatedTask = (IsolatedTask)Activator.CreateInstance(message.IsolatedTaskType);
                result = isolatedTask.Run(message.Arguments);
            }
            catch (Exception ex)
            {
                link.Send(new IsolatedTaskFinishedMessage()
                {
                    Id = message.Id,
                    Exception = new ExceptionData(ex)
                });
                return;
            }

            link.Send(new IsolatedTaskFinishedMessage()
            {
                Id = message.Id,
                Result = result
            });
        }
    }
}
