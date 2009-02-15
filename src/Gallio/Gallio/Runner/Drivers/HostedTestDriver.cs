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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Remoting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// <para>
    /// A test driver that runs tests remotely within a remote host.  The driver further partitions
    /// assemblies into test domains based on their configuration requirements.
    /// </para>
    /// </summary>
    public class HostedTestDriver : AggregateTestDriver
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFramework[] frameworks;
        private readonly IRuntime runtime;
        private readonly bool shareAppDomain;

        private IHost remoteHost;
        private Remote remote;

        /// <summary>
        /// Creates a hosted test driver.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworks">The test frameworks that should participate in test domain configuration</param>
        /// <param name="runtime">The Gallio runtime</param>
        /// <param name="sharedAppDomain">If true, uses a shared app-domain for all test domains</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworks"/>, or <paramref name="runtime"/> is null</exception>
        public HostedTestDriver(IHostFactory hostFactory, ITestFramework[] frameworks, IRuntime runtime, bool sharedAppDomain)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.hostFactory = hostFactory;
            this.frameworks = frameworks;
            this.runtime = runtime;
            this.shareAppDomain = sharedAppDomain;
        }

        /// <summary>
        /// Provides an opportunity for subclasses to configure the host.
        /// </summary>
        /// <param name="hostSetup">The host setup, not null</param>
        protected virtual void ConfigureHost(HostSetup hostSetup)
        {
        }

        /// <summary>
        /// Provides an opportunity for subclasses to initialize the host once created.
        /// </summary>
        /// <param name="host">The host, not null</param>
        protected virtual void InitializeHost(IHost host)
        {
        }

        /// <summary>
        /// Provides an opportunity for subclasses to clean up the host as part of its disposal.
        /// </summary>
        /// <param name="host">The host, not null</param>
        protected virtual void DisposeHost(IHost host)
        {
            host.Dispose();
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();

            if (remoteHost != null)
            {
                DisposeHost(remoteHost);
                remoteHost = null;
            }

            remote = null;
        }

        /// <inheritdoc />
        protected override IEnumerable<Partition> CreatePartitions(TestPackageConfig testPackageConfig)
        {
            ICollection<TestDomainSetup> testDomains = GetTestDomains(testPackageConfig);

            ProcessorArchitecture arch = testPackageConfig.HostSetup.ProcessorArchitecture;
            if (arch == ProcessorArchitecture.None)
                arch = GetCommonProcessorArchitecture(testDomains);

            remoteHost = CreateRemoteHost(testPackageConfig.HostSetup.WorkingDirectory,
                testPackageConfig.HostSetup.ShadowCopy, arch);

            remote = HostUtils.CreateInstance<Remote>(remoteHost);

            foreach (TestDomainSetup testDomain in testDomains)
            {
                ITestDriver testDriver = CreateTestDriver(testDomain);
                testDriver.Initialize(RuntimeSetup, Logger);

                yield return new Partition(testDriver, testDomain.TestPackageConfig);
            }
        }

        private ITestDriver CreateTestDriver(TestDomainSetup testDomain)
        {
            if (shareAppDomain)
                return new LocalTestDriver();
            return new ProxyTestDriver(remote.CreateRemoteTestDriver(testDomain, new RemoteLogger(Logger)));
        }

        private IHost CreateRemoteHost(string workingDirectory, bool shadowCopy, ProcessorArchitecture arch)
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ApplicationBaseDirectory = RuntimeSetup.RuntimePath;
            hostSetup.WorkingDirectory = workingDirectory;
            hostSetup.ShadowCopy = shadowCopy;
            hostSetup.ProcessorArchitecture = arch;
            hostSetup.ConfigurationFileLocation = ConfigurationFileLocation.Temp;

            ConfigureHost(hostSetup);

            IHost remoteHost = hostFactory.CreateHost(hostSetup, Logger);

            InitializeHost(remoteHost);
            return remoteHost;
        }

        private ICollection<TestDomainSetup> GetTestDomains(TestPackageConfig testPackageConfig)
        {
            Dictionary<HostSetup, TestDomainSetup> testDomains = new Dictionary<HostSetup, TestDomainSetup>();
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
                testDomain.TestPackageConfig.HostSetup.ProcessorArchitecture = GetProcessorArchitecture(assemblyFile);

                string assemblyConfigFile = assemblyFile + @".config";
                if (File.Exists(assemblyConfigFile))
                    testDomain.TestPackageConfig.HostSetup.Configuration.ConfigurationXml = File.ReadAllText(assemblyConfigFile);

                foreach (KeyValuePair<string, AssemblyName> pair in runtime.GetPluginAssemblyPaths())
                    testDomain.TestPackageConfig.HostSetup.Configuration.AddAssemblyBinding(pair.Value, new Uri(pair.Key).ToString(), true);

                foreach (ITestFramework framework in frameworks)
                    framework.ConfigureTestDomain(testDomain);

                TestDomainSetup existingTestDomain;
                if (testDomains.TryGetValue(testDomain.TestPackageConfig.HostSetup, out existingTestDomain))
                {
                    existingTestDomain.MergeFrom(testDomain);
                }
                else
                {
                    testDomains.Add(testDomain.TestPackageConfig.HostSetup, testDomain);
                }
            }

            return testDomains.Values;
        }

        private static ProcessorArchitecture GetCommonProcessorArchitecture(ICollection<TestDomainSetup> testDomains)
        {
            ProcessorArchitecture commonArch = ProcessorArchitecture.MSIL;
            foreach (TestDomainSetup testDomain in testDomains)
            {
                ProcessorArchitecture testDomainArch = testDomain.TestPackageConfig.HostSetup.ProcessorArchitecture;
                switch (testDomainArch)
                {
                    case ProcessorArchitecture.Amd64:
                    case ProcessorArchitecture.IA64:
                    case ProcessorArchitecture.X86:
                        if (commonArch != testDomainArch && commonArch != ProcessorArchitecture.MSIL)
                            throw new RunnerException(String.Format("Cannot run all test assemblies together because some require the {0} architecture while others require the {1} architecture.", commonArch, testDomainArch));
                        commonArch = testDomainArch;
                        break;
                }
            }

            return commonArch;
        }

        private static ProcessorArchitecture GetProcessorArchitecture(string assemblyFile)
        {
            try
            {
                return AssemblyName.GetAssemblyName(assemblyFile).ProcessorArchitecture;
            }
            catch (Exception)
            {
                return ProcessorArchitecture.None;
            }
        }

        private class Remote : LongLivedMarshalByRefObject
        {
            public ITestDriver CreateRemoteTestDriver(TestDomainSetup setup, ILogger logger)
            {
                IHostFactory hostFactory = new IsolatedAppDomainHostFactory();
                IHost host = hostFactory.CreateHost(setup.TestPackageConfig.HostSetup, logger);

                ITestDriver remoteTestDriver = HostUtils.CreateInstance<LocalTestDriver>(host);
                ProxyTestDriver proxyTestDriver = new ProxyTestDriver(remoteTestDriver);
                proxyTestDriver.Disposed += delegate { host.Dispose(); };

                return proxyTestDriver;
            }
        }
    }
}
