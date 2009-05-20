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
using System.Reflection;
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
        private static Memoizer<string> runtimeVersionMemoizer = new Memoizer<string>();

        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        /// <remarks>
        /// It is occasionally necessary to tailor the execution of the test runner
        /// depending on whether Mono is running.  However, the number of such
        /// customizations should be very limited.
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
        /// <param name="executablePath">The executable path</param>
        /// <param name="arguments">The arguments</param>
        /// <returns>The process start info</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="executablePath"/>
        /// or <paramref name="arguments"/> is null</exception>
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
        /// Gets the Major.Minor.Build components of the .Net runtime version in use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The runtime version indicates the version of the virtual machine that is running.
        /// It cannot be used to distinguish among the .Net 2.0, 3.0 and 3.5 frameworks because
        /// those frameworks are just libraries that usually run on the 2.0.50727 runtime.
        /// </para>
        /// </remarks>
        /// <returns>The Major.Minor.Build components of the current runtime, eg. "2.0.50727" or "4.0.20506"</returns>
        public static string RuntimeVersion
        {
            get
            {
                return runtimeVersionMemoizer.Memoize(() =>
                {
                    Version version = Assembly.GetAssembly(typeof (Int32)).GetName().Version;
                    return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
                });
            }
        }
        
        /// <summary>
        /// Gets the Major.Minor.Build components of the installed .Net 2.0 runtime version,
        /// or null if not installed.
        /// </summary>
        public static string InstalledDotNet20RuntimeVersion
        {
            get
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727", "Version", null) != null
                    ? "2.0.50727"
                    : null;
            }
        }

        /// <summary>
        /// Gets the Major.Minor.Build components of the installed .Net 4.0 runtime version,
        /// or null if not installed.
        /// </summary>
        public static string InstalledDotNet40RuntimeVersion
        {
            get
            {
                return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0", "Version", null);
            }
        }
    }
}