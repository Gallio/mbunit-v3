// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using EnvDTE;
using Gallio.Navigator.Native;
using Thread = System.Threading.Thread;

namespace Gallio.Navigator
{
    internal class VisualStudioSupport
    {
        private static readonly string[] VisualStudioDTEProgIds = new[]
        {
            "VisualStudio.DTE.9.0",
            "VisualStudio.DTE.8.0",
            "VisualStudio.DTE"
        };

        /// <summary>
        /// Runs a block of code with the Visual Studio DTE either by finding the currently active instance or by starting one.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static void WithDTE(Action<DTE> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Exception exception = null;
            var thread = new Thread((ThreadStart) delegate
            {
                IOleMessageFilter newFilter = new MessageFilter();
                IOleMessageFilter oldFilter;
                NativeMethods.CoRegisterMessageFilter(newFilter, out oldFilter);
                try
                {
                    DTE dte = (DTE)GetDTEObject();
                    action(dte);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    NativeMethods.CoRegisterMessageFilter(oldFilter, out newFilter);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
                throw new ApplicationException("Could not perform the requested Visual Studio operation.", exception);
        }

        /// <summary>
        /// Makes Visual Studio the foreground window.
        /// </summary>
        /// <param name="dte">The Visual Studio DTE</param>
        public static void BringVisualStudioToFront(DTE dte)
        {
            // Inspired from FxCop GUI implementation.
            Window window = dte.MainWindow;

            IntPtr hWnd = (IntPtr) window.HWnd;
            if (NativeMethods.IsIconic(hWnd))
                NativeMethods.ShowWindowAsync(hWnd, NativeConstants.SW_RESTORE);

            NativeMethods.SetForegroundWindow(hWnd);
            Thread.Sleep(50);

            window.Activate();
            window.Visible = true;
        }

        private static object GetDTEObject()
        {
            // If VisualStudio is running, get the active instance.
            foreach (string progId in VisualStudioDTEProgIds)
            {
                object obj = GetActiveObject(progId);
                if (obj != null)
                    return obj;
            }

            // Otherwise try to start a fresh instance.
            foreach (string progId in VisualStudioDTEProgIds)
            {
                Type dteType = Type.GetTypeFromProgID(progId);
                if (dteType != null)
                    return Activator.CreateInstance(dteType, true);
            }

            throw new ApplicationException("Could not find or start an instance of Visual Studio.");
        }

        private static object GetActiveObject(string progId)
        {
            try
            {
                return Marshal.GetActiveObject(progId);
            }
            catch (COMException)
            {
                return null;
            }
        }

        // This fancy bit of logic is from an MSDN article:
        // Fixing 'Application is Busy' and 'Call was Rejected By Callee' Errors
        // http://msdn.microsoft.com/en-us/library/ms228772(VS.80).aspx
        private sealed class MessageFilter : IOleMessageFilter
        {
            private const int QuickRetryMilliseconds = 50;
            private const int MaxRetryMilliseconds = 30000;

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
                    // Retry the thread call after 100ms, or immediately.
                    return dwTickCount < QuickRetryMilliseconds ? 0 : 100;
                }

                // Too busy; cancel call.
                return -1;
            }

            int IOleMessageFilter.MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType)
            {
                return NativeConstants.PENDINGMSG_WAITDEFPROCESS;
            }
        }
    }
}
