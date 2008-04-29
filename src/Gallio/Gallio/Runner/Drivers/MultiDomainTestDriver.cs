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
using System.IO;
using Gallio.Model;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Remoting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A test driver that configures one test domain per test assembly.
    /// </summary>
    public class MultiDomainTestDriver : AggregateTestDriver
    {
        private readonly ITestFramework[] frameworks;
        private readonly IHost primaryHost;
        private Remote remote;

        /// <summary>
        /// Creates a multi-domain test driver.
        /// </summary>
        /// <param name="frameworks">The test frameworks that should participate in test domain configuration</param>
        /// <param name="primaryHost">The primary host within which to create additional hosts for each test domain</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworks"/>,
        /// or <paramref name="primaryHost"/> is null</exception>
        public MultiDomainTestDriver(ITestFramework[] frameworks, IHost primaryHost)
        {
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (primaryHost == null)
                throw new ArgumentNullException("primaryHost");

            this.frameworks = frameworks;
            this.primaryHost = primaryHost;

            remote = HostUtils.CreateInstance<Remote>(primaryHost);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            remote = null;
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override IEnumerable<Partition> CreatePartitions(TestPackageConfig testPackageConfig)
        {
            foreach (string assemblyFile in testPackageConfig.AssemblyFiles)
            {
                TestDomainSetup testDomain = new TestDomainSetup(testPackageConfig.Copy());
                testDomain.TestPackageConfig.AssemblyFiles.Clear();
                testDomain.TestPackageConfig.AssemblyFiles.Add(assemblyFile);

                string assemblyConfigFile = assemblyFile + @".config";
                if (File.Exists(assemblyConfigFile))
                    testDomain.TestPackageConfig.HostSetup.Configuration.ConfigurationXml = File.ReadAllText(assemblyConfigFile);

                foreach (ITestFramework framework in frameworks)
                    framework.ConfigureTestDomain(testDomain);

                ILogger logger = new RemoteLogger(Logger);
                ITestDriver testDriver = remote.CreateRemoteTestDriver(testDomain, logger);
                testDriver.Initialize(RuntimeSetup, logger);

                yield return new Partition(new ProxyTestDriver(testDriver), testDomain.TestPackageConfig);
            }
        }

        private class Remote : LongLivedMarshalByRefObject
        {
            public ITestDriver CreateRemoteTestDriver(TestDomainSetup setup, ILogger logger)
            {
                IHostFactory hostFactory = new IsolatedAppDomainHostFactory();
                IHost host = hostFactory.CreateHost(setup.TestPackageConfig.HostSetup, logger);
                HostAssemblyResolverHook.Install(host);

                ITestDriver remoteTestDriver = HostUtils.CreateInstance<LocalTestDriver>(host);
                ProxyTestDriver proxyTestDriver = new ProxyTestDriver(remoteTestDriver);
                proxyTestDriver.Disposed += delegate { host.Dispose(); };

                return proxyTestDriver;
            }
        }
    }
}
