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
using Gallio.Icarus.Services.Interfaces;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Icarus.Services
{
    public class TestRunnerService : ITestRunnerService
    {
        private ITestRunner testRunner;
        private readonly TestExplorationOptions testExplorationOptions = new TestExplorationOptions();
        private readonly TestExecutionOptions testExecutionOptions = new TestExecutionOptions();

        private LockBox<Report>? previousReportFromUnloadedPackage;

        public event EventHandler<LoadFinishedEventArgs> LoadFinished;
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

        public ITestRunner TestRunner
        {
            get { return testRunner; }
            set
            {
                testRunner = value;

                testRunner.Events.LoadFinished += ((sender, e) =>
                    EventHandlerUtils.SafeInvoke(LoadFinished, this, e));
                testRunner.Events.TestStepFinished += ((sender, e) =>
                    EventHandlerUtils.SafeInvoke(TestStepFinished, this, e));
            }
        }

        public void Dispose()
        {
            NullProgressMonitorProvider.Instance.Run(delegate(IProgressMonitor progressMonitor)
            {
                if (testRunner != null)
                    testRunner.Dispose(progressMonitor);
            });
        }

        public void Load(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor.BeginTask("Loading test package.", 2))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    Unload(subProgressMonitor);
                previousReportFromUnloadedPackage = null;

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                {
                    if (testRunner != null)
                        testRunner.Load(testPackageConfig, subProgressMonitor);
                }
            }
        }

        public void Initialize()
        {
            NullProgressMonitorProvider.Instance.Run(progressMonitor =>
                testRunner.Initialize(new TestRunnerOptions(), RuntimeAccessor.Logger,
                progressMonitor));
        }

        public TestModelData Explore(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor.BeginTask("Exploring test package.", 100))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(50))
                    if (testRunner != null)
                        testRunner.Explore(testExplorationOptions, subProgressMonitor);

                using (progressMonitor.CreateSubProgressMonitor(50))
                {
                    // We extract the test model from the report.
                    // This is safe because the test model should not change after explore is finished.
                    TestModelData testModelData = null;
                    Report.Read(report => testModelData = report.TestModel);
                    return testModelData;
                }
            }
        }

        public void Run(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor.BeginTask("Running tests.", 1))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    if (testRunner != null)
                        testRunner.Run(testExecutionOptions, subProgressMonitor);
            }
        }

        public void SetFilter(Filter<ITest> filter, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Setting test filter.", 2))
            {
                testExecutionOptions.Filter = filter;
                progressMonitor.Worked(1);
                testExecutionOptions.ExactFilter = true;
            }
        }

        public void Unload(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            using (progressMonitor.BeginTask("Unloading test package.", 1))
            {
                previousReportFromUnloadedPackage = testRunner.Report;

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(1))
                    if (testRunner != null)
                        testRunner.Unload(subProgressMonitor);
            }
        }
    }
}
