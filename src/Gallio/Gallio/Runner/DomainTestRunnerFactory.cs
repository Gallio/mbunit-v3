using System;
using System.Collections.Specialized;
using Gallio.Runner.Domains;

namespace Gallio.Runner
{
    /// <summary>
    /// A <see cref="ITestRunnerFactory" /> for <see cref="DomainTestRunner" /> using
    /// different implementations of <see cref="ITestDomainFactory" />.
    /// </summary>
    public class DomainTestRunnerFactory : ITestRunnerFactory
    {
        private readonly ITestDomainFactory domainFactory;
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates a test runner factory.
        /// </summary>
        /// <param name="domainFactory">The test domain factory</param>
        /// <param name="name">The test runner factory name</param>
        /// <param name="description">The test runner factory description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainFactory"/>,
        /// <paramref name="name"/> or <paramref name="description"/> is null</exception>
        public DomainTestRunnerFactory(ITestDomainFactory domainFactory, string name, string description)
        {
            if (domainFactory == null)
                throw new ArgumentNullException("domainFactory");
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.domainFactory = domainFactory;
            this.name = name;
            this.description = description;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
        }

        /// <inheritdoc />
        public string Description
        {
            get { return description; }
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner(NameValueCollection runnerOptions)
        {
            return new DomainTestRunner(domainFactory);
        }
    }
}
