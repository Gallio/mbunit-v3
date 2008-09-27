using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Describes the status of a test run.
    /// </summary>
    public enum TestRunStatus
    {
        /// <summary>
        /// The test run has not yet been started.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The test run has been started and is running.
        /// </summary>
        Running,

        /// <summary>
        /// The test run has stopped because an error occurred.
        /// </summary>
        Error,

        /// <summary>
        /// The test run has stopped because it was canceled.
        /// </summary>
        Canceled,

        /// <summary>
        /// The test run has finished normally.
        /// </summary>
        Finished
    }
}
