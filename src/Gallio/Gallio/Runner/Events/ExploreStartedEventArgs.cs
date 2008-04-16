using System;
using Gallio.Model;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test exploration has started.
    /// </summary>
    public sealed class ExploreStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestExplorationOptions testExplorationOptions;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testExplorationOptions">The test exploration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExplorationOptions"/> is null</exception>
        public ExploreStartedEventArgs(TestExplorationOptions testExplorationOptions)
        {
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");

            this.testExplorationOptions = testExplorationOptions;
        }

        /// <summary>
        /// Gets the test exploration options.
        /// </summary>
        public TestExplorationOptions TestExplorationOptions
        {
            get { return testExplorationOptions; }
        }
    }
}