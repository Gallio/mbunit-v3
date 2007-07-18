using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Harness
{
    /// <summary>
    /// A test harness factory.
    /// </summary>
    public interface ITestHarnessFactory
    {
        /// <summary>
        /// Creates a test harness and associates it with its test harness
        /// contributors, if any.  Does not call <see cref="ITestHarness.Initialize" />.
        /// </summary>
        /// <returns>The test harness</returns>
        ITestHarness CreateHarness();
    }
}
