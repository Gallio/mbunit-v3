// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Runtime.Serialization;

namespace Gallio.Loader
{
    /// <summary>
    /// <para>
    /// An exception that expresses a problem in a manner that may be safely communicated
    /// across a remote context to a client that may not be able to load the original exception type.
    /// </para>
    /// <para>
    /// Using this type prevents spurious <see cref="SerializationException"/> exceptions
    /// from being thrown due to the server exposing an internal exception type that
    /// is not accessible to the client.
    /// </para>
    /// </summary>
    [Serializable]
    public class SafeException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public SafeException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        public SafeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public SafeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected SafeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Wraps an exception as a <see cref="SafeException" />.
        /// </summary>
        /// <param name="ex">The exception type</param>
        /// <returns>The wrapped exception</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        public static SafeException Wrap(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            SafeException safeEx = ex as SafeException;
            if (safeEx != null)
                return safeEx;

            return new SafeException(ex.ToString());
        }
    }
}
