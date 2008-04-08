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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Castle.Core.Logging;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;

namespace Gallio.Runner
{
    /// <summary>
    /// <para>
    /// The test launcher encapsulated the entire test execution lifecycle from
    /// start to finish and provides a simplified pattern for running tests.
    /// </para>
    /// <para>
    /// The lifecycle is as follows:
    /// <list type="numbered">
    /// <item>Create the launcher.</item>
    /// <item>Set properties to specify the inputs and outputs of the test run.</item>
    /// <item>Run the tests all in one go.</item>
    /// <item>Optionally do something with the contents of the final report.</item>
    /// </list>
    /// </para>
    /// <para>
    /// By default, a new <see cref="Runtime" /> environment is established just before
    /// test execution begins and is disposed just afterwards.  If you already have a
    /// <see cref="Runtime" /> environment, set the <see cref="RuntimeSetup" /> to <c>null</c>.
    /// You can also override the default <see cref="ITestRunner" /> that is created
    /// by setting the <see cref="TestRunnerFactoryName" /> property.
    /// </para>
    /// </summary>
    /// <todo>
    /// More validation of arguments up front.  Particularly report formats.
    /// </todo>
    public class TestLauncher : IDisposable
    {
        #region Private Members

        private TestPackageConfig testPackageConfig;
        private RuntimeSetup runtimeSetup;

        private readonly List<string> reportFormats;
        private readonly NameValueCollection reportFormatOptions;

        private IProgressMonitorProvider progressMonitorProvider;
        private ILogger logger;

        private Filter<ITest> filter;

        private string testRunnerFactoryName;
        private NameValueCollection testRunnerOptions;

        private bool echoResults;
        private bool doNotRun;
        private bool ignoreAnnotations;

        private string reportDirectory;
        private string reportNameFormat;

        private ReportMonitor reportMonitor;
        private List<ITestRunnerMonitor> customMonitors;

        private bool showReports;

        #endregion

        /// <summary>
        /// Creates a launcher with default options and no test assemblies specified.
        /// </summary>
        public TestLauncher()
        {
            testPackageConfig = new TestPackageConfig();

            testRunnerFactoryName = StandardTestRunnerFactoryNames.IsolatedAppDomain;
            testRunnerOptions = new NameValueCollection();

            reportDirectory = @"";
            reportNameFormat = @"test-report-{0}-{1}";
            reportFormats = new List<string>();
            reportFormatOptions = new NameValueCollection();

            progressMonitorProvider = NullProgressMonitorProvider.Instance;
            logger = NullLogger.Instance;

            filter = new AnyFilter<ITest>();

            reportMonitor = new ReportMonitor();
            customMonitors = new List<ITestRunnerMonitor>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Help out the GC a little bit.
            runtimeSetup = null;
            testPackageConfig = null;
            progressMonitorProvider = null;
            logger = null;
            filter = null;
            reportMonitor = null;
            customMonitors = null;
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
            get
            {
                ThrowIfDisposed();
                return progressMonitorProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
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
            get
            {
                ThrowIfDisposed();
                return logger;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
                logger = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the test filter to apply.
        /// </para>
        /// <para>
        /// The default filter is an instance of <see cref="AnyFilter{T}" />.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public Filter<ITest> Filter
        {
            get
            {
                ThrowIfDisposed();
                return filter;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
                filter = value;
            }
        }

        /// <summary>
        /// Gets or sets the test package.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestPackageConfig TestPackageConfig
        {
            get
            {
                ThrowIfDisposed();
                return testPackageConfig;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
                testPackageConfig = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the <see cref="RuntimeSetup" /> to use for automatically initializing
        /// the <see cref="Runtime"/> during test execution or <c>null</c> if the <see cref="Runtime" />
        /// is already initialized.
        /// </para>
        /// <para>
        /// If this value if not <c>null</c> then the launcher will initialize the <see cref="Runtime" />
        /// using this <see cref="RuntimeSetup" /> just prior to test execution and will automatically
        /// shut down the <see cref="Runtime" /> just afterwards.
        /// </para>
        /// <para>
        /// The default value is <c>null</c> which assumes that the <see cref="Runtime" /> is already initialized.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Runtimes cannot be nested.  So if the value of this property is not <c>null</c> and
        /// the runtime has already been initialized, then an exception will be thrown when
        /// running the tests because the <see cref="TestLauncher" /> will attempt to reinitialize
        /// the runtime unnecessarily.
        /// </remarks>
        public RuntimeSetup RuntimeSetup
        {
            get
            {
                ThrowIfDisposed();
                return runtimeSetup;
            }
            set
            {
                ThrowIfDisposed();
                runtimeSetup = value;
            }
        }

        /// <summary>
        /// <para>
        /// Specifies the name of a <see cref="ITestRunnerFactory" /> to use for constructing
        /// the <see cref="ITestRunner" /> at test execution time.
        /// </para>
        /// <para>
        /// The default value is <see cref="StandardTestRunnerFactoryNames.IsolatedAppDomain"/>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string TestRunnerFactoryName
        {
            get
            {
                ThrowIfDisposed();
                return testRunnerFactoryName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                ThrowIfDisposed();
                testRunnerFactoryName = value;
            }
        }

        /// <summary>
        /// Gets the mutable collection of options for the test runner.
        /// </summary>
        public NameValueCollection TestRunnerOptions
        {
            get
            {
                ThrowIfDisposed();
                return testRunnerOptions;
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
            get
            {
                ThrowIfDisposed();
                return echoResults;
            }
            set
            {
                ThrowIfDisposed();
                echoResults = value;
            }
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
            get
            {
                ThrowIfDisposed();
                return doNotRun;
            }
            set
            {
                ThrowIfDisposed();
                doNotRun = value;
            }
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
            get
            {
                ThrowIfDisposed();
                return reportDirectory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
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
            get
            {
                ThrowIfDisposed();
                return reportNameFormat;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                ThrowIfDisposed();
                reportNameFormat = value; 
            }
        }

        /// <summary>
        /// Gets the mutable list of report formats to generate.
        /// </summary>
        public IList<string> ReportFormats
        {
            get
            {
                ThrowIfDisposed();
                return reportFormats;
            }
        }

        /// <summary>
        /// Gets the mutable collection of options for the report formatters.
        /// </summary>
        public NameValueCollection ReportFormatOptions
        {
            get
            {
                ThrowIfDisposed();
                return reportFormatOptions;
            }
        }

        /// <summary>
        /// <para>
        /// Gets the test runner's report monitor.
        /// </para>
        /// <para>
        /// The report monitor fires events during test execution to allow test results
        /// to be retrieved incrementally as the test executes.
        /// </para>
        /// </summary>
        public ReportMonitor ReportMonitor
        {
            get
            {
                ThrowIfDisposed();
                return reportMonitor;
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
            get
            {
                ThrowIfDisposed();
                return showReports;
            }
            set
            {
                ThrowIfDisposed();
                showReports = value;
            }
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
            get
            {
                ThrowIfDisposed();
                return ignoreAnnotations;
            }
            set
            {
                ThrowIfDisposed();
                ignoreAnnotations = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets a mutable list of custom <see cref="ITestRunnerMonitor" /> objects that
        /// will be attached to the <see cref="ITestRunner" /> just prior to test execution
        /// then detached afterwards.
        /// </para>
        /// <para>
        /// The test runner fires events throughout the test execution lifecycle.  Custom
        /// monitors can capture those events and perform real-time updates as required.
        /// </para>
        /// </summary>
        public IList<ITestRunnerMonitor> CustomMonitors
        {
            get
            {
                ThrowIfDisposed();
                return customMonitors;
            }
        }

        #region Public Methods

        /// <summary>
        /// <para>
        /// Runs the test package as configured.
        /// </para>
        /// <para>
        /// If <see cref="RuntimeSetup" /> is non-<c>null</c>, initializes the <see cref="Runtime" />
        /// for the duration of this method then shuts it down automatically before returning.
        /// </para>
        /// </summary>
        /// <returns>An integer representing the result of the execution.</returns>
        public TestLauncherResult Run()
        {
            ThrowIfDisposed();

            reportMonitor.ResetReport();

            TestLauncherResult result = new TestLauncherResult(reportMonitor.Report);

            if (!PrepareToRun(result))
                return result;

            logger.Info("Initializing the test runner.");

            if (runtimeSetup != null)
                Runtime.Initialize(runtimeSetup, logger);

            try
            {
                using (ITestRunner runner = CreateRunner(result))
                {
                    if (runner == null)
                        return result;

                    IReportManager reportManager = Runtime.Instance.Resolve<IReportManager>();
                    if (!InitializeRunner(result, runner, reportManager))
                        return result;

                    Stopwatch stopWatch = Stopwatch.StartNew();
                    logger.InfoFormat("Start time: {0}", DateTime.Now.ToShortTimeString());

                    try
                    {
                        // Run initial phases.
                        if (!RunInitialPhases(result, runner))
                            return result;

                        // Run tests.
                        if (!doNotRun)
                        {
                            try
                            {
                                RunTests(runner);

                                if (result.Report.PackageRun.Statistics.FailedCount > 0)
                                    result.SetResultCode(ResultCode.Failure);
                                else if (result.Report.PackageRun.Statistics.TestCount == 0)
                                    result.SetResultCode(ResultCode.NoTests);
                            }
                            catch (OperationCanceledException)
                            {
                                result.SetResultCode(ResultCode.Canceled);
                            }
                        }
                    }
                    finally
                    {
                        // Create a new report object before unloading to ensure we retain the previous report.
                        reportMonitor.ResetReport();

                        // Unload the package now since we're done with it.
                        // This also provides more meaningful progress information to the user
                        // than if we're simply waited until the runner was disposed.
                        try
                        {
                            UnloadTestPackage(runner);
                        }
                        catch (OperationCanceledException)
                        {
                            result.SetResultCode(ResultCode.Canceled);
                        }
                    }

                    // Generate reports even if the test run is canceled, unless this step
                    // also gets canceled.
                    try
                    {
                        GenerateReports(result, reportManager);
                    }
                    catch (OperationCanceledException)
                    {
                        result.SetResultCode(ResultCode.Canceled);
                    }

                    // Done.
                    logger.InfoFormat("Stop time: {0} (Total execution time: {1:#0.000} seconds)",
                        DateTime.Now.ToShortTimeString(),
                        stopWatch.Elapsed.TotalSeconds);

                    if (showReports)
                    {
                        logger.Info("Displaying reports.");
                        ShowReportDocuments(result);
                    }

                    // Produce a failure in case there were error annotations.
                    if (! ignoreAnnotations
                        && result.ResultCode == ResultCode.Success
                        && result.Report.TestModelData.GetErrorAnnotationCount() != 0)
                        result.SetResultCode(ResultCode.Failure);

                    return result;
                }
            }
            finally
            {
                if (runtimeSetup != null)
                    Runtime.Shutdown();
            }
        }

        /// <summary>
        /// Method to generate reports in the specified formats for a result set.
        /// </summary>
        /// <param name="result">The test results to use</param>
        /// <param name="reportManager">The report manager</param>
        public void GenerateReports(TestLauncherResult result, IReportManager reportManager)
        {
            if (reportFormats.Count == 0)
                return;

            Report report = result.Report;

            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Generating reports.", reportFormats.Count);

                IReportContainer reportContainer = CreateReportContainer(report);
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);

                // Delete the report if it exists already.
                reportContainer.DeleteReport();

                // Format the report in all of the desired ways.
                foreach (string reportFormat in reportFormats)
                {
                    reportManager.Format(reportWriter, reportFormat, reportFormatOptions,
                        progressMonitor.CreateSubProgressMonitor(1));
                }

                // Save the full paths of the documents.
                foreach (string reportDocumentPath in reportWriter.ReportDocumentPaths)
                    result.AddReportDocumentPath(Path.Combine(reportDirectory, reportDocumentPath));
            });
        }

        /// <summary>
        /// Shows the report documents enumerated in the launcher result.
        /// </summary>
        /// <param name="result">The result</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null</exception>
        public void ShowReportDocuments(TestLauncherResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            foreach (string reportDocumentPath in result.ReportDocumentPaths)
            {
                try
                {
                    TestRunnerUtils.ShowReportDocument(reportDocumentPath);
                }
                catch (Exception ex)
                {
                    logger.FatalFormat("Could not open report '{0}' for display.", reportDocumentPath, ex);
                }
            }
        }

        #endregion

        #region Private Methods

        private bool PrepareToRun(TestLauncherResult result)
        {
            Canonicalize(null);
            DisplayConfiguration();

            VerifyAssemblies();

            if (!HasTestAssemblies())
            {
                result.SetResultCode(ResultCode.NoTests);
                return false;
            }

            return true;
        }

        private ITestRunner CreateRunner(TestLauncherResult result)
        {
            ITestRunnerManager manager = Runtime.Instance.Resolve<ITestRunnerManager>();
            ITestRunnerFactory factory = manager.FactoryResolver.Resolve(testRunnerFactoryName);

            if (factory == null)
            {
                result.SetResultCode(ResultCode.InvalidArguments);
                logger.ErrorFormat("Unrecognized test runner factory name: '{0}'.", testRunnerFactoryName);
                return null;
            }

            return factory.CreateTestRunner(testRunnerOptions);
        }

        private bool InitializeRunner(TestLauncherResult result, ITestRunner runner, IReportManager reportManager)
        {
            if (!ValidateReportFormats(reportManager))
            {
                result.SetResultCode(ResultCode.InvalidArguments);
                return false;
            }

            reportMonitor.Attach(runner);
            AttachDebugMonitorIfNeeded(runner);
            AttachLogMonitorIfNeeded(runner);
            AttachCustomMonitors(runner);
            return true;
        }

        private bool ValidateReportFormats(IReportManager reportManager)
        {
            foreach (string reportFormat in reportFormats)
            {
                IReportFormatter formatter = reportManager.FormatterResolver.Resolve(reportFormat);
                if (formatter == null)
                {
                    logger.ErrorFormat("Unrecognized report format: '{0}'.", reportFormat);
                    return false;
                }
            }

            if (reportNameFormat.Length == 0)
            {
                logger.ErrorFormat("Report name format must not be empty.");
                return false;
            }

            return true;
        }

        private void AttachDebugMonitorIfNeeded(ITestRunner runner)
        {
            if (logger.IsDebugEnabled)
                new DebugMonitor(logger).Attach(runner);
        }

        private void AttachLogMonitorIfNeeded(ITestRunner runner)
        {
            if (echoResults)
            {
                LogMonitor logMonitor = new LogMonitor(logger, reportMonitor);
                logMonitor.Attach(runner);
            }
        }

        private void AttachCustomMonitors(ITestRunner runner)
        {
            foreach (ITestRunnerMonitor monitor in customMonitors)
            {
                monitor.Attach(runner);
            }
        }

        private bool RunInitialPhases(TestLauncherResult result, ITestRunner runner)
        {
            try
            {
                ApplyFilter(runner);
                LoadTestPackage(runner);
                BuildTestModel(runner);
                return true;
            }
            catch (OperationCanceledException)
            {
                result.SetResultCode(ResultCode.Canceled);
                return false;
            }
        }

        /// <summary>
        /// Removes any non-existing assemblies from the list of test assemblies.
        /// </summary>
        private void VerifyAssemblies()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Verifying assembly names.", 1);

                List<string> assembliesToRemove = new List<string>();
                foreach (string assemblyName in testPackageConfig.AssemblyFiles)
                {
                    if (!File.Exists(assemblyName))
                    {
                        assembliesToRemove.Add(assemblyName);
                        logger.Error("Cannot find assembly: {0}", assemblyName);
                    }
                }

                // Remove invalid assemblies
                foreach (string assemblyName in assembliesToRemove)
                    testPackageConfig.AssemblyFiles.Remove(assemblyName);
            });
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

                logger.Info(message.ToString());
            }
        }

        private void Canonicalize(string baseDirectory)
        {
            testPackageConfig.HostSetup.Canonicalize(baseDirectory);

            if (runtimeSetup != null)
                runtimeSetup.Canonicalize(baseDirectory);
        }

        private bool HasTestAssemblies()
        {
            if (testPackageConfig.AssemblyFiles.Count == 0)
            {
                logger.Warn("No test assemblies to execute!");
                return false;
            }

            return true;
        }

        private void ApplyFilter(ITestRunner runner)
        {
            runner.TestExecutionOptions.Filter = filter;
        }

        private void LoadTestPackage(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.LoadTestPackage(testPackageConfig, progressMonitor);
            });
        }

        private void BuildTestModel(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.BuildTestModel(progressMonitor);
            });
        }

        private void RunTests(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.RunTests(progressMonitor);
            });
        }

        private void UnloadTestPackage(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.UnloadTestPackage(progressMonitor);
            });
        }

        private IReportContainer CreateReportContainer(Report report)
        {
            string reportName = GenerateReportName(report);
            return new FileSystemReportContainer(reportDirectory, reportName);
        }

        private string GenerateReportName(Report report)
        {
            DateTime reportTime = report.PackageRun != null ? report.PackageRun.StartTime : DateTime.Now;

            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }

        private void ThrowIfDisposed()
        {
            if (testPackageConfig == null)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion
    }
}
