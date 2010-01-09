// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Gallio.Model;
using Gallio.Model.Isolation;
using Gallio.Runner.Extensions;

namespace Gallio.Runner
{
    /// <summary>
    /// A factory for <see cref="DefaultTestRunner" />.
    /// </summary>
    public class DefaultTestRunnerFactory : ITestRunnerFactory
    {
        private readonly ITestIsolationProvider testIsolationProvider;
        private readonly ITestFrameworkManager testFrameworkManager;
        private readonly ITestRunnerExtensionManager testRunnerExtensionManager;

        /// <summary>
        /// Creates a test runner factory.
        /// </summary>
        /// <param name="testIsolationProvider">The test isolation provider.</param>
        /// <param name="testFrameworkManager">The test framework manager.</param>
        /// <param name="testRunnerExtensionManager">The extension manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationProvider"/>
        /// or <paramref name="testRunnerExtensionManager" /> is null.</exception>
        public DefaultTestRunnerFactory(ITestIsolationProvider testIsolationProvider,
            ITestFrameworkManager testFrameworkManager,
            ITestRunnerExtensionManager testRunnerExtensionManager)
        {
            if (testIsolationProvider == null)
                throw new ArgumentNullException("testIsolationProvider");
            if (testFrameworkManager == null)
                throw new ArgumentNullException("testFrameworkManager");
            if (testRunnerExtensionManager == null)
                throw new ArgumentNullException("testRunnerExtensionManager");

            this.testIsolationProvider = testIsolationProvider;
            this.testFrameworkManager = testFrameworkManager;
            this.testRunnerExtensionManager = testRunnerExtensionManager;
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner()
        {
            var runner = new DefaultTestRunner(testIsolationProvider, testFrameworkManager);
            testRunnerExtensionManager.RegisterAutoActivatedExtensions(runner);
            return runner;
        }
    }
}
