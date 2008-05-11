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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// A proxy for a remote test driver.
    /// Wraps a test driver and forwards all messages to it.
    /// </summary>
    public class ProxyTestDriver : BaseTestDriver
    {
        private ITestDriver testDriver;

        /// <summary>
        /// Creates a proxy for a test driver.
        /// </summary>
        /// <param name="testDriver">The test driver to which messages should be forwarded</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriver"/> is null</exception>
        public ProxyTestDriver(ITestDriver testDriver)
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
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, ILogger logger)
        {
            testDriver.Initialize(runtimeSetup, new RemoteLogger(logger));
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
        protected override void LoadImpl(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            testDriver.Load(testPackageConfig, new RemoteProgressMonitor(progressMonitor));
        }

        /// <inheritdoc />
        protected override TestModelData ExploreImpl(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            return testDriver.Explore(options, new RemoteProgressMonitor(progressMonitor));
        }

        /// <inheritdoc />
        protected override void RunImpl(TestExecutionOptions options, ITestListener listener,
            IProgressMonitor progressMonitor)
        {
            testDriver.Run(options, new RemoteTestListener(listener), new RemoteProgressMonitor(progressMonitor));
        }

        /// <inheritdoc />
        protected override void UnloadImpl(IProgressMonitor progressMonitor)
        {
            testDriver.Unload(new RemoteProgressMonitor(progressMonitor));
        }
    }
}
