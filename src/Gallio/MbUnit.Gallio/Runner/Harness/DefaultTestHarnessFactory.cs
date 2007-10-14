// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using Castle.Core;
using MbUnit.Model.Execution;
using MbUnit.Model;
using MbUnit.Hosting;

namespace MbUnit.Runner.Harness
{
    /// <summary>
    /// The default test harness factory creates a <see cref="DefaultTestHarness" />
    /// and 
    /// </summary>
    [Singleton]
    public class DefaultTestHarnessFactory : ITestHarnessFactory
    {
        private readonly IRuntime runtime;
        private readonly ITestPlanFactory testPlanFactory;

        /// <summary>
        /// Creates a default test harness factory that adds all registered
        /// <see cref="ITestFramework" /> and <see cref="ITestEnvironment" />
        /// services to the test harness when created.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="testPlanFactory">The test plan factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> or
        /// <paramref name="testPlanFactory"/> is null</exception>
        public DefaultTestHarnessFactory(IRuntime runtime, ITestPlanFactory testPlanFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException(@"runtime");
            if (testPlanFactory == null)
                throw new ArgumentNullException(@"testPlanFactory");

            this.runtime = runtime;
            this.testPlanFactory = testPlanFactory;
        }

        /// <inheritdoc />
        public ITestHarness CreateHarness()
        {
            DefaultTestHarness harness = new DefaultTestHarness(testPlanFactory);

            foreach (ITestEnvironment environment in runtime.ResolveAll<ITestEnvironment>())
                harness.AddTestEnvironment(environment);

            foreach (ITestFramework framework in runtime.ResolveAll<ITestFramework>())
                harness.AddTestFramework(framework);

            return harness;
        }
    }
}
