// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Runner.Drivers;
using Gallio.Runner.Extensions;

namespace Gallio.Runner
{
    /// <summary>
    /// A <see cref="ITestRunnerFactory" /> for <see cref="DefaultTestRunner" /> using
    /// different implementations of <see cref="ITestDriver" />.
    /// </summary>
    public class DefaultTestRunnerFactory : ITestRunnerFactory
    {
        private readonly ITestDriverFactory testDriverFactory;
        private readonly ITestRunnerExtensionManager extensionManager;

        /// <summary>
        /// Creates a test runner factory.
        /// </summary>
        /// <param name="testDriverFactory">The test driver factory</param>
        /// <param name="extensionManager">The extension manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriverFactory"/>
        /// or <paramref name="extensionManager" /> is null</exception>
        public DefaultTestRunnerFactory(ITestDriverFactory testDriverFactory, ITestRunnerExtensionManager extensionManager)
        {
            if (testDriverFactory == null)
                throw new ArgumentNullException("testDriverFactory");
            if (extensionManager == null)
                throw new ArgumentNullException("extensionManager");

            this.testDriverFactory = testDriverFactory;
            this.extensionManager = extensionManager;
        }

        /// <inheritdoc />
        public ITestRunner CreateTestRunner()
        {
            var runner = new DefaultTestRunner(testDriverFactory);
            extensionManager.RegisterAutoActivatedExtensions(runner);
            return runner;
        }
    }
}
