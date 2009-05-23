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
using System.IO;
using System.Reflection;
using Gallio.Common.Platform;
using Gallio.Runtime.Logging;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Hosting;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// An NCover host is a variation on a <see cref="IsolatedProcessHost" />
    /// launches the process with the NCover profiler attached.
    /// </summary>
    public class NCoverHost : IsolatedProcessHost
    {
        private readonly NCoverVersion version;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <param name="installationPath">The runtime installation path where the hosting executable will be found</param>
        /// <param name="version">The NCover version</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// <paramref name="logger"/>, or <paramref name="installationPath"/> is null</exception>
        public NCoverHost(HostSetup hostSetup, ILogger logger, string installationPath, NCoverVersion version)
            : base(ForceProcessorArchitectureAndRuntimeVersionIfRequired(hostSetup, version), logger, installationPath)
        {
            this.version = version;
        }

        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            string ncoverArguments = HostSetup.Properties.GetValue("NCoverArguments");
            string ncoverCoverageFile = HostSetup.Properties.GetValue("NCoverCoverageFile");

            if (ncoverArguments == null)
                ncoverArguments = string.Empty;
            if (string.IsNullOrEmpty(ncoverCoverageFile))
                ncoverCoverageFile = "Coverage.xml";

            ncoverCoverageFile = Path.Combine(workingDirectory, ncoverCoverageFile);
            return NCoverTool.CreateProcessTask(executablePath, arguments, workingDirectory, version, Logger, ncoverArguments, ncoverCoverageFile);
        }

        private static HostSetup ForceProcessorArchitectureAndRuntimeVersionIfRequired(HostSetup hostSetup, NCoverVersion version)
        {
            hostSetup = hostSetup.Copy();

            // NCover v1 only supports x86
            if (version == NCoverVersion.V1)
            {
                ProcessorArchitecture currentArch = hostSetup.ProcessorArchitecture;
                if (currentArch == ProcessorArchitecture.Amd64 || currentArch == ProcessorArchitecture.IA64)
                    throw new HostException("NCover v1.5.8 must run code as a 32bit process but the requested architecture was 64bit.");

                hostSetup.ProcessorArchitecture = ProcessorArchitecture.X86;
            }

            // All NCover versions currently support .Net 2.0 only.
            if (hostSetup.RuntimeVersion == null)
            {
                hostSetup.RuntimeVersion = DotNetRuntimeSupport.InstalledDotNet20RuntimeVersion;
            }
            else
            {
                if (!hostSetup.RuntimeVersion.Contains("2.0."))
                    throw new HostException(string.Format("NCover does not support .Net runtime {0} at this time.", hostSetup.RuntimeVersion));
            }

            return hostSetup;
        }
    }
}
