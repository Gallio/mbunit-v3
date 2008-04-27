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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runner.Domains;
using Gallio.Runner.Harness;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Remoting;

namespace Gallio.Runner
{
    internal sealed class HostedTestDomainController
    {
        private readonly IHost host;
        private Remote remote;

        public HostedTestDomainController(IHost host)
        {
            this.host = host;
        }

        public void LoadTestDomains(TestPackageConfig testPackageConfig, ILogger logger, IProgressMonitor progressMonitor)
        {
            if (! host.IsLocal)
                HostAssemblyResolverHook.Install(host);

            Type remoteType = typeof(Remote);
            remote = (Remote)host.GetHostService().CreateInstance(remoteType.Assembly.FullName, remoteType.FullName).Unwrap();

            if (! host.IsLocal)
                remote.InitializeRuntime(RuntimeAccessor.Instance.GetRuntimeFactory(),
                    RuntimeAccessor.Instance.GetRuntimeSetup(), new RemoteLogger(logger));

            remote.LoadTestDomains(testPackageConfig, new RemoteProgressMonitor(progressMonitor));
        }

        public TestModelData ExploreTestDomains(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            return remote.ExploreTestDomains(options, new RemoteProgressMonitor(progressMonitor));
        }

        public void RunTestDomains(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            remote.RunTestDomains(options, new RemoteTestListener(listener), new RemoteProgressMonitor(progressMonitor));
        }

        public void UnloadTestDomains(IProgressMonitor progressMonitor)
        {
            if (remote != null)
            {
                remote.UnloadTestDomains(new RemoteProgressMonitor(progressMonitor));

                if (!host.IsLocal)
                    remote.ShutdownRuntime();

                remote = null;
            }
        }

        private sealed class Remote : LongLivedMarshalByRefObject
        {
            private ITestDomain domain;

            public void InitializeRuntime(RuntimeFactory runtimeFactory, RuntimeSetup runtimeSetup, ILogger logger)
            {
                RuntimeBootstrap.Initialize(runtimeFactory, runtimeSetup, logger);
            }

            public void ShutdownRuntime()
            {
                RuntimeBootstrap.Shutdown();
            }

            public void LoadTestDomains(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
            {
                // HACK: Temporary workaround until we refactor ITestDomain.
                ITestHarnessFactory harnessFactory = RuntimeAccessor.Instance.Resolve<ITestHarnessFactory>();
                domain = new LocalTestDomain(harnessFactory);
                domain.Load(testPackageConfig, progressMonitor);
            }

            public TestModelData ExploreTestDomains(TestExplorationOptions options, IProgressMonitor progressMonitor)
            {
                domain.Explore(options, progressMonitor);
                return domain.TestModelData;
            }

            public void RunTestDomains(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
            {
                domain.Run(options, listener, progressMonitor);
            }

            public void UnloadTestDomains(IProgressMonitor progressMonitor)
            {
                domain.Unload(progressMonitor);
                domain = null;
            }
        }
    }
}
