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
using MbUnit.Core.Services.Runtime;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Harness;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// The bootstrapper configures a newly created AppDomain and prepares
    /// it for test execution.
    /// </summary>
    public sealed class Bootstrapper : LongLivingMarshalByRefObject
    {
        private WindsorRuntime runtime;

        /// <summary>
        /// Initializes the AppDomain's runtime environment.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been initialized</exception>
        public void Initialize(RuntimeSetup runtimeSetup)
        {
            if (runtime != null)
                throw new InvalidOperationException("The runtime has already been initialized.");

            DefaultAssemblyResolverManager assemblyResolverManager = new DefaultAssemblyResolverManager();
            assemblyResolverManager.AddMbUnitDirectories();

            runtime = new WindsorRuntime(assemblyResolverManager, runtimeSetup);
            runtime.Initialize();
        }

        /// <summary>
        /// Creates a remotely accessible test domain within the AppDomain.
        /// </summary>
        /// <returns>The test domain</returns>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has not been initialized</exception>
        public ITestDomain CreateTestDomain()
        {
            if (runtime == null)
                throw new InvalidOperationException("The runtime has not been initialized.");

            ITestHarnessFactory harnessFactory = runtime.Resolve<ITestHarnessFactory>();
            return new LocalTestDomain(runtime, harnessFactory);
        }

        /// <summary>
        /// Shuts down the bootstrapped environment.
        /// </summary>
        public void Shutdown()
        {
            if (runtime != null)
            {
                runtime.Dispose();
                runtime = null;
            }
        }
    }
}
