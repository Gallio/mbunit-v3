using System.Collections.Specialized;
using Gallio.Hosting;

namespace Gallio.Runner
{
    /// <summary>
    /// A test tunner factory is a service that creates an <see cref="ITestRunner" />
    /// given a set of options.  Each factory has a name which is used by
    /// <see cref="ITestRunnerManager" /> to select the particular factory to use
    /// for a given test run.
    /// </summary>
    public interface ITestRunnerFactory : IRegisteredComponent
    {
        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="runnerOptions">The test runner options, not null</param>
        /// <returns>The test runner</returns>
        ITestRunner CreateTestRunner(NameValueCollection runnerOptions);
    }
}
