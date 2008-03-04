// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using Gallio.Utilities;

namespace Gallio.Utilities
{
    /// <summary>
    /// <para>
    /// This class provides a mechanism for reporting unhandled exceptions when the infrastructure
    /// is otherwise unable to deal with them locally.
    /// </para>
    /// <para>
    /// The methods of this class should be considered as the last resort for reporting failures
    /// that might otherwise cause the system to crash.
    /// </para>
    /// </summary>
    public static class UnhandledExceptionPolicy
    {
        [ThreadStatic]
        private static int recursionGuard;

        private static readonly object syncRoot = new object();
        private static EventHandler<CorrelatedExceptionEventArgs> correlateUnhandledException;
        private static EventHandler<CorrelatedExceptionEventArgs> reportUnhandledException;

        static UnhandledExceptionPolicy()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleAppDomainUnhandledException;
        }

        /// <summary>
        /// Adds or removes an event handler that is notified when unhandled exceptions occur
        /// and is given a chance to add additional information the event about the
        /// context in which the exception occurred.
        /// </summary>
        /// <seealso cref="CorrelatedExceptionEventArgs.AddCorrelationMessage"/>
        public static event EventHandler<CorrelatedExceptionEventArgs> CorrelateUnhandledException
        {
            add
            {
                lock (syncRoot)
                    correlateUnhandledException += value;
            }
            remove
            {
                lock (syncRoot)
                    correlateUnhandledException -= value;
            }
        }

        /// <summary>
        /// Adds or removes an event handler that is notified when unhandled exceptions occur.
        /// </summary>
        public static event EventHandler<CorrelatedExceptionEventArgs> ReportUnhandledException
        {
            add
            {
                lock (syncRoot)
                    reportUnhandledException += value;
            }
            remove
            {
                lock (syncRoot)
                    reportUnhandledException -= value;
            }
        }

        /// <summary>
        /// Reports an unhandled exception.
        /// </summary>
        /// <param name="message">A message to explain how the exception was intercepted</param>
        /// <param name="unhandledException">The unhandled exception</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or <paramref name="unhandledException"/> is null</exception>
        public static void Report(string message, Exception unhandledException)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (unhandledException == null)
                throw new ArgumentNullException("unhandledException");

            ReportInternal(message, unhandledException, true);
        }

        [NonInlined(SecurityAction.Demand)]
        private static void ReportInternal(string message, Exception unhandledException, bool includeReporterStackTrace)
        {
            if (recursionGuard == 2)
                return; // We've already had one recursion, stop recursing.

            int oldRecursionGuard = recursionGuard;
            try
            {
                recursionGuard += 1;

                CorrelatedExceptionEventArgs e = new CorrelatedExceptionEventArgs(message,
                    unhandledException,
                    includeReporterStackTrace ? GetCallersCallerStackTrace() : null,
                    oldRecursionGuard != 0);

                EventHandlerUtils.SafeInvoke(correlateUnhandledException, null, e);
                EventHandlerUtils.SafeInvoke(reportUnhandledException, null, e);
            }
            finally
            {
                recursionGuard = oldRecursionGuard;
            }
        }

        [NonInlined(SecurityAction.Demand)]
        private static string GetCallersCallerStackTrace()
        {
            try
            {
                return typeof(UnhandledExceptionPolicy).Name + "\n" + new StackTrace(3, true);
            }
            catch
            {
                return null;
            }
        }

        private static void HandleAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string threadName = Thread.CurrentThread.Name;
            string message;
            if (string.IsNullOrEmpty(threadName))
                message = "An unhandled exception occurred.";
            else
                message = String.Format("An unhandled exception occurred in thread '{0}'.", threadName);

            // Note: There is no need to include the reporter's stack trace because it will already
            //       be fully contained in the exception since it must have bubbled up to the thread entry point.
            Exception unhandledException = e.ExceptionObject as Exception ?? new Exception("An unknown exception occurred.");
            ReportInternal(message, unhandledException, false);
        }
    }
}