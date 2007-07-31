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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A test listener implementation that dispatches notifications as events.
    /// </summary>
    public class EventDispatcher : IEventListener
    {
        private List<IEventListener> listeners;

        /// <summary>
        /// The event fired when notified of a message event.
        /// </summary>
        public EventHandler<MessageEventArgs> Message;

        /// <summary>
        /// The event fired when notified of a test lifecycle event.
        /// </summary>
        public EventHandler<TestLifecycleEventArgs> TestLifecycle;

        /// <summary>
        /// The event fired when notified of a test execution log event.
        /// </summary>
        public EventHandler<TestExecutionLogEventArgs> TestExecutionLog;

        /// <summary>
        /// Gets a list of listeners to which events are forwarded in addition
        /// to the event handlers.
        /// </summary>
        public IList<IEventListener> Listeners
        {
            get
            {
                if (listeners == null)
                    listeners = new List<IEventListener>();
                return listeners;
            }
        }

        /// <inheritdoc />
        public void NotifyMessageEvent(MessageEventArgs e)
        {
            if (Message != null)
                Message(this, e);

            if (listeners != null)
                foreach (IEventListener listener in listeners)
                    listener.NotifyMessageEvent(e);
        }

        /// <inheritdoc />
        public void NotifyTestExecutionLogEvent(TestExecutionLogEventArgs e)
        {
            if (TestExecutionLog != null)
                TestExecutionLog(this, e);

            if (listeners != null)
                foreach (IEventListener listener in listeners)
                    listener.NotifyTestExecutionLogEvent(e);
        }

        /// <inheritdoc />
        public void NotifyTestLifecycleEvent(TestLifecycleEventArgs e)
        {
            if (TestLifecycle != null)
                TestLifecycle(this, e);

            if (listeners != null)
                foreach (IEventListener listener in listeners)
                    listener.NotifyTestLifecycleEvent(e);
        }
    }
}