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
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Schema;
using Gallio.Runtime;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;
using ITestContext=Microsoft.VisualStudio.TestTools.Execution.ITestContext;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// <para>
    /// The Gallio test adapter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test adapter is started by VSTS and typically runs as a standalone process.
    /// If you are attempting to debug the adapter, be sure to attach the debugger
    /// to the VSTS host process (VSTestHost.exe).
    /// </para>
    /// </remarks>
    public class GallioTestAdapter : ITestAdapter
    {
        private IProgressMonitor currentProgressMonitor;

        private IRunContext runContext;
        private bool running;
        private volatile bool isCanceled;

        public void Initialize(IRunContext runContext)
        {
            this.runContext = runContext;
        }

        public void Cleanup()
        {
            CancelRun();
        }

        public void ReceiveMessage(object obj)
        {
        }

        public void Run(ITestElement testElement, ITestContext testContext)
        {
            if (! running && runContext != null)
            {
                running = true;
                RunAllTests(runContext);
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

        private void RunAllTests(IRunContext runContext)
        {
            ITestRunnerManager runnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
            var runner = runnerManager.CreateTestRunner(StandardTestRunnerFactoryNames.IsolatedAppDomain);
            runner.RegisterExtension(new RunContextExtension(runContext));

            ILogger logger = new RunContextLogger(runContext);
            TestRunnerOptions testRunnerOptions = new TestRunnerOptions();

            try
            {
                RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                {
                    runner.Initialize(testRunnerOptions, logger, progressMonitor);
                });

                if (isCanceled)
                    return;

                TestPackage testPackage = new TestPackage();
                testPackage.AddExcludedTestFrameworkId("MSTestAdapter.TestFramework");

                foreach (ITestElement testElement in runContext.RunConfig.TestElements)
                {
                    GallioTestElement gallioTestElement = testElement as GallioTestElement;
                    if (gallioTestElement != null)
                    {
                        testPackage.AddFile(new FileInfo(gallioTestElement.AssemblyPath));
                    }
                }

                TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
                TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

                List<Filter<string>> idFilters = new List<Filter<string>>();
                foreach (ITestElement includedTestElement in runContext.RunConfig.TestElements)
                {
                    GallioTestElement gallioTestElement = includedTestElement as GallioTestElement;
                    if (gallioTestElement != null)
                        idFilters.Add(new EqualityFilter<string>(gallioTestElement.GallioTestId));
                }

                testExecutionOptions.FilterSet = new FilterSet<ITestDescriptor>(new IdFilter<ITestDescriptor>(new OrFilter<string>(idFilters)));

                RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
                {
                    runner.Run(testPackage, testExplorationOptions, testExecutionOptions, progressMonitor);
                });
            }
            finally
            {
                runner.Dispose(NullProgressMonitor.CreateInstance());
            }
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
