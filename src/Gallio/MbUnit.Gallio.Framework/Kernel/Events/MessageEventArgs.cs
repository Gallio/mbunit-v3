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

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A message event allows the framework to send messages to the test runner
    /// such as status updates or debug log messages.
    /// </summary>
    [Serializable]
    public class MessageEventArgs : EventArgs
    {
        private string message;

        /// <summary>
        /// Creates a message event.
        /// </summary>
        /// <param name="message">The message text</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public MessageEventArgs(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.message = message;
        }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message
        {
            get { return message; }
        }
    }
}
