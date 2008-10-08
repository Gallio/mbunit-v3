using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using Gallio.Utilities;

namespace Gallio.Runtime
{
    /// <summary>
    /// Detects the kind of process that is being used.
    /// </summary>
    public static class ProcessTypeDetection
    {
        private static readonly Memoizer<ProcessType> processType;

        /// <summary>
        /// Gets the process type of the current process.
        /// </summary>
        public static ProcessType ProcessType
        {
            get { return processType.Memoize(DetectProcessType); }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
        private static ProcessType DetectProcessType()
        {
            if (!RuntimeDetection.IsUsingMono)
            {
                if (HasConsoleWindow())
                    return ProcessType.Console;

                if (HasServiceUserToken())
                    return ProcessType.Service;
            }

            return Environment.UserInteractive ? ProcessType.Interactive : ProcessType.Console;
        }

        private static bool HasConsoleWindow()
        {
            return Native.GetConsoleWindow() != IntPtr.Zero;
        }

        private static bool HasServiceUserToken()
        {
            // We guess that a process is a server based on whether the user of the process
            // has been granted RunAsService.  This might not be completely accurate.
            IntPtr currentProcess = Native.GetCurrentProcess();
            IntPtr tokenHandle;
            if (!Native.OpenProcessToken(new HandleRef(null, currentProcess), Native.TOKEN_QUERY, out tokenHandle))
                throw new Win32Exception();

            WindowsIdentity processIdentity = new WindowsIdentity(tokenHandle);
            foreach (IdentityReference group in processIdentity.Groups)
            {
                string value = group.Value;
                if (value == Native.SECURITY_SERVICE_RID
                    || value == Native.SECURITY_LOCAL_SYSTEM_RID)
                    return true;
            }

            return false;
        }

        private static class Native
        {
            public const int TOKEN_QUERY = 0x00000008;
            public const string SECURITY_SERVICE_RID = "S-1-5-6";
            public const string SECURITY_LOCAL_SYSTEM_RID = "S-1-5-18";

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool OpenProcessToken(HandleRef ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();
        }
    }
}
