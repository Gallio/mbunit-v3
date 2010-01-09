// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Provides support for working with different process execution models.
    /// </summary>
    public static class ProcessSupport
    {
        private static Memoizer<ProcessType> processType = new Memoizer<ProcessType>();
        private static Memoizer<ProcessIntegrityLevel> processIntegrityLevel = new Memoizer<ProcessIntegrityLevel>();

        /// <summary>
        /// Returns true if the current process is running in 32bit mode.
        /// </summary>
        public static bool Is32BitProcess
        {
            get { return IntPtr.Size == 4; }
        }

        /// <summary>
        /// Returns true if the current process is running in 64bit mode.
        /// </summary>
        public static bool Is64BitProcess
        {
            get { return IntPtr.Size == 8; }
        }
 
        /// <summary>
        /// Gets the process type of the current process.
        /// </summary>
        public static ProcessType ProcessType
        {
            get { return processType.Memoize(DetectProcessType); }
        }

        /// <summary>
        /// Gets the integrity level of the current process.
        /// </summary>
        public static ProcessIntegrityLevel ProcessIntegrityLevel
        {
            get { return processIntegrityLevel.Memoize(DetectProcessIntegrityLevel); }
        }

        /// <summary>
        /// Returns true if the current process has elevated privileges.
        /// </summary>
        public static bool HasElevatedPrivileges
        {
            get
            {
                if (DotNetRuntimeSupport.IsUsingMono)
                    return true; // FIXME

                WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
        private static ProcessType DetectProcessType()
        {
            if (!DotNetRuntimeSupport.IsUsingMono)
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
            IntPtr tokenHandle = GetProcessToken();
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

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private static ProcessIntegrityLevel DetectProcessIntegrityLevel()
        {
            IntPtr tokenHandle = GetProcessToken();

            int labelSize;
            if (Native.GetTokenInformation(tokenHandle, Native.TokenIntegrityLevel, IntPtr.Zero, 0, out labelSize)
                || Marshal.GetLastWin32Error() != Native.ERROR_INSUFFICIENT_BUFFER)
                return ProcessIntegrityLevel.Unknown;

            IntPtr labelPtr = IntPtr.Zero;
            try
            {
                labelPtr = Marshal.AllocHGlobal(labelSize);

                if (!Native.GetTokenInformation(tokenHandle, Native.TokenIntegrityLevel, labelPtr, labelSize, out labelSize))
                    return ProcessIntegrityLevel.Unknown;

                var label = (TOKEN_MANDATORY_LABEL) Marshal.PtrToStructure(labelPtr, typeof(TOKEN_MANDATORY_LABEL));

                IntPtr sidSubAuthorityCountPtr = Native.GetSidSubAuthorityCount(label.Label.Sid);
                int sidSubAuthorityCount = Marshal.ReadInt32(sidSubAuthorityCountPtr);

                IntPtr sidSubAuthorityPtr = Native.GetSidSubAuthority(label.Label.Sid, sidSubAuthorityCount - 1);
                int sidSubAuthority = Marshal.ReadInt32(sidSubAuthorityPtr);

                if (sidSubAuthority < Native.SECURITY_MANDATORY_UNTRUSTED_RID)
                    return ProcessIntegrityLevel.Unknown;

                if (sidSubAuthority < Native.SECURITY_MANDATORY_LOW_RID)
                    return ProcessIntegrityLevel.Untrusted;

                if (sidSubAuthority < Native.SECURITY_MANDATORY_MEDIUM_RID)
                    return ProcessIntegrityLevel.Low;

                if (sidSubAuthority < Native.SECURITY_MANDATORY_HIGH_RID)
                    return ProcessIntegrityLevel.Medium;

                if (sidSubAuthority < Native.SECURITY_MANDATORY_SYSTEM_RID)
                    return ProcessIntegrityLevel.High;

                if (sidSubAuthority < Native.SECURITY_MANDATORY_PROTECTED_PROCESS_RID)
                    return ProcessIntegrityLevel.System;

                return ProcessIntegrityLevel.ProtectedProcess;
            }
            finally
            {
                Marshal.FreeHGlobal(labelPtr);
            }
        }

        private static IntPtr GetProcessToken()
        {
            IntPtr currentProcess = Native.GetCurrentProcess();
            IntPtr tokenHandle;
            if (!Native.OpenProcessToken(new HandleRef(null, currentProcess), Native.TOKEN_QUERY, out tokenHandle))
                throw new Win32Exception();
            return tokenHandle;
        }

        private static class Native
        {
            public const uint ERROR_INSUFFICIENT_BUFFER = 122;

            public const int TOKEN_QUERY = 0x00000008;

            public const string SECURITY_SERVICE_RID = "S-1-5-6";
            public const string SECURITY_LOCAL_SYSTEM_RID = "S-1-5-18";

            public const int SECURITY_MANDATORY_UNTRUSTED_RID = 0x00000000;
            public const int SECURITY_MANDATORY_LOW_RID = 0x00001000;
            public const int SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
            public const int SECURITY_MANDATORY_HIGH_RID = 0x00003000;
            public const int SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;
            public const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = 0x00005000;

            public const int TokenIntegrityLevel = 25;

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool OpenProcessToken(HandleRef ProcessHandle, int DesiredAccess, out IntPtr TokenHandle);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool GetTokenInformation(IntPtr TokenHandle, int TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);

            [DllImport("kernel32.dll")]
            public static extern IntPtr GetConsoleWindow();

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern IntPtr GetSidSubAuthority(IntPtr pSid, int nSubAuthority);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern IntPtr GetSidSubAuthorityCount(IntPtr pSid);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TOKEN_MANDATORY_LABEL
        {
            public SID_AND_ATTRIBUTES Label;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SID_AND_ATTRIBUTES
        {
            public IntPtr Sid;
            public int Attributes;
        }
    }
}