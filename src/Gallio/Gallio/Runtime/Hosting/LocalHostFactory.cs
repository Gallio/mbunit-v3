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
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// <para>
    /// A factory for initialized <see cref="LocalHost" /> hosts.
    /// </para>
    /// </summary>
    public class LocalHostFactory : BaseHostFactory
    {
        private readonly IDebuggerManager debuggerManager;

        /// <summary>
        /// Gets the component Id of this factory.
        /// </summary>
        public static readonly string ComponentId = "Gallio.LocalHostFactory";

        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="debuggerManager">A reference to the debugger manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="debuggerManager"/> is null</exception>
        public LocalHostFactory(IDebuggerManager debuggerManager)
        {
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");

            this.debuggerManager = debuggerManager;
        }

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
        {
            LocalHost host = new LocalHost(hostSetup, logger, debuggerManager);
            host.Connect();
            return host;
        }
    }
}
