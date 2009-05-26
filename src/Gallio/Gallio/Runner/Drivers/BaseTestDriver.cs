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
using Gallio.Common.Remoting;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// Base implementation of a test driver.
    /// </summary>
    /// <remarks>
    /// The base implementation inherits from <see cref="LongLivedMarshalByRefObject" />
    /// so that test driver's services can be accessed remotely if needed.
    /// </remarks>
    public abstract class BaseTestDriver : LongLivedMarshalByRefObject, ITestDriver
    {
        private RuntimeSetup runtimeSetup;
        private TestRunnerOptions testRunnerOptions;
        private ILogger logger = NullLogger.Instance;
        private bool initializedRuntime;

        /// <inheritdoc />
        public void Initialize(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger)
        {
            if (runtimeSetup == null)
                throw new ArgumentNullException("runtimeSetup");
            if (testRunnerOptions == null)
                throw new ArgumentNullException("testRunnerOptions");
            if (logger == null)
                throw new ArgumentNullException("logger");

            InitializeImpl(runtimeSetup, testRunnerOptions, logger);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public void Explore(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (testExplorationListener == null)
                throw new ArgumentNullException("testExplorationListener");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ExploreImpl(testPackageConfig, testExplorationOptions, testExplorationListener, progressMonitor);
        }

        /// <inheritdoc />
        public void Run(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (testExplorationListener == null)
                throw new ArgumentNullException("testExplorationListener");
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");
            if (testExecutionListener == null)
                throw new ArgumentNullException("testExecutionListener");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            RunImpl(testPackageConfig, testExplorationOptions, testExplorationListener, testExecutionOptions, testExecutionListener, progressMonitor);
        }

        /// <summary>
        /// Gets the runtime setup, or null if not initialized.
        /// </summary>
        protected RuntimeSetup RuntimeSetup
        {
            get { return runtimeSetup; }
        }

        /// <summary>
        /// Gets the test runner options, or null if not initialized.
        /// </summary>
        protected TestRunnerOptions TestRunnerOptions
        {
            get { return testRunnerOptions; }
        }

        /// <summary>
        /// Gets the logger, or a null logger instance if not initialized.
        /// </summary>
        protected ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Internal implementation of <see cref="Initialize" />.
        /// Called after argument validation takes place.
        /// </summary>
        protected virtual void InitializeImpl(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger)
        {
            this.runtimeSetup = runtimeSetup;
            this.testRunnerOptions = testRunnerOptions;
            this.logger = logger;

            if (!RuntimeAccessor.IsInitialized)
            {
                RuntimeBootstrap.Initialize(runtimeSetup, logger);
                initializedRuntime = true;
            }
        }

        /// <summary>
        /// Disposes the test driver.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && initializedRuntime)
            {
                initializedRuntime = false;

                runtimeSetup = null;
                logger = NullLogger.Instance;

                RuntimeBootstrap.Shutdown();
            }
        }

        /// <summary>
        /// Internal implementation of <see cref="Explore" />.
        /// Called after argument validation takes place.
        /// </summary>
        protected abstract void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor);

        /// <summary>
        /// Internal implementation of <see cref="Run" />.
        /// Called after argument validation takes place.
        /// </summary>
        protected abstract void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor);
    }
}
