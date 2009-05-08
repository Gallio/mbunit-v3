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
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Remoting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A test driver that runs tests remotely within a remote host.  The driver further partitions
    /// assemblies into test domains based on their configuration requirements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Copies down the test runner options to the containing host.
    /// </para>
    /// </remarks>
    public class HostedTestDriver : AggregateTestDriver
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFrameworkManager frameworkManager;
        private readonly IRuntime runtime;
        private readonly bool shareAppDomain;

        /// <summary>
        /// Creates a hosted test driver.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworkManager">The test framework manager</param>
        /// <param name="runtime">The Gallio runtime</param>
        /// <param name="shareAppDomain">If true, uses a shared app-domain for all test domains</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworkManager"/>, or <paramref name="runtime"/> is null</exception>
        public HostedTestDriver(IHostFactory hostFactory, ITestFrameworkManager frameworkManager, IRuntime runtime, bool shareAppDomain)
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
            this.shareAppDomain = shareAppDomain;
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
        protected override void DoWithPartitions(TestPackageConfig testPackageConfig, Action<IList<AggregateTestDriver.Partition>> action,
            Action<string> setStatus)
        {
            setStatus("Partitioning tests.");

            string workingDirectory = testPackageConfig.HostSetup.WorkingDirectory;
            bool shadowCopy = testPackageConfig.HostSetup.ShadowCopy;
            bool debug = testPackageConfig.HostSetup.Debug;

            ICollection<TestDomainSetup> testDomains = SplitPackageIntoTestDomains(testPackageConfig);

            ProcessorArchitecture arch = testPackageConfig.HostSetup.ProcessorArchitecture;
            if (arch == ProcessorArchitecture.None)
                arch = GetCommonProcessorArchitecture(testDomains);

            setStatus("Initializing the test host.");

            IHost remoteHost = CreateRemoteHost(workingDirectory, shadowCopy, debug, arch);
            try
            {
                Remote remote = HostUtils.CreateInstance<Remote>(remoteHost);

                var partitions = GenericCollectionUtils.ConvertAllToArray(testDomains,
                    testDomain => new Partition(new Factory(testDomain, remote, shareAppDomain, Logger), testDomain.TestPackageConfig));

                setStatus("");

                action(partitions);
            }
            finally
            {
                setStatus("Disposing the test host.");

                DisposeHost(remoteHost);

                setStatus("");
            }
        }

        private IHost CreateRemoteHost(string workingDirectory, bool shadowCopy, bool debug, ProcessorArchitecture arch)
        {
            HostSetup hostSetup = new HostSetup();
            hostSetup.ApplicationBaseDirectory = RuntimeSetup.RuntimePath;
            hostSetup.WorkingDirectory = workingDirectory;
            hostSetup.ShadowCopy = shadowCopy;
            hostSetup.Debug = debug;
            hostSetup.ProcessorArchitecture = arch;
            hostSetup.ConfigurationFileLocation = ConfigurationFileLocation.Temp;
            hostSetup.Properties.AddAll(TestRunnerOptions.Properties);

            ConfigureHost(hostSetup);

            IHost remoteHost = hostFactory.CreateHost(hostSetup, Logger);

            InitializeHost(remoteHost);
            return remoteHost;
        }

        private ICollection<TestDomainSetup> SplitPackageIntoTestDomains(TestPackageConfig testPackageConfig)
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

                foreach (AssemblyReference reference in runtime.GetAllPluginAssemblyReferences())
                {
                    if (reference.CodeBase != null)
                    {
                        testDomain.TestPackageConfig.HostSetup.Configuration.AddAssemblyBinding(reference.AssemblyName,
                            reference.CodeBase, true);
                    }
                }

                ITestExplorer explorer = frameworkManager.GetTestExplorer(traits => testDomain.TestPackageConfig.IsFrameworkRequested(traits.Id));
                explorer.ConfigureTestDomain(testDomain);

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

        private static ProcessorArchitecture GetCommonProcessorArchitecture(IEnumerable<TestDomainSetup> testDomains)
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

        private sealed class Remote : LongLivedMarshalByRefObject
        {
            public ITestDriver CreateTestDriverInIsolatedAppDomain(TestDomainSetup setup, ILogger logger)
            {
                IHostFactory hostFactory = new IsolatedAppDomainHostFactory(new DefaultDebuggerManager());
                IHost host = hostFactory.CreateHost(setup.TestPackageConfig.HostSetup, logger);

                ITestDriver hostTestDriver = HostUtils.CreateInstance<LocalTestDriver>(host);
                RemoteTestDriver remoteTestDriver = new RemoteTestDriver(hostTestDriver);
                remoteTestDriver.Disposed += delegate { host.Dispose(); };

                return remoteTestDriver;
            }
        }

        private sealed class Factory : ITestDriverFactory
        {
            private readonly TestDomainSetup testDomainSetup;
            private readonly Remote remote;
            private readonly bool shareAppDomain;
            private readonly ILogger logger;

            public Factory(TestDomainSetup testDomainSetup, Remote remote, bool shareAppDomain, ILogger logger)
            {
                this.testDomainSetup = testDomainSetup;
                this.remote = remote;
                this.shareAppDomain = shareAppDomain;
                this.logger = logger;
            }

            public ITestDriver CreateTestDriver()
            {
                if (shareAppDomain)
                    return new LocalTestDriver();
                return new RemoteTestDriver(remote.CreateTestDriverInIsolatedAppDomain(testDomainSetup, new RemoteLogger(logger)));
            }
        }
    }
}
