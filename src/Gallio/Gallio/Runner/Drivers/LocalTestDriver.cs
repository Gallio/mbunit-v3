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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Messages;
using Gallio.Runner.Harness;
using Gallio.Runtime;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A simple test driver implementation that runs tests locally within the
    /// current domain without any additional consultation of the test frameworks.
    /// </summary>
    public class LocalTestDriver : BaseTestDriver
    {
        private readonly LocalHostFactory localHostFactory;

        private ITestHarness harness;

        /// <summary>
        /// Creates a test driver.
        /// </summary>
        public LocalTestDriver()
            : this(new DefaultDebuggerManager()) // FIXME: Should use IoC always
        {
        }

        /// <summary>
        /// Creates a test driver.
        /// </summary>
        /// <param name="debuggerManager">A reference to the debugger manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="debuggerManager"/> is null</exception>
        public LocalTestDriver(IDebuggerManager debuggerManager)
        {
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");

            localHostFactory = new LocalHostFactory(debuggerManager);
        }

        /// <inheritdoc />
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger)
        {
            base.InitializeImpl(runtimeSetup, testRunnerOptions, logger);

            ITestHarnessFactory factory = RuntimeAccessor.Registry.Resolve<ITestHarnessFactory>();
            harness = factory.CreateHarness();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && harness != null)
                harness.Dispose();
        }

        /// <inheritdoc />
        protected override void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring the tests.", 4))
            {
                using (CreateLocalHost(testPackageConfig))
                {
                    try
                    {
                        harness.Load(testPackageConfig, progressMonitor.CreateSubProgressMonitor(1));
                        harness.Explore(testExplorationOptions, testExplorationListener,
                            progressMonitor.CreateSubProgressMonitor(2));
                    }
                    finally
                    {
                        harness.Unload(progressMonitor.CreateSubProgressMonitor(1));
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running the tests.", 14))
            {
                using (CreateLocalHost(testPackageConfig))
                {
                    try
                    {
                        harness.Load(testPackageConfig, progressMonitor.CreateSubProgressMonitor(1));
                        harness.Explore(testExplorationOptions, testExplorationListener,
                            progressMonitor.CreateSubProgressMonitor(2));
                        harness.Run(testExecutionOptions, testExecutionListener,
                            progressMonitor.CreateSubProgressMonitor(10));
                    }
                    finally
                    {
                        harness.Unload(progressMonitor.CreateSubProgressMonitor(1));
                    }
                }
            }
        }

        // We create a local host to ensure that the working directory and debugging options
        // are set correctly during exploration and execution
        private IHost CreateLocalHost(TestPackageConfig testPackageConfig)
        {
            return localHostFactory.CreateHost(testPackageConfig.HostSetup, Logger);
        }
    }
}
