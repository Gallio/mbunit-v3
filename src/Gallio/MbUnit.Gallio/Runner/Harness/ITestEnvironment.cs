using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Runner.Harness
{
    /// <summary>
    /// The test environment configures global behavior related to preparing
    /// the test execution environment for running tests and tearing them down.
    /// </summary>
    public interface ITestEnvironment
    {
        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        /// <returns>Returns an object that when disposed causes the test environment to be torn down</returns>
        IDisposable SetUp();
    }
}
