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
using Gallio.NCoverIntegration.Tools;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Hosting;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// A factory for initialized <see cref="NCoverHost" /> hosts.
    /// </summary>
    public class NCoverHostFactory : IsolatedProcessHostFactory
    {
        private readonly NCoverTool tool;

        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <param name="tool">The NCover tool.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/>
        /// or <paramref name="tool"/> is null.</exception>
        public NCoverHostFactory(IRuntime runtime, NCoverTool tool)
            : base(runtime)
        {
            if (tool == null)
                throw new ArgumentNullException("tool");

            this.tool = tool;
        }

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
        {
            NCoverHost host = new NCoverHost(hostSetup, logger, RuntimePath, tool);
            host.Connect();
            return host;
        }
    }
}
