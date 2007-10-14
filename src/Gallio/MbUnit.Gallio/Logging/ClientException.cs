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
using System.Runtime.Serialization;
using System.Text;

namespace MbUnit.Logging
{
    /// <summary>
    /// A client exception wraps an error that occurred within client code that
    /// the framework is interacting with.  When client exceptions are reported to
    /// the user, the description should should exclude details about what the framework
    /// was doing and focus purely on the problem that actually occurred within
    /// the client code to ensure that the information is relevant.
    /// </summary>
    [Serializable]
    public class ClientException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public ClientException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public ClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}