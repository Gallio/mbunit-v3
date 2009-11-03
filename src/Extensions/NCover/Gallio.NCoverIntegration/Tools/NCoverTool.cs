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
using System.Reflection;
using System.Text;
using Gallio.Common.Concurrency;
using Gallio.Common.Platform;
using Gallio.Runtime.Logging;
using Microsoft.Win32;

namespace Gallio.NCoverIntegration.Tools
{
    public abstract class NCoverTool
    {
        public static NCoverTool GetInstance(NCoverVersion version)
        {
            switch (version)
            {
                case NCoverVersion.V1:
                    return NCoverV1Tool.Instance;

                case NCoverVersion.V2:
                    return NCoverV2Tool.Instance;

                case NCoverVersion.V3:
                    return NCoverV3Tool.Instance;

                default:
                    throw new NotSupportedException("Unrecognized NCover version.");
            }
        }

        public abstract string Name { get; }

        public bool IsInstalled()
        {
            return GetInstallDir() != null;
        }

        public abstract string GetInstallDir();

        public ProcessTask CreateNCoverConsoleTask(string executablePath, string arguments, string workingDirectory,
            string ncoverArguments, string ncoverCoverageFile, ILogger logger)
        {
            string ncoverConsolePath = Path.Combine(GetInstallDir(), "NCover.Console.exe");

            StringBuilder ncoverArgumentsCombined = new StringBuilder();
            BuildNCoverConsoleArguments(ncoverArgumentsCombined, executablePath, arguments, workingDirectory, ncoverArguments, ncoverCoverageFile);

            logger.Log(LogSeverity.Info, string.Format("Starting {0}: \"{1}\" {2}", Name, ncoverConsolePath, ncoverArgumentsCombined));

            RegisterNCoverIfNecessary();
            return new ProcessTask(ncoverConsolePath, ncoverArgumentsCombined.ToString(), workingDirectory);
        }

        public void Merge(IList<string> sources, string destination, ILogger logger)
        {
            if (File.Exists(destination))
                File.Delete(destination);

            if (sources.Count == 0)
                return;

            if (sources.Count == 1)
            {
                File.Move(sources[0], destination);
                return;
            }

            ProcessTask mergeTask = CreateMergeTask(sources, destination);

            logger.Log(LogSeverity.Info, string.Format("Merging {0} coverage files to '{1}': \"{2}\" {3}", Name, destination, mergeTask.ExecutablePath, mergeTask.Arguments));

            mergeTask.CaptureConsoleOutput = true;
            mergeTask.ConsoleOutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    logger.Log(LogSeverity.Info, e.Data);
            };

            mergeTask.CaptureConsoleError = true;
            mergeTask.ConsoleErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    logger.Log(LogSeverity.Error, e.Data);
            };
            
            if (!mergeTask.Run(null))
                throw new NCoverToolException(string.Format("Failed to merge {0} coverage files.  The merge command failed with exit code {1}.", Name, mergeTask.ExitCode));

            if (!File.Exists(destination))
                throw new NCoverToolException(string.Format("Failed to merge {0} coverage files.  The merge command did not produce the merged file as expected.", Name));

            foreach (string source in sources)
                File.Delete(source);
        }

        public ProcessorArchitecture NegotiateProcessorArchitecture(ProcessorArchitecture requestedArchitecture)
        {
            if (!RequiresX86())
                return requestedArchitecture;

            if (requestedArchitecture == ProcessorArchitecture.Amd64 || requestedArchitecture == ProcessorArchitecture.IA64)
                throw new NCoverToolException(string.Format("NCover {0} must run code as a 32bit process but the requested architecture was 64bit.  Please use a newer version of NCover.", Name));

            return ProcessorArchitecture.X86;
        }

        public string NegotiateRuntimeVersion(string requestedRuntimeVersion)
        {
            if (!RequiresDotNet20())
                return requestedRuntimeVersion;

            if (requestedRuntimeVersion == null)
                return DotNetRuntimeSupport.InstalledDotNet20RuntimeVersion;

            if (requestedRuntimeVersion.Contains("2.0."))
                return requestedRuntimeVersion;

            throw new NCoverToolException(string.Format("{0} does not support .Net runtime {1} at this time.  Please use a newer version of NCover.", Name, requestedRuntimeVersion));
        }

        protected virtual bool RequiresX86()
        {
            return false;
        }

        protected virtual bool RequiresDotNet20()
        {
            return false;
        }

        protected virtual void BuildNCoverConsoleArguments(StringBuilder result, string executablePath, string arguments, string workingDirectory,
            string ncoverArguments, string ncoverCoverageFile)
        {
            result.Append('"').Append(executablePath).Append('"');
            result.Append(' ').Append(arguments);
            result.Append(" //w \"").Append(RemoveTrailingBackslash(workingDirectory)).Append('"');
            result.Append(" //x \"").Append(ncoverCoverageFile).Append('"');

            if (ncoverArguments.Length != 0)
                result.Append(' ').Append(ncoverArguments);
        }

        protected abstract ProcessTask CreateMergeTask(IList<string> sources, string destination);

        protected virtual void RegisterNCoverIfNecessary()
        {
        }

        protected static bool GetNCoverInstallInfoFromRegistry(string versionPrefix, out string version, out string installDir)
        {
            string resultVersion = null;
            string resultInstallDir = null;

            bool resultSuccess = RegistryUtils.TryActionOnOpenSubKeyWithBitness(Registry.LocalMachine,
                @"Software\Gnoso\NCover",
                @"Software\Wow6432Node\Gnoso\NCover",
                key =>
                {
                    string candidateVersion = key.GetValue("CurrentVersion") as string;
                    if (candidateVersion != null && candidateVersion.StartsWith(versionPrefix))
                    {
                        resultInstallDir = key.GetValue("InstallDir") as string;
                        if (resultInstallDir != null)
                        {
                            resultVersion = candidateVersion;
                            return true;
                        }
                    }

                    return false;
                });

            version = resultVersion;
            installDir = resultInstallDir;
            return resultSuccess;
        }

        protected static string RemoveTrailingBackslash(string path)
        {
            if (path.EndsWith(@"\"))
                return path.Substring(0, path.Length - 1);
            return path;
        }

        protected ProcessTask CreateNCoverReportingMergeTask(IList<string> sources, string destination)
        {
            string exeDir = GetInstallDir();
            string exePath = Path.Combine(exeDir, "NCover.Reporting.exe");

            var arguments = new StringBuilder();
            arguments.Append("//q //s \"").Append(Path.GetFullPath(destination)).Append('"');
            AppendSources(arguments, sources);

            return new ProcessTask(exePath, arguments.ToString(), exeDir);
        }

        protected ProcessTask CreateNCoverExplorerConsoleMergeTask(string relativeDirPath, IList<string> sources, string destination)
        {
            string exeDir = Path.GetFullPath(Path.Combine(GetInstallDir(), relativeDirPath));
            string exePath = Path.Combine(exeDir, "NCoverExplorer.Console.exe");

            var arguments = new StringBuilder();
            arguments.Append("/q /s:\"").Append(Path.GetFullPath(destination)).Append('"');
            AppendSources(arguments, sources);

            return new ProcessTask(exePath, arguments.ToString(), exeDir);
        }

        private static void AppendSources(StringBuilder arguments, IList<string> sources)
        {
            foreach (string source in sources)
            {
                arguments.Append(" \"").Append(Path.GetFullPath(source)).Append('"');
            }
        }
    }
}
