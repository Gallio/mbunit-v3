// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Hosting;
using TypeMock.Integration;

namespace Gallio.TypeMockIntegration
{
    /// <summary>
    /// <para>
    /// A factory for initialized <see cref="TypeMockHost" /> hosts.
    /// </para>
    /// </summary>
    public class TypeMockHostFactory : IsolatedProcessHostFactory
    {
        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public TypeMockHostFactory(IRuntime runtime)
            : base(runtime)
        {
        }

        /// <summary>
        /// Creates a host factory.
        /// </summary>
        /// <param name="installationPath">The installation path of the host executable</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="installationPath"/> is null</exception>
        public TypeMockHostFactory(string installationPath)
            : base(installationPath)
        {
        }

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
        {
            if (! Service.IsInstalled)
                throw new IntegrationNotInstalledException("TypeMock does not appear to be installed.");

            TypeMockHost host = new TypeMockHost(hostSetup, logger, InstallationPath);
            host.Connect();
            return host;
        }
    }
}
