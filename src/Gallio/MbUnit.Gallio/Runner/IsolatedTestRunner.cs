using System;
using MbUnit.Hosting;
using MbUnit.Runner.Domains;
using MbUnit.Runner.Harness;

namespace MbUnit.Runner
{
    /// <summary>
    /// An isolated test runner runs tests in an isolated AppDomain using
    /// an <see cref="IsolatedTestDomain" />.  The <see cref="Runtime" /> must
    /// be initialized prior to using an isolated runner.
    /// </summary>
    public sealed class IsolatedTestRunner : BaseTestRunner
    {
        /// <summary>
        /// Creates an isolated runner using the current runtime stored in <see cref="Runtime.Instance" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Runtime" /> is not initialized</exception>
        public IsolatedTestRunner()
            : base(new IsolatedTestDomainFactory(Runtime.Instance))
        {
        }
    }
}