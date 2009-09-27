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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Gallio.Common.Concurrency;
using Gallio.Common.Text;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Default AutoCAD command runner that runs commands by
    /// sending <c>WM_COPYDATA</c> messages to the AutoCAD process.
    /// </summary>
    public class CopyDataCommandRunner : IAcadCommandRunner
    {
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.5);

        /// <inheritdoc/>
        public void Run(AcadCommand command, IProcess process)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (process == null)
                throw new ArgumentNullException("process");

            var lisp = CreateLispExpression(command);

            Thread.Sleep(TimeSpan.FromSeconds(10));

            IntPtr hWnd = FindWindowHandle(process, ReadyTimeout, ReadyPollInterval);
            SendCopyDataMessage(hWnd, lisp);
        }

        private static IntPtr FindWindowHandle(IProcess process, TimeSpan timeout, TimeSpan pollInterval)
        {
            var stopwatch = Stopwatch.StartNew();

            process.WaitForInputIdle(timeout.Milliseconds);

            do
            {
                IntPtr hWnd = AcadWindowFinder.FindMainWindow(process.Id);
                if (hWnd != IntPtr.Zero)
                    return hWnd;

                Thread.Sleep(pollInterval);
            }
            while (stopwatch.Elapsed < timeout);

            throw new TimeoutException("Timeout waiting for AutoCAD.");
        }

        private static void SendCopyDataMessage(IntPtr hWnd, string message)
        {
            var cds = new COPYDATASTRUCT(message);
            NativeMethods.SendMessage(new HandleRef(cds, hWnd), ref cds);
        }

        private static string CreateLispExpression(AcadCommand command)
        {
            var builder = new StringBuilder();

            builder.Append("(command ");
            builder.Append(StringUtils.ToStringLiteral("_" + command.GlobalName));

            var args = command.GetArguments();
            if (args != null)
            {
                foreach (var arg in args)
                    builder.Append(" " + StringUtils.ToStringLiteral(arg ?? string.Empty));
            }

            builder.Append(")\n");

            return builder.ToString();
        }

        #region P/Invoke related
        // ReSharper disable InconsistentNaming

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public COPYDATASTRUCT(string message)
            {
                dwData = new IntPtr(1);
                cbData = (message.Length + 1) * Marshal.SystemDefaultCharSize;
                lpData = message;
            }

            IntPtr dwData;
            int cbData;
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpData;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static class NativeMethods
        {
            private const uint WM_COPYDATA = 0x4A;
            private static readonly IntPtr FALSE = IntPtr.Zero;
            public const int ERROR_SUCCESS = 0;

            /// <summary>
            /// Sends a <c>WM_COPYDATA</c> message to the specified window handle.
            /// </summary>
            public static void SendMessage(HandleRef hWnd, ref COPYDATASTRUCT data)
            {
                IntPtr sourceHandle = IntPtr.Zero; // There isn't a handle for the sender.

                if (SendMessage(hWnd, WM_COPYDATA, sourceHandle, ref data) == FALSE)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != ERROR_SUCCESS)
                        throw new Win32Exception(error);
                }
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

            [DllImport("kernel32.dll")]
            public static extern void SetLastError(uint dwErrCode);

            /// <summary>
            /// Passes the handle to all top level windows on the screen to the specified callback.
            /// </summary>
            public static void EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam)
            {
                if (EnumWindowsInternal(lpEnumFunc, lParam) == false)
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != ERROR_SUCCESS)
                        throw new Win32Exception(error);
                }
            }

            [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool EnumWindowsInternal(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            /// <summary>
            /// Gets the handle to the owner of the specified window handle.
            /// </summary>
            public static IntPtr GetWindowOwner(HandleRef hWnd)
            {
                return GetWindow(hWnd, 4 /* GW_OWNER */);
            }

            [DllImport("user32.dll", SetLastError = false)]
            private static extern IntPtr GetWindow(HandleRef hWnd, uint uCmd);

            /// <summary>
            /// Gets the text of the specified window's title bar or the
            /// empty string if the window has no title bar or text, or
            /// if the window handle is invalid.
            /// </summary>
            public static string GetWindowText(HandleRef hWnd)
            {
                int len = GetWindowTextLength(hWnd);
                if (len == 0)
                    return string.Empty;

                var lpString = new StringBuilder(len + 1);
                if (GetWindowText(hWnd, lpString, lpString.Capacity) == 0)
                    return string.Empty;

                return lpString.ToString();
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            private static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
            private static extern int GetWindowTextLength(HandleRef hWnd);

            /// <summary>
            /// Gets the ID of the process that created the specified window handle
            /// or zero if the handle is invalid.
            /// </summary>
            public static int GetWindowProcessId(HandleRef hWnd)
            {
                uint dwProcessId;
                GetWindowThreadProcessId(hWnd, out dwProcessId);
                return (int)dwProcessId;
            }

            [DllImport("user32.dll", SetLastError = false)]
            private static extern uint GetWindowThreadProcessId(HandleRef hWnd, out uint lpdwProcessId);

            /// <summary>
            /// Gets visibility of the specified window.
            /// </summary>
            [DllImport("user32.dll", SetLastError = false)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(HandleRef hWnd);
        }

        private class AcadWindowFinder
        {
            // Cache of the previously discovered handle. Good for perf.
            private static IntPtr? previousBestHandle;

            private readonly int processId;
            private IntPtr bestHandle;

            private AcadWindowFinder(int processId)
            {
                this.processId = processId;
                bestHandle = IntPtr.Zero;
            }

            public static IntPtr FindMainWindow(int processId)
            {
                if (previousBestHandle.HasValue)
                {
                    if (IsMainWindow(new HandleRef(null, previousBestHandle.Value), processId))
                        return previousBestHandle.Value;

                    previousBestHandle = null;
                }

                var finder = new AcadWindowFinder(processId);

                EnumWindowsProc callback = finder.EnumWindowsCallback;
                NativeMethods.EnumWindows(callback, IntPtr.Zero);
                GC.KeepAlive(callback);

                previousBestHandle = finder.bestHandle;
                return finder.bestHandle;
            }

            /// <summary>
            /// Returns whether the specified handle is for the "main window" of AutoCAD.
            /// </summary>
            /// <remarks>
            /// This method will return false if either <paramref name="hWnd"/>
            /// or <paramref name="processId"/> are invalid.
            /// </remarks>
            private static bool IsMainWindow(HandleRef hWnd, int processId)
            {
                int windowProcessId = NativeMethods.GetWindowProcessId(hWnd);
                if (windowProcessId != processId)
                    return false;

                IntPtr ownerHandle = NativeMethods.GetWindowOwner(hWnd);
                if (ownerHandle != IntPtr.Zero)
                    return false;

                if (NativeMethods.IsWindowVisible(hWnd) == false)
                    return false;

                string title = NativeMethods.GetWindowText(hWnd);
                if (title.IndexOf("AUTOCAD", StringComparison.InvariantCultureIgnoreCase) == -1)
                    return false;

                return true;
            }

            private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
            {
                if (IsMainWindow(new HandleRef(this, handle), processId) == false)
                    return true;

                NativeMethods.SetLastError(NativeMethods.ERROR_SUCCESS);
                bestHandle = handle;
                return false;
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion
    }
}
