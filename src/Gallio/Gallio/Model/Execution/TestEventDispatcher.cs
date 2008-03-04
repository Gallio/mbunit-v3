// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Utilities;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test listener implementation that dispatches notifications as events
    /// or delegates them to other listeners.
    /// </summary>
    public class TestEventDispatcher : ITestListener
    {
        /// <summary>
        /// The event fired when notified of a test lifecycle event.
        /// </summary>
        public EventHandler<LifecycleEventArgs> Lifecycle;

        /// <summary>
        /// The event fired when notified of a test execution log event.
        /// </summary>
        public EventHandler<LogEventArgs> ExecutionLog;

        /// <inheritdoc />
        public void NotifyLogEvent(LogEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(ExecutionLog, this, e);
        }

        /// <inheritdoc />
        public void NotifyLifecycleEvent(LifecycleEventArgs e)
        {
            EventHandlerUtils.SafeInvoke(Lifecycle, this, e);
        }
    }
}