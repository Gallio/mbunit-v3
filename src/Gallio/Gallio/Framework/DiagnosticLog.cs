// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Gallio.Model.Messages.Logging;
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
    /// Diagnostic messages will appear in the test report but they are not associated with
    /// any particular tests.  Unlike <see cref="TestLog"/> the messages may not include rich
    /// content such as attachments or embedded images.
    /// </para>
    /// <para>
    /// If you use the diagnostic log to report progress messages, keep in mind that they
    /// will all be preserved in the report, making the report larger.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestLog"/>
    public static class DiagnosticLog
    {
        /// <summary>
        /// Writes a message to the diagnostic log.
        /// </summary>
        /// <param name="message">The message to write, may be null.</param>
        public static void WriteLine(string message)
        {
            if (message != null)
            {
                TestContext context = TestContext.CurrentContext;
                if (context != null)
                {
                    context.PublishMessage(new LogEntrySubmittedMessage()
                    {
                        Severity = LogSeverity.Important,
                        Message = message
                    });
                }
                else if (RuntimeAccessor.IsInitialized)
                {
                    RuntimeAccessor.Logger.Log(LogSeverity.Important, message);
                }
                else
                {
                    Debug.WriteLine(message);
                }
            }
        }

        /// <summary>
        /// Writes a formatted message to the diagnostic log.
        /// </summary>
        /// <seealso cref="String.Format(string, object[])"/>
        /// <param name="messageFormat">The message format string, may be null.</param>
        /// <param name="messageArgs">The message arguments.</param>
        public static void WriteLine(string messageFormat, params object[] messageArgs)
        {
            if (messageFormat != null)
            {
                WriteLine(string.Format(messageFormat, messageArgs));
            }
        }
    }
}
