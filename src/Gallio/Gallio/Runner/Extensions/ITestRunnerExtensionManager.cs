using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Manages the set of available test runner extensions.
    /// </summary>
    public interface ITestRunnerExtensionManager
    {
        /// <summary>
        /// Registers auto-activated extensions with the test runner.
        /// </summary>
        /// <param name="testRunner">The test runner</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testRunner"/> is null</exception>
        void RegisterAutoActivatedExtensions(ITestRunner testRunner);
    }
}
