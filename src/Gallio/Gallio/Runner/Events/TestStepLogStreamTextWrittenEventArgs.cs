using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that text has been written to a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamTextWrittenEventArgs : TestStepLogStreamEventArgs
    {
        private readonly string text;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <param name="text">The text that was written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/> or <paramref name="logStreamName"/> is null</exception>
        public TestStepLogStreamTextWrittenEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName, string text)
            : base(report, test, testStepRun, logStreamName)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
        }

        /// <summary>
        /// Gets the text that was written.
        /// </summary>
        public string Text
        {
            get { return text; }
        }
    }
}