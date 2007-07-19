using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A factory for <see cref="IsolatedTestDomain" /> that automatically applies
    /// all registered <see cref="IIsolatedTestDomainContributor" />s.
    /// </summary>
    public class IsolatedTestDomainFactory : ITestDomainFactory
    {
        private ICoreRuntime runtime;
        private List<IIsolatedTestDomainContributor> contributors;

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
