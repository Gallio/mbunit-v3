using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Creates instances of <see cref="ITestRunnerExtension" />.
    /// </summary>
    [Traits(typeof(TestRunnerExtensionFactoryTraits))]
    public interface ITestRunnerExtensionFactory
    {
        /// <summary>
        /// Creates the extension.
        /// </summary>
        /// <returns>The extension to create</returns>
        ITestRunnerExtension CreateExtension();
    }
}
