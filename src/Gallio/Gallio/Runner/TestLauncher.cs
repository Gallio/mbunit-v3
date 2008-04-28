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
using Gallio.Model.Execution;
using Gallio.Runner.Extensions;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Runner.Reports;

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
        private readonly NameValueCollection reportFormatOptions;

        private IProgressMonitorProvider progressMonitorProvider;
        private ILogger logger;

        private bool echoResults;
        private bool doNotRun;
        private bool ignoreAnnotations;

        private string reportDirectory;
        private string reportNameFormat;

        private readonly List<ITestRunnerExtension> extensions;
        private readonly List<string> extensionSpecifications;

        private bool showReports;

        #endregion

        /// <summary>
        /// Creates a launcher with default options and no test assemblies specified.
        /// </summary>
        public TestLauncher()
        {
            testRunnerFactoryName = StandardTestRunnerFactoryNames.Local;
            testRunnerOptions = new TestRunnerOptions();
            testPackageConfig = new TestPackageConfig();
            testExplorationOptions = new TestExplorationOptions();
            testExecutionOptions = new TestExecutionOptions();

            reportDirectory = @"";
            reportNameFormat = @"test-report-{0}-{1}";
            reportFormats = new List<string>();
            reportFormatOptions = new NameValueCollection();

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
        /// <para>
        /// If this value if not <c>null</c> then the launcher will initialize the runtime
        /// using this <see cref="RuntimeSetup" /> just prior to test execution and will
        /// automatically shut down the runtime just afterwards.
        /// </para>
        /// <para>
        /// The default value is <c>null</c> which assumes that the runtime is already initialized.
        /// </para>
        /// </summary>
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
        /// The default value is <see cref="StandardTestRunnerFactoryNames.Local"/>.
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
        /// Gets the mutable collection of options for the report formatters.
        /// </summary>
        public NameValueCollection ReportFormatOptions
        {
            get { return reportFormatOptions; }
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

        #region Public Methods

        /// <summary>
        /// <para>
        /// Runs the test package as configured.
        /// </para>
        /// <para>
        /// If <see cref="RuntimeSetup" /> is non-<c>null</c>,
        /// initializes the runtime for the duration of this method then shuts it down automatically
        /// before returning.  Otherwise assumes the runtime has already been initialized and
        /// accesses it using <see cref="RuntimeAccessor" />.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Runtimes cannot be nested.  So if the launcher is asked to initialize the runtime and
        /// it has already been initialized then an exception will be thrown.
        /// </remarks>
        /// <returns>A result object</returns>
        public TestLauncherResult Run()
        {
            Canonicalize(null);
            DisplayConfiguration();

            VerifyAssemblies();

            if (testPackageConfig.AssemblyFiles.Count == 0)
            {
                logger.Log(LogSeverity.Warning, "No test assemblies to execute!");
                return CreateResult(ResultCode.NoTests);
            }

            logger.Log(LogSeverity.Info, "Initializing the test runner.");

            using (runtimeSetup != null
                ? RuntimeBootstrap.Initialize(runtimeSetup, logger)
                : null)
            {
                return RunWithRuntime();
            }
        }

        private TestLauncherResult RunWithRuntime()
        {
            IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
            if (!ValidateReportFormats(reportManager))
                return CreateResult(ResultCode.InvalidArguments);

            ITestRunnerManager manager = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>();
            ITestRunnerFactory factory = manager.FactoryResolver.Resolve(testRunnerFactoryName);
            if (factory == null)
            {
                logger.Log(LogSeverity.Error, String.Format("Unrecognized test runner factory name: '{0}'.", testRunnerFactoryName));
                return CreateResult(ResultCode.InvalidArguments);
            }

            ITestRunner runner = factory.CreateTestRunner();
            try
            {
                DoRegisterExtensions(runner);
                DoInitialize(runner);

                return RunWithInitializedRunner(runner, reportManager);
            }
            finally
            {
                DoDispose(runner);
            }
        }

        private TestLauncherResult RunWithInitializedRunner(ITestRunner runner, IReportManager reportManager)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            logger.Log(LogSeverity.Info, String.Format("Start time: {0}", DateTime.Now.ToShortTimeString()));

            TestLauncherResult result;
            bool wasCanceled = false;
            try
            {
                // Run initial phases.
                try
                {
                    DoLoad(runner);
                    DoExplore(runner);
                }
                catch (OperationCanceledException)
                {
                    wasCanceled = true;
                }

                // Run tests.
                if (! wasCanceled && !doNotRun)
                {
                    try
                    {
                        DoRun(runner);
                    }
                    catch (OperationCanceledException)
                    {
                        wasCanceled = true;
                    }
                }

                result = new TestLauncherResult(runner.Report);
            }
            finally
            {
                // Unload the package now since we're done with it.
                // This also provides more meaningful progress information to the user
                // than if we're simply waited until the runner was disposed.
                try
                {
                    DoUnload(runner);
                }
                catch (OperationCanceledException)
                {
                    wasCanceled = true;
                }
            }

            // Generate reports even if the test run is canceled, unless this step
            // also gets canceled.
            try
            {
                if (result.Report.TestPackageRun != null)
                    GenerateReports(result, reportManager);
            }
            catch (OperationCanceledException)
            {
                wasCanceled = true;
            }

            // Done.
            logger.Log(LogSeverity.Info, String.Format("Stop time: {0} (Total execution time: {1:#0.000} seconds)",
                DateTime.Now.ToShortTimeString(),
                stopWatch.Elapsed.TotalSeconds));

            if (showReports && result.ReportDocumentPaths.Count != 0)
            {
                logger.Log(LogSeverity.Info, "Displaying reports.");
                ShowReportDocuments(result);
            }

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

        /// <summary>
        /// Method to generate reports in the specified formats for a result set.
        /// </summary>
        /// <param name="result">The test results to use</param>
        /// <param name="reportManager">The report manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/>
        /// or <paramref name="reportManager"/> is null</exception>
        public void GenerateReports(TestLauncherResult result, IReportManager reportManager)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            if (reportManager == null)
                throw new ArgumentNullException("reportManager");

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
                    logger.Log(LogSeverity.Error, String.Format("Could not open report '{0}' for display.", reportDocumentPath), ex);
                }
            }
        }

        #endregion

        #region Private Methods

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
                        logger.Log(LogSeverity.Error, String.Format("Cannot find assembly: {0}", assemblyName));
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

                logger.Log(LogSeverity.Info, message.ToString());
            }
        }

        private void Canonicalize(string baseDirectory)
        {
            testPackageConfig.HostSetup.Canonicalize(baseDirectory);

            if (runtimeSetup != null)
                runtimeSetup.Canonicalize(baseDirectory);
        }

        private void DoInitialize(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Initialize(testRunnerOptions, logger, progressMonitor);
            });
        }

        private void DoLoad(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Load(testPackageConfig, progressMonitor);
            });
        }

        private void DoExplore(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Explore(testExplorationOptions, progressMonitor);
            });
        }

        private void DoRun(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Run(testExecutionOptions, progressMonitor);
            });
        }

        private void DoUnload(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Unload(progressMonitor);
            });
        }

        private void DoDispose(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Dispose(progressMonitor);
            });
        }

        private IReportContainer CreateReportContainer(Report report)
        {
            string reportName = GenerateReportName(report);
            return new FileSystemReportContainer(reportDirectory, reportName);
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

        #endregion
    }
}
