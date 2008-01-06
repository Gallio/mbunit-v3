// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using Castle.Core;
using Gallio.Model.Execution;
using Gallio.Model;
using Gallio.Hosting;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// The default test harness factory creates a <see cref="DefaultTestHarness" />.
    /// </summary>
    /// <remarks>
    /// The <see cref="Runtime" /> must be initialized prior to using this factory
    /// because the tests will run within the current <see cref="AppDomain" /> and
    /// <see cref="Runtime"/>.
    /// </remarks>
    [Singleton]
    public class DefaultTestHarnessFactory : ITestHarnessFactory
    {
        private readonly ITestPlanFactory testPlanFactory;
        private readonly IEnumerable<ITestFramework> frameworks;
        private readonly IEnumerable<ITestEnvironment> environments;

        /// <summary>
        /// Creates a test harness using the default <see cref="ITestPlanFactory" />
        /// and all <see cref="ITestFramework" /> and <see cref="ITestEnvironment" />
        /// components registered with the <see cref="Runtime" />.
        /// </summary>
        public DefaultTestHarnessFactory()
            : this(Runtime.Instance)
        {
        }

        /// <summary>
        /// Creates a test harness using the default <see cref="ITestPlanFactory" />
        /// and all <see cref="ITestFramework" /> and <see cref="ITestEnvironment" />
        /// components registered with the specified <see cref="IRuntime" />.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public DefaultTestHarnessFactory(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");

            testPlanFactory = runtime.Resolve<ITestPlanFactory>();
            frameworks = runtime.ResolveAll<ITestFramework>();
            environments = runtime.ResolveAll<ITestEnvironment>();
        }

        /// <summary>
        /// Creates a default test harness factory with the specified <see cref="ITestPlanFactory" />,
        /// <see cref="ITestFramework" /> and <see cref="ITestEnvironment" /> components.
        /// </summary>
        /// <param name="testPlanFactory">The test plan factory</param>
        /// <param name="frameworks">The collection of test frameworks</param>
        /// <param name="environments">The collection of test environments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPlanFactory"/>,
        /// <paramref name="frameworks" /> or <paramref name="environments"/> is null</exception>
        public DefaultTestHarnessFactory(ITestPlanFactory testPlanFactory,
            IEnumerable<ITestFramework> frameworks, IEnumerable<ITestEnvironment> environments)
        {
            if (testPlanFactory == null)
                throw new ArgumentNullException(@"testPlanFactory");
            if (frameworks == null)
                throw new ArgumentNullException(@"frameworks");
            if (environments == null)
                throw new ArgumentNullException(@"environments");

            this.testPlanFactory = testPlanFactory;
            this.frameworks = frameworks;
            this.environments = environments;
        }

        /// <inheritdoc />
        public ITestHarness CreateHarness()
        {
            DefaultTestHarness harness = new DefaultTestHarness(testPlanFactory);

            foreach (ITestFramework framework in frameworks)
                harness.AddTestFramework(framework);

            foreach (ITestEnvironment environment in environments)
                harness.AddTestEnvironment(environment);

            return harness;
        }
    }
}
