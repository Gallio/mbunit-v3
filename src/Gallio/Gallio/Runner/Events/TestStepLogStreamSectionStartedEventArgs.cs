using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a section has been started within a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamSectionStartedEventArgs : TestStepLogStreamEventArgs
    {
        private readonly string sectionName;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <param name="sectionName">The name of the section that was started</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="logStreamName"/>, or <paramref name="sectionName"/> is null</exception>
        public TestStepLogStreamSectionStartedEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName, string sectionName)
            : base(report, test, testStepRun, logStreamName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            this.sectionName = sectionName;
        }

        /// <summary>
        /// Gets the name of the section that was started.
        /// </summary>
        public string SectionName
        {
            get { return sectionName; }
        }
    }
}