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
using System.IO;
using Gallio.NCoverIntegration.Tools;
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
        private readonly NCoverTool tool;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="logger">The logger for host message output.</param>
        /// <param name="installationPath">The runtime installation path where the hosting executable will be found.</param>
        /// <param name="tool">The NCover tool.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// <paramref name="logger"/>, <paramref name="installationPath"/>
        /// or <paramref name="tool"/> is null.</exception>
        public NCoverHost(HostSetup hostSetup, ILogger logger, string installationPath, NCoverTool tool)
            : base(ForceProcessorArchitectureAndRuntimeVersionIfRequired(hostSetup, tool), logger, installationPath)
        {
            if (tool == null)
                throw new ArgumentNullException("tool");

            this.tool = tool;
        }

        /// <inheritdoc />
        protected override ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            string ncoverArguments, ncoverCoverageFile;
            GetNCoverProperties(HostSetup, out ncoverArguments, out ncoverCoverageFile);

            return tool.CreateNCoverConsoleTask(executablePath, arguments, workingDirectory, ncoverArguments, ncoverCoverageFile, Logger);
        }

        internal static void GetNCoverProperties(HostSetup hostSetup,
            out string ncoverArguments, out string ncoverCoverageFile)
        {
            ncoverArguments = hostSetup.Properties.GetValue("NCoverArguments");
            ncoverCoverageFile = hostSetup.Properties.GetValue("NCoverCoverageFile");

            if (ncoverArguments == null)
                ncoverArguments = string.Empty;
            if (string.IsNullOrEmpty(ncoverCoverageFile))
                ncoverCoverageFile = "Coverage.xml";

            ncoverCoverageFile = Path.GetFullPath(ncoverCoverageFile);
        }

        internal static void SetNCoverCoverageFile(HostSetup hostSetup, string ncoverCoverageFile)
        {
            hostSetup.RemoveProperty("NCoverCoverageFile");
            hostSetup.AddProperty("NCoverCoverageFile", ncoverCoverageFile);
        }

        private static HostSetup ForceProcessorArchitectureAndRuntimeVersionIfRequired(HostSetup hostSetup, NCoverTool tool)
        {
            hostSetup = hostSetup.Copy();

            hostSetup.ProcessorArchitecture = tool.NegotiateProcessorArchitecture(hostSetup.ProcessorArchitecture);
            hostSetup.RuntimeVersion = tool.NegotiateRuntimeVersion(hostSetup.RuntimeVersion);

            return hostSetup;
        }
    }
}
