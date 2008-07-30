using System;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using Gallio.Model.Logging;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a marker region has been started within a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamBeginMarkerEventArgs : TestStepLogStreamEventArgs
    {
        private readonly string @class;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="logStreamName">The log stream name</param>
        /// <param name="class">The marker class identifier</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, <paramref name="logStreamName"/>, or <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="MarkerClasses.Validate"/></exception>
        public TestStepLogStreamBeginMarkerEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName, string @class)
            : base(report, test, testStepRun, logStreamName)
        {
            MarkerClasses.Validate(@class);
            this.@class = @class;
        }

        /// <summary>
        /// Gets the marker class identifier.
        /// </summary>
        public string Class
        {
            get { return @class; }
        }
    }
}