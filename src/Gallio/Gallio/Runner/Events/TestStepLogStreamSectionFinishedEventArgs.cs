using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a section has been finished within a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamSectionFinishedEventArgs : TestStepLogStreamEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/> or <paramref name="logStreamName"/> is null</exception>
        public TestStepLogStreamSectionFinishedEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName)
            : base(report, test, testStepRun, logStreamName)
        {
        }
    }
}