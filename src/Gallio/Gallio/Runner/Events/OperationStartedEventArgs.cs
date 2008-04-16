using System;

namespace Gallio.Runner
{
    /// <summary>
    /// Base arguments for events raised to indicate the beginning of an operation.
    /// </summary>
    public abstract class OperationStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        protected OperationStartedEventArgs()
        {
        }
    }
}