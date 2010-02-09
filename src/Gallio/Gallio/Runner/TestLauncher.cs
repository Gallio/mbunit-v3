// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Model.Schema;
using Gallio.Runner.Extensions;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Model;
using Gallio.Runner.Reports;
using System.Threading;

namespace Gallio.Runner
{
    /// <summary>
    /// The test launcher encapsulated the entire test execution lifecycle from
    /// start to finish and provides a simplified pattern for running tests.
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public class TestLauncher
    {
        private readonly List<string> filePatterns;
        private TestProject testProject;
        private TestRunnerOptions testRunnerOptions;
        private TestExplorationOptions testExplorationOptions;
        private TestExecutionOptions testExecutionOptions;
        private readonly List<string> reportFormats;
        private ReportFormatterOptions reportFormatterOptions;
        private IProgressMonitorProvider progressMonitorProvider;
        private ILogger logger;
        private readonly object cancelationSyncRoot = new object();
        private bool isCanceled;
        private IProgressMonitor cancelableProgressMonitor;

        /// <summary>
        /// Creates a launcher with default options and no test assemblies specified.
        /// </summary>
        public TestLauncher()
        {
            filePatterns = new List<string>();
            testProject = new TestProject();
            testRunnerOptions = new TestRunnerOptions();
            testExplorationOptions = new TestExplorationOptions();
            testExecutionOptions = new TestExecutionOptions();
            reportFormats = new List<string>();
            reportFormatterOptions = new ReportFormatterOptions();
            progressMonitorProvider = NullProgressMonitorProvider.Instance;
            logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets or sets the progress monitor provider to use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default provider is <see cref="NullProgressMonitorProvider.Instance" />.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public IProgressMonitorProvider ProgressMonitorProvider
        {
            get
            {
                return progressMonitorProvider;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                progressMonitorProvider = value;
            }
        }

        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default logger is <see cref="NullLogger.Instance" />.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public ILogger Logger
        {
            get
            {
                return logger;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                logger = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RuntimeSetup" /> to use for automatically initializing
        /// the runtime during test execution or <c>null</c> if the runtime is already initialized.
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
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the test runner options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestRunnerOptions TestRunnerOptions
        {
            get
            {
                return testRunnerOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                testRunnerOptions = value;
            }
        }

        /// <summary>
        /// Gets a read-only list of test file patterns (with wildcards) or test project files
        /// that are to be resolved and included in the test package prior to execution.
        /// </summary>
        public IList<string> FilePatterns
        {
            get
            {
                return new ReadOnlyCollection<string>(filePatterns);
            }
        }

        /// <summary>
        /// Gets or sets the test project.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestProject TestProject
        {
            get
            {
                return testProject;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testProject = value;
            }
        }

        /// <summary>
        /// Gets or sets the test exploration options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestExplorationOptions TestExplorationOptions
        {
            get
            {
                return testExplorationOptions;
            }
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestExecutionOptions TestExecutionOptions
        {
            get
            {
                return testExecutionOptions;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                testExecutionOptions = value;
            }
        }

        /// <summary>
        /// Controls whether the test runner will echo result to the <see cref="Logger" />
        /// as each test finishes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool EchoResults
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to skip test execution.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This option may be used to produce a report that contains test
        /// metadata for consumption by other tools.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool DoNotRun
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a read-only list of report formats to generate.
        /// </summary>
        public IList<string> ReportFormats
        {
            get
            {
                return new ReadOnlyCollection<string>(reportFormats);
            }
        }

        /// <summary>
        /// Gets or sets the report formatter options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public ReportFormatterOptions ReportFormatterOptions
        {
            get
            {
                return reportFormatterOptions;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                reportFormatterOptions = value;
            }
        }

        /// <summary>
        /// Gets or sets whether to show the reports after the test run finishes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool ShowReports
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to ignore annotations when determining the result code.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If false, then error annotations, usually indicative of broken tests, will cause
        /// a failure result to be generated.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>.
        /// </para>
        /// </remarks>
        public bool IgnoreAnnotations
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum amount of time the tests can run before they are canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is <c>null</c>, meaning an infinite time.
        /// </para>
        /// </remarks>
        public TimeSpan? RunTimeLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Clears the list of file patterns.
        /// </summary>
        public void ClearFilePatterns()
        {
            filePatterns.Clear();
        }

        /// <summary>
        /// Adds a file pattern if not already added to the launcher.
        /// </summary>
        /// <param name="filePattern">The file path or a wildcard pattern to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePattern"/> is null.</exception>
        public void AddFilePattern(string filePattern)
        {
            if (filePattern == null)
                throw new ArgumentNullException("filePattern");

            if (!filePatterns.Contains(filePattern))
                filePatterns.Add(filePattern);
        }

        /// <summary>
        /// Removes a file pattern.
        /// </summary>
        /// <param name="filePattern">The file pattern to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filePattern"/> is null.</exception>
        public void RemoveFilePattern(string filePattern)
        {
            if (filePattern == null)
                throw new ArgumentNullException("filePattern");

            filePatterns.Remove(filePattern);
        }

        /// <summary>
        /// Clears the list of report formats.
        /// </summary>
        public void ClearReportFormats()
        {
            reportFormats.Clear();
        }

        /// <summary>
        /// Adds a report format if not already added to the launcher.
        /// </summary>
        /// <param name="reportFormat">The report format to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportFormat"/> is null.</exception>
        public void AddReportFormat(string reportFormat)
        {
            if (reportFormat == null)
                throw new ArgumentNullException("reportFormat");

            if (!reportFormats.Contains(reportFormat))
                reportFormats.Add(reportFormat);
        }

        /// <summary>
        /// Removes a report format.
        /// </summary>
        /// <param name="reportFormat">The report format to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reportFormat"/> is null.</exception>
        public void RemoveReportFormat(string reportFormat)
        {
            if (reportFormat == null)
                throw new ArgumentNullException("reportFormat");

            reportFormats.Remove(reportFormat);
        }

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
        /// Runs the test package as configured.
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
        /// <returns>A result object.</returns>
        public TestLauncherResult Run()
        {
            Canonicalize(null);
            DisplayConfiguration();

            Timer runTimeTimer = null;
            if (RunTimeLimit != null)
            {                
                runTimeTimer = new Timer(delegate
                {
                    Cancel();
                    logger.Log(LogSeverity.Warning, "Run time limit reached!  Canceled test run.");
                }, null, (int)RunTimeLimit.Value.TotalMilliseconds, Timeout.Infinite);
            }

            Stopwatch stopWatch = Stopwatch.StartNew();
            logger.Log(LogSeverity.Important, String.Format("Start time: {0}", DateTime.Now.ToShortTimeString()));
            bool wasCanceled = false;
            try
            {
                using (InitializeRuntimeIfNeeded(ref wasCanceled))
                {
                    if (wasCanceled)
                        return CreateResult(ResultCode.Canceled, testProject);

                    return RunWithRuntime();
                }
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

        private IDisposable InitializeRuntimeIfNeeded(ref bool canceled)
        {
            IDisposable result = null;

            if (RuntimeSetup != null && !RuntimeAccessor.IsInitialized)
            {
                RunWithProgress(progressMonitor =>
                {
                    progressMonitor.BeginTask("Initializing the runtime and loading plugins.", 1);
                    result = RuntimeBootstrap.Initialize(RuntimeSetup, logger);
                }, ref canceled);
            }

            return result;
        }

        private TestLauncherResult RunWithRuntime()
        {
            bool wasCanceled = false;

            ITestProjectManager testProjectManager = RuntimeAccessor.ServiceLocator.Resolve<ITestProjectManager>();

            TestProject consolidatedTestProject = ConsolidateTestProject(testProjectManager, ref wasCanceled);
            if (wasCanceled)
                return CreateResult(ResultCode.Canceled, testProject);
            if (consolidatedTestProject == null)
                return CreateResult(ResultCode.InvalidArguments, testProject);

            if (consolidatedTestProject.TestPackage.Files.Count == 0)
            {
                logger.Log(LogSeverity.Warning, "No test files to execute!");
                return CreateResult(ResultCode.NoTests, consolidatedTestProject);
            }

            IReportManager reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();
            if (!ValidateReportFormats(reportManager, consolidatedTestProject))
                return CreateResult(ResultCode.InvalidArguments, consolidatedTestProject);

            ITestRunnerManager testRunnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
            ITestRunnerFactory testRunnerFactory = testRunnerManager.GetFactory(consolidatedTestProject.TestRunnerFactoryName);
            if (testRunnerFactory == null)
            {
                logger.Log(LogSeverity.Error, String.Format("Unrecognized test runner factory name: '{0}'.", consolidatedTestProject.TestRunnerFactoryName));
                return CreateResult(ResultCode.InvalidArguments, consolidatedTestProject);
            }

            ITestRunner runner = testRunnerFactory.CreateTestRunner();
            var result = new TestLauncherResult(new Report { TestPackage = new TestPackageData(TestProject.TestPackage) });
            try
            {
                DoRegisterExtensions(runner, consolidatedTestProject);
                DoInitialize(runner, ref wasCanceled);

                if (!wasCanceled)
                {
                    result = RunWithInitializedRunner(runner, consolidatedTestProject, reportManager);
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

        private TestLauncherResult RunWithInitializedRunner(ITestRunner runner, TestProject consolidatedTestProject, IReportManager reportManager)
        {
            bool wasCanceled = false;

            // Explore or Run tests.
            Report report = DoExploreOrRun(runner, consolidatedTestProject.TestPackage, ref wasCanceled);

            if (report == null)
                report = new Report();
            var result = new TestLauncherResult(report);

            // Generate reports even if the test run is canceled, unless this step
            // also gets canceled.
            if (result.Report.TestPackageRun != null)
                GenerateReports(result, reportManager, consolidatedTestProject, ref wasCanceled);

            // Done.
            if (ShowReports)
                ShowReportDocuments(result);

            // Produce the final result code.
            if (wasCanceled)
            {
                result.SetResultCode(ResultCode.Canceled);
            }
            else
            {
                if (! IgnoreAnnotations
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

        private void GenerateReports(TestLauncherResult result, IReportManager reportManager,
            TestProject consolidatedTestProject, ref bool canceled)
        {
            if (reportFormats.Count == 0)
                return;

            RunWithProgress(progressMonitor => result.GenerateReports(consolidatedTestProject.ReportDirectory,
                result.Report.FormatReportName(consolidatedTestProject.ReportNameFormat), consolidatedTestProject.ReportArchive, 
                reportFormats, reportFormatterOptions, reportManager, progressMonitor), ref canceled);
        }

        private void ShowReportDocuments(TestLauncherResult result)
        {
            if (result.ReportDocumentPaths.Count == 0) 
                return;

            logger.Log(LogSeverity.Important, "Displaying reports.");

            if (!result.ShowReportDocuments())
                logger.Log(LogSeverity.Important, "There was an error opening the report documents.");
        }

        private bool ValidateReportFormats(IReportManager reportManager, TestProject consolidatedTestProject)
        {
            foreach (string reportFormat in reportFormats)
            {
                IReportFormatter formatter = reportManager.GetReportFormatter(reportFormat);
                
                if (formatter != null) 
                    continue;

                logger.Log(LogSeverity.Error, String.Format("Unrecognized report format: '{0}'.", reportFormat));
                return false;
            }

            if (consolidatedTestProject.ReportNameFormat.Length == 0)
            {
                logger.Log(LogSeverity.Error, "Report name format must not be empty.");
                return false;
            }

            return true;
        }

        private void DoRegisterExtensions(ITestRunner runner, TestProject consolidatedTestProject)
        {
            if (EchoResults)
                runner.RegisterExtension(new LogExtension());

            foreach (ITestRunnerExtension extension in consolidatedTestProject.TestRunnerExtensions)
                runner.RegisterExtension(extension);

            // de-dupe extension specs
            List<string> uniqueExtensionSpecifications = new List<string>();
            GenericCollectionUtils.AddAllIfNotAlreadyPresent(consolidatedTestProject.TestRunnerExtensionSpecifications, 
                uniqueExtensionSpecifications);

            foreach (string extensionSpecification in uniqueExtensionSpecifications)
            {
                var testRunnerExtension = 
                    TestRunnerExtensionUtils.CreateExtensionFromSpecification(extensionSpecification);
                runner.RegisterExtension(testRunnerExtension);
            }
        }

        private void DisplayConfiguration()
        {
            DisplayPaths(testProject.TestPackage.Files, "Test Files:");
            DisplayPaths(testProject.TestPackage.HintDirectories, "Hint Directories:");

            if (RuntimeSetup != null)
                DisplayPaths(RuntimeSetup.PluginDirectories, "Plugin Directories:");
        }

        private void DisplayPaths<T>(ICollection<T> paths, string name)
            where T : FileSystemInfo
        {
            DisplayPaths(GenericCollectionUtils.ConvertAllToArray(paths, path => path.ToString()), name);
        }

        private void DisplayPaths(ICollection<string> paths, string name)
        {
            if (paths == null || paths.Count <= 0) 
                return;

            var message = new StringBuilder();
            message.Append(name);

            foreach (string path in paths)
                message.Append("\n\t").Append(path);
            message.AppendLine();

            logger.Log(LogSeverity.Info, message.ToString());
        }

        private void Canonicalize(string baseDirectory)
        {
            if (RuntimeSetup != null)
                RuntimeSetup.Canonicalize(baseDirectory);
        }

        /// <summary>
        /// Processes test file patterns and generates a consolidated test project.
        /// </summary>
        private TestProject ConsolidateTestProject(ITestProjectManager testProjectManager, ref bool canceled)
        {
            TestProject consolidatedTestProject = null;

            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                List<string> allFilePatterns = new List<string>(filePatterns);
                GenericCollectionUtils.ConvertAndAddAll(testProject.TestPackage.Files, allFilePatterns, x => x.ToString());

                TestProject overlayTestProject = testProject.Copy();
                overlayTestProject.TestPackage.ClearFiles();
                TestProject baseTestProject = null;

                bool haveProject = false;
                using (progressMonitor.BeginTask("Verifying test files.", Math.Max(allFilePatterns.Count, 1)))
                {
                    foreach (string filePattern in allFilePatterns)
                    {
                        IList<FileInfo> files = ExpandFilePattern(filePattern);
                        if (files == null)
                            return;

                        foreach (FileInfo file in files)
                        {
                            bool isProject = file.Extension == TestProject.Extension;
                            if (isProject && overlayTestProject.TestPackage.Files.Count != 0 || haveProject)
                            {
                                logger.Log(LogSeverity.Error, "At most one test project can be specified at a time and it cannot be combined with other test files.");
                                return;
                            }

                            if (isProject)
                            {
                                haveProject = true;

                                try
                                {
                                    baseTestProject = testProjectManager.LoadProject(file);
                                }
                                catch (Exception ex)
                                {
                                    logger.Log(LogSeverity.Error, string.Format("Could not load test project '{0}'.", file.FullName), ex);
                                }
                            }
                            else
                            {
                                overlayTestProject.TestPackage.AddFile(file);
                            }
                        }

                        progressMonitor.Worked(1);
                    }

                    if (baseTestProject != null)
                    {
                        baseTestProject.ApplyOverlay(overlayTestProject);
                        consolidatedTestProject = baseTestProject;
                    }
                    else
                    {
                        consolidatedTestProject = overlayTestProject;
                    }
                }
            }, ref canceled);

            return consolidatedTestProject;
        }

        private IList<FileInfo> ExpandFilePattern(string filePattern)
        {
            if (filePattern.Contains("?") || filePattern.Contains("*"))
            {
                string directory = Path.GetDirectoryName(filePattern);
                if (string.IsNullOrEmpty(directory))
                    directory = Environment.CurrentDirectory;
                else
                    directory = Path.GetFullPath(directory);

                if (! Directory.Exists(directory))
                {
                    logger.Log(LogSeverity.Error,
                        String.Format("Cannot find directory containing file pattern '{0}'.", filePattern));
                    return null;
                }

                var files = new List<FileInfo>();

                string searchPattern = Path.GetFileName(filePattern);

                foreach (string file in Directory.GetFiles(directory, searchPattern))
                    files.Add(new FileInfo(Path.Combine(directory, file)));

                return files;
            }

            if (! File.Exists(filePattern))
            {
                logger.Log(LogSeverity.Error, String.Format("Cannot find file '{0}'.", filePattern));
                return null;
            }

            return new[] { new FileInfo(Path.GetFullPath(filePattern)) };
        }

        private void DoInitialize(ITestRunner runner, ref bool canceled)
        {
            RunWithProgress(progressMonitor => runner.Initialize(testRunnerOptions, 
                logger, progressMonitor), ref canceled);
        }

        private Report DoExploreOrRun(ITestRunner runner, TestPackage testPackage, ref bool canceled)
        {
            Report report = null;
            RunWithProgress(delegate(IProgressMonitor progressMonitor)
            {
                report = DoNotRun ? runner.Explore(testPackage, testExplorationOptions, progressMonitor) : 
                    runner.Run(testPackage, testExplorationOptions, testExecutionOptions, progressMonitor);
            }, ref canceled);

            return report;
        }

        private void DoDispose(ITestRunner runner, ref bool canceled)
        {
            RunWithProgress(runner.Dispose, ref canceled);
        }

        private TestLauncherResult CreateResult(int resultCode, TestProject consolidatedTestProject)
        {
            Report report = new Report();
            report.TestPackage = new TestPackageData(consolidatedTestProject.TestPackage);
            var result = new TestLauncherResult(report);
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
    }
}
