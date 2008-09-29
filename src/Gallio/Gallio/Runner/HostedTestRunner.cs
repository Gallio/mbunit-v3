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
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="ITestRunner" /> that runs tests within
    /// the context of particular <see cref="IHost" />.  The host is created
    /// when the package is loaded and is disposed when the package is unloaded.
    /// Consequently each new package runs in a new host.
    /// </para>
    /// </summary>
    public class HostedTestRunner : ITestRunner
    {
        private readonly IHostFactory hostFactory;
        private readonly ITestFramework[] frameworks;
        private readonly IRuntime runtime;

        private readonly TestRunnerEventDispatcher eventDispatcher;

        private readonly List<ITestRunnerExtension> extensions;

        private ILogger logger;
        private TestRunnerOptions options;

        private State state;
        private IHost host;
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
        /// Creates a hosted test runner.
        /// </summary>
        /// <param name="hostFactory">The host factory</param>
        /// <param name="frameworks">The test frameworks</param>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="frameworks"/> or <paramref name="runtime"/> is null</exception>
        public HostedTestRunner(IHostFactory hostFactory, ITestFramework[] frameworks, IRuntime runtime)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (frameworks == null)
                throw new ArgumentNullException("frameworks");
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.hostFactory = hostFactory;
            this.frameworks = frameworks;
            this.runtime = runtime;

            eventDispatcher = new TestRunnerEventDispatcher();
            state = State.Created;
            extensions = new List<ITestRunnerExtension>();

            ResetReport();
        }

        /// <summary>
        /// Gets the host, or null if no package has been loaded.
        /// </summary>
        protected IHost Host
        {
            get { return host; }
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

                    DoInitialize(progressMonitor);
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

                    DoLoad(testPackageConfig, progressMonitor);
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

                    DoExplore(options, progressMonitor);
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

                    DoRun(options, progressMonitor);
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

                    DoUnload(progressMonitor);
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

                    DoDispose(progressMonitor);
                    success = true;
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("Failed to safely dispose the host.", ex);
                    success = false;
                }

                state = State.Disposed;
                eventDispatcher.NotifyDisposeFinished(new DisposeFinishedEventArgs(success));
            }
        }

        /// <summary>
        /// Provides an opportunity for subclasses to configure the host.
        /// </summary>
        /// <param name="hostSetup">The host setup, not null</param>
        protected virtual void ConfigureHost(HostSetup hostSetup)
        {
        }

        /// <summary>
        /// Provides an opportunity for subclasses to initialize the host once created.
        /// </summary>
        /// <param name="host">The host, not null</param>
        protected virtual void InitializeHost(IHost host)
        {
        }

        private void DoInitialize(IProgressMonitor progressMonitor)
        {
            progressMonitor.Worked(10);
        }

        private void DoLoad(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            Report = new LockBox<Report>(new Report() { TestPackageConfig = testPackageConfig });

            InitializeHost(testPackageConfig.HostSetup, progressMonitor, 5);
            LoadTestDomains(testPackageConfig, progressMonitor, 5);
        }

        private void InitializeHost(HostSetup packageHostSetup, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Initializing the host environment.");

            // Override the runtime path for development.
            RuntimeSetup runtimeSetup = RuntimeAccessor.Instance.GetRuntimeSetup();
            if (runtimeSetup.InstallationConfiguration.IsDevelopmentRuntimePathValid())
                runtimeSetup.RuntimePath = runtimeSetup.InstallationConfiguration.DevelopmentRuntimePath;

            // Configure the host's primary parameters.
            HostSetup hostSetup = new HostSetup();
            hostSetup.ApplicationBaseDirectory = RuntimeAccessor.RuntimePath;
            hostSetup.WorkingDirectory = packageHostSetup.WorkingDirectory;
            hostSetup.ShadowCopy = packageHostSetup.ShadowCopy;

            ConfigureHost(hostSetup);

            host = hostFactory.CreateHost(hostSetup, logger);

            InitializeHost(host);

            testDriver = new MultiDomainTestDriver(frameworks, host, runtime);
            testDriver.Initialize(runtimeSetup, logger);

            progressMonitor.Worked(totalWork);
        }

        private void LoadTestDomains(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Loading tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                testDriver.Load(testPackageConfig, subProgressMonitor);
        }

        private void DoExplore(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            Report.Read(report =>
            {
                Report = new LockBox<Report>(new Report() { TestPackageConfig = report.TestPackageConfig });
            });

            ExploreTestDomains(options, progressMonitor, 10);
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

        private void DoRun(TestExecutionOptions options, IProgressMonitor progressMonitor)
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
                    RunTestDomains(options, listener, progressMonitor, 10);
                }
            }
            finally
            {
                Report.Write(report =>
                {
                    report.TestPackageRun.EndTime = DateTime.Now;
                    report.TestPackageRun.Statistics.Duration = stopwatch.Elapsed.TotalSeconds;
                });
            }
        }

        private void RunTestDomains(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Running tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                testDriver.Run(options, listener, subProgressMonitor);
        }

        private void DoUnload(IProgressMonitor progressMonitor)
        {
            ResetReport();

            try
            {
                UnloadTestDomains(progressMonitor, 5);
            }
            finally
            {
                DisposeHost(progressMonitor, 5);
            }
        }

        private void UnloadTestDomains(IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Unloading tests.");

            using (IProgressMonitor subProgressMonitor = progressMonitor.CreateSubProgressMonitor(totalWork))
                testDriver.Unload(subProgressMonitor);
        }

        private void DisposeHost(IProgressMonitor progressMonitor, double totalWork)
        {
            progressMonitor.SetStatus("Shutting down the host environment.");

            if (testDriver != null)
            {
                testDriver.Dispose();
                testDriver = null;
            }

            if (host != null)
            {
                host.Dispose();
                host = null;
            }

            progressMonitor.Worked(totalWork);
        }

        private void DoDispose(IProgressMonitor progressMonitor)
        {
            ResetReport();
        }

        private void ResetReport()
        {
            Report = new LockBox<Report>(new Report());
        }

        private void ThrowIfDisposed()
        {
            if (state == State.Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
