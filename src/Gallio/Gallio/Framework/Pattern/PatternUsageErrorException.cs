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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// The type of exception thrown when a test declaration is malformed or
    /// contains an error.  Implementations of <see cref="IPattern" /> may use
    /// this exception to report that a pattern is malformed or not valid in
    /// the context in which it appears.
    /// </para>
    /// <para>
    /// The exception effectively halts the processing of the pattern.  The message
    /// text is then manifested as an error annotation that may be displayed to the user.
    /// </para>
    /// <para>
    /// When you see this error, check to make sure that the syntax of the test
    /// is correct, all required parameters have been provided and they contain
    /// valid values.
    /// </para>
    /// </summary>
    [Serializable]
    public class PatternUsageErrorException : Exception
    {
        /// <summary>
        /// Creates a usage error exception.
        /// </summary>
        /// <param name="message">A message that describes how the pattern was misused and possibly what should be done to correct it.</param>
        public PatternUsageErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a usage error exception.
        /// </summary>
        /// <param name="message">A message that describes how the pattern was misused and possibly what should be done to correct it.</param>
        /// <param name="innerException">An optional exception that will be used to provide aditional details about the usage error</param>
        public PatternUsageErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected PatternUsageErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
