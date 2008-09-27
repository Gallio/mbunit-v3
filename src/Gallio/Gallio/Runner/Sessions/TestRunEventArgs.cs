using System;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Event arguments describing an event affecting a test run.
    /// </summary>
    public class TestRunEventArgs : EventArgs
    {
        /// <summary>
        /// Creates event arguments for a test run event.
        /// </summary>
        /// <param name="run">The test run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="run"/> is null</exception>
        public TestRunEventArgs(ITestRun run)
        {
            if (run == null)
                throw new ArgumentNullException("run");

            Run = run;
        }

        /// <summary>
        /// Gets the test run.
        /// </summary>
        public ITestRun Run { get; private set; }
    }
}