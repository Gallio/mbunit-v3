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

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// An interface implemented by the server and registered on the server remoting channel
    /// to allow message exchange with the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is interim API to be used until the new message broker is ready.
    /// </para>
    /// </remarks>
    public interface IMessageExchangeLink
    {
        /// <summary>
        /// Gets the next message.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for a message.</param>
        /// <returns>The next message, or null if a timeout occurred.</returns>
        Message Receive(TimeSpan timeout);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        void Send(Message message);
    }
}
