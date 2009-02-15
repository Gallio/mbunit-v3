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
using System.Threading;
using EnvDTE;
using Gallio.VisualStudio.Interop.Native;
using Thread = System.Threading.Thread;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// <para>
    /// Provides support for finding and launching instances of VisualStudio.
    /// </para>
    /// </summary>
    public static class VisualStudioSupport
    {
        private const string VisualStudio10DTEProgId = "VisualStudio.DTE.10.0";
        private const string VisualStudio9DTEProgId = "VisualStudio.DTE.9.0";
        private const string VisualStudio8DTEProgId = "VisualStudio.DTE.8.0";
        private const string VisualStudio7DTEProgId = "VisualStudio.DTE";

        private static readonly string[] VisualStudioDTEProgIds = new[]
        {
            VisualStudio10DTEProgId, VisualStudio9DTEProgId, VisualStudio8DTEProgId, VisualStudio7DTEProgId
        };

        /// <summary>
        /// <para>
        /// Runs a block of code with the currently active Visual Studio DTE either by finding the currently
        /// active instance or by starting one.
        /// </para>
        /// <para>
        /// The action runs in an STA thread context and the appropriate <see cref="ComRetryMessageFilter" /> installed.
        /// </para>
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="retryTimeout">The COM retry timeout</param>
        /// <returns>True if the action ran, false if no active instance of Visual Studio was found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static bool SafeRunWithActiveVisualStudio(Action<DTE> action, TimeSpan retryTimeout)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Exception exception = null;
            bool ran = false;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                SafeRunWithActiveVisualStudio(action, retryTimeout, out ran, out exception);
            }
            else
            {
                var thread = new Thread(() => SafeRunWithActiveVisualStudio(action, retryTimeout, out ran, out exception));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }

            if (exception != null)
                throw new ApplicationException("Could not perform the requested Visual Studio operation.", exception);

            return ran;
        }

        private static void SafeRunWithActiveVisualStudio(Action<DTE> action, TimeSpan retryTimeout, out bool ran, out Exception exception)
        {
            ran = false;
            exception = null;

            ComRetryMessageFilter.Install(retryTimeout);
            try
            {
                DTE dte = GetActiveVisualStudio();
                if (dte != null)
                {
                    ran = true;
                    action(dte);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                ComRetryMessageFilter.Uninstall();
            }
        }

        /// <summary>
        /// Launches Visual Studio 9.
        /// </summary>
        /// <returns>The Visual Studio DTE, or null on failure</returns>
        public static DTE LaunchVisualStudio9()
        {
            Type dteType = Type.GetTypeFromProgID(VisualStudio9DTEProgId);
            if (dteType != null)
                return (DTE) Activator.CreateInstance(dteType, true);

            return null;
        }

        /// <summary>
        /// Gets the Active VisualStudio DTE object.
        /// </summary>
        /// <returns>The Visual Studio DTE, or null on failure</returns>
        public static DTE GetActiveVisualStudio()
        {
            // If VisualStudio is running, get the active instance.
            foreach (string progId in VisualStudioDTEProgIds)
            {
                object obj = GetActiveObject(progId);
                if (obj != null)
                    return (DTE) obj;
            }

            return null;
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
    }
}
