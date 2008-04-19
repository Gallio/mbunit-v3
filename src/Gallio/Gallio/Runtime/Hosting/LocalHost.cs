// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Runtime.Remoting;
using Gallio.Runtime.Logging;
using Gallio.Utilities;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IHost" /> that runs code
    /// locally within the current AppDomain.
    /// </para>
    /// </summary>
    public class LocalHost : BaseHost
    {
        private bool wasRuntimeInitializedByThisHost;
        private CurrentDirectorySwitcher currentDirectorySwitcher;

        /// <summary>
        /// Creates a local host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>
        /// or <paramref name="logger"/> is null</exception>
        public LocalHost(HostSetup hostSetup, ILogger logger)
            : base(hostSetup, logger)
        {
            if (hostSetup.WorkingDirectory.Length != 0)
                currentDirectorySwitcher = new CurrentDirectorySwitcher(hostSetup.WorkingDirectory);
        }

        /// <inheritdoc />
        public override bool IsLocal
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (currentDirectorySwitcher != null)
            {
                currentDirectorySwitcher.Dispose();
                currentDirectorySwitcher = null;
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void DoCallbackImpl(CrossAppDomainDelegate callback)
        {
            callback();
        }

        /// <inheritdoc />
        protected override void PingImpl()
        {
            // Nothing to do.
        }

        /// <inheritdoc />
        protected override ObjectHandle CreateInstanceImpl(string assemblyName, string typeName)
        {
            return Activator.CreateInstance(assemblyName, typeName);
        }

        /// <inheritdoc />
        protected override ObjectHandle CreateInstanceFromImpl(string assemblyPath, string typeName)
        {
            return Activator.CreateInstanceFrom(assemblyPath, typeName);
        }

        /// <inheritdoc />
        protected override void InitializeRuntimeImpl(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger)
        {
            if (!wasRuntimeInitializedByThisHost && !RuntimeAccessor.IsInitialized)
            {
                RuntimeBootstrap.Initialize(runtimeFactory, runtimeSetup, logger);
                wasRuntimeInitializedByThisHost = true;
            }
        }

        /// <inheritdoc />
        protected override void ShutdownRuntimeImpl()
        {
            if (wasRuntimeInitializedByThisHost)
            {
                wasRuntimeInitializedByThisHost = false;
                RuntimeBootstrap.Shutdown();
            }
        }
    }
}
