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
using MbUnit.Runner.Harness;
using MbUnit.Hosting;
using MbUnit.Utilities;

namespace MbUnit.Runner.Domains
{
    /// <summary>
    /// The bootstrapper configures a newly created AppDomain and prepares
    /// it for test execution.
    /// </summary>
    public sealed class Bootstrapper : LongLivingMarshalByRefObject
    {
        /// <summary>
        /// Initializes the AppDomain's runtime environment.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been initialized</exception>
        public void Initialize(RuntimeSetup runtimeSetup)
        {
            Runtime.Initialize(runtimeSetup);
        }

        /// <summary>
        /// Creates a remotely accessible test domain within the AppDomain.
        /// </summary>
        /// <returns>The test domain</returns>
        public ITestDomain CreateTestDomain()
        {
            ITestHarnessFactory harnessFactory = Runtime.Instance.Resolve<ITestHarnessFactory>();
            return new LocalTestDomain(harnessFactory);
        }

        /// <summary>
        /// Shuts down the bootstrapped environment.
        /// </summary>
        public void Shutdown()
        {
            Runtime.Shutdown();
        }
    }
}