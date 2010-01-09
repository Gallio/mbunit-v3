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
using System.Runtime.Serialization;
using Gallio.Model;

namespace Gallio.Framework
{
    /// <summary>
    /// This exception type is used to signal that a test has failed to satisfy
    /// some predetermined condition established by the test author.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For example, an assertion failure may be signaled using a <see cref="TestFailedException" />
    /// to distinguish it from other application or runtime errors that a test might encounter
    /// at runtime.
    /// </para>
    /// </remarks>
    [Serializable]
    public class TestFailedException : TestException
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public TestFailedException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public TestFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TestFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TestFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Returns <see cref="TestOutcome.Failed" />.
        /// </summary>
        public sealed override TestOutcome Outcome
        {
            get { return TestOutcome.Failed; }
        }
    }
}
