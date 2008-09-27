using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Gallio.Concurrency;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Caching;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// A local in-memory test run implementation.
    /// </summary>
    public class LocalTestRun : ITestRun
    {
        private static readonly TemporaryDiskCache diskCache = new TemporaryDiskCache();
        private const string HtmlReportFileName = "Report.html";
        private const string ReportBaseName = "Report";

        private readonly ITestRunnerFactory runnerFactory;
        private readonly IProgressMonitorProvider progressMonitorProvider;
        private readonly IReportManager reportManager;
        private readonly TestPackageConfig packageConfig;
        private readonly TestRunnerOptions runnerOptions;
        private readonly TestExplorationOptions explorationOptions;
        private readonly TestExecutionOptions executionOptions;

        private readonly Guid id;
        private readonly List<ITestRunnerExtension> extensions;
        private readonly TestRunnerEventDispatcher runnerEventDispatcher;
        private readonly EventLogger logger;

        private ThreadTask runnerTask;
        private TestRunStatus status;
        private LockBox<Report> report;
        private IProgressMonitor progressMonitor;

        private event EventHandler started;
        private event EventHandler stopped;

        /// <summary>
        /// Creates a test run.
        /// </summary>
        /// <param name="runnerFactory">The test runner factory</param>
        /// <param name="progressMonitorProvider">The progress monitor provider</param>
        /// <param name="reportManager">The report manager</param>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="runnerOptions">The test runner options</param>
        /// <param name="explorationOptions">The test exploration options</param>
        /// <param name="executionOptions">The test execution options</param>
        /// <returns>The new test run</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runnerFactory"/>,
        /// <paramref name="progressMonitorProvider"/>, <paramref name="reportManager"/>,
        /// <paramref name="packageConfig"/>, <paramref name="runnerOptions"/>, <paramref name="explorationOptions"/> or
        /// <paramref name="executionOptions"/> is null</exception>
        public LocalTestRun(ITestRunnerFactory runnerFactory, IProgressMonitorProvider progressMonitorProvider,
            IReportManager reportManager,
            TestPackageConfig packageConfig, TestRunnerOptions runnerOptions,
            TestExplorationOptions explorationOptions, TestExecutionOptions executionOptions)
        {
            if (runnerFactory == null)
                throw new ArgumentNullException("runnerFactory");
            if (progressMonitorProvider == null)
                throw new ArgumentNullException("progressMonitorProvider");
            if (reportManager == null)
                throw new ArgumentNullException("reportManager");
            if (packageConfig == null)
                throw new ArgumentNullException("packageConfig");
            if (runnerOptions == null)
                throw new ArgumentNullException("runnerOptions");
            if (explorationOptions == null)
                throw new ArgumentNullException("explorationOptions");
            if (executionOptions == null)
                throw new ArgumentNullException("executionOptions");

            this.runnerFactory = runnerFactory;
            this.progressMonitorProvider = progressMonitorProvider;
            this.reportManager = reportManager;
            this.packageConfig = packageConfig;
            this.runnerOptions = runnerOptions;
            this.explorationOptions = explorationOptions;
            this.executionOptions = executionOptions;

            id = Guid.NewGuid();
            report = new LockBox<Report>(new Report() { TestPackageConfig = packageConfig });
            extensions = new List<ITestRunnerExtension>();
            runnerEventDispatcher = new TestRunnerEventDispatcher();
            logger = new EventLogger();
        }

        /// <inheritdoc />
        public event EventHandler Started
        {
            add { lock (this) started += value; }
            remove { lock (this) started -= value; }
        }

        /// <inheritdoc />
        public event EventHandler Stopped
        {
            add { lock (this) stopped += value; }
            remove { lock (this) stopped -= value; }
        }

        /// <inheritdoc />
        public event EventHandler<LogMessageEventArgs> LogMessage
        {
            add { lock (this) logger.LogMessage += value; }
            remove { lock (this) logger.LogMessage -= value; }
        }

        /// <inheritdoc />
        public ITestRunnerEvents TestRunnerEvents
        {
            get { return runnerEventDispatcher; }
        }

        /// <inheritdoc />
        public Guid Id
        {
            get { return id; }
        }

        /// <inheritdoc />
        public TestRunStatus Status
        {
            get
            {
                lock (this)
                    return status;
            }
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
        public void AddExtension(ITestRunnerExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            lock (this)
            {
                if (status != TestRunStatus.Pending)
                    throw new InvalidOperationException("An extension cannot be added at this time because the test run has already started.");
                extensions.Add(extension);
            }
        }

        /// <inheritdoc />
        public FileInfo GetHtmlFormattedReport(bool condensed)
        {
            IDiskCacheGroup group = GetReportCacheGroup();
            string directory = condensed ? "Condensed" : "Full";

            FileInfo htmlReportFile = group.GetFileInfo(Path.Combine(directory, HtmlReportFileName));
            if (!htmlReportFile.Exists)
            {
                group.CreateSubdirectory(directory);
                FileSystemReportContainer reportContainer = new FileSystemReportContainer(htmlReportFile.DirectoryName, ReportBaseName);
                Report.Read(report =>
                {
                    IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);
                    NameValueCollection options = new NameValueCollection();
                    reportManager.Format(reportWriter, condensed ? "Html-Condensed" : "Html", options,
                        NullProgressMonitor.CreateInstance());
                });
            }

            return htmlReportFile;
        }

        /// <inheritdoc />
        public void Start()
        {
            lock (this)
            {
                if (runnerTask == null)
                {
                    status = TestRunStatus.Running;

                    runnerTask = new ThreadTask("Test run", (Action) Run);
                    runnerTask.Terminated += HandleStopped;
                    runnerTask.Start();
                }
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            lock (this)
            {
                if (progressMonitor != null)
                    progressMonitor.Cancel();
            }
        }

        /// <inheritdoc />
        public bool WaitForCompletion(TimeSpan? timeSpan)
        {
            ThreadTask task;
            lock (this)
                task = runnerTask;

            return task.Join(timeSpan);
        }

        private void HandleStopped(object sender, EventArgs e)
        {
            lock (this)
            {
                if (runnerTask != null)
                {
                    if (runnerTask.Result.Exception != null)
                    {
                        logger.Log(LogSeverity.Error, "The test run task failed with a fatal exception.",
                            runnerTask.Result.Exception);
                        status = TestRunStatus.Error;
                    }
                    else if (status == TestRunStatus.Running)
                    {
                        logger.Log(LogSeverity.Error, "The test run task failed to update its status.");
                        status = TestRunStatus.Error;
                    }

                    runnerTask = null;
                    progressMonitor = null;
                }
            }
        }

        private void Run()
        {
            progressMonitorProvider.Run(progressMonitor =>
            {
                EventHandler startedHandler;
                lock (this)
                {
                    this.progressMonitor = progressMonitor;
                    startedHandler = started;
                }

                progressMonitor.BeginTask("Running tests.", 10);

                EventHandlerUtils.SafeInvoke(startedHandler, this, new EventArgs());

                ITestRunner runner = runnerFactory.CreateTestRunner();
                foreach (ITestRunnerExtension extension in extensions)
                    runner.RegisterExtension(extension);

                runnerEventDispatcher.SubscribeTo(runner.Events);
                runner.Events.LoadFinished += delegate { Report = runner.Report; };
                runner.Events.ExploreFinished += delegate { Report = runner.Report; };
                runner.Events.RunStarted += delegate { Report = runner.Report; };
                runner.Events.TestStepFinished += delegate { Report = runner.Report; };
                runner.Events.RunFinished += delegate { Report = runner.Report; };

                try
                {
                    runner.Initialize(runnerOptions, logger, progressMonitor.CreateSubProgressMonitor(1));
                    try
                    {
                        runner.Load(packageConfig, progressMonitor.CreateSubProgressMonitor(1));

                        lock (this)
                            report = runner.Report;

                        runner.Explore(explorationOptions, progressMonitor.CreateSubProgressMonitor(1));

                        lock (this)
                            report = runner.Report;

                        runner.Run(executionOptions, progressMonitor.CreateSubProgressMonitor(1));
                        runner.Unload(progressMonitor.CreateSubProgressMonitor(1));
                    }
                    finally
                    {
                        runner.Dispose(progressMonitor.CreateSubProgressMonitor(1));
                    }
                }
                finally
                {
                    EventHandler stoppedHandler;
                    lock (this)
                    {
                        if (progressMonitor.IsCanceled && status == TestRunStatus.Running)
                            status = TestRunStatus.Canceled;

                        this.progressMonitor = null;
                        stoppedHandler = stopped;
                    }

                    EventHandlerUtils.SafeInvoke(stoppedHandler, this, new EventArgs());
                }
            });
        }

        private IDiskCacheGroup GetReportCacheGroup()
        {
            return diskCache.Groups["Gallio.Runner.Sessions.LocalTestRun:" + id];
        }
    }
}
