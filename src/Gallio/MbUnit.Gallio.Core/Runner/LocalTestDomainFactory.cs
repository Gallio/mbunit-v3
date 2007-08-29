using System;
using System.Collections.Generic;
using Castle.Core;
using MbUnit.Core.Harness;
using MbUnit.Core.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A factory for <see cref="LocalTestDomain" />.
    /// </summary>
    [Singleton]
    public class LocalTestDomainFactory : ITestDomainFactory
    {
        private readonly ICoreRuntime runtime;
        private readonly ITestHarnessFactory harnessFactory;

        /// <summary>
        /// Creates an local test domain factory.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="harnessFactory">The harness factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public LocalTestDomainFactory(ICoreRuntime runtime, ITestHarnessFactory harnessFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (harnessFactory == null)
                throw new ArgumentNullException("harnessFactory");

            this.runtime = runtime;
            this.harnessFactory = harnessFactory;
        }

        /// <inheritdoc />
        public ITestDomain CreateDomain()
        {
            LocalTestDomain domain = new LocalTestDomain(runtime, harnessFactory);
            return domain;
        }
    }
}