// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Runtime.InteropServices;
using Gallio.VisualStudio.Interop.Native;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Installs a COM message filter that automatically retries COM RPC requests until a timeout expires.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used to handle "Application is Busy" and "Call was Rejected By Callee" transient errors.
    /// </para>
    /// </remarks>
    public static class ComRetryMessageFilter
    {
        private static readonly object syncRoot = new object();
        private static IOleMessageFilter oldFilter, newFilter;

        /// <summary>
        /// Installs a retry message filter.
        /// </summary>
        /// <param name="retryTimeout">The retry timeout.</param>
        /// <exception cref="InvalidOperationException">Thrown if the filter has already been installed.</exception>
        public static void Install(TimeSpan retryTimeout)
        {
            lock (syncRoot)
            {
                if (oldFilter != null)
                    throw new InvalidOperationException("The retry message filter has already been installed.");

                newFilter = new MessageFilter(retryTimeout);
                NativeMethods.CoRegisterMessageFilter(newFilter, out oldFilter);
            }
        }

        /// <summary>
        /// Uninstalls the retry message filter.
        /// </summary>
        public static void Uninstall()
        {
            lock (syncRoot)
            {
                if (oldFilter != null)
                {
                    IOleMessageFilter filter;
                    NativeMethods.CoRegisterMessageFilter(oldFilter, out filter);

                    oldFilter = null;
                    newFilter = null;
                }
            }
        }

        // This fancy bit of logic is from an MSDN article:
        // Fixing 'Application is Busy' and 'Call was Rejected By Callee' Errors
        // http://msdn.microsoft.com/en-us/library/ms228772(VS.80).aspx
        private sealed class MessageFilter : IOleMessageFilter
        {
            private const int QuickRetryMilliseconds = 10;

            private const int FlagSleepThenRetry = 100;
            private const int FlagRetryImmediately = 0;
            private const int FlagCancel = -1;

            private readonly int MaxRetryMilliseconds = 30000;

            public MessageFilter(TimeSpan timeout)
            {
                MaxRetryMilliseconds = (int) timeout.TotalMilliseconds;
            }

            // IOleMessageFilter functions.
            // Handle incoming thread requests.
            int IOleMessageFilter.HandleInComingCall(int dwCallType, IntPtr hTaskCaller, int dwTickCount, IntPtr lpInterfaceInfo)
            {
                return NativeConstants.SERVERCALL_ISHANDLED;
            }

            // Thread call was rejected, so try again.
            int IOleMessageFilter.RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, SERVERCALL dwRejectType)
            {
                if (dwTickCount < MaxRetryMilliseconds
                    && dwRejectType == SERVERCALL.SERVERCALL_RETRYLATER)
                {
                    // Retry after a short 100ms sleep or immediately depending on how long we've been waiting.
                    return dwTickCount < QuickRetryMilliseconds ? FlagRetryImmediately : FlagSleepThenRetry;
                }

                // Too busy; cancel call.
                return FlagCancel;
            }

            int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                return NativeConstants.PENDINGMSG_WAITDEFPROCESS;
            }
        }
    }
}
