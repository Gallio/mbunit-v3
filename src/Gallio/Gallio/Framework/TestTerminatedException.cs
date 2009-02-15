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
using Gallio.Model;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// This exception type is used to signal that a test has been terminated with
    /// a particular outcome.  The exception message, if present, should explain the
    /// reason that the test was terminated.
    /// </para>
    /// </summary>
    [Serializable]
    public class TestTerminatedException : TestException
    {
        private const string OutcomeKey = "Outcome";
        private readonly TestOutcome outcome;

        /// <summary>
        /// Creates a silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        public TestTerminatedException(TestOutcome outcome)
            : this(outcome, null, null)
        {
        }

        /// <summary>
        /// Creates a silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        /// <param name="message">The message, or null if none</param>
        public TestTerminatedException(TestOutcome outcome, string message)
            : this(outcome, message, null)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        /// <param name="message">The message, or null if none</param>
        /// <param name="innerException">The inner exception, or null if none</param>
        public TestTerminatedException(TestOutcome outcome, string message, Exception innerException)
            : base(message, innerException)
        {
            this.outcome = outcome;
        }

        /// <summary>
        /// Creates a exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected TestTerminatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            outcome = (TestOutcome) info.GetValue(OutcomeKey, typeof(TestOutcome));
        }

        /// <inheritdoc />
        public override TestOutcome Outcome
        {
            get { return outcome; }
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(OutcomeKey, outcome);
        }
    }
}