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
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Remoting;
using Gallio.Common.Reflection;

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
        private readonly IsolationMode isolationMode;

        /// <summary>
        /// Creates a hosted test driver.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworkManager">The test framework manager</param>
        /// <param name="runtime">The Gallio runtime</param>
        /// <param name="isolationMode">Specifies the isolation mode to use</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworkManager"/>, or <paramref name="runtime"/> is null</exception>
        public HostedTestDriver(IHostFactory hostFactory, ITestFrameworkManager frameworkManager, IRuntime runtime, IsolationMode isolationMode)
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
            this.isolationMode = isolationMode;
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
        protected override void DoWithPartitions(TestPackageConfig testPackageConfig, Action<IList<Partition>> action, Action<string> setStatus)
        {
            setStatus("Partitioning tests.");

            if (isolationMode == IsolationMode.HostPerAssembly)
            {
                ICollection<TestDomainSetup> testDomains = CreateTestDomainPerAssembly(testPackageConfig);
                foreach (TestDomainSetup testDomain in testDomains)
                {
                    // CreateRemoteHostAndPerformAction expects a collection of test domains, so fake it
                    TestDomainSetup[] domains = new[] { testDomain };
                    CreateRemoteHostAndPerformAction(setStatus, testDomain.TestPackageConfig, domains, false, action);
                }
            }
            else
            {
                ICollection<TestDomainSetup> testDomains = CreateTestDomainPerGroupOfIdenticallyConfiguredAssemblies(testPackageConfig);
                bool separateAppDomain = isolationMode == IsolationMode.AppDomainPerAssembly;
                CreateRemoteHostAndPerformAction(setStatus, testPackageConfig, testDomains, separateAppDomain, action);
            }
        }

        private void CreateRemoteHostAndPerformAction(Action<string> setStatus, TestPackageConfig testPackageConfig, ICollection<TestDomainSetup> testDomains, bool separateAppDomain, Action<IList<Partition>> action)
        {
            HostSetup hostSetup = testPackageConfig.HostSetup.Copy();
            hostSetup.Properties.AddAll(TestRunnerOptions.Properties);

            if (separateAppDomain)
            {
                hostSetup.ApplicationBaseDirectory = RuntimeSetup.RuntimePath;
                hostSetup.ConfigurationFileLocation = ConfigurationFileLocation.Temp;
            }

            var assemblyMetadataList = GetAssemblyMetadataList(testDomains);
            hostSetup.ProcessorArchitecture = ResolveProcessArchitecture(hostSetup.ProcessorArchitecture, assemblyMetadataList);

            if (hostSetup.Configuration.SupportedRuntimeVersions.Count == 0)
            {
                string runtimeVersion = ResolveRuntimeVersion(testDomains);
                if (runtimeVersion != null)
                    hostSetup.Configuration.SupportedRuntimeVersions.Add(runtimeVersion);
            }

            setStatus("Initializing the test host.");

            IHost remoteHost = CreateRemoteHost(hostSetup);
            try
            {
                Remote remote = HostUtils.CreateInstance<Remote>(remoteHost);

                var partitions = GenericCollectionUtils.ConvertAllToArray(testDomains,
                    testDomain => new Partition(new Factory(testDomain, remote, separateAppDomain, Logger), testDomain.TestPackageConfig));

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

        private IHost CreateRemoteHost(HostSetup hostSetup)
        {
            ConfigureHost(hostSetup);

            IHost remoteHost = hostFactory.CreateHost(hostSetup, Logger);

            InitializeHost(remoteHost);
            return remoteHost;
        }

        private ICollection<TestDomainSetup> CreateTestDomainPerAssembly(TestPackageConfig testPackageConfig)
        {
            var testDomains = new List<TestDomainSetup>();

            foreach (string assemblyFile in testPackageConfig.AssemblyFiles)
            {
                TestDomainSetup testDomain = CreateTestDomain(testPackageConfig.Copy(), assemblyFile);
                testDomains.Add(testDomain);
            }

            return testDomains;
        }

        private ICollection<TestDomainSetup> CreateTestDomainPerGroupOfIdenticallyConfiguredAssemblies(TestPackageConfig testPackageConfig)
        {
            var testDomains = new Dictionary<HostSetup, TestDomainSetup>();

            foreach (string assemblyFile in testPackageConfig.AssemblyFiles)
            {
                TestDomainSetup testDomain = CreateTestDomain(testPackageConfig.Copy(), assemblyFile);

                TestDomainSetup existingIdenticallyConfiguredTestDomain;
                if (testDomains.TryGetValue(testDomain.TestPackageConfig.HostSetup, out existingIdenticallyConfiguredTestDomain))
                {
                    existingIdenticallyConfiguredTestDomain.MergeFrom(testDomain);
                }
                else
                {
                    testDomains.Add(testDomain.TestPackageConfig.HostSetup, testDomain);
                }
            }

            return testDomains.Values;
        }

        private TestDomainSetup CreateTestDomain(TestPackageConfig testPackageConfig, string assemblyFile)
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
            {
                string configurationXml = File.ReadAllText(assemblyConfigFile);
                testDomain.TestPackageConfig.HostSetup.Configuration.ConfigurationXml = configurationXml;
            }

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

            return testDomain;
        }

        private static List<AssemblyMetadata> GetAssemblyMetadataList(ICollection<TestDomainSetup> testDomains)
        {
            var metadataList = new List<AssemblyMetadata>();
            foreach (var testDomain in testDomains)
            {
                foreach (var assemblyFile in testDomain.TestPackageConfig.AssemblyFiles)
                {
                    AssemblyMetadata metadata = AssemblyUtils.GetAssemblyMetadata(assemblyFile);
                    if (metadata != null)
                        metadataList.Add(metadata);
                }
            }

            return metadataList;
        }

        private static string ResolveRuntimeVersion(IEnumerable<TestDomainSetup> testDomains)
        {
            List<string> commonRuntimeVersions = new List<string>();

            foreach (TestDomainSetup testDomain in testDomains)
            {
                string configurationXml = testDomain.TestPackageConfig.HostSetup.Configuration.ConfigurationXml;
                if (configurationXml == null)
                    continue;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configurationXml);

                List<string> runtimeVersions = new List<string>();
                foreach (XmlElement supportedRuntimeNode in doc.SelectNodes("/configuration/startup/supportedRuntime"))
                {
                    string runtimeVersion = supportedRuntimeNode.GetAttribute("version");
                    if (runtimeVersion != null)
                        runtimeVersions.Add(runtimeVersion);
                }

                if (runtimeVersions.Count != 0)
                {
                    if (commonRuntimeVersions.Count == 0)
                    {
                        commonRuntimeVersions.AddRange(runtimeVersions);
                    }
                    else
                    {
                        for (int i = 0; i < commonRuntimeVersions.Count; i++)
                        {
                            string commonRuntimeVersion = commonRuntimeVersions[0];
                            if (runtimeVersions.Contains(commonRuntimeVersion))
                                continue;

                            if (commonRuntimeVersions.Count == 1)
                            {
                                throw new RunnerException(String.Format(
                                    "Cannot run all test assemblies together because some require the '{0}' runtime while others require the '{1}' runtime.",
                                    runtimeVersions[0], commonRuntimeVersion));
                            }

                            commonRuntimeVersions.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
            }

            return commonRuntimeVersions.Count == 0 ? null : commonRuntimeVersions[0];
        }

        private static ProcessorArchitecture ResolveProcessArchitecture(ProcessorArchitecture requestedArch, IEnumerable<AssemblyMetadata> assemblyMetadataList)
        {
            if (requestedArch == ProcessorArchitecture.None)
                return GetCommonProcessorArchitecture(assemblyMetadataList);
            return requestedArch;
        }

        private static ProcessorArchitecture GetCommonProcessorArchitecture(IEnumerable<AssemblyMetadata> assemblyMetadataList)
        {
            ProcessorArchitecture commonArch = ProcessorArchitecture.MSIL;
            foreach (AssemblyMetadata assemblyMetadata in assemblyMetadataList)
            {
                ProcessorArchitecture assemblyArch = assemblyMetadata.ProcessorArchitecture;
                switch (assemblyArch)
                {
                    case ProcessorArchitecture.Amd64:
                    case ProcessorArchitecture.IA64:
                    case ProcessorArchitecture.X86:
                        if (commonArch != assemblyArch && commonArch != ProcessorArchitecture.MSIL)
                        {
                            throw new RunnerException(String.Format(
                                "Cannot run all test assemblies together because some require the {0} architecture while others require the {1} architecture.",
                                commonArch, assemblyArch));
                        }

                        commonArch = assemblyArch;
                        break;
                }
            }

            return commonArch;
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

            public ITestDriver CreateTestDriver()
            {
                return new LocalTestDriver();
            }
        }

        private sealed class Factory : ITestDriverFactory
        {
            private readonly TestDomainSetup testDomainSetup;
            private readonly Remote remote;
            private readonly bool separateAppDomain;
            private readonly ILogger logger;

            public Factory(TestDomainSetup testDomainSetup, Remote remote, bool separateAppDomain, ILogger logger)
            {
                this.testDomainSetup = testDomainSetup;
                this.remote = remote;
                this.separateAppDomain = separateAppDomain;
                this.logger = logger;
            }

            public ITestDriver CreateTestDriver()
            {
                return new RemoteTestDriver(separateAppDomain
                    ? remote.CreateTestDriverInIsolatedAppDomain(testDomainSetup, new RemoteLogger(logger))
                    : remote.CreateTestDriver());
            }
        }
    }
}
