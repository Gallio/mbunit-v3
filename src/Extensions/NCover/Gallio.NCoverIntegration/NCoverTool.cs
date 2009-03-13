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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Concurrency;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Microsoft.Win32;

namespace Gallio.NCoverIntegration
{
    internal static class NCoverTool
    {
        public static ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory, int majorVersion,
            ILogger logger, string ncoverArguments, string ncoverCoverageFile)
        {
            string installDir = GetNCoverInstallDir(majorVersion);
            if (installDir == null)
                throw new HostException("NCover v" + majorVersion + " does not appear to be installed.");

            string ncoverConsolePath = Path.Combine(installDir, "NCover.Console.exe");

            StringBuilder ncoverArgumentsCombined = new StringBuilder();
            ncoverArgumentsCombined.Append('"').Append(executablePath).Append('"');
            ncoverArgumentsCombined.Append(' ').Append(arguments);
            ncoverArgumentsCombined.Append(" //w \"").Append(RemoveTrailingBackslash(workingDirectory)).Append('"');
            ncoverArgumentsCombined.Append(" //x \"").Append(ncoverCoverageFile).Append('"');

            if (ncoverArguments.Length != 0)
                ncoverArgumentsCombined.Append(' ').Append(ncoverArguments);

            logger.Log(LogSeverity.Info, string.Format("Starting NCover v{0} with arguments: {1}", majorVersion, ncoverArgumentsCombined));

            return new ProcessTask(ncoverConsolePath, ncoverArgumentsCombined.ToString(), workingDirectory);
        }

        public static bool IsNCoverVersionInstalled(int majorVersion)
        {
            return GetNCoverInstallDir(majorVersion) != null;
        }

        private static string GetNCoverInstallDir(int majorVersion)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Gnoso\NCover")
                ?? Registry.LocalMachine.OpenSubKey(@"WOW6432Node\Software\Gnoso\NCover");
            if (key != null)
            {
                string currentVersion = (string) key.GetValue("CurrentVersion");
                if (currentVersion != null && currentVersion.StartsWith(majorVersion + "."))
                {
                    string installDir = (string) key.GetValue("InstallDir");
                    if (installDir != null)
                        return installDir;
                }
            }

            return null;
        }

        private static string RemoveTrailingBackslash(string path)
        {
            if (path.EndsWith(@"\"))
                return path.Substring(0, path.Length - 1);
            return path;
        }
    }
}
