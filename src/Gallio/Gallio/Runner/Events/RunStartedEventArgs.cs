using System;
using Gallio.Model.Execution;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test execution has started.
    /// </summary>
    public sealed class RunStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestExecutionOptions testExecutionOptions;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testExecutionOptions">The test execution options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExecutionOptions"/> is null</exception>
        public RunStartedEventArgs(TestExecutionOptions testExecutionOptions)
        {
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");

            this.testExecutionOptions = testExecutionOptions;
        }

        /// <summary>
        /// Gets the test execution options.
        /// </summary>
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
        }
    }
}