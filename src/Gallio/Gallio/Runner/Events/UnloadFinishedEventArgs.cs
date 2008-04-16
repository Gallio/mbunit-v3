using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test package has finished unloading.
    /// </summary>
    public sealed class UnloadFinishedEventArgs : OperationFinishedEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if the test package was successfully unloaded</param>
        public UnloadFinishedEventArgs(bool success)
            : base(success)
        {
        }
    }
}