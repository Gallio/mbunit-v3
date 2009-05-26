using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.UI.Controls
{
    internal static class Native
    {
        public const uint BCM_SETSHIELD = 0x0000160C;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
