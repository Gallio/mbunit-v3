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
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// A factory for initialized <see cref="IsolatedProcessHost" /> hosts.
    /// </summary>
    public class IsolatedProcessHostFactory : BaseHostFactory
    {
        private readonly string runtimePath;

        /// <summary>
        /// Gets the component Id of this factory.
        /// </summary>
        public static readonly string ComponentId = "Gallio.IsolatedProcessHostFactory";

        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="runtime">The runtime.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null.</exception>
        public IsolatedProcessHostFactory(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            runtimePath = runtime.GetRuntimeSetup().RuntimePath;
        }

        /// <summary>
        /// Gets the path of the runtime components, in particular the folder where Gallio.Host.exe is located.
        /// </summary>
        protected string RuntimePath
        {
            get { return runtimePath; }
        }

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
        {
            IsolatedProcessHost host = new IsolatedProcessHost(hostSetup, logger, runtimePath);
            host.Connect();
            return host;
        }
    }
}
