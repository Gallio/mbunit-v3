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
using Castle.Core;
using Gallio.Hosting;
using Gallio.Runner.Domains;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// A factory for <see cref="IsolatedTestDomain" /> that automatically applies
    /// all registered <see cref="IIsolatedTestDomainContributor" />s.
    /// </summary>
    [Singleton]
    public class IsolatedTestDomainFactory : ITestDomainFactory
    {
        private readonly RuntimeSetup runtimeSetup;
        private readonly IEnumerable<IIsolatedTestDomainContributor> contributors;

        /// <summary>
        /// Creates an isolated test domain factory using the same <see cref="RuntimeSetup" />
        /// as the current <see cref="Runtime" /> and automaticallyed registers all available
        /// <see cref="IIsolatedTestDomainContributor" /> components.
        /// </summary>
        /// <remarks>
        /// The <see cref="Runtime" /> must be initialized prior to using this constructor.
        /// </remarks>
        public IsolatedTestDomainFactory()
            : this(Runtime.Instance)
        {
        }

        /// <summary>
        /// Creates an isolated test domain factory using the same <see cref="RuntimeSetup" />
        /// as the specified <see cref="IRuntime" /> and all registered
        /// <see cref="IIsolatedTestDomainContributor" /> components.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public IsolatedTestDomainFactory(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");

            runtimeSetup = runtime.GetRuntimeSetup();
            contributors = runtime.ResolveAll<IIsolatedTestDomainContributor>();
        }

        /// <summary>
        /// Creates an isolated test domain factory that will create isolated
        /// runtimes using the provided runtime setup.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <param name="contributors">The collection of domain contributors</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimeSetup"/> or
        /// <paramref name="contributors"/> is null</exception>
        public IsolatedTestDomainFactory(RuntimeSetup runtimeSetup, IEnumerable<IIsolatedTestDomainContributor> contributors)
        {
            if (runtimeSetup == null)
                throw new ArgumentNullException(@"runtimeSetup");
            if (contributors == null)
                throw new ArgumentNullException(@"contributors");

            this.runtimeSetup = runtimeSetup;
            this.contributors = contributors;
        }

        /// <inheritdoc />
        public ITestDomain CreateDomain()
        {
            IsolatedTestDomain domain = new IsolatedTestDomain(runtimeSetup);

            foreach (IIsolatedTestDomainContributor contributor in contributors)
                domain.AddContributor(contributor);

            return domain;
        }
    }
}