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
using System.Globalization;
using System.Runtime.InteropServices;

namespace Gallio.AutoCAD.Native
{
    /// <summary>
    /// Contains P/Invoke signatures for unmanaged functions.
    /// </summary>
    internal static class NativeMethods
    {
        private static readonly IntPtr FALSE = IntPtr.Zero;
        private const int ERROR_SUCCESS = 0;

        /// <summary>
        /// Sends a <c>WM_COPYDATA</c> message to he specified window handle.
        /// </summary>
        /// <param name="handle">The window handle to send the message to.</param>
        /// <param name="data">The data to be passed.</param>
        public static void SendMessage(HandleRef handle, ref COPYDATASTRUCT data)
        {
            if (SendMessage(handle, MessageTypes.WM_COPYDATA, IntPtr.Zero, ref data) == FALSE)
            {
                if (Marshal.GetLastWin32Error() == ERROR_SUCCESS)
                    return;

                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
    }
}
