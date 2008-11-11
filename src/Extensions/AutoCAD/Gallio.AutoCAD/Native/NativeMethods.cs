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
