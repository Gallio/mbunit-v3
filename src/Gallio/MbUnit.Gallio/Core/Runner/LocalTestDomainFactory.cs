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
using Castle.Core;
using MbUnit.Core.Harness;
using MbUnit.Core.RuntimeSupport;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A factory for <see cref="LocalTestDomain" />.
    /// </summary>
    [Singleton]
    public class LocalTestDomainFactory : ITestDomainFactory
    {
        private readonly IRuntime runtime;
        private readonly ITestHarnessFactory harnessFactory;

        /// <summary>
        /// Creates an local test domain factory.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <param name="harnessFactory">The harness factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public LocalTestDomainFactory(IRuntime runtime, ITestHarnessFactory harnessFactory)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");
            if (harnessFactory == null)
                throw new ArgumentNullException("harnessFactory");

            this.runtime = runtime;
            this.harnessFactory = harnessFactory;
        }

        /// <inheritdoc />
        public ITestDomain CreateDomain()
        {
            LocalTestDomain domain = new LocalTestDomain(runtime, harnessFactory);
            return domain;
        }
    }
}