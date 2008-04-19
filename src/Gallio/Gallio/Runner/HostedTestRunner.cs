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
using System.Diagnostics;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Domains;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Harness;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner
{
    /// <summary>
    /// An implementation of <see cref="ITestRunner" /> that runs tests within
    /// the context of particular <see cref="IHost" />.  The host is created
    /// when the package is loaded and is disposed when the package is unloaded.
    /// Consequently each new package runs in a new host.
    /// </summary>
    public class HostedTestRunner : ITestRunner
    {
        private readonly IHostFactory hostFactory;

        private readonly TestRunnerEventDispatcher eventDispatcher;
        private readonly object syncRoot;

        private readonly List<ITestRunnerExtension> extensions;

        private ILogger logger;
        private TestRunnerOptions options;

        private State state;
        private IHost host;
        private Report report;

        private ITestDomain domain;

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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/> is null</exception>
        public HostedTestRunner(IHostFactory hostFactory)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");

            this.hostFactory = hostFactory;

            eventDispatcher = new TestRunnerEventDispatcher();
            syncRoot = new object();
            state = State.Created;
            extensions = new List<ITestRunnerExtension>();

            report = new Report();
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
        public Report Report
        {
            get { return report; }
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

            using (progressMonitor)
            {

                int extensionCount = extensions.Count;
                progressMonitor.BeginTask("Initalizing the test runner.", 10 + extensionCount);

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

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading the test package.", 10);

                try
                {
                    eventDispatcher.NotifyLoadStarted(new LoadStartedEventArgs(testPackageConfig));

                    DoLoad(testPackageConfig, progressMonitor);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyLoadFinished(new LoadFinishedEventArgs(false, report));
                    throw new RunnerException("Failed to load the test package.", ex);
                }

                state = State.Loaded;
                eventDispatcher.NotifyLoadFinished(new LoadFinishedEventArgs(true, report));
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

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Exploring the tests.", 10);

                try
                {
                    eventDispatcher.NotifyExploreStarted(new ExploreStartedEventArgs(options));

                    DoExplore(options, progressMonitor);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(false, report));
                    throw new RunnerException("Failed to explore the tests.", ex);
                }

                state = State.Explored;
                eventDispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(true, report));
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

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running the tests.", 10);

                try
                {
                    eventDispatcher.NotifyRunStarted(new RunStartedEventArgs(options));

                    DoRun(options, progressMonitor);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyRunFinished(new RunFinishedEventArgs(false, report));
                    throw new RunnerException("Failed to run the tests.", ex);
                }

                state = State.RunFinished;
                eventDispatcher.NotifyRunFinished(new RunFinishedEventArgs(true, report));
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

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Unloading the test package.", 10);

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

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Disposing the test runner.", 10);

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

        private void DoInitialize(IProgressMonitor progressMonitor)
        {
            progressMonitor.Worked(10);
        }

        private void DoLoad(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            report = new Report();
            report.TestPackageConfig = testPackageConfig;

            InitializeHost(5, progressMonitor);
            LoadTestDomains(testPackageConfig, 5, progressMonitor);
        }

        private void InitializeHost(double totalWork, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Initializing the host environment.");

            HostSetup hostSetup = new HostSetup();
            hostSetup.WorkingDirectory = RuntimeAccessor.InstallationPath;
            hostSetup.ApplicationBaseDirectory = RuntimeAccessor.InstallationPath;
            hostSetup.ShadowCopy = false;
            hostSetup.Configuration.LegacyUnhandledExceptionPolicyEnabled = true;
            hostSetup.Configuration.AssertUiEnabled = false;
            hostSetup.Configuration.RemotingCustomErrorsEnabled = false;

            host = hostFactory.CreateHost(hostSetup, logger);

            progressMonitor.Worked(totalWork);
        }

        private void LoadTestDomains(TestPackageConfig testPackageConfig, double totalWork, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Creating test domains.");

            // HACK: Temporary workaround until we refactor ITestDomain.
            ITestHarnessFactory harnessFactory = RuntimeAccessor.Instance.Resolve<ITestHarnessFactory>();
            domain = new LocalTestDomain(harnessFactory);
            domain.Load(testPackageConfig, progressMonitor.CreateSubProgressMonitor(totalWork));
        }

        private void DoExplore(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            Report oldReport = report;
            report = new Report();
            report.TestPackageConfig = oldReport.TestPackageConfig;

            ExploreTestDomains(options, 10, progressMonitor);
        }

        private void ExploreTestDomains(TestExplorationOptions options, double totalWork, IProgressMonitor progressMonitor)
        {
            domain.Explore(options, progressMonitor.CreateSubProgressMonitor(totalWork));

            report.TestModel = domain.TestModelData;
        }

        private void DoRun(TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            Report oldReport = report;
            report = new Report();
            report.TestPackageConfig = oldReport.TestPackageConfig;
            report.TestModel = oldReport.TestModel;
            report.TestPackageRun = new TestPackageRun();

            Stopwatch stopwatch = Stopwatch.StartNew();
            report.TestPackageRun = new TestPackageRun();
            report.TestPackageRun.StartTime = DateTime.Now;
            try
            {
                using (HostedTestListener listener = new HostedTestListener(eventDispatcher, report, syncRoot))
                {
                    RunTestDomains(options, listener, 10, progressMonitor);
                }
            }
            finally
            {
                report.TestPackageRun.EndTime = DateTime.Now;
                report.TestPackageRun.Statistics.Duration = stopwatch.Elapsed.TotalSeconds;
            }
        }

        private void RunTestDomains(TestExecutionOptions options, ITestListener listener, double totalWork, IProgressMonitor progressMonitor)
        {
            domain.Run(options, listener, progressMonitor.CreateSubProgressMonitor(totalWork));
        }

        private void DoUnload(IProgressMonitor progressMonitor)
        {
            report = new Report();

            try
            {
                UnloadTestDomains(5, progressMonitor);
            }
            finally
            {
                DisposeHost(5, progressMonitor);
            }
        }

        private void UnloadTestDomains(double totalWork, IProgressMonitor progressMonitor)
        {
            domain.Unload(progressMonitor.CreateSubProgressMonitor(totalWork));
            domain = null;
        }

        private void DisposeHost(double totalWork, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Shutting down the host environment.");

            if (host != null)
            {
                host.Dispose();
                host = null;
            }

            progressMonitor.Worked(totalWork);
        }

        private void DoDispose(IProgressMonitor progressMonitor)
        {
            report = new Report();
        }

        private void ThrowIfDisposed()
        {
            if (state == State.Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
