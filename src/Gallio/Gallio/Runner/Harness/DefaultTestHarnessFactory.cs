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
using Gallio.Runtime.Loader;
using Gallio.Runtime;
using Gallio.Model.Execution;
using Gallio.Model;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// The default test harness factory creates a <see cref="DefaultTestHarness" />.
    /// </summary>
    /// <remarks>
    /// The <see cref="RuntimeAccessor" /> must be initialized prior to using this factory
    /// because the tests will run within the current <see cref="AppDomain" /> and
    /// <see cref="RuntimeAccessor"/>.
    /// </remarks>
    public class DefaultTestHarnessFactory : ITestHarnessFactory
    {
        private readonly ITestContextTracker contextTracker;
        private readonly ITestFramework[] frameworks;
        private readonly ITestEnvironment[] environments;
        private readonly ILoader loader;

        /// <summary>
        /// Creates a default test harness factory with the specified <see cref="ITestContextTracker" />,
        /// <see cref="ITestFramework" /> and <see cref="ITestEnvironment" /> components.
        /// </summary>
        /// <param name="contextTracker">The test context tracker</param>
        /// <param name="loader">The loader</param>
        /// <param name="frameworks">The collection of test frameworks</param>
        /// <param name="environments">The collection of test environments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>,
        /// <paramref name="loader"/>, <paramref name="frameworks" /> or <paramref name="environments"/> is null</exception>
        public DefaultTestHarnessFactory(ITestContextTracker contextTracker, ILoader loader,
            ITestFramework[] frameworks, ITestEnvironment[] environments)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (environments == null)
                throw new ArgumentNullException("environments");

            this.contextTracker = contextTracker;
            this.loader = loader;
            this.frameworks = frameworks;
            this.environments = environments;
        }

        /// <inheritdoc />
        public ITestHarness CreateHarness()
        {
            DefaultTestHarness harness = new DefaultTestHarness(contextTracker, loader);

            foreach (ITestFramework framework in frameworks)
                harness.AddTestFramework(framework);

            foreach (ITestEnvironment environment in environments)
                harness.AddTestEnvironment(environment);

            return harness;
        }
    }
}
