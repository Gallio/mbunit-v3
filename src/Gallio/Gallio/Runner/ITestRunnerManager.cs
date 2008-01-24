using System;
using System.Collections.Specialized;
using Gallio.Hosting;

namespace Gallio.Runner
{
    /// <summary>
    /// A test runner manager enumerates the names of the
    /// <see cref="ITestRunnerFactory" /> services that are available and
    /// provides a mechanism for creating <see cref="ITestRunner" /> instances.
    /// </summary>
    public interface ITestRunnerManager
    {
        /// <summary>
        /// Gets a resolver for resolving registered
        /// <see cref="ITestRunnerFactory" /> components by name.
        /// </summary>
        IRegisteredComponentResolver<ITestRunnerFactory> FactoryResolver { get; }

        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="factoryName">The name of the test runner factory, matched case-insensitively</param>
        /// <param name="runnerOptions">The test runner options</param>
        /// <returns>The test runner</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryName"/> or <paramref name="runnerOptions"/>
        /// is null</exception>
        ITestRunner CreateTestRunner(string factoryName, NameValueCollection runnerOptions);
    }
}
