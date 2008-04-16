using System;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test step has entered a new lifecycle phase.
    /// </summary>
    public sealed class TestStepLifecyclePhaseChangedEventArgs : TestStepEventArgs
    {
        private readonly string lifecyclePhase;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run</param>
        /// <param name="lifecyclePhase">The lifecycle phase name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/>, or <paramref name="lifecyclePhase"/> is null</exception>
        public TestStepLifecyclePhaseChangedEventArgs(Report report, TestData test, TestStepRun testStepRun, string lifecyclePhase)
            : base(report, test, testStepRun)
        {
            if (lifecyclePhase == null)
                throw new ArgumentNullException("lifecyclePhase");

            this.lifecyclePhase = lifecyclePhase;
        }

        /// <summary>
        /// Gets the lifecycle phase name.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        public string LifecyclePhase
        {
            get { return lifecyclePhase; }
        }
    }
}