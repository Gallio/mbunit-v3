// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using Gallio.Utilities;

namespace Gallio.Runtime
{
    /// <summary>
    /// Describes an exception that is to be reported.
    /// </summary>
    public sealed class CorrelatedExceptionEventArgs : EventArgs
    {
        private string message;
        private readonly Exception exception;
        private readonly string reporterStackTrace;
        private readonly bool isRecursive;

        /// <summary>
        /// Creates event arguments for reporting an exception.
        /// </summary>
        /// <param name="message">The message associated with the exception</param>
        /// <param name="exception">The exception that occurred</param>
        /// <param name="reporterStackTrace">The stack trace of the code that called to report the exception, or null if not available</param>
        /// <param name="isRecursive">True if a second exception occurred while attempting to report a previous
        /// unhandled exception on the same thread</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or
        /// <paramref name="exception"/> is null</exception>
        public CorrelatedExceptionEventArgs(string message, Exception exception, string reporterStackTrace, bool isRecursive)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (exception == null)
                throw new ArgumentNullException("exception");

            this.message = message;
            this.exception = exception;
            this.reporterStackTrace = reporterStackTrace;
            this.isRecursive = isRecursive;
        }

        /// <summary>
        /// Gets the message associated with the exception.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets the exception that occurred.
        /// </summary>
        /// <remarks>
        /// The event handler should use care when processing the exception object as it
        /// may have been obtained from an untrusted source.
        /// </remarks>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Gets the stack trace of the code that called to report the exception or null if not available.
        /// </summary>
        public string ReporterStackTrace
        {
            get { return reporterStackTrace; }
        }

        /// <summary>
        /// Returns true if a second exception occurred while attempting to report a previous
        /// exception on the same thread.
        /// </summary>
        public bool IsRecursive
        {
            get { return isRecursive; }
        }

        /// <summary>
        /// Adds a message that serves to describe the context in which the exception occurred.
        /// The <see cref="Message" /> will be augmented with these details.
        /// </summary>
        /// <param name="correlationMessage">The correlation message to append</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="correlationMessage"/> is null</exception>
        /// <seealso cref="UnhandledExceptionPolicy.CorrelateUnhandledException"/>
        public void AddCorrelationMessage(string correlationMessage)
        {
            if (correlationMessage == null)
                throw new ArgumentNullException("correlationMessage");

            message = string.Concat(message, "\n", correlationMessage);
        }

        /// <summary>
        /// Formats a description of the exception to a string like: "Message\nException\nReported by: ReporterStackTrace".
        /// </summary>
        /// <returns>The formatted string</returns>
        public string GetDescription()
        {
            StringBuilder description = new StringBuilder(message);
            description.AppendLine();
            description.AppendLine(ExceptionUtils.SafeToString(exception));

            if (reporterStackTrace != null)
            {
                if (description[description.Length - 1] != '\n')
                    description.AppendLine();

                description.AppendLine("Reported by: ").Append(ReporterStackTrace);
            }

            return description.ToString();
        }
    }
}