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
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// <para>
    /// Creates hosted test driver instances.
    /// </para>
    /// </summary>
    public class HostedTestDriverFactory : ITestDriverFactory
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFrameworkManager frameworkManager;
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates a hosted test driver factory.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworkManager">The test framework manager</param>
        /// <param name="runtime">The Gallio runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworkManager"/>, or <paramref name="runtime"/> is null</exception>
        public HostedTestDriverFactory(IHostFactory hostFactory, ITestFrameworkManager frameworkManager, IRuntime runtime)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworkManager == null)
                throw new ArgumentNullException("frameworks");
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.hostFactory = hostFactory;
            this.frameworkManager = frameworkManager;
            this.runtime = runtime;
        }

        /// <summary>
        /// Specifies whether to share the app-domain among all test domains.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If false, creates a separate app-domain for each test domain.  Otherwise
        /// shares the host's own app-domain across all test domains.
        /// </para>
        /// <para>
        /// When this mode is enabled, any features that depend on test assembly
        /// configuration will not work.
        /// </para>
        /// </remarks>
        public bool ShareAppDomain { get; set; }

        /// <inheritdoc />
        public ITestDriver CreateTestDriver()
        {
            return new HostedTestDriver(hostFactory, frameworkManager, runtime, ShareAppDomain);
        }
    }
}
