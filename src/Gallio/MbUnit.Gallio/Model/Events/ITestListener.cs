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

namespace MbUnit.Model.Events
{
    /// <summary>
    /// An test listener observes the progress of test execution.
    /// </summary>
    public interface ITestListener
    {
        /// <summary>
        /// Notifes the listener about a message event. 
        /// </summary>
        /// <param name="e">The event arguments</param>
        void NotifyMessageEvent(MessageEventArgs e);

        /// <summary>
        /// Notifes the listener about a lifecycle event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        void NotifyLifecycleEvent(LifecycleEventArgs e);

        /// <summary>
        /// Notifes the listener about an execution log event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        void NotifyLogEvent(LogEventArgs e);
    }
}