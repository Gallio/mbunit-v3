// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Runtime.Remoting.Messaging;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A remote event listener is a serializable wrapper for another listener.
    /// The wrapper can be passed to another AppDomain and communication occurs over
    /// .Net remoting.
    /// </summary>
    /// <remarks>
    /// The implementation is defined so as to guard against latency and failures in the remoting channel.
    /// </remarks>
    [Serializable]
    public sealed class RemoteEventListener : IEventListener
    {
        private readonly Forwarder forwarder;

        /// <summary>
        /// Creates a wrapper for the specified listener.
        /// </summary>
        /// <param name="listener">The listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listener"/> is null</exception>
        public RemoteEventListener(IEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            forwarder = new Forwarder(listener);
        }

        /// <inheritdoc />
        public void NotifyMessageEvent(MessageEventArgs e)
        {
            try
            {
                forwarder.NotifyMessageEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a Message event to the remote event listener: " + ex);
            }
        }

        /// <inheritdoc />
        public void NotifyLifecycleEvent(LifecycleEventArgs e)
        {
            try
            {
                forwarder.NotifyTestLifecycleEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a TestLifecycle event to the remote event listener: " + ex);
            }
        }

        /// <inheritdoc />
        public void NotifyExecutionLogEvent(ExecutionLogEventArgs e)
        {
            try
            {
                forwarder.NotifyTestExecutionLogEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a TestExecutionLog event to the remote event listener: " + ex);
            }
        }

        /// <summary>
        /// The forwarding event listener forwards events to the host's event listener.
        /// </summary>
        private sealed class Forwarder : MarshalByRefObject
        {
            private readonly IEventListener listener;

            public Forwarder(IEventListener listener)
            {
                this.listener = listener;
            }

            public void NotifyMessageEvent(MessageEventArgs e)
            {
                listener.NotifyMessageEvent(e);
            }

            public void NotifyTestLifecycleEvent(LifecycleEventArgs e)
            {
                listener.NotifyLifecycleEvent(e);
            }

            public void NotifyTestExecutionLogEvent(ExecutionLogEventArgs e)
            {
                listener.NotifyExecutionLogEvent(e);
            }
        }
    }
}