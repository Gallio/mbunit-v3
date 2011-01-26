// Copyright 2010 Nicolas Graziano 
// largely inspired by NCoverIntegration in Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Reflection;

namespace Gallio.PartCoverIntegration.Tools
{
    public sealed class PartCoverTool
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="PartCoverTool"/> class.
        /// <remarks>Private contructor, to prevent initialize one.</remarks>
        /// </summary>
        private PartCoverTool()
        { }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public static string Name
        {
            get { return "PartCover"; }

        }

        /// <summary>
        /// Determines whether PartCover is installed.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if PartCover is installed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInstalled()
        {
            return GetPartCoverExePath() != null;
        }

        /// <summary>
        /// Gets the PartCover executable path.
        /// </summary>
        /// <returns>The PartCover excutable path.</returns>
        private static string GetPartCoverExePath()
        {
            string pluginDir = Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(PartCoverTool).Assembly));
            string partcoverDir = Path.Combine(pluginDir, @"libs\PartCover\PartCover.exe");
            if (File.Exists(partcoverDir))
                return partcoverDir;
#if DEBUG
            partcoverDir = Path.GetFullPath(Path.Combine(pluginDir, @"..\..\libs\PartCover\PartCover.exe"));
            if (File.Exists(partcoverDir))
                return partcoverDir;
#endif
            return null;
        }

        /// <summary>
        /// Gets the report generator executable path.
        /// </summary>
        /// <returns>The report generator executable path.</returns>
        private static string GetReportGeneratorExePath()
        {
            string pluginDir = Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(PartCoverTool).Assembly));
            string partcoverDir = Path.Combine(pluginDir, @"libs\ReportGenerator\ReportGenerator.exe");
            if (File.Exists(partcoverDir))
                return partcoverDir;
#if DEBUG
            partcoverDir = Path.GetFullPath(Path.Combine(pluginDir, @"..\..\libs\ReportGenerator\ReportGenerator.exe"));
            if (File.Exists(partcoverDir))
                return partcoverDir;
#endif
            return null;
        }


        /// <summary>
        /// Creates the part cover console task.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments of the executable.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="partcoverIncludes">The partcover includes rules.</param>
        /// <param name="partcoverExcludes">The partcover excludes rules.</param>
        /// <param name="partcoverCoverageFile">The partcover coverage file.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The process task wich run under PartCover</returns>
        public static ProcessTask CreatePartCoverConsoleTask(string executablePath, string arguments, string workingDirectory,
            string partcoverIncludes, string partcoverExcludes, string partcoverCoverageFile, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (String.IsNullOrEmpty(executablePath) || !File.Exists(executablePath))
                throw new ArgumentException("The executable path can't be null or empty", "executablePath");
            string partcoverConsolePath = GetPartCoverExePath();
            string partcoverArgumentsCombined = BuildPartCoverArguments(executablePath, arguments, workingDirectory, partcoverIncludes, partcoverExcludes, partcoverCoverageFile);

            logger.Log(LogSeverity.Info, string.Format("Starting {0}: \"{1}\" {2}", Name, partcoverConsolePath, partcoverArgumentsCombined));

            return new ProcessTask(partcoverConsolePath, partcoverArgumentsCombined, workingDirectory);
        }

        /// <summary>
        /// Creates the coverage report from the xml coverage files spécified.
        /// </summary>
        /// <param name="sources">The sources XML files.</param>
        /// <param name="destination">The destination directory.</param>
        /// <param name="logger">The logger.</param>
        public static void CreateReport(IList<string> sources, string destination, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (sources == null || sources.Count == 0)
                return;

            if (String.IsNullOrEmpty(destination))
                throw new ArgumentException("Destination must be a valid path.", "destination");
            
            if (Directory.Exists(destination))
                Directory.Delete(destination, true);

            ProcessTask mergeTask = CreateMergeTask(sources, destination);

            logger.Log(LogSeverity.Info, string.Format("Genrating {0} coverage report to '{1}': \"{2}\" {3}", Name, destination, mergeTask.ExecutablePath, mergeTask.Arguments));

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
                throw new PartCoverToolException(string.Format("Failed to generate {0} coverage report.  The report command failed with exit code {1}.", Name, mergeTask.ExitCode));

            int i = 0;
            foreach (string source in sources)
            {
                if (!string.IsNullOrEmpty(source) && File.Exists(source))
                {
                    i++;
                    File.Move(source,Path.Combine(destination,String.Format("coverage_{0:D4}.xml",i)));
                }
            }
        }

        /// <summary>
        /// Negotiates the processor architecture.
        /// </summary>
        /// <param name="requestedArchitecture">The requested architecture.</param>
        /// <returns>The acepted architecture.</returns>
        public static ProcessorArchitecture NegotiateProcessorArchitecture(ProcessorArchitecture requestedArchitecture)
        {
            //TODO add limitation if any on partcover
            if (requestedArchitecture == ProcessorArchitecture.IA64)
                throw new PartCoverToolException(string.Format("PartCover {0} does not support IA64.", Name));

            return requestedArchitecture;
        }

        /// <summary>
        /// Negotiates the runtime version.
        /// </summary>
        /// <param name="requestedRuntimeVersion">The requested runtime version.</param>
        /// <returns>The accepted runtime version.</returns>
        public static string NegotiateRuntimeVersion(string requestedRuntimeVersion)
        {
            //TODO add limitation on runtime if necessary
            return requestedRuntimeVersion;
        }

        /// <summary>
        /// Builds the PartCover arguments.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="partcoverIncludes">The partcover includes.</param>
        /// <param name="partcoverExcludes">The partcover excludes.</param>
        /// <param name="partcoverCoverageFile">The partcover coverage file.</param>
        /// <returns>The arguments</returns>
        private static string BuildPartCoverArguments(string executablePath, string arguments, string workingDirectory,
            string partcoverIncludes, string partcoverExcludes, string partcoverCoverageFile)
        {
            StringBuilder result = new StringBuilder();
            result.Append(" --target \"").Append(executablePath).Append('"');
            if (!String.IsNullOrEmpty(workingDirectory))
            {
                result.Append(" --target-work-dir \"").Append(RemoveTrailingBackslash(workingDirectory)).Append('"');
            }
            if (!String.IsNullOrEmpty(arguments))
            {
                // Caracter " must be escaped in argument
                result.Append(" --target-args \"").Append(arguments.Replace("\"", "\\\"")).Append('"');
            }
            if (!String.IsNullOrEmpty(partcoverCoverageFile))
            {
                result.Append(" --output \"").Append(partcoverCoverageFile).Append('"');
            }
            if (!String.IsNullOrEmpty(partcoverIncludes))
            {
                foreach (string partCoverInclude in partcoverIncludes.Split(';'))
                {
                    if (!String.IsNullOrEmpty(partCoverInclude))
                        result.Append(" --include \"").Append(partCoverInclude).Append("\"");
                }
            }
            if (!String.IsNullOrEmpty(partcoverExcludes))
            {
                foreach (string partCoverExclude in partcoverExcludes.Split(';'))
                {
                    if (!String.IsNullOrEmpty(partCoverExclude))
                        result.Append(" --exclude \"").Append(partCoverExclude).Append("\"");
                }
            }

            return result.ToString();
        }

        private static ProcessTask CreateMergeTask(IList<string> sources, string destination)
        {
            string exePath = GetReportGeneratorExePath();

            string[] sourcesArray = new string[sources.Count];
            sources.CopyTo(sourcesArray, 0);
            string arguments = '"' + string.Join(";", sourcesArray) + "\" \"" + destination + "\"";

            return new ProcessTask(exePath, arguments, Path.GetDirectoryName(exePath));

        }

        /// <summary>
        /// Removes the trailing backslash.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The path</returns>
        private static string RemoveTrailingBackslash(string path)
        {
            return path.TrimEnd('\\');
        }

    }
}
