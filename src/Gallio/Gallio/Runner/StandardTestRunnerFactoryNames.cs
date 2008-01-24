using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner
{
    /// <summary>
    /// Provides constant names for the standard test runner factories.
    /// </summary>
    public static class StandardTestRunnerFactoryNames
    {
        /// <summary>
        /// Runs tests in the local AppDomain.
        /// </summary>
        public const string LocalAppDomain = "LocalAppDomain";

        /// <summary>
        /// Runs tests in an isolated AppDomain of the current process.
        /// </summary>
        public const string IsolatedAppDomain = "IsolatedAppDomain";

        /// <summary>
        /// Runs tests in an isolated process.
        /// </summary>
        public const string IsolatedProcess = "IsolatedProcess";
    }
}
