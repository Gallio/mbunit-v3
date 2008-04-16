using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that the test runner initialization has started.
    /// </summary>
    public sealed class InitializeStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestRunnerOptions options;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="options">The test runner options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> is null</exception>
        public InitializeStartedEventArgs(TestRunnerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            this.options = options;
        }

        /// <summary>
        /// Gets the test runner options.
        /// </summary>
        public TestRunnerOptions Options
        {
            get { return options; }
        }
    }
}