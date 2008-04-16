using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test step has finished execution.
    /// </summary>
    public sealed class TestStepFinishedEventArgs : TestStepEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// or <paramref name="testStepRun"/> is null</exception>
        public TestStepFinishedEventArgs(Report report, TestData test, TestStepRun testStepRun)
            : base(report, test, testStepRun)
        {
        }
    }
}