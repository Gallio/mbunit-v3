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
using Gallio.Common;
using Gallio.Common.Concurrency;
using Gallio.Runtime;

namespace Gallio.Model.Environments
{
    /// <summary>
    /// A thread task that uses the <see cref="ITestEnvironmentManager" /> to configure
    /// the test environment for the thread.
    /// </summary>
    public class TestEnvironmentAwareThreadTask : ThreadTask
    {
        private readonly ITestEnvironmentManager environmentManager;
        private IDisposable environmentState;

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread
        /// test thread and a configured test environemnt.
        /// When the task terminates successfully, its result will contain the value
        /// returned by <paramref name="func"/>.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="func">The function to perform within the thread.</param>
        /// <param name="environmentManager">The test environment manager, or null to use the default environment manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="func"/> is null.</exception>
        public TestEnvironmentAwareThreadTask(string name, Func<object> func, ITestEnvironmentManager environmentManager)
            : base(name, func)
        {
            this.environmentManager = ResolveTestEnvironmentManager(environmentManager);
        }

        /// <summary>
        /// Creates a task that will execute code within a new locally running thread
        /// test thread and a configured test environemnt.
        /// When the task terminates successfully, its result will contain the value <c>null</c>.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="action">The action to perform within the thread.</param>
        /// <param name="environmentManager">The test environment manager, or null to use the default environment manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> 
        /// or <paramref name="action"/> is null.</exception>
        public TestEnvironmentAwareThreadTask(string name, Action action, ITestEnvironmentManager environmentManager)
            : base(name, action)
        {
            this.environmentManager = ResolveTestEnvironmentManager(environmentManager);
        }

        private static ITestEnvironmentManager ResolveTestEnvironmentManager(ITestEnvironmentManager environmentManager)
        {
            return environmentManager ?? RuntimeAccessor.ServiceLocator.Resolve<ITestEnvironmentManager>();
        }

        /// <inheritdoc />
        protected override void BeforeTask()
        {
            environmentState = environmentManager.SetUpThread();
        }

        /// <inheritdoc />
        protected override void AfterTask()
        {
            if (environmentState != null)
                environmentState.Dispose();
        }
    }
}
