using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Base arguments for an event raised to indicate that a test step log stream has been modified in some way.
    /// </summary>
    public abstract class TestStepLogStreamEventArgs : TestStepEventArgs
    {
        private readonly string logStreamName;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="logStreamName"/> or <paramref name="text"/> is null</exception>
        protected TestStepLogStreamEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName)
            : base(report, test, testStepRun)
        {
            if (logStreamName == null)
                throw new ArgumentNullException("logStreamName");

            this.logStreamName = logStreamName;
        }

        /// <summary>
        /// Gets the log stream name.
        /// </summary>
        public string LogStreamName
        {
            get { return logStreamName; }
        }
    }
}