using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime
{
    /// <summary>
    /// Describes the kind of process that is executing.
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// The process is running in the console.
        /// </summary>
        Console,

        /// <summary>
        /// The process is running interactively.
        /// </summary>
        Interactive,

        /// <summary>
        /// The process is running as a Windows service.
        /// </summary>
        Service
    }
}
