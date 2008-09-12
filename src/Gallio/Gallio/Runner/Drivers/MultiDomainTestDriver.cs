// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
        private readonly string runtimePath;
        private Remote remote;

        /// <summary>
        /// Creates a multi-domain test driver.
        /// </summary>
        /// <param name="frameworks">The test frameworks that should participate in test domain configuration</param>
        /// <param name="primaryHost">The primary host within which to create additional hosts for each test domain</param>
        /// <param name="runtimePath">The Gallio runtime path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworks"/>,
        /// <paramref name="primaryHost"/>, <paramref name="runtimePath"/> is null</exception>
        public MultiDomainTestDriver(ITestFramework[] frameworks, IHost primaryHost, string runtimePath)
        {
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (primaryHost == null)
                throw new ArgumentNullException("primaryHost");
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            this.frameworks = frameworks;
            this.primaryHost = primaryHost;
            this.runtimePath = runtimePath;

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
            Dictionary<HostSetup, TestDomainSetup> testDomains = new Dictionary<HostSetup,TestDomainSetup>();
            foreach (string assemblyFile in testPackageConfig.AssemblyFiles)
            {
                TestDomainSetup testDomain = new TestDomainSetup(testPackageConfig.Copy());
                testDomain.TestPackageConfig.AssemblyFiles.Clear();
                testDomain.TestPackageConfig.AssemblyFiles.Add(assemblyFile);

                string assemblyDir = Path.GetDirectoryName(assemblyFile);
                if (testDomain.TestPackageConfig.HostSetup.ApplicationBaseDirectory == null)
                    testDomain.TestPackageConfig.HostSetup.ApplicationBaseDirectory = assemblyDir;
                if (testDomain.TestPackageConfig.HostSetup.WorkingDirectory == null)
                    testDomain.TestPackageConfig.HostSetup.WorkingDirectory = assemblyDir;

                string assemblyConfigFile = assemblyFile + @".config";
                if (File.Exists(assemblyConfigFile))
                    testDomain.TestPackageConfig.HostSetup.Configuration.ConfigurationXml = File.ReadAllText(assemblyConfigFile);

                foreach (ITestFramework framework in frameworks)
                    framework.ConfigureTestDomain(testDomain);

                TestDomainSetup existingTestDomain;
                if (testDomains.TryGetValue(testDomain.TestPackageConfig.HostSetup, out existingTestDomain))
                {
                    MergeDomain(existingTestDomain, testDomain);
                }
                else
                {
                    testDomains.Add(testDomain.TestPackageConfig.HostSetup, testDomain);
                }
            }

            foreach (TestDomainSetup testDomain in testDomains.Values)
            {
                ILogger logger = new RemoteLogger(Logger);
                ITestDriver testDriver = remote.CreateRemoteTestDriver(testDomain, logger, runtimePath);
                testDriver.Initialize(RuntimeSetup, logger);

                yield return new Partition(new ProxyTestDriver(testDriver), testDomain.TestPackageConfig);
            }
        }

        private void MergeDomain(TestDomainSetup target, TestDomainSetup source)
        {
            foreach (string assemblyFile in source.TestPackageConfig.AssemblyFiles)
                if (!target.TestPackageConfig.AssemblyFiles.Contains(assemblyFile))
                    target.TestPackageConfig.AssemblyFiles.Add(assemblyFile);

            foreach (string hintDir in source.TestPackageConfig.HintDirectories)
                if (!target.TestPackageConfig.HintDirectories.Contains(hintDir))
                    target.TestPackageConfig.HintDirectories.Add(hintDir);
        }

        private class Remote : LongLivedMarshalByRefObject
        {
            public ITestDriver CreateRemoteTestDriver(TestDomainSetup setup, ILogger logger, string runtimePath)
            {
                IHostFactory hostFactory = new IsolatedAppDomainHostFactory();
                IHost host = hostFactory.CreateHost(setup.TestPackageConfig.HostSetup, logger);
                HostAssemblyResolverHook.Bootstrap(host, runtimePath);

                ITestDriver remoteTestDriver = HostUtils.CreateInstance<LocalTestDriver>(host);
                ProxyTestDriver proxyTestDriver = new ProxyTestDriver(remoteTestDriver);
                proxyTestDriver.Disposed += delegate { host.Dispose(); };

                return proxyTestDriver;
            }
        }
    }
}
