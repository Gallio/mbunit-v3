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
using System.Runtime.Serialization;
using Gallio.Hosting;

namespace Gallio.ReSharperRunner.Hosting
{
    /// <summary>
    /// The type of exception thrown when the runtime proxy cannot perform
    /// a requested service.  This can happen because the <see cref="Runtime" />
    /// is not accessible.
    /// </summary>
    [Serializable]
    public class RuntimeProxyException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public RuntimeProxyException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public RuntimeProxyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public RuntimeProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected RuntimeProxyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}