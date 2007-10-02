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
using MbUnit.Core.RuntimeSupport;
using MbUnit.Framework.Kernel.RuntimeSupport;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Runs tests and reports progress by writing log messages.
    /// </summary>
    /// <remarks>
    /// The implementation is provided as a convenience to test runner integrators
    /// since it further encapsulates the lifecycle of the <see cref="IRuntime" />
    /// including configuration of the <see cref="IAssemblyResolverManager" /> and
    /// plugin loading.
    /// </remarks>
    public class AutoRunner : BaseRunner
    {
        /// <summary>
        /// Creates an instance of the auto-runner.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="domainFactory">The domain factory</param>
        protected AutoRunner(ICoreRuntime runtime, ITestDomainFactory domainFactory)
            : base(runtime, domainFactory)
        {
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            if (Runtime != null)
            {
                Runtime.Dispose();
                Runtime = null;
            }
        }

        /// <summary>
        /// Creates a runner using the default <see cref="ITestDomainFactory" /> over
        /// a newly created instance of <see cref="WindsorRuntime" />.  This is
        /// a convenience for applications that are not also using the runtime
        /// to manage their services.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup parameters</param>
        public static AutoRunner CreateRunner(RuntimeSetup runtimeSetup)
        {
            DefaultAssemblyResolverManager assemblyResolverManager = new DefaultAssemblyResolverManager();
            assemblyResolverManager.AddMbUnitDirectories();

            WindsorRuntime runtime = new WindsorRuntime(assemblyResolverManager, runtimeSetup);
            runtime.Initialize();

            ITestDomainFactory domainFactory = runtime.Resolve<ITestDomainFactory>();
            return new AutoRunner(runtime, domainFactory);
        }
    }
}
