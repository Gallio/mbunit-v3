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
