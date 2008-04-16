using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that the test runner initialization has finished.
    /// </summary>
    public sealed class InitializeFinishedEventArgs : OperationFinishedEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if the test runner was successfully initialized</param>
        public InitializeFinishedEventArgs(bool success)
            : base(success)
        {
        }
    }
}