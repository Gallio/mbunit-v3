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
using Gallio.Common;
using Gallio.Model.Environments;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A test harness implementation based on <see cref="ITestEnvironment"/>.
    /// </summary>
    public class TestEnvironmentAwareTestHarness : TestHarness
    {
        private readonly ITestEnvironmentManager testEnvironmentManager;

        /// <summary>
        /// Creates the test harness.
        /// </summary>
        /// <param name="testEnvironmentManager">The test environment manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testEnvironmentManager"/> is null.</exception>
        public TestEnvironmentAwareTestHarness(ITestEnvironmentManager testEnvironmentManager)
        {
            if (testEnvironmentManager == null)
                throw new ArgumentNullException("testEnvironmentManager");

            this.testEnvironmentManager = testEnvironmentManager;
        }

        /// <inheritdoc />
        public override IDisposable SetUpAppDomain()
        {
            return testEnvironmentManager.SetUpAppDomain();
        }

        /// <inheritdoc />
        public override IDisposable SetUpThread()
        {
            return testEnvironmentManager.SetUpThread();
        }
    }
}