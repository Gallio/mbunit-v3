using System;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// <para>
    /// Creates hosted test driver instances.
    /// </para>
    /// </summary>
    public class HostedTestDriverFactory : ITestDriverFactory
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFramework[] frameworks;
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates a hosted test driver factory.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworks">The test frameworks that should participate in test domain configuration</param>
        /// <param name="runtime">The Gallio runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworks"/>, or <paramref name="runtime"/> is null</exception>
        public HostedTestDriverFactory(IHostFactory hostFactory, ITestFramework[] frameworks, IRuntime runtime)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.hostFactory = hostFactory;
            this.frameworks = frameworks;
            this.runtime = runtime;
        }

        /// <inheritdoc />
        public ITestDriver CreateTestDriver()
        {
            return new HostedTestDriver(hostFactory, frameworks, runtime);
        }
    }
}
