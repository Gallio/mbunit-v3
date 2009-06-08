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
    /// <para>
    /// The <see cref="RuntimeAccessor" /> must be initialized prior to using this factory
    /// because the tests will run within the current <see cref="AppDomain" /> and
    /// <see cref="RuntimeAccessor"/>.
    /// </para>
    /// </remarks>
    public class DefaultTestHarnessFactory : ITestHarnessFactory
    {
        private readonly ITestContextTracker contextTracker;
        private readonly ITestEnvironment[] environments;
        private readonly ILoader loader;
        private readonly ITestFrameworkManager frameworkManager;

        /// <summary>
        /// Creates a default test harness factory.
        /// </summary>
        /// <param name="contextTracker">The test context tracker.</param>
        /// <param name="loader">The loader.</param>
        /// <param name="environments">The collection of test environments.</param>
        /// <param name="frameworkManager">The framework manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>,
        /// <paramref name="loader"/>, <paramref name="frameworks" /> or <paramref name="environments"/> is null.</exception>
        public DefaultTestHarnessFactory(ITestContextTracker contextTracker, ILoader loader,
            ITestEnvironment[] environments, ITestFrameworkManager frameworkManager)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (environments == null)
                throw new ArgumentNullException("environments");
            if (frameworkManager == null)
                throw new ArgumentNullException("frameworkManager");

            this.contextTracker = contextTracker;
            this.loader = loader;
            this.environments = environments;
            this.frameworkManager = frameworkManager;
        }

        /// <inheritdoc />
        public ITestHarness CreateHarness()
        {
            var harness = new DefaultTestHarness(contextTracker, loader, frameworkManager);

            foreach (ITestEnvironment environment in environments)
                harness.AddTestEnvironment(environment);

            return harness;
        }
    }
}
