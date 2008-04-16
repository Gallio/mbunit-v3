using System;
using Gallio.Model;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test package is being loaded.
    /// </summary>
    public sealed class LoadStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestPackageConfig testPackageConfig;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/> is null</exception>
        public LoadStartedEventArgs(TestPackageConfig testPackageConfig)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");

            this.testPackageConfig = testPackageConfig;
        }

        /// <summary>
        /// Gets the test package configuration being loaded.
        /// </summary>
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
        }
    }
}
