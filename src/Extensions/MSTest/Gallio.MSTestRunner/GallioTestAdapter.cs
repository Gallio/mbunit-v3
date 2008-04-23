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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.MSTestRunner.Runtime;
using Gallio.Runner;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;
using ITestContext=Microsoft.VisualStudio.TestTools.Execution.ITestContext;

namespace Gallio.MSTestRunner
{
    internal class GallioTestAdapter : ILoadTestAdapter
    {
        private Gallio.Runner.ITestRunner runner;

        private IProgressMonitor currentProgressMonitor;

        private volatile bool isCanceled;

        public void Initialize(IRunContext runContext)
        {
            ITestRunnerManager runnerManager = RuntimeProvider.GetRuntime().Resolve<ITestRunnerManager>();
            runner = runnerManager.CreateTestRunner(StandardTestRunnerFactoryNames.IsolatedProcess);
            runner.RegisterExtension(new ResultPublisherExtension(runContext));

            ILogger logger = NullLogger.Instance;
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
        }

        public void LoadRun(ITestElement testElement, ITestContext testContext)
        {
        }

        public void PreTestRunFinished(IRunContext runContext)
        {
            TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

            List<Filter<string>> idFilters = new List<Filter<string>>();
            foreach (ITestElement testElement in runContext.RunConfig.TestElements)
            {
                GallioTestElement gallioTestElement = testElement as GallioTestElement;
                if (gallioTestElement != null)
                    idFilters.Add(new EqualityFilter<string>(gallioTestElement.GallioTestId));
            }

            testExecutionOptions.Filter = new IdFilter<ITest>(new OrFilter<string>(idFilters.ToArray()));

            RunWithProgressMonitor(delegate(IProgressMonitor progressMonitor)
            {
                runner.Run(testExecutionOptions, progressMonitor);
            });
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
