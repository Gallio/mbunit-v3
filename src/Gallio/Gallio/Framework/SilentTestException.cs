using System;
using System.Runtime.Serialization;
using Gallio.Model;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// This exception type is used to signal a test outcome silently without logging the exception.
    /// </para>
    /// </summary>
    [Serializable]
    public class SilentTestException : TestException
    {
        private const string OutcomeKey = "Outcome";
        private readonly TestOutcome outcome;

        /// <summary>
        /// Creates an silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        public SilentTestException(TestOutcome outcome)
            : this(outcome, null)
        {
        }

        /// <summary>
        /// Creates an silent test exception with the specified outcome.
        /// </summary>
        /// <param name="outcome">The test outcome</param>
        /// <param name="message">The message, or null if none</param>
        public SilentTestException(TestOutcome outcome, string message)
            : base(message)
        {
            this.outcome = outcome;
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info</param>
        /// <param name="context">The streaming context</param>
        protected SilentTestException(SerializationInfo info, StreamingContext context)
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
        public override bool ExcludeStackTrace
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(OutcomeKey, outcome);
        }
    }
}