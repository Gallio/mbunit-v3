using System;
using System.Collections.Specialized;
using Gallio.Hosting;

namespace Gallio.Runner
{
    /// <summary>
    /// The default implementation of <see cref="ITestRunnerManager" />.
    /// </summary>
    public class DefaultTestRunnerManager : ITestRunnerManager
    {
        private readonly IRegisteredComponentResolver<ITestRunnerFactory> factoryResolver;

        /// <summary>
        /// Creates a test runner manager.
        /// </summary>
        /// <param name="factoryResolver">The formatter resolver</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryResolver"/> is null</exception>
        public DefaultTestRunnerManager(IRegisteredComponentResolver<ITestRunnerFactory> factoryResolver)
        {
            if (factoryResolver == null)
                throw new ArgumentNullException("factoryResolver");

            this.factoryResolver = factoryResolver;
        }

        /// <inheritdoc />
        public IRegisteredComponentResolver<ITestRunnerFactory> FactoryResolver
        {
            get { return factoryResolver; }
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner(string factoryName, NameValueCollection runnerOptions)
        {
            if (factoryName == null)
                throw new ArgumentNullException(@"factoryName");
            if (runnerOptions == null)
                throw new ArgumentNullException(@"runnerOptions");

            ITestRunnerFactory factory = FactoryResolver.Resolve(factoryName);
            if (factory == null)
                throw new InvalidOperationException(String.Format("There is no test runner factory named '{0}'.", factoryName));

            return factory.CreateTestRunner(runnerOptions);
        }
    }
}
