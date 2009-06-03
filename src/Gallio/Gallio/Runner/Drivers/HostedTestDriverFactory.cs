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
        /// <param name="hostFactory">The host factory.</param>
        /// <param name="frameworkManager">The test framework manager.</param>
        /// <param name="runtime">The Gallio runtime.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworkManager"/>, or <paramref name="runtime"/> is null.</exception>
        public HostedTestDriverFactory(IHostFactory hostFactory, ITestFrameworkManager frameworkManager, IRuntime runtime)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworkManager == null)
                throw new ArgumentNullException( "frameworkManager" );
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.hostFactory = hostFactory;
            this.frameworkManager = frameworkManager;
            this.runtime = runtime;

            IsolationMode = IsolationMode.AppDomainPerAssembly;
        }

        /// <summary>
        /// Specifies the isolation mode to use.
        /// </summary>
        /// <value>The isolation mode.  Defaults to <see cref="Drivers.IsolationMode.AppDomainPerAssembly" />.</value>
        public IsolationMode IsolationMode { get; set; }

        /// <summary>
        /// The host factory
        /// </summary>
        protected IHostFactory HostFactory
        {
            get { return hostFactory; }
        }

        /// <summary>
        /// The test framework manager
        /// </summary>
        protected ITestFrameworkManager FrameworkManager
        {
            get { return frameworkManager; }
        }

        /// <summary>
        /// The Gallio runtime
        /// </summary>
        protected IRuntime Runtime
        {
            get { return runtime; }
        }

        /// <inheritdoc />
        public ITestDriver CreateTestDriver()
        {
            return new HostedTestDriver(hostFactory, frameworkManager, runtime, IsolationMode);
        }
    }
}
