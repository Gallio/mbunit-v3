// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using Castle.Core.Logging;
using Gallio.Hosting;
using Gallio.Runner.Domains;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// A factory for <see cref="HostTestDomain" /> that automatically applies
    /// all registered <see cref="IHostTestDomainContributor" />s.
    /// </summary>
    public class HostTestDomainFactory : MarshalByRefObject, ITestDomainFactory
    {
        private IHostFactory hostFactory;
        private RuntimeSetup runtimeSetup;
        private ILogger logger;
        private IEnumerable<IHostTestDomainContributor> contributors;

        /// <summary>
        /// Creates an host test domain factory using the same <see cref="RuntimeSetup" />
        /// as the current <see cref="Runtime" /> and automaticallyed registers all available
        /// <see cref="IHostTestDomainContributor" /> components.
        /// </summary>
        /// <remarks>
        /// The <see cref="Runtime" /> must be initialized prior to using this constructor.
        /// </remarks>
        /// <param name="hostFactory">The host factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Runtime" /> is not initialized</exception>
        public HostTestDomainFactory(IHostFactory hostFactory)
            : this(hostFactory, Runtime.Instance)
        {
        }

        /// <summary>
        /// Creates an host test domain factory using the same <see cref="RuntimeSetup" />
        /// as the specified <see cref="IRuntime" /> and all registered
        /// <see cref="IHostTestDomainContributor" /> components.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>
        /// or <paramref name="runtime"/> is null</exception>
        public HostTestDomainFactory(IHostFactory hostFactory, IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");

            Initialize(hostFactory, runtime.GetRuntimeSetup(),
                runtime.Resolve<ILogger>(), runtime.ResolveAll<IHostTestDomainContributor>());
        }

        /// <summary>
        /// Creates an host test domain factory that will create isolated
        /// runtimes using the provided runtime setup.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <param name="logger">The logger</param>
        /// <param name="contributors">The collection of domain contributors</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="runtimeSetup"/>, <paramref name="logger"/> or <paramref name="contributors"/> is null</exception>
        public HostTestDomainFactory(IHostFactory hostFactory, RuntimeSetup runtimeSetup, ILogger logger, IEnumerable<IHostTestDomainContributor> contributors)
        {
            Initialize(hostFactory, runtimeSetup, logger, contributors);
        }

        /// <inheritdoc />
        public ITestDomain CreateDomain()
        {
            HostTestDomain domain = new HostTestDomain(hostFactory, runtimeSetup, logger);

            foreach (IHostTestDomainContributor contributor in contributors)
                domain.AddContributor(contributor);

            return domain;
        }

        private void Initialize(IHostFactory hostFactory, RuntimeSetup runtimeSetup, ILogger logger, IEnumerable<IHostTestDomainContributor> contributors)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (runtimeSetup == null)
                throw new ArgumentNullException(@"runtimeSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (contributors == null)
                throw new ArgumentNullException(@"contributors");

            this.hostFactory = hostFactory;
            this.runtimeSetup = runtimeSetup;
            this.logger = logger;
            this.contributors = contributors;
        }
    }
}