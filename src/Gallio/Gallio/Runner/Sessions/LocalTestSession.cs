using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// An in-memory test session.
    /// </summary>
    internal class LocalTestSession : ITestSession
    {
        private readonly LocalTestSessionManager manager;
        private readonly ITestRunnerFactory runnerFactory;
        private readonly IProgressMonitorProvider progressMonitorProvider;
        private readonly IReportManager reportManager;

        private readonly Guid id;
        private bool isClosed;
        private ITestRun currentRun;

        /// <summary>
        /// Creates a test session.
        /// </summary>
        /// <param name="manager">The test session manager</param>
        /// <param name="runnerFactory">The test runner factory</param>
        /// <param name="progressMonitorProvider">The progress monitor provider to use
        /// for reporting progress of long running operations</param>
        /// <param name="reportManager">The report manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="manager"/>,
        /// <paramref name="runnerFactory"/>, <paramref name="progressMonitorProvider"/>
        /// or <paramref name="reportManager"/> is null</exception>
        internal LocalTestSession(LocalTestSessionManager manager,
            ITestRunnerFactory runnerFactory, IProgressMonitorProvider progressMonitorProvider,
            IReportManager reportManager)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (runnerFactory == null)
                throw new ArgumentNullException("runnerFactory");
            if (progressMonitorProvider == null)
                throw new ArgumentNullException("progressMonitorProvider");
            if (reportManager == null)
                throw new ArgumentNullException("reportManager");

            this.manager = manager;
            this.runnerFactory = runnerFactory;
            this.progressMonitorProvider = progressMonitorProvider;
            this.reportManager = reportManager;

            id = Guid.NewGuid();
        }

        /// <inheritdoc />
        public event EventHandler<TestRunEventArgs> TestRunChanged;

        /// <inheritdoc />
        public ITestSessionManager Manager
        {
            get { return manager; }
        }

        /// <inheritdoc />
        public Guid Id
        {
            get { return id; }
        }

        /// <inheritdoc />
        public bool IsOpen
        {
            get { return ! isClosed; }
        }

        /// <inheritdoc />
        public ITestRun CurrentTestRun
        {
            get
            {
                lock (this)
                    return currentRun;
            }
            private set
            {
                lock (this)
                    currentRun = value;
                EventHandlerUtils.SafeInvoke(TestRunChanged, this, new TestRunEventArgs(value));
            }
        }

        /// <inheritdoc />
        public ITestRunHistory TestRunHistory
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <inheritdoc />
        public ITestRun CreateRun(TestPackageConfig packageConfig, TestRunnerOptions runnerOptions,
            TestExplorationOptions explorationOptions, TestExecutionOptions executionOptions)
        {
            ThrowIfSessionClosed();

            ITestRun run = new LocalTestRun(runnerFactory, progressMonitorProvider, reportManager,
                packageConfig, runnerOptions, explorationOptions, executionOptions);
            CurrentTestRun = run;
            return run;
        }

        /// <summary>
        /// Closes the test session.
        /// </summary>
        public void Close()
        {
            isClosed = true;
        }

        private void ThrowIfSessionClosed()
        {
            if (isClosed)
                throw new InvalidOperationException("This operation cannot be performed because the test session has been closed.");
        }
    }
}
