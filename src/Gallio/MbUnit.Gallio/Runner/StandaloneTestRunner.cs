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

using MbUnit.Hosting;
using MbUnit.Runner.Domains;

namespace MbUnit.Runner
{
    /// <summary>
    /// A standalone test runner initializes the runtime and runs tests in a domain
    /// created by the default registered <see cref="ITestDomainFactory" />.
    /// </summary>
    /// <remarks>
    /// The implementation is provided as a convenience to test runner integrators
    /// since it encapsulates the lifecycle of the <see cref="Runtime" />
    /// This makes it particularly convenient for applications that only use the
    /// runtime services for hosting tests.
    /// </remarks>
    public sealed class StandaloneTestRunner : BaseTestRunner
    {
        /// <summary>
        /// Creates a runner using the default <see cref="ITestDomainFactory" /> over
        /// a freshly initialized <see cref="Runtime" />.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup parameters</param>
        public StandaloneTestRunner(RuntimeSetup runtimeSetup)
            : base(InitializeRuntimeAndGetDomainFactory(runtimeSetup))
        {
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            try
            {
                base.Dispose();
            }
            finally
            {
                Runtime.Shutdown();
            }
        }

        private static ITestDomainFactory InitializeRuntimeAndGetDomainFactory(RuntimeSetup runtimeSetup)
        {
            Runtime.Initialize(runtimeSetup);

            try
            {
                return Runtime.Instance.Resolve<ITestDomainFactory>();
            }
            catch
            {
                Runtime.Shutdown();
                throw;
            }
        }
    }
}
