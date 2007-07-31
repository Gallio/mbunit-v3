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
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// <para>
    /// A remote event listener is a serializable wrapper for another listener.
    /// The wrapper can be passed to another AppDomain and communication occurs over
    /// .Net remoting.
    /// </para>
    /// <para>
    /// The implementation is defined so as to protect the sender from latency and failures
    /// in the remoting channel.
    /// </para>
    /// </summary>
    [Serializable]
    public sealed class RemoteEventListener : IEventListener
    {
        private readonly ForwardingEventListener forwardingEventListener;

        /// <summary>
        /// Creates a wrapper for the specified listener.
        /// </summary>
        /// <param name="listener">The listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listener"/> is null</exception>
        public RemoteEventListener(IEventListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            forwardingEventListener = new ForwardingEventListener(listener);
        }

        /// <inheritdoc />
        public void NotifyMessageEvent(MessageEventArgs e)
        {
            try
            {
                forwardingEventListener.NotifyMessageEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a Message event to the remote event listener: " + ex);
            }
        }

        /// <inheritdoc />
        public void NotifyTestLifecycleEvent(TestLifecycleEventArgs e)
        {
            try
            {
                forwardingEventListener.NotifyTestLifecycleEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a TestLifecycle event to the remote event listener: " + ex);
            }
        }

        /// <inheritdoc />
        public void NotifyTestExecutionLogEvent(TestExecutionLogEventArgs e)
        {
            try
            {
                forwardingEventListener.NotifyTestExecutionLogEvent(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to send a TestExecutionLog event to the remote event listener: " + ex);
            }
        }

        /// <summary>
        /// The forwarding event listener handles the recipient's end of the communication
        /// which is on the opposite side of the remote channel from the sender.
        /// </summary>
        private sealed class ForwardingEventListener : LongLivingMarshalByRefObject, IEventListener
        {
            private readonly IEventListener listener;

            public ForwardingEventListener(IEventListener listener)
            {
                this.listener = listener;
            }

            [OneWay]
            public void NotifyMessageEvent(MessageEventArgs e)
            {
                listener.NotifyMessageEvent(e);
            }

            [OneWay]
            public void NotifyTestLifecycleEvent(TestLifecycleEventArgs e)
            {
                listener.NotifyTestLifecycleEvent(e);
            }

            [OneWay]
            public void NotifyTestExecutionLogEvent(TestExecutionLogEventArgs e)
            {
                listener.NotifyTestExecutionLogEvent(e);
            }
        }
    }
}
