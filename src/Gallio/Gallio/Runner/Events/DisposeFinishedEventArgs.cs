using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that the test runner disposal has finished.
    /// </summary>
    public sealed class DisposeFinishedEventArgs : OperationFinishedEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if the disposal was completed successfully</param>
        public DisposeFinishedEventArgs(bool success)
            : base(success)
        {
        }
    }
}