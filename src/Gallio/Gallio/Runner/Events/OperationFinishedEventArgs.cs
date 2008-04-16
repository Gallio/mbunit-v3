using System;

namespace Gallio.Runner
{
    /// <summary>
    /// Base arguments for events raised to indicate the completion of an operation.
    /// </summary>
    public abstract class OperationFinishedEventArgs : EventArgs
    {
        private readonly bool success;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if the operation completed successfully</param>
        protected OperationFinishedEventArgs(bool success)
        {
            this.success = success;
        }

        /// <summary>
        /// Returns true if the operation completed successfully.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }
    }
}
