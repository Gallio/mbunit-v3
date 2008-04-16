using System;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test package is being unloaded.
    /// </summary>
    public sealed class UnloadStartedEventArgs : OperationStartedEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        public UnloadStartedEventArgs()
        {
        }
    }
}