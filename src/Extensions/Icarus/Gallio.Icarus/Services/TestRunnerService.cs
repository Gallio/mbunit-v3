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
using Gallio.Concurrency;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Services
{
    public class TestRunnerService : ITestRunnerService
    {
        private readonly ITestRunner testRunner;
        private readonly TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
        private readonly TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

        private readonly ProgressMonitorProvider progressMonitorProvider = new ProgressMonitorProvider();

        private LockBox<Report>? previousReportFromUnloadedPackage;

        public event EventHandler<ProgressUpdateEventArgs> ProgressUpdate;
        public event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        public LockBox<Report> Report
        {
            get { return previousReportFromUnloadedPackage ?? testRunner.Report; }
        }
        
        public IList<string> TestFrameworks
        {
            get
            {
                List<string> frameworks = new List<string>();
                foreach (ITestFramework framework in RuntimeAccessor.Instance.ResolveAll<ITestFramework>())
                    frameworks.Add(framework.Name);
                return frameworks;
            }
        }

        public TestRunnerService(ITestRunner testRunner)
        {
            this.testRunner = testRunner;

            this.testRunner.Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                EventHandlerUtils.SafeInvoke(TestStepFinished, this, e);
            };

            // hook up progress monitor
            progressMonitorProvider.ProgressUpdate += delegate(object sender, ProgressUpdateEventArgs e)
            {
                EventHandlerUtils.SafeInvoke(ProgressUpdate, this, e);
            };
        }

        public void Initialize()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                TestRunnerOptions options = new TestRunnerOptions();
                ILogger logger = RuntimeAccessor.Logger;
                testRunner.Initialize(options, logger, progressMonitor);
            });
        }

        public void Dispose()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Dispose(progressMonitor);
            });
        }

        public void Load(TestPackageConfig testPackageConfig)
        {
            Unload();
            previousReportFromUnloadedPackage = null;

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Load(testPackageConfig, progressMonitor);
            });
        }

        public TestModelData Explore()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Explore(testExplorationOptions, progressMonitor);
            });

            // We extract the test model from the report.
            // This is safe because the test model should not change after explore is finished.
            TestModelData testModelData = null;
            Report.Read(report => testModelData = report.TestModel);
            return testModelData;
        }

        public void Run()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Run(testExecutionOptions, progressMonitor);
            });
        }

        public void Cancel()
        {
            if (progressMonitorProvider.ProgressMonitor != null)
                progressMonitorProvider.ProgressMonitor.Cancel();
        }

        public void SetFilter(Filter<ITest> filter)
        {
            testExecutionOptions.Filter = filter;
            testExecutionOptions.ExactFilter = true;
        }

        public void Unload()
        {
            previousReportFromUnloadedPackage = testRunner.Report;

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Unload(progressMonitor);
            });
        }
    }
}
