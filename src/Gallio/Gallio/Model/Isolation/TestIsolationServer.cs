// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Common.Diagnostics;
using Gallio.Model.Isolation.Messages;
using Gallio.Common.Messaging;
using Gallio.Common.Remoting;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// Manages the server end of a remote connection that dispatches isolated tasks to a client.
    /// </summary>
    public class TestIsolationServer : IDisposable
    {
        private const string MessageExchangeLinkServicePrefix = "TestIsolationServer.MessageExchangeLink.";

        private readonly BinaryIpcClientChannel clientChannel;
        private readonly BinaryIpcServerChannel serverChannel;
        private readonly MessageExchange messageExchange;

        private readonly Dictionary<Guid, IsolatedTaskState> activeTasks;

        /// <summary>
        /// Creates a test isolation server.
        /// </summary>
        /// <param name="ipcPortName">The IPC port name.</param>
        /// <param name="linkId">The unique id of the client/server pair.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ipcPortName"/> is null.</exception>
        public TestIsolationServer(string ipcPortName, Guid linkId)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");

            serverChannel = new BinaryIpcServerChannel(ipcPortName);
            clientChannel = new BinaryIpcClientChannel(ipcPortName + ".ServerCallback");
            activeTasks = new Dictionary<Guid, IsolatedTaskState>();

            MessageConsumer messageConsumer = new MessageConsumer()
                .Handle<IsolatedTaskFinishedMessage>(HandleIsolatedTaskFinished);

            messageExchange = new MessageExchange(messageConsumer);
            serverChannel.RegisterService(GetMessageExchangeLinkServiceName(linkId), messageExchange);
        }

        internal static string GetMessageExchangeLinkServiceName(Guid uniqueId)
        {
            return MessageExchangeLinkServicePrefix + uniqueId;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the server.
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
        /// Runs an isolated task on the client.
        /// </summary>
        /// <param name="isolatedTaskType">The type of isolated task to run.</param>
        /// <param name="args">The isolated task arguments.</param>
        /// <returns>The isolated task result.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="isolatedTaskType"/> is null.</exception>
        public object RunIsolatedTaskOnClient(Type isolatedTaskType, object[] args)
        {
            if (isolatedTaskType == null)
                throw new ArgumentNullException("isolatedTaskType");

            Guid id = Guid.NewGuid();
            IsolatedTaskState state = new IsolatedTaskState();

            try
            {
                lock (activeTasks)
                    activeTasks.Add(id, state);

                messageExchange.Publish(new RunIsolatedTaskMessage()
                {
                    Id = id,
                    IsolatedTaskType = isolatedTaskType,
                    Arguments = args
                });

                return state.WaitForCompletion();
            }
            finally
            {
                lock (activeTasks)
                    activeTasks.Remove(id);
            }
        }

        /// <summary>
        /// Instructs a client to shutdown nicely.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this method is not called, the client will still eventually shut down when the
        /// server is disposed but it may take longer and may result in an error being
        /// logged on the client.
        /// </para>
        /// </remarks>
        /// <param name="timeout">The maximum amount of time to wait for the client to receive the shutdown message.</param>
        public void Shutdown(TimeSpan timeout)
        {
            messageExchange.Publish(new ShutdownMessage());
            messageExchange.WaitForPublishedMessagesToBeReceived(timeout);
        }

        private void HandleIsolatedTaskFinished(IsolatedTaskFinishedMessage message)
        {
            lock (activeTasks)
            {
                IsolatedTaskState state;
                if (activeTasks.TryGetValue(message.Id, out state))
                    state.Finished(message.Result, message.Exception);
            }
        }

        private sealed class IsolatedTaskState
        {
            private object result;
            private ExceptionData exception;

            private readonly ManualResetEvent finish;

            public IsolatedTaskState()
            {
                finish = new ManualResetEvent(false);
            }

            public object WaitForCompletion()
            {
                finish.WaitOne();

                if (exception != null)
                    throw new TestIsolationException(string.Format("The isolated task thread an exception: {0}", exception));
                return result;
            }

            public void Finished(object result, ExceptionData exception)
            {
                this.result = result;
                this.exception = exception;
                finish.Set();
            }
        }
    }
}
