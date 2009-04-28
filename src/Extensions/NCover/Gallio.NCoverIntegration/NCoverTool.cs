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
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Microsoft.Win32;

namespace Gallio.NCoverIntegration
{
    internal static class NCoverTool
    {
        public static ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory, NCoverVersion version,
            ILogger logger, string ncoverArguments, string ncoverCoverageFile)
        {
            switch (version)
            {
                case NCoverVersion.V1:
                    return CreateNCoverEmbeddedProcessTask(executablePath, arguments, workingDirectory, logger, ncoverArguments, ncoverCoverageFile);

                case NCoverVersion.V2:
                    return CreateNCoverConsoleProcessTask(executablePath, arguments, workingDirectory, 2, logger, ncoverArguments, ncoverCoverageFile);

                case NCoverVersion.V3:
                    return CreateNCoverConsoleProcessTask(executablePath, arguments, workingDirectory, 3, logger, ncoverArguments, ncoverCoverageFile);

                default:
                    throw new NotSupportedException("Unrecognized NCover version.");
            }
        }

        private static ProcessTask CreateNCoverEmbeddedProcessTask(string executablePath, string arguments, string workingDirectory,
            ILogger logger, string ncoverArguments, string ncoverCoverageFile)
        {
            // Can host directly inside 32bit process.
            if (IntPtr.Size == 4)
                return new EmbeddedNCoverProcessTask(executablePath, arguments, workingDirectory, logger, ncoverArguments, ncoverCoverageFile);

            // When running as 64bit process we need to use another process as a shim.
            // We have less control over what's going on but at least it might work.
            return CreateNCoverConsoleProcessTask(executablePath, arguments, workingDirectory, 1, logger, ncoverArguments, ncoverCoverageFile);
        }

        private static ProcessTask CreateNCoverConsoleProcessTask(string executablePath, string arguments, string workingDirectory, int majorVersion,
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

            if (majorVersion == 1)
            {
                ncoverArgumentsCombined.Append(" //reg");

                if (!ncoverArguments.Contains("//l"))
                    ncoverArgumentsCombined.Append(" //q");
            }

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
            if (majorVersion == 1)
                return GetEmbeddedNCoverInstallDir();

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

        private static string GetEmbeddedNCoverInstallDir()
        {
            return EmbeddedNCoverProcessTask.GetEmbeddedNCoverInstallDir();
        }

        private static string RemoveTrailingBackslash(string path)
        {
            if (path.EndsWith(@"\"))
                return path.Substring(0, path.Length - 1);
            return path;
        }
    }
}
