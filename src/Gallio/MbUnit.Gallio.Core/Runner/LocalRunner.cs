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
using MbUnit.Core.Harness;
using MbUnit.Core.RuntimeSupport;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A local runner runs tests locally within the current AppDomain using
    /// a <see cref="LocalTestDomain" />.
    /// </summary>
    public class LocalRunner : BaseRunner
    {
        /// <summary>
        /// Creates a local runner using the current runtime stored in <see cref="Framework.Runtime.Instance" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="Framework.Runtime.Instance" /> is null</exception>
        public LocalRunner()
            : this(GetRuntime())
        {
        }

        /// <summary>
        /// Creates a local runner using the specified runtime.
        /// </summary>
        /// <param name="runtime">The runtime to use</param>
        public LocalRunner(ICoreRuntime runtime)
            : base(runtime, new LocalTestDomainFactory(runtime, runtime.Resolve<ITestHarnessFactory>()))
        {
        }

        private static ICoreRuntime GetRuntime()
        {
            ICoreRuntime runtime = CoreRuntimeHolder.Instance;
            if (runtime == null)
                throw new InvalidOperationException("The framework's runtime holder has not been initialized.");

            return runtime;
        }
    }
}
