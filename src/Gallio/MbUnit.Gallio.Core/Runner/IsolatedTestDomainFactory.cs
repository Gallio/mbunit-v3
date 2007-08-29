// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using Castle.Core;
using MbUnit.Core.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A factory for <see cref="IsolatedTestDomain" /> that automatically applies
    /// all registered <see cref="IIsolatedTestDomainContributor" />s.
    /// </summary>
    [Singleton]
    public class IsolatedTestDomainFactory : ITestDomainFactory
    {
        private readonly ICoreRuntime runtime;
        private readonly List<IIsolatedTestDomainContributor> contributors;

        /// <summary>
        /// Creates an isolated test domain factory that adds all registered
        /// <see cref="IIsolatedTestDomainContributor" /> services to the test domain
        /// as contributors.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public IsolatedTestDomainFactory(ICoreRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;

            contributors = new List<IIsolatedTestDomainContributor>();
            contributors.AddRange(runtime.ResolveAll<IIsolatedTestDomainContributor>());
        }

        /// <inheritdoc />
        public ITestDomain CreateDomain()
        {
            IsolatedTestDomain domain = new IsolatedTestDomain(runtime);

            foreach (IIsolatedTestDomainContributor contributor in contributors)
                domain.AddContributor(contributor);

            return domain;
        }
    }
}
