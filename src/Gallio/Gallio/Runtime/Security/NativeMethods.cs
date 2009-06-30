using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Gallio.Runtime.Security
{
    internal class NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern int LogonUser(string userName, string domain, string password, int logonType, int logonProvider, ref IntPtr token);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int DuplicateToken(IntPtr token, int impersonationLevel, ref IntPtr newToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern bool CloseHandle(IntPtr handle);
    }
}
