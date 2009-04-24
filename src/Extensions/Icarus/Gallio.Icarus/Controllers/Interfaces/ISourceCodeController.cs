using System;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ISourceCodeController
    {
        /// <summary>
        /// Event raised when showing source code.
        /// </summary>
        event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        /// <summary>
        /// Views the source code associated with a particular test.
        /// </summary>
        /// <param name="testId">The test id</param>
        /// <param name="progressMonitor">The progress monitor</param>
        void ViewSourceCode(string testId, IProgressMonitor progressMonitor);
    }
}
