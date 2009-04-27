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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Gallio.Model.Execution;
using Gallio.Runner.Extensions;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Runner.Reports;
using System.Threading;

namespace Gallio.Runner
{
    /// <summary>
    /// <para>
    /// The test launcher encapsulated the entire test execution lifecycle from
    /// start to finish and provides a simplified pattern for running tests.
    /// </para>
    /// <para>
    /// The basic usage pattern is as follows:
    /// <list type="numbered">
    /// <item>Create the launcher.</item>
    /// <item>Set properties to specify the inputs and outputs of the test run.</item>
    /// <item>Run the tests all in one go.</item>
    /// <item>Optionally do something with the contents of the final report.</item>
    /// </list>
    /// </para>
    /// <para>
    /// By default, the launcher assumes that a runtime environment has already been
    /// established and is accessible via the <see cref="RuntimeAccessor" />.  If there
    /// is no runtime yet, then you can cause one to be configured automatically for the
    /// duration of the test run by setting the <see cref="RuntimeSetup"/> property accordingly.
    /// </para>
    /// <para>
    /// You can override the default <see cref="ITestRunner" /> that is created
    /// by setting the <see cref="TestRunnerFactoryName" /> property.
    /// </para>
    /// </summary>
    public class TestLauncher
    {
        #region Private Members

        private RuntimeSetup runtimeSetup;

        private string testRunnerFactoryName;
        private TestRunnerOptions testRunnerOptions;
        private TestPackageConfig testPackageConfig;
        private TestExplorationOptions testExplorationOptions;
        private TestExecutionOptions testExecutionOptions;

        private readonly List<string> reportFormats;
        private ReportFormatterOptions reportFormatterOptions;

        private IProgressMonitorProvider progressMonitorProvider;
        private ILogger logger;

        private bool echoResults;
        private bool doNotRun;
        private bool ignoreAnnotations;
        private TimeSpan? runTimeLimit;

        private string reportDirectory;
        private string reportNameFormat;

        private readonly List<ITestRunnerExtension> extensions;
        private readonly List<string> extensionSpecifications;

        private bool showReports;

        private readonly object cancelationSyncRoot = new object();
        private bool isCanceled;
        private IProgressMonitor cancelableProgressMonitor;

        #endregion

        /// <summary>
        /// Creates a launcher with default options and no test assemblies specified.
        /// </summary>
        public TestLauncher()
        {
            testRunnerFactoryName = StandardTestRunnerFactoryNames.IsolatedProcess;
            testRunnerOptions = new TestRunnerOptions();
            testPackageConfig = new TestPackageConfig();
            testExplorationOptions = new TestExplorationOptions();
            testExecutionOptions = new TestExecutionOptions();

            reportDirectory = @"";
            reportNameFormat = @"test-report-{0}-{1}";
            reportFormats = new List<string>();
            reportFormatterOptions = new ReportFormatterOptions();

            progressMonitorProvider = NullProgressMonitorProvider.Instance;
            logger = NullLogger.Instance;

            extensions = new List<ITestRunnerExtension>();
            extensionSpecifications = new List<string>();
        }

        /// <summary>
        /// <para>
        /// Gets or sets the progress monitor provider to use.
        /// </para>
        /// <para>
        /// The default provider is <see cref="NullProgressMonitorProvider.Instance" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IProgressMonitorProvider ProgressMonitorProvider
        {
            get { return progressMonitorProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                progressMonitorProvider = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the logger to use.
        /// </para>
        /// <para>
        /// The default logger is <see cref="NullLogger.Instance" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public ILogger Logger
        {
            get { return logger; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                logger = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the <see cref="RuntimeSetup" /> to use for automatically initializing
        /// the runtime during test execution or <c>null</c> if the runtime is already initialized.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value if not <c>null</c> then the launcher will initialize the runtime
        /// using this <see cref="RuntimeSetup" /> (unless already initialized) just prior to
        /// test execution and will automatically shut down the runtime just afterwards.
        /// </para>
        /// <para>
        /// The default value is <c>null</c> which assumes that the runtime is already initialized.
        /// </para>
        /// </remarks>
        public RuntimeSetup RuntimeSetup
        {
            get { return runtimeSetup; }
            set { runtimeSetup = value; }
        }

        /// <summary>
        /// <para>
        /// Specifies the name of a <see cref="ITestRunnerFactory" /> to use for constructing
        /// the <see cref="ITestRunner" /> at test execution time.
        /// </para>
        /// <para>
        /// The default value is <see cref="StandardTestRunnerFactoryNames.IsolatedProcess"/>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string TestRunnerFactoryName
        {
            get { return testRunnerFactoryName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testRunnerFactoryName = value;
            }
        }

        /// <summary>
        /// Gets or sets the test runner options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestRunnerOptions TestRunnerOptions
        {
            get { return testRunnerOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                testRunnerOptions = value;
            }
        }

        /// <summary>
        /// Gets a mutable list of test runner extensions to register with
        /// the test runner during test execution.
        /// </summary>
        public IList<ITestRunnerExtension> TestRunnerExtensions
        {
            get { return extensions; }
        }

        /// <summary>
        /// Gets a mutable list of test runner extension specifications to instantiate
        /// and register with the test runner during test execution.
        /// </summary>
        /// <seealso cref="TestRunnerExtensionUtils.CreateExtensionFromSpecification"/>
        /// for an explanation of the specification syntax.
        public IList<string> TestRunnerExtensionSpecifications
        {
            get { return extensionSpecifications; }
        }

        /// <summary>
        /// Gets or sets the test package.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testPackageConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the test exploration options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestExplorationOptions TestExplorationOptions
        {
            get { return testExplorationOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testExplorationOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets the test execution options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testExecutionOptions = value;
            }
        }

        /// <summary>
        /// <para>
        /// Controls whether the test runner will echo result to the <see cref="Logger" />
        /// as each test finishes.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool EchoResults
        {
            get { return echoResults; }
            set { echoResults = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to skip test execution.
        /// This option may be used to produce a report that contains test
        /// metadata for consumption by other tools.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool DoNotRun
        {
            get { return doNotRun; }
            set { doNotRun = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the path of the directory to which reports will be written.
        /// </para>
        /// <para>
        /// The default value is <c>""</c> which causes reports to be written to the current directory.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                reportDirectory = value;
            }
        }

        /// <summary>
        /// <para>
        /// </para>
        /// Gets or sets a format string used to construct the name of report files (without the extension).
        /// Within the format string, <c>{0}</c> is replaced by the date and <c>{1}</c> by the time.
        /// <para>
        /// The default value is <c>"test-report-{0}-{1}"</c>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                reportNameFormat = value; 
            }
        }

        /// <summary>
        /// Gets the mutable list of report formats to generate.
        /// </summary>
        public IList<string> ReportFormats
        {
            get { return reportFormats; }
        }

        /// <summary>
        /// Gets or sets the report formatter options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public ReportFormatterOptions ReportFormatterOptions
        {
            get { return reportFormatterOptions; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                reportFormatterOptions = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to show the reports after the test run finishes.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool ShowReports
        {
            get { return showReports; }
            set { showReports = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets whether to ignore annotations when determining the result code.
        /// If false, then error annotations, usually indicative of broken tests, will cause
        /// a failure result to be generated.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </summary>
        public bool IgnoreAnnotations
        {
            get { return ignoreAnnotations; }
            set { ignoreAnnotations = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the maximum amount of time the tests can run before they are canceled.
        /// </para>
        /// <para>The default value is <c>null</c>, meaning an infinite time.</para>
        /// </summary>
        public TimeSpan? RunTimeLimit
        {
            get { return runTimeLimit; }
            set { runTimeLimit = value; }
        }

        #region Public Methods

        /// <summary>
        /// Cancels the test run and prevents a new one from starting.
        /// </summary>
        public void Cancel()
        {
            lock (cancelationSyncRoot)
            {
                isCanceled = true;

                if (cancelableProgressMonitor != null)
                    cancelableProgressMonitor.Cancel();
            }
        }

        /// <summary>
        /// <para>
        /// Runs the test package as configured.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <see cref="RuntimeSetup" /> is non-<c>null</c>,
        /// initializes the runtime for the duration of this method then shuts it down automatically
        /// before returning.  Otherwise assumes the runtime has already been initialized and
        /// accesses it using <see cref="RuntimeAccessor" />.
        /// </para>
        /// <para>
        /// Runtimes cannot be nested.  So if the launcher is asked to initialize the runtime and
        /// it has already been initialized, then it will not be initialized again.
        /// </para>
        /// </remarks>
        /// <returns>A result object</returns>
        public TestLauncherResult Run()
        {
            Canonicalize(null);
            DisplayConfiguration();

            Timer runTimeTimer = null;
            if (runTimeLimit != null)
            {                
                runTimeTimer = new Timer(delegate
                {
                    Cancel();
                    logger.Log(LogSeverity.Warning, "Run time limit reached! Canceled test run.");
                }, null, (int)runTimeLimit.Value.TotalMilliseconds, Timeout.Infinite);
            }

            Stopwatch stopWatch = Stopwatch.StartNew();
            logger.Log(LogSeverity.Important, String.Format("Start time: {0}", DateTime.Now.ToShortTimeString()));
            bool wasCanceled = false;
            try
            {
                VerifyAssemblies(ref wasCanceled);
                if (wasCanceled)
                    return CreateResult(ResultCode.Canceled);

                if (testPackageConfig.AssemblyFiles.Count == 0)
                {
                    logger.Log(LogSeverity.Warning, "No test assemblies to execute!");
                    return CreateResult(ResultCode.NoTests);
                }

                using (InitializeRuntimeIfNeeded())
                    return RunWithRuntime();
            }
            finally
            {
                logger.Log(LogSeverity.Important,
                    String.Format("Stop time: {0} (Total execution time: {1:#0.000} seconds)",
                        DateTime.Now.ToShortTimeString(),
                        stopWatch.Elapsed.TotalSeconds));                

                if (runTimeTimer != null)
                    runTimeTimer.Dispose();
            }
        }

        private IDisposable InitializeRuntimeIfNeeded()
        {
            if (runtimeSetup != null && ! RuntimeAccessor.IsInitialized)
            {
                logger.Log(LogSeverity.Important, "Initializing the runtime and loading plugins.");
                return RuntimeBootstrap.Initialize(runtimeSetup, logger);
            }

            return null;
        }

        private TestLauncherResult RunWithRuntime()
        {
            IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
            if (!ValidateReportFormats(reportManager))
                return CreateResult(ResultCode.InvalidArguments);

            ITestRunnerManager manager = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>();
            ITestRunnerFactory factory = manager.GetFactory(testRunnerFactoryName);
            if (factory == null)
            {
                logger.Log(LogSeverity.Error, String.Format("Unrecognized test runner factory name: '{0}'.", testRunnerFactoryName));
                return CreateResult(ResultCode.InvalidArguments);
            }

            ITestRunner runner = factory.CreateTestRunner();
            TestLauncherResult result = new TestLauncherResult(new Report() { TestPackageConfig = TestPackageConfig });
            bool wasCanceled = false;
            try
            {
                DoRegisterExtensions(runner);
                DoInitialize(runner, ref wasCanceled);

                if (!wasCanceled)
                {
                    result = RunWithInitializedRunner(runner, reportManager);
                }
            }
            finally
            {
                DoDispose(runner, ref wasCanceled);
            }

            if (wasCanceled)
                result.SetResultCode(ResultCode.Canceled);

            return result;
        }

        private TestLauncherResult RunWithInitializedRunner(ITestRunner runner, IReportManager reportManager)
        {
            bool wasCanceled = false;

            // Explore or Run tests.
            Report report = DoExploreOrRun(runner, ref wasCanceled);

            if (report == null)
                report = new Report();
            var result = new TestLauncherResult(report);

            // Generate reports even if the test run is canceled, unless this step
            // also gets canceled.
            if (result.Report.TestPackageRun != null)
                GenerateReports(result, reportManager, ref wasCanceled);

            // Done.
            if (showReports)
                ShowReportDocuments(result);

            // Produce the final result code.
            if (wasCanceled)
            {
                result.SetResultCode(ResultCode.Canceled);
            }
            else
            {
                if (! ignoreAnnotations
                    && result.Report.TestModel != null
                    && result.Report.TestModel.GetErrorAnnotationCount() != 0)
                    result.SetResultCode(ResultCode.Failure);
                else if (result.Report.TestPackageRun != null
                    && result.Statistics.FailedCount > 0)
                    result.SetResultCode(ResultCode.Failure);
                else if (result.Report.TestPackageRun != null
                    && result.Statistics.TestCount == 0)
                    result.SetResultCode(ResultCode.NoTests);
            }

            return result;
        }

        #endregion

        #region Private Methods

        private void GenerateReports(TestLauncherResult result, IReportManager reportManager,
            ref bool canceled)
        {
            if (reportFormats.Count == 0)
                return;

            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                result.GenerateReports(reportDirectory, GenerateReportName(result.Report), reportFormats,
                    reportFormatterOptions, reportManager, progressMonitor);
            }, ref canceled);
        }

        private void ShowReportDocuments(TestLauncherResult result)
        {
            if (result.ReportDocumentPaths.Count != 0)
            {
                logger.Log(LogSeverity.Important, "Displaying reports.");

                if (! result.ShowReportDocuments())
                    logger.Log(LogSeverity.Important, "There was an error opening the report documents.");
            }
        }

        private bool ValidateReportFormats(IReportManager reportManager)
        {
            foreach (string reportFormat in reportFormats)
            {
                IReportFormatter formatter = reportManager.FormatterResolver.Resolve(reportFormat);
                if (formatter == null)
                {
                    logger.Log(LogSeverity.Error, String.Format("Unrecognized report format: '{0}'.", reportFormat));
                    return false;
                }
            }

            if (reportNameFormat.Length == 0)
            {
                logger.Log(LogSeverity.Error, "Report name format must not be empty.");
                return false;
            }

            return true;
        }

        private void DoRegisterExtensions(ITestRunner runner)
        {
            if (echoResults)
                runner.RegisterExtension(new LogExtension());

            foreach (ITestRunnerExtension extension in extensions)
                runner.RegisterExtension(extension);

            foreach (string extensionSpecification in extensionSpecifications)
                runner.RegisterExtension(TestRunnerExtensionUtils.CreateExtensionFromSpecification(extensionSpecification));
        }

        /// <summary>
        /// Removes any non-existing assemblies from the list of test assemblies.
        /// </summary>
        private void VerifyAssemblies(ref bool canceled)
        {
            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                using (progressMonitor.BeginTask("Verifying assembly names.", 1))
                {
                    List<string> assembliesToRemove = new List<string>();
                    foreach (string assemblyName in testPackageConfig.AssemblyFiles)
                    {
                        if (!File.Exists(assemblyName) || assemblyName != assemblyName.TrimEnd('\\', '/'))
                        {
                            assembliesToRemove.Add(assemblyName);
                            logger.Log(LogSeverity.Error, String.Format("Cannot find assembly: {0}", assemblyName));
                        }
                    }

                    // Remove invalid assemblies
                    foreach (string assemblyName in assembliesToRemove)
                        testPackageConfig.AssemblyFiles.Remove(assemblyName);
                }
            }, ref canceled);
        }

        private void DisplayConfiguration()
        {
            DisplayPaths(testPackageConfig.AssemblyFiles, "Test Assemblies:");
            DisplayPaths(testPackageConfig.HintDirectories, "Hint Directories:");

            if (runtimeSetup != null)
                DisplayPaths(runtimeSetup.PluginDirectories, "Plugin Directories:");
        }

        private void DisplayPaths(ICollection<string> paths, string name)
        {
            if (paths != null && paths.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.Append(name);

                foreach (string path in paths)
                    message.Append("\n\t").Append(path);
                message.AppendLine();

                logger.Log(LogSeverity.Info, message.ToString());
            }
        }

        private void Canonicalize(string baseDirectory)
        {
            testPackageConfig.HostSetup.Canonicalize(baseDirectory);

            if (runtimeSetup != null)
                runtimeSetup.Canonicalize(baseDirectory);
        }

        private void DoInitialize(ITestRunner runner, ref bool canceled)
        {
            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                runner.Initialize(testRunnerOptions, logger, progressMonitor);
            }, ref canceled);
        }

        private Report DoExploreOrRun(ITestRunner runner, ref bool canceled)
        {
            Report report = null;
            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                if (doNotRun)
                    report = runner.Explore(testPackageConfig, testExplorationOptions, progressMonitor);
                else
                    report = runner.Run(testPackageConfig, testExplorationOptions, testExecutionOptions, progressMonitor);
            }, ref canceled);

            return report;
        }

        private void DoDispose(ITestRunner runner, ref bool canceled)
        {
            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                runner.Dispose(progressMonitor);
            }, ref canceled);
        }

        private string GenerateReportName(Report report)
        {
            DateTime reportTime = report.TestPackageRun != null ? report.TestPackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        private TestLauncherResult CreateResult(int resultCode)
        {
            Report report = new Report();
            report.TestPackageConfig = testPackageConfig;

            TestLauncherResult result = new TestLauncherResult(report);
            result.SetResultCode(resultCode);
            return result;
        }

        private void RunWithProgress(TaskWithProgress task, ref bool canceled)
        {
            try
            {
                progressMonitorProvider.Run(progressMonitor =>
                {
                    try
                    {
                        bool wasCanceled;
                        lock (cancelationSyncRoot)
                        {
                            wasCanceled = isCanceled;
                            cancelableProgressMonitor = progressMonitor;
                        }

                        if (wasCanceled)
                        {
                            progressMonitor.Cancel();
                        }
                        else
                        {
                            task(progressMonitor);
                        }
                    }
                    finally
                    {
                        lock (cancelationSyncRoot)
                            cancelableProgressMonitor = null;
                    }
                });
            }
            catch (OperationCanceledException)
            {
                canceled = true;
            }
        }

        #endregion
    }
}
