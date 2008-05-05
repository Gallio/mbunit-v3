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
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Gallio.Loader;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.MSTestRunner.Runtime;
using Gallio.Runner;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;
using ITestContext=Microsoft.VisualStudio.TestTools.Execution.ITestContext;

namespace Gallio.MSTestRunner
{
    /// <summary>
    /// <para>
    /// The Gallio test adapter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// We launch tests using a separate Gallio test runner AppDomain because we need to override
    /// the assembly resolution policy of the VSTS host.  Otherwise it's possible for test
    /// assemblies to interfere with the Gallio runtime particularly if the tests link to Gallio.dll
    /// itself.
    /// </para>
    /// <para>
    /// It's important to note that the adapter will run in a different context from the rest
    /// of the infrastructure.  Typically this will be inside of a standalone hosting process.
    /// If you are attempting to debug the adapter, be sure to attach your debugger to the
    /// hosting process.
    /// </para>
    /// </remarks>
    internal class GallioTestAdapter : ITestAdapter
    {
        private static readonly object runnerAppDomainSyncRoot = new object();
        private static AppDomain runnerAppDomain;

        private readonly IShim shim;

        public GallioTestAdapter()
        {
            PrepareRunnerAppDomain();

            shim = (IShim)runnerAppDomain.CreateInstanceFromAndUnwrap(typeof(Shim).Assembly.Location, typeof(Shim).FullName);
            shim.AddHintDirectory(Path.GetDirectoryName(typeof(IRunContext).Assembly.Location));
        }

        public void Initialize(IRunContext runContext)
        {
            shim.Initialize(runContext);
        }

        public void ReceiveMessage(object obj)
        {
            shim.ReceiveMessage(obj);
        }

        public void PreTestRunFinished(IRunContext runContext)
        {
            shim.PreTestRunFinished(runContext);
        }

        public void Run(ITestElement testElement, ITestContext testContext)
        {
            shim.Run(testElement, testContext);
        }

        public void Cleanup()
        {
            shim.Cleanup();
        }

        public void StopTestRun()
        {
            shim.StopTestRun();
        }

        public void AbortTestRun()
        {
            shim.AbortTestRun();
        }

        public void PauseTestRun()
        {
            shim.PauseTestRun();
        }

        public void ResumeTestRun()
        {
            shim.ResumeTestRun();
        }

        private static void PrepareRunnerAppDomain()
        {
            lock (runnerAppDomainSyncRoot)
            {
                if (runnerAppDomain != null)
                    return;

                string installationPath = GallioLoader.Initialize(typeof(GallioTestAdapter).Assembly).InstallationPath;
                AppDomainSetup appDomainSetup = new AppDomainSetup();
                appDomainSetup.ApplicationName = "Gallio";
                appDomainSetup.ApplicationBase = installationPath;
                Evidence evidence = AppDomain.CurrentDomain.Evidence;
                PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
                StrongName[] fullTrustAssemblies = new StrongName[0];
                runnerAppDomain = AppDomain.CreateDomain(appDomainSetup.ApplicationName, evidence, appDomainSetup, defaultPermissionSet, fullTrustAssemblies);
            }
        }

        private interface IShim : ITestAdapter
        {
            void AddHintDirectory(string path);
        }

        private sealed class Shim : MarshalByRefObject, IShim
        {
            private Gallio.Runner.ITestRunner runner;
            private IRunContext runContext;

            private IProgressMonitor currentProgressMonitor;

            private volatile bool isCanceled;

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void AddHintDirectory(string path)
            {
                IAssemblyResolverManager resolverManager = RuntimeProvider.GetRuntime().Resolve<IAssemblyResolverManager>();
                resolverManager.AddHintDirectory(path);
            }

            public void Initialize(IRunContext runContext)
            {
                this.runContext = runContext;

                ITestRunnerManager runnerManager = RuntimeProvider.GetRuntime().Resolve<ITestRunnerManager>();
                runner = runnerManager.CreateTestRunner(StandardTestRunnerFactoryNames.IsolatedProcess);
                runner.RegisterExtension(new RunContextExtension(runContext));

                ILogger logger = new RunContextLogger(runContext);
                TestRunnerOptions testRunnerOptions = new TestRunnerOptions();

                RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                {
                    runner.Initialize(testRunnerOptions, logger, progressMonitor);
                });

                if (isCanceled)
                    return;

                TestPackageConfig testPackageConfig = new TestPackageConfig();
                foreach (ITestElement testElement in runContext.RunConfig.TestElements)
                {
                    GallioTestElement gallioTestElement = testElement as GallioTestElement;
                    if (gallioTestElement != null
                        && !testPackageConfig.AssemblyFiles.Contains(gallioTestElement.AssemblyPath))
                    {
                        testPackageConfig.AssemblyFiles.Add(gallioTestElement.AssemblyPath);
                    }
                }

                RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                {
                    runner.Load(testPackageConfig, progressMonitor);
                });

                if (isCanceled)
                    return;

                TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
                RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                {
                    runner.Explore(testExplorationOptions, progressMonitor);
                });
            }

            public void Cleanup()
            {
                runContext = null;

                if (runner != null)
                {
                    runner.Dispose(NullProgressMonitor.CreateInstance());
                    runner = null;
                }
            }

            public void ReceiveMessage(object obj)
            {
            }

            public void Run(ITestElement testElement, ITestContext testContext)
            {
                if (runContext != null)
                {
                    TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

                    List<Filter<string>> idFilters = new List<Filter<string>>();
                    foreach (ITestElement includedTestElement in runContext.RunConfig.TestElements)
                    {
                        GallioTestElement gallioTestElement = includedTestElement as GallioTestElement;
                        if (gallioTestElement != null)
                            idFilters.Add(new EqualityFilter<string>(gallioTestElement.GallioTestId));
                    }

                    testExecutionOptions.Filter = new IdFilter<ITest>(new OrFilter<string>(idFilters.ToArray()));

                    RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                    {
                        runner.Run(testExecutionOptions, progressMonitor);
                    });

                    runContext = null;
                }
            }

            public void PreTestRunFinished(IRunContext runContext)
            {
            }

            public void StopTestRun()
            {
                CancelRun();
            }

            public void AbortTestRun()
            {
                CancelRun();
            }

            public void PauseTestRun()
            {
                throw new NotSupportedException();
            }

            public void ResumeTestRun()
            {
                throw new NotSupportedException();
            }

            private void CancelRun()
            {
                lock (this)
                {
                    isCanceled = true;

                    if (currentProgressMonitor != null)
                        currentProgressMonitor.Cancel();
                }
            }

            private void RunWithProgressMonitor(Action<IProgressMonitor> action)
            {
                try
                {
                    lock (this)
                    {
                        if (isCanceled)
                            return;

                        currentProgressMonitor = NullProgressMonitor.CreateInstance();
                    }

                    action(currentProgressMonitor);
                }
                finally
                {
                    lock (this)
                        currentProgressMonitor = null;
                }
            }
        }
    }
}
