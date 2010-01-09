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
using System.Globalization;
using System.IO;
using Gallio.Common.Platform;
using Microsoft.Win32;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Provides services for resolving the path of the MSTest installation.
    /// </summary>
    internal static class MSTestResolver
    {
        /// <summary>
        /// Finds the path of a particular version of MSTest.
        /// </summary>
        /// <param name="visualStudioVersion">The visual studio version (eg. "8.0" or "9.0").</param>
        /// <returns>The full path of the MSTest.exe program, or null if not found.</returns>
        public static string FindMSTestPathForVisualStudioVersion(string visualStudioVersion)
        {
            string result = null;

            RegistryUtils.TryActionOnOpenSubKeyWithBitness(Registry.LocalMachine,
                @"SOFTWARE\Microsoft\VisualStudio\" + visualStudioVersion,
                @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\" + visualStudioVersion,
                key =>
                {
                    string visualStudioInstallDir = (string)key.GetValue("InstallDir");
                    if (visualStudioInstallDir != null)
                    {
                        string msTestExecutablePath = Path.Combine(visualStudioInstallDir, "MSTest.exe");
                        if (File.Exists(msTestExecutablePath))
                        {
                            result = msTestExecutablePath;
                            return true;
                        }
                    }

                    return false;
                });

            return result;
        }
    }
}
