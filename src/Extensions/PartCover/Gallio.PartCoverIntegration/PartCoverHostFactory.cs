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
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.PartCoverIntegration
{
    /// <summary>
    /// A factory for initialized <see cref="PartCoverHost" /> hosts.
    /// </summary>
    public class PartCoverHostFactory : IsolatedProcessHostFactory
    {

        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null.</exception>
        public PartCoverHostFactory(IRuntime runtime)
            : base(runtime)
        {
        }

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
        {
            PartCoverHost host = new PartCoverHost(hostSetup, logger, RuntimePath);
            host.Connect();
            return host;
        }
    }
}
