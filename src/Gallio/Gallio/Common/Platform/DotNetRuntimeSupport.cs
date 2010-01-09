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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Common.Reflection;
using Microsoft.Win32;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Provides support for working with different implementations of the .Net runtime.
    /// </summary>
    public static class DotNetRuntimeSupport
    {
        private static Memoizer<DotNetRuntimeType> runtimeTypeMemoizer = new Memoizer<DotNetRuntimeType>();

        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is occasionally necessary to tailor the execution of the test runner
        /// depending on whether Mono is running. However, the number of such
        /// customizations should be very limited.
        /// </para>
        /// </remarks>
        public static bool IsUsingMono
        {
            get { return RuntimeType == DotNetRuntimeType.Mono; }
        }

        /// <summary>
        /// Get the type of the .Net runtime currently executing this process.
        /// </summary>
        public static DotNetRuntimeType RuntimeType
        {
            get
            {
                return runtimeTypeMemoizer.Memoize(() =>
                {
                    if (Type.GetType(@"Mono.Runtime") != null)
                        return DotNetRuntimeType.Mono;

                    return DotNetRuntimeType.CLR;
                });
            }
        }

        /// <summary>
        /// When using Mono, creates <see cref="ProcessStartInfo" /> that re-enters the Mono runtime
        /// if the executable is .Net otherwise creates a standard process start info.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>The process start info.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null.</exception>
        public static ProcessStartInfo CreateReentrantProcessStartInfo(string executablePath, string arguments)
        {
            if (executablePath == null)
                throw new ArgumentNullException("executablePath");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (IsUsingMono && AssemblyUtils.IsAssembly(executablePath))
            {
                string monoExe = GetCurrentMonoRuntimePath();
                if (monoExe != null)
                    return new ProcessStartInfo(monoExe, String.Concat("\"", executablePath, "\" ", arguments));
            }

            return new ProcessStartInfo(executablePath, arguments);
        }

        private static string GetCurrentMonoRuntimePath()
        {
            Process process = Process.GetCurrentProcess();
            if (process.MainModule != null)
                return process.MainModule.FileName;

            return null;
        }

        /// <summary>
        /// Gets the .Net runtime version in use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The runtime version indicates the version of the virtual machine that is running.
        /// It cannot be used to distinguish among the .Net 2.0, 3.0 and 3.5 frameworks because
        /// those frameworks are just libraries that usually run on the 2.0.50727 runtime on the CLR.
        /// </para>
        /// </remarks>
        /// <returns>The runtime version, eg. "v2.0.50727" or "v4.0.21006"</returns>
        public static string RuntimeVersion
        {
            get
            {
                return RuntimeEnvironment.GetSystemVersion(); // starts with 'v'
            }
        }
        
        /// <summary>
        /// Gets the version of the installed .Net 2.0 runtime, or null if not installed.
        /// </summary>
        /// <returns>The runtime version, eg. "v2.0.50727"</returns>
        public static string InstalledDotNet20RuntimeVersion
        {
            get
            {
                // FIXME: faking it might not be a good idea...
                if (IsUsingMono)
                    return "v2.0.50727";

                // fixme: more than a little naive...
                return RegistryUtils.GetValueWithBitness(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727",
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\NET Framework Setup\NDP\v2.0.50727",
                    "Version", null) != null
                    ? "v2.0.50727"
                    : null;
            }
        }

        /// <summary>
        /// Gets the version of the installed .Net 4.0 runtime version, or null if not installed.
        /// </summary>
        /// <returns>The runtime version, eg. "v4.0.21006"</returns>
        public static string InstalledDotNet40RuntimeVersion
        {
            get
            {
                // FIXME: faking it might not be a good idea...
                if (IsUsingMono)
                    return "v4.0.21006";

                string runtimeVersion = (string) RegistryUtils.GetValueWithBitness(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0",
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\NET Framework Setup\NDP\v4.0",
                        "Version", null);
                return runtimeVersion != null ? "v" + runtimeVersion : null;
            }
        }

        /// <summary>
        /// Gets the version of the most recent installed .Net runtime version.
        /// </summary>
        /// <returns>The runtime version, eg. "v4.0.21006"</returns>
        public static string MostRecentInstalledDotNetRuntimeVersion
        {
            get
            {
                string mostRecentInstalledRuntimeVersion = InstalledDotNet40RuntimeVersion ?? InstalledDotNet20RuntimeVersion;
                if (mostRecentInstalledRuntimeVersion == null || string.Compare(mostRecentInstalledRuntimeVersion, RuntimeVersion) < 0)
                    return RuntimeVersion;

                return mostRecentInstalledRuntimeVersion;
            }
        }
    }
}