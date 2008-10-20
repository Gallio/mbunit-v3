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
using System.Diagnostics;
using Gallio.Concurrency;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Drivers;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="ITestRunner" /> that runs tests using
    /// a <see cref="ITestDriver" />.  The driver is created when the first package
    /// is loaded and is disposed when the runner is disposed.  Consequently the test
    /// driver may be reused for multiple test runs.
    /// </para>
    /// </summary>
    public class DefaultTestRunner : ITestRunner
    {
        private readonly ITestDriverFactory testDriverFactory;
        private readonly TestRunnerEventDispatcher eventDispatcher;
        private readonly List<ITestRunnerExtension> extensions;

        private ILogger logger;
        private TestRunnerOptions options;

        private State state;
        private LockBox<Report> report;
        private ITestDriver testDriver;

        private enum State
        {
            Created,
            Initialized,
            Loaded,
            Explored,
            RunFinished,
            Disposed
        }

        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="testDriverFactory">The test driver factory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriverFactory"/> is null</exception>
        public DefaultTestRunner(ITestDriverFactory testDriverFactory)
        {
            if (testDriverFactory == null)
                throw new ArgumentNullException("testDriverFactory");

            this.testDriverFactory = testDriverFactory;

            eventDispatcher = new TestRunnerEventDispatcher();
            state = State.Created;
            extensions = new List<ITestRunnerExtension>();

            ResetReport();
        }

        /// <summary>
        /// Gets the logger, or null if the test runner has not been initialized.
        /// </summary>
        protected ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Gets the test runner options, or null if the test runner has not been initialized.
        /// </summary>
        protected TestRunnerOptions Options
        {
            get { return options; }
        }

        /// <inheritdoc />
        public ITestRunnerEvents Events
        {
            get { return eventDispatcher; }
        }

        /// <inheritdoc />
        public LockBox<Report> Report
        {
            get
            {
                lock (this)
                    return report;
            }
            private set
            {
                lock (this)
                    report = value;
            }
        }

        /// <inheritdoc />
        public void RegisterExtension(ITestRunnerExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            ThrowIfDisposed();
            if (state != State.Created)
                throw new InvalidOperationException("Extensions cannot be registered after the test runner has been initialized.");

            extensions.Add(extension);
        }

        /// <inheritdoc />
        public void Initialize(TestRunnerOptions options, ILogger logger, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state != State.Created)
                throw new InvalidOperationException("The test runner has already been initialized.");

            options = options.Copy();

            this.options = options;
            this.logger = logger;

            int extensionCount = extensions.Count;
            using (progressMonitor.BeginTask("Initializing the test runner.", 10 + extensionCount))
            {
                foreach (ITestRunnerExtension extension in extensions)
                {
                    string extensionName = extension.GetType().Name; // TODO: improve me

                    progressMonitor.SetStatus(String.Format("Installing extension '{0}'.", extensionName));
                    try
                    {
                        extension.Install(eventDispatcher, logger);
                        progressMonitor.Worked(1);
                    }
                    catch (Exception ex)
                    {
                        throw new RunnerException(String.Format("Failed to install extension '{0}'.", extensionName), ex);
                    }
                    progressMonitor.SetStatus("");
                }

                try
                {
                    eventDispatcher.NotifyInitializeStarted(new InitializeStartedEventArgs(options));

                    DoInitialize(progressMonitor, 10);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyInitializeFinished(new InitializeFinishedEventArgs(false));
                    throw new RunnerException("Failed to initialize the host.", ex);
                }

                state = State.Initialized;
                eventDispatcher.NotifyInitializeFinished(new InitializeFinishedEventArgs(true));
            }
        }

        /// <inheritdoc />
        public void Load(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state != State.Initialized)
                throw new InvalidOperationException("The previous test package must be unloaded before a new one can be loaded.");

            testPackageConfig = testPackageConfig.Copy();
            testPackageConfig.Canonicalize(null);

            using (progressMonitor.BeginTask("Loading the test package.", 10))
            {
                try
                {
                    eventDispatcher.NotifyLoadStarted(new LoadStartedEventArgs(testPackageConfig));

                    DoLoad(testPackageConfig, progressMonitor, 10);
                }
                catch (Exception ex)
                {
                    Report.Write(report => eventDispatcher.NotifyLoadFinished(new LoadFinishedEventArgs(false, report)));
                    throw new RunnerException("Failed to load the test package.", ex);
                }

                state = State.Loaded;
                Report.Write(report => eventDispatcher.NotifyLoadFinished(new LoadFinishedEventArgs(true, report)));
            }
        }

        /// <inheritdoc />
        public void Explore(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state < State.Loaded)
                throw new InvalidOperationException("A test package must be loaded before tests can be explored.");

            options = options.Copy();

            using (progressMonitor.BeginTask("Exploring the tests.", 10))
            {
                try
                {
                    eventDispatcher.NotifyExploreStarted(new ExploreStartedEventArgs(options));

                    DoExplore(options, progressMonitor, 10);
                }
                catch (Exception ex)
                {
                    Report.Write(report => eventDispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(false, report)));
                    throw new RunnerException("Failed to explore the tests.", ex);
                }

                state = State.Explored;
                Report.Write(report => eventDispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(true, report)));
            }
        }

        /// <inheritdoc />
        public void Run(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            if (options == null)
                throw new ArgumentNullException("options");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state < State.Explored)
                throw new InvalidOperationException("A test package must be loaded and explored before tests can be run.");

            options = options.Copy();

            using (progressMonitor.BeginTask("Running the tests.", 10))
            {
                try
                {
                    eventDispatcher.NotifyRunStarted(new RunStartedEventArgs(options));

                    DoRun(options, progressMonitor, 10);
                }
                catch (Exception ex)
                {
                    Report.Write(report => eventDispatcher.NotifyRunFinished(new RunFinishedEventArgs(false, report)));
                    throw new RunnerException("Failed to run the tests.", ex);
                }

                state = State.RunFinished;
                Report.Write(report => eventDispatcher.NotifyRunFinished(new RunFinishedEventArgs(true, report)));
            }
        }

        /// <inheritdoc />
        public void Unload(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state < State.Initialized)
                throw new InvalidOperationException("The test runner must be initialized in order for a test package to be unloaded.");
            if (state < State.Loaded)
                return;

            using (progressMonitor.BeginTask("Unloading the test package.", 10))
            {
                try
                {
                    eventDispatcher.NotifyUnloadStarted(new UnloadStartedEventArgs());

                    DoUnload(progressMonitor, 10);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyUnloadFinished(new UnloadFinishedEventArgs(false));
                    throw new RunnerException("Failed to unload the test package.", ex);
                }

                state = State.Initialized;
                eventDispatcher.NotifyUnloadFinished(new UnloadFinishedEventArgs(true));
            }
        }

        /// <inheritdoc />
        public void Dispose(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (state == State.Disposed)
                return;

            using (progressMonitor.BeginTask("Disposing the test runner.", 10))
            {
                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(9))
                {
                    if (state == State.Loaded || state == State.Explored || state == State.RunFinished)
                    {
                        try
                        {
                            Unload(subProgressMonitor);
                        }
                        catch
                        {
                            // Ignore unloading problems during disposal.
                            // It's quite likely there are other issues in play also.
                        }
                    }
                }

                bool success;
                try
                {
                    eventDispatcher.NotifyDisposeStarted(new DisposeStartedEventArgs());

                    DoDispose(progressMonitor, 1);
                    success = true;
                }
                catch (Exception ex)
                {
                    if (logger != null)
                        logger.Log(LogSeverity.Warning, "Failed to safely dispose the host.", ex);
                    success = false;
                }

                state = State.Disposed;
                eventDispatcher.NotifyDisposeFinished(new DisposeFinishedEventArgs(success));
            }
        }

        private void DoInitialize(IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.Worked(totalWork);
        }

        private void DoLoad(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor, double totalWork)
        {
            Report = new LockBox<Report>(new Report() { TestPackageConfig = testPackageConfig });

            InitializeTestDriver(progressMonitor, totalWork * 0.1);
            LoadTestDomains(testPackageConfig, progressMonitor, totalWork * 0.9);
        }

        private void InitializeTestDriver(IProgressMonitor progressMonitor, double totalWork)
        {
            if (testDriver == null)
            {
                progressMonitor.SetStatus("Initializing the test driver.");

                // Override the runtime path for development.
                RuntimeSetup runtimeSetup = RuntimeAccessor.Instance.GetRuntimeSetup();
                if (runtimeSetup.InstallationConfiguration.IsDevelopmentRuntimePathValid())
                    runtimeSetup.RuntimePath = runtimeSetup.InstallationConfiguration.DevelopmentRuntimePath;

                // Create test driver.
                testDriver = testDriverFactory.CreateTestDriver();
                testDriver.Initialize(runtimeSetup, logger);
            }

            progressMonitor.Worked(totalWork);
        }

        private void LoadTestDomains(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Loading tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                testDriver.Load(testPackageConfig, subProgressMonitor);
        }

        private void DoExplore(TestExplorationOptions options, IProgressMonitor progressMonitor, double totalWork)
        {
            Report.Read(report =>
            {
                Report = new LockBox<Report>(new Report() { TestPackageConfig = report.TestPackageConfig });
            });

            ExploreTestDomains(options, progressMonitor, totalWork);
        }

        private void ExploreTestDomains(TestExplorationOptions options, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Exploring tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
            {
                Report.Write(report =>
                {
                    report.TestModel = testDriver.Explore(options, subProgressMonitor);
                });
            }
        }

        private void DoRun(TestExecutionOptions options, IProgressMonitor progressMonitor, double totalWork)
        {
            Report.Read(report =>
            {
                Report = new LockBox<Report>(new Report()
                {
                    TestPackageConfig = report.TestPackageConfig,
                    TestModel = report.TestModel,
                    TestPackageRun = new TestPackageRun() { StartTime = DateTime.Now }
                });
            });

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                using (ReportTestListener listener = new ReportTestListener(eventDispatcher, Report))
                {
                    RunTestDomains(options, listener, progressMonitor, totalWork);
                }
            }
            finally
            {
                Report.Write(report =>
                {
                    if (report.TestPackageRun != null)
                    {
                        report.TestPackageRun.EndTime = DateTime.Now;
                        report.TestPackageRun.Statistics.Duration = stopwatch.Elapsed.TotalSeconds;
                    }
                });
            }
        }

        private void RunTestDomains(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Running tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                testDriver.Run(options, listener, subProgressMonitor);
        }

        private void DoUnload(IProgressMonitor progressMonitor, double totalWork)
        {
            ResetReport();

            UnloadTestDomains(progressMonitor, totalWork);
        }

        private void UnloadTestDomains(IProgressMonitor progressMonitor, double totalWork)
        {
            if (testDriver != null)
            {
                progressMonitor.SetStatus("Unloading tests.");

                using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                    testDriver.Unload(subProgressMonitor);
            }
        }

        private void DoDispose(IProgressMonitor progressMonitor, double totalWork)
        {
            DisposeTestDriver(progressMonitor, totalWork);
            ResetReport();
        }

        private void ResetReport()
        {
            Report = new LockBox<Report>(new Report());
        }

        private void DisposeTestDriver(IProgressMonitor progressMonitor, double totalWork)
        {
            if (testDriver != null)
            {
                progressMonitor.SetStatus("Disposing the test driver.");

                testDriver.Dispose();
                testDriver = null;
            }

            progressMonitor.Worked(totalWork);
        }

        private void ThrowIfDisposed()
        {
            if (state == State.Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
