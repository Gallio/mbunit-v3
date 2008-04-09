using System;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Describes the severity of a log message.
    /// </summary>
    public enum LogSeverity
    {
        /// <summary>
        /// The severity used for debug messages.
        /// </summary>
        Debug = 0,

        /// <summary>
        /// The severity used for informational messages.
        /// </summary>
        Info = 1,

        /// <summary>
        /// The severity used for warning messages.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// The severity used for error messages.
        /// </summary>
        Error = 3
    }
}
