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
using System.Diagnostics;
using System.IO;
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

            var hwnd = WaitForMessagePump(process, ReadyTimeout, ReadyPollInterval);
            SendCopyDataMessage(hwnd, lisp);
        }

        private HandleRef WaitForMessagePump(IProcess process, TimeSpan timeout, TimeSpan pollInterval)
        {
            // Poll the process until it creates a "main" window. Using MainWindowHandle
            // may become problematic if acad.exe created multiple top-level unowned windows.
            // See http://blogs.msdn.com/oldnewthing/archive/2008/02/20/7806911.aspx for discussion.

            process.Refresh();
            var stopwatch = Stopwatch.StartNew();
            while (process.MainWindowHandle == IntPtr.Zero)
            {
                if (stopwatch.Elapsed > timeout)
                    throw new TimeoutException("Timeout waiting for AutoCAD to create message pump.");

                Thread.Sleep(pollInterval);
                process.Refresh();
            }

            var remaining = timeout - stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero || !process.WaitForInputIdle((int)remaining.TotalMilliseconds))
                throw new TimeoutException("Timeout waiting for AutoCAD to enter an idle state.");

            return new HandleRef(this, process.MainWindowHandle);
        }

        private static void SendCopyDataMessage(HandleRef hwnd, string message)
        {
            try
            {
                var cds = new COPYDATASTRUCT(message);
                SendMessage(hwnd, ref cds);
                GC.KeepAlive(cds);
            }
            catch (FileNotFoundException)
            {
                // Can fail if the handle is not valid.
            }
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

        private const uint WM_COPYDATA = 0x4A;
        private static readonly IntPtr FALSE = IntPtr.Zero;
        private const int ERROR_SUCCESS = 0;

        struct COPYDATASTRUCT
        {
            public COPYDATASTRUCT(string message)
            {
                dwData = new IntPtr(1);
                cbData = (message.Length + 1) * Marshal.SystemDefaultCharSize;
                lpData = message;
            }

            // ReSharper disable UnaccessedField.Local
            IntPtr dwData;
            int cbData;
            [MarshalAs(UnmanagedType.LPTStr)]
            string lpData;
            // ReSharper restore UnaccessedField.Local
        }

        private static void SendMessage(HandleRef handle, ref COPYDATASTRUCT data)
        {
            if (SendMessage(handle, WM_COPYDATA, IntPtr.Zero, ref data) == FALSE)
            {
                if (Marshal.GetLastWin32Error() == ERROR_SUCCESS)
                    return;

                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        // ReSharper restore InconsistentNaming
        #endregion
    }
}
