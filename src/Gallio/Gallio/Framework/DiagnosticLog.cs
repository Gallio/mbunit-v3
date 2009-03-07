using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Gallio.Runtime;
using Gallio.Runtime.Logging;

namespace Gallio.Framework
{
    /// <summary>
    /// The diagnostic log provides services for writing diagnostic messages to the console
    /// or runtime log during a test run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Diagnostic messages are not associated with particular tests and are not stored in the
    /// test report.  Use <see cref="TestLog" /> instead if you wish to retain the log
    /// content in the report or if you wish to save attachments and other rich content for later.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestLog"/>
    public static class DiagnosticLog
    {
        /// <summary>
        /// Writes a message to the diagnostic log.
        /// </summary>
        /// <param name="message">The message to write, may be null</param>
        public static void WriteLine(string message)
        {
            if (message != null)
            {
                if (RuntimeAccessor.IsInitialized)
                    RuntimeAccessor.Logger.Log(LogSeverity.Important, message);
                else
                    Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// Writes a formatted message to the diagnostic log.
        /// </summary>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <param name="messageFormat">The message format string, may be null</param>
        /// <param name="messageArgs">The message arguments</param>
        public static void WriteLine(string messageFormat, params object[] messageArgs)
        {
            if (messageFormat != null)
            {
                WriteLine(string.Format(messageFormat, messageArgs));
            }
        }
    }
}
