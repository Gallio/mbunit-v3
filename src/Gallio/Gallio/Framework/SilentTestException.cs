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
using Gallio.Common.Diagnostics;
using Gallio.Model;

namespace Gallio.Framework
{
    /// <summary>
    /// This exception type is used to signal a test outcome silently without logging the exception.
    /// </summary>
    [Serializable]
    public class SilentTestException : TestTerminatedException
    {
        /// <summary>
        /// Creates a silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome.</param>
        public SilentTestException(TestOutcome outcome)
            : base(outcome)
        {
        }

        /// <summary>
        /// Creates a silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome.</param>
        /// <param name="message">The message, or null if none.</param>
        public SilentTestException(TestOutcome outcome, string message)
            : base(outcome, message)
        {
        }

        /// <summary>
        /// Creates a exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected SilentTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc />
        [SystemInternal]
        public override bool ExcludeStackTrace
        {
            get { return true; }
        }
    }
}
