using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that the test runner disposal has started.
    /// </summary>
    public sealed class DisposeStartedEventArgs : OperationStartedEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        public DisposeStartedEventArgs()
        {
        }
    }
}