// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Model;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// <para>
    /// A host test domain runs tests within a <see cref="IHost" /> that is configured
    /// as needed to provide a suitable runtime environment for each test package.
    /// The host is recreated each time a new test package is loaded.
    /// </para>
    /// <para>
    /// The behavior of the test domain may be augmented by registering <see cref="IHostTestDomainContributor" />
    /// instances to fine-tune how the host is configured as needed.
    /// </para>
    /// </summary>
    public class HostTestDomain : RemoteTestDomain
    {
        private readonly IHostFactory hostFactory;
        private readonly RuntimeFactory runtimeFactory;
        private readonly RuntimeSetup runtimeSetup;
        private readonly ILogger logger;

        private readonly List<IHostTestDomainContributor> contributors;

        private IHost host;

        /// <summary>
        /// Creates a host test domain.
        /// </summary>
        /// <param name="hostFactory">The host factory to use</param>
        /// <param name="runtimeFactory">The runtime factory for the runtime to initialize within the host</param>
        /// <param name="runtimeSetup">The runtime setup for the runtime to initialize within the host</param>
        /// <param name="logger">The logger to use within the host</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>, <paramref name="runtimeFactory"/>,
        /// <paramref name="runtimeSetup"/>, or <paramref name="logger"/> is null</exception>
        public HostTestDomain(IHostFactory hostFactory, RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (runtimeFactory == null)
                throw new ArgumentNullException("runtimeFactory");
            if (runtimeSetup == null)
                throw new ArgumentNullException("runtimeSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.hostFactory = hostFactory;
            this.runtimeFactory = runtimeFactory;
            this.runtimeSetup = runtimeSetup;
            this.logger = logger;

            contributors = new List<IHostTestDomainContributor>();
        }

        /// <summary>
        /// Adds a contributor to the test domain.
        /// </summary>
        /// <param name="contributor">The contributor</param>
        public void AddContributor(IHostTestDomainContributor contributor)
        {
            contributors.Add(contributor);
        }

        /// <inheritdoc />
        protected override ITestDomain InternalConnect(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Initializing the host environment.");
            CreateHost(packageConfig);
            progressMonitor.Worked(0.7);

            progressMonitor.SetStatus("Initializing the test runner.");
            ITestDomain testDomain = InitializeTestRunner();
            progressMonitor.Worked(0.3);
            return testDomain;
        }

        /// <inheritdoc />
        protected override void InternalDisconnect(IProgressMonitor progressMonitor)
        {
            try
            {
                progressMonitor.SetStatus("Shutting down the test runner.");
                ShutdownTestRunner();
                progressMonitor.Worked(0.7);
            }
            finally
            {
                progressMonitor.SetStatus("Shutting down the host environment.");
                DisposeHost();
                progressMonitor.Worked(0.7);
            }
        }

        private void CreateHost(TestPackageConfig packageConfig)
        {
            HostSetup hostSetup = CreateHostSetup(packageConfig);
            host = hostFactory.CreateHost(hostSetup, logger);
        }

        private HostSetup CreateHostSetup(TestPackageConfig packageConfig)
        {
            HostSetup hostSetup = packageConfig.HostSetup.Copy();

            hostSetup.Configuration.AddAssemblyBinding(runtimeFactory.GetType().Assembly, false);

            foreach (IHostTestDomainContributor contributor in contributors)
                contributor.ConfigureHost(hostSetup, packageConfig);

            return hostSetup;
        }

        private void DisposeHost()
        {
            try
            {
                if (host != null)
                    host.Dispose();
            }
            finally
            {
                host = null;
            }
        }

        private ITestDomain InitializeTestRunner()
        {
            try
            {
                host.InitializeRuntime(runtimeFactory, runtimeSetup, logger);

                Type factoryType = typeof(LocalTestDomainFactory);
                ITestDomainFactory factory = (ITestDomainFactory)
                    host.CreateInstance(factoryType.Assembly.FullName, factoryType.FullName).Unwrap();
                return factory.CreateDomain();
            }
            catch (Exception ex)
            {
                throw new RunnerException("Could not create the hosted test domain.", ex);
            }
        }

        private void ShutdownTestRunner()
        {
            if (host != null)
                host.ShutdownRuntime();
        }
    }
}
