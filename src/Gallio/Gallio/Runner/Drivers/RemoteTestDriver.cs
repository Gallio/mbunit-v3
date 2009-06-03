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
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A proxy for a remote test driver.
    /// Wraps a test driver and forwards all messages to it.
    /// </summary>
    public class RemoteTestDriver : BaseTestDriver
    {
        private ITestDriver testDriver;

        /// <summary>
        /// Creates a proxy for a test driver.
        /// </summary>
        /// <param name="testDriver">The test driver to which messages should be forwarded.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriver"/> is null.</exception>
        public RemoteTestDriver(ITestDriver testDriver)
        {
            if (testDriver == null)
                throw new ArgumentNullException("testDriver");

            this.testDriver = testDriver;
        }

        /// <summary>
        /// An event that is fired when the test driver is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <inheritdoc />
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger)
        {
            testDriver.Initialize(runtimeSetup, testRunnerOptions, new RemoteLogger(logger));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && testDriver != null)
            {
                try
                {
                    testDriver.Dispose();
                    testDriver = null;
                }
                finally
                {
                    if (Disposed != null)
                        Disposed(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc />
        protected override void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            if (testDriver == null)
                throw new ObjectDisposedException("Test driver");

            testDriver.Explore(testPackageConfig,
                testExplorationOptions, new RemoteTestExplorationListener(testExplorationListener),
                new RemoteProgressMonitor(progressMonitor));
        }

        /// <inheritdoc />
        protected override void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            if (testDriver == null)
                throw new ObjectDisposedException("Test driver");

            testDriver.Run(testPackageConfig,
                testExplorationOptions, new RemoteTestExplorationListener(testExplorationListener),
                testExecutionOptions, new RemoteTestExecutionListener(testExecutionListener),
                new RemoteProgressMonitor(progressMonitor));
        }
    }
}
