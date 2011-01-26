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
using System.IO;
using Gallio.Common.Concurrency;
using Gallio.PartCoverIntegration.Tools;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.PartCoverIntegration
{
    /// <summary>
    /// An PartCover host is a variation on a <see cref="IsolatedProcessHost" />
    /// launches the process with the PartCover profiler attached.
    /// </summary>
    public class PartCoverHost : IsolatedProcessHost
    {
        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="logger">The logger for host message output.</param>
        /// <param name="installationPath">The runtime installation path where the hosting executable will be found.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// 	<paramref name="logger"/>, <paramref name="installationPath"/> is null.</exception>
        public PartCoverHost(HostSetup hostSetup, ILogger logger, string installationPath)
            : base(ForceProcessorArchitectureAndRuntimeVersionIfRequired(hostSetup), logger, installationPath)
        {
        }

        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            string partcoverIncludes, partcoverExcludes, partCoverCoverageFile;
            GetPartCoverProperties(HostSetup, out partcoverIncludes, out partcoverExcludes, out partCoverCoverageFile);

            return PartCoverTool.CreatePartCoverConsoleTask(executablePath, arguments, workingDirectory,
                partcoverIncludes, partcoverExcludes, partCoverCoverageFile, Logger);
        }

        /// <summary>
        /// Gets the part cover properties.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="partcoverIncludes">The partcover includes.</param>
        /// <param name="partcoverExcludes">The partcover excludes.</param>
        /// <param name="partcoverCoverageFile">The partcover coverage file.</param>
        internal static void GetPartCoverProperties(HostSetup hostSetup,
            out string partcoverIncludes, out string partcoverExcludes, out string partcoverCoverageFile)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            partcoverIncludes = hostSetup.Properties.GetValue("PartCoverIncludes");
            partcoverExcludes = hostSetup.Properties.GetValue("PartCoverExcludes");
            partcoverCoverageFile = hostSetup.Properties.GetValue("PartCoverCoverageFile");

            if (partcoverIncludes == null)
                partcoverIncludes = "[*]*";
            if (partcoverExcludes == null)
                partcoverExcludes = "[Gallio*]*;[MbUnit*]*;[Microsoft*]*";
            if (string.IsNullOrEmpty(partcoverCoverageFile))
                partcoverCoverageFile = "Coverage.xml";

            partcoverCoverageFile = Path.GetFullPath(partcoverCoverageFile);
        }

        /// <summary>
        /// Sets the part cover coverage file.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="partCoverCoverageFile">The part cover coverage file.</param>
        internal static void SetPartCoverCoverageFile(HostSetup hostSetup, string partCoverCoverageFile)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            hostSetup.RemoveProperty("PartCoverCoverageFile");
            hostSetup.AddProperty("PartCoverCoverageFile", partCoverCoverageFile);
        }

        internal static string GetPartCoverCoverageReportDir(HostSetup hostSetup)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            string result = hostSetup.Properties.GetValue("PartCoverCoverageReportDir");
            if (string.IsNullOrEmpty(result))
            {
                if (!string.IsNullOrEmpty(hostSetup.WorkingDirectory))
                {
                    result = Path.GetFullPath(hostSetup.WorkingDirectory);
                    result = Path.Combine(result, "CoverageReport");
                }
                else
                    result = "CoverageReport";
            }

            return result;
        }


        /// <summary>
        /// Forces the processor architecture and runtime version if required.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <returns>The modified host setup.</returns>
        private static HostSetup ForceProcessorArchitectureAndRuntimeVersionIfRequired(HostSetup hostSetup)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            hostSetup = hostSetup.Copy();
            hostSetup.ProcessorArchitecture = PartCoverTool.NegotiateProcessorArchitecture(hostSetup.ProcessorArchitecture);
            hostSetup.RuntimeVersion = PartCoverTool.NegotiateRuntimeVersion(hostSetup.RuntimeVersion);

            return hostSetup;
        }
    }
}
