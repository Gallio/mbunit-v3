// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Text;
using Castle.Core.Logging;
using MbUnit.Core.Harness;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Core.Runtime;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Running tests with Gallio involves creating specific objects in a predefined
    /// way. This class tries to simplify this process by implementing a common
    /// pattern.
    /// </summary>
    /// <todo>
    /// More validation of arguments up front.  Particularly report formats.
    /// </todo>
    public class TestRunnerHelper : IDisposable
    {
        #region Private Members

        private readonly TestPackage package;
        private readonly IProgressMonitorProvider progressMonitorProvider;
        private readonly RuntimeSetup runtimeSetup;
        private readonly ILogger logger;

        private Filter<ITest> filter = new AnyFilter<ITest>();

        private bool echoResults;
        private string templateModelFilename;
        private string testModelFilename;

        private string reportDirectory = "";
        private string reportNameFormat = "mbunit-{0}-{1}";
        private readonly List<string> reportFormats;
        private readonly NameValueCollection reportFormatOptions;

        private readonly Dictionary<string, string> generatedReportFilenames = new Dictionary<string, string>();
        private string resultSummary;
        private PackageRunStatistics statistics;
        private Stopwatch stopWatch;
        private readonly ReportMonitor reportMonitor;
        private readonly List<ITestRunnerMonitor> customMonitors = new List<ITestRunnerMonitor>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TestRunnerHelper class.
        /// </summary>
        /// <param name="progressMonitorProvider">The progress monitor provider</param>
        /// <param name="logger">The logger to which messages should be written</param>
        public TestRunnerHelper(IProgressMonitorProvider progressMonitorProvider,
            ILogger logger)
        {
            CheckRequiredArgument(progressMonitorProvider, "progressMonitorProvider");
            CheckRequiredArgument(logger, "logger");

            runtimeSetup = new RuntimeSetup();
            package = new TestPackage();

            this.progressMonitorProvider = progressMonitorProvider;
            this.logger = logger;

            reportFormats = new List<string>();
            reportFormatOptions = new NameValueCollection();

            reportMonitor = new ReportMonitor();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the filter to apply.
        /// Defaults to an instance of <see cref="AnyFilter{T}" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public Filter<ITest> Filter
        {
            get { return filter; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                filter = value;
            }
        }

        /// <summary>
        /// Gets the test package.
        /// </summary>
        public TestPackage Package
        {
            get { return package; }
        }

        /// <summary>
        /// Gets the runtime setup.
        /// </summary>
        public RuntimeSetup RuntimeSetup
        {
            get { return runtimeSetup; }
        }

        /// <summary>
        /// If set to true, the test runner will echo results to the <see cref="ILogger" />
        /// as each test finished.
        /// </summary>
        public bool EchoResults
        {
            get { return echoResults; }
            set { echoResults = value; }
        }

        /// <summary>
        /// The name of a file to which the template model should be persisted,
        /// or null if none.
        /// </summary>
        public string TemplateModelFilename
        {
            get { return templateModelFilename; }
            set { templateModelFilename = value; }
        }

        /// <summary>
        /// The name of a file to which the test model should be persisted,
        /// or null if none.
        /// </summary>
        public string TestModelFilename
        {
            get { return testModelFilename; }
            set { testModelFilename = value; }
        }

        /// <summary>
        /// Gets or sets the report directory.
        /// Defaults to "".
        /// </summary>
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set { reportDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the report name format.
        /// {0} is replaced by the date, {1} by the time.
        /// Defaults to "mbunit-{0}-{1}".
        /// </summary>
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set { reportNameFormat = value; }
        }

        /// <summary>
        /// Gets the list of report formats to generate.
        /// </summary>
        public List<string> ReportFormats
        {
            get { return reportFormats; }
        }

        /// <summary>
        /// Gets the options for the report formatters.
        /// </summary>
        public NameValueCollection ReportFormatOptions
        {
            get { return reportFormatOptions; }
        }

        /// <summary>
        /// A short summary with the number of test that passed, failed and so on.
        /// </summary>
        public string ResultSummary
        {
            get { return resultSummary; }
        }

        /// <summary>
        /// The statistics of the execution.
        /// </summary>
        public PackageRunStatistics Statistics
        {
            get { return statistics; }
        }

        /// <summary>
        /// Gets the test runner's report monitor.
        /// </summary>
        public ReportMonitor ReportMonitor
        {
            get { return reportMonitor; }
        }

        /// <summary>
        /// A list of custom ITestRunnerMonitor objects.
        /// </summary>
        public List<ITestRunnerMonitor> CustomMonitors
        {
            get { return customMonitors; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs a project.
        /// </summary>
        /// <returns>An integer representing the result of the execution.</returns>
        public int Run()
        {
            Canonicalize();
            DisplayConfiguration();

            VerifyAssemblies();
            if (!HasTestAssemblies())
                return ResultCode.NoTests;

            using (AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup))
            {
                CreateStopWatch();

                IReportManager reportManager = runner.Runtime.Resolve<IReportManager>();
                if (!Validate(reportManager))
                    return ResultCode.InvalidArguments;

                reportMonitor.Attach(runner);
                AttachDebugMonitorIfNeeded(runner);
                AttachLogMonitorIfNeeded(runner);
                AttachCustomMonitors(runner);

                try
                {
                    RunInitialPhases(runner);
                }
                catch (OperationCanceledException)
                {
                    return ResultCode.Canceled;
                }

                // Run the tests.
                bool runCanceled = false;
                try
                {
                    RunTests(runner);
                }
                catch (OperationCanceledException)
                {
                    runCanceled = true;
                }

                // Generate reports even if the test run is canceled, unless this step
                // also gets canceled.
                try
                {
                    GenerateReports(reportManager, reportMonitor.Report);
                }
                catch (OperationCanceledException)
                {
                    runCanceled = true;
                }

                statistics = reportMonitor.Report.PackageRun.Statistics;
                resultSummary = reportMonitor.Report.PackageRun.Statistics.FormatTestCaseResultSummary();
                DisposeStopWatch();

                if (runCanceled)
                    return ResultCode.Canceled;

                if (reportMonitor.Report.PackageRun.Statistics.FailureCount > 0)
                    return ResultCode.Failure;

                return ResultCode.Success;
            }
        }

        private void AttachCustomMonitors(ITestRunner runner)
        {
            foreach(ITestRunnerMonitor monitor in customMonitors)
            {
                monitor.Attach(runner);
            }
        }

        /// <summary>
        /// Returns the filename of the generated report for the given report type.
        /// </summary>
        /// <param name="reportType">The report type whose generated report's filename
        /// wants to be retrieved.</param>
        /// <returns>The full filename of the generated report, or null if no report
        /// was generated for the given report type.</returns>
        public string GetReportFilename(string reportType)
        {
            if (String.IsNullOrEmpty(reportType))
            {
                throw new ArgumentNullException("reportType");
            }

            string loweredReportType = reportType.ToLower(CultureInfo.InvariantCulture);
            if (generatedReportFilenames.ContainsKey(loweredReportType))
            {
                return generatedReportFilenames[loweredReportType];
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void RunInitialPhases(ITestRunner runner)
        {
            ApplyFilter(runner);
            LoadProject(runner);
            BuildTemplates(runner);
            BuildTests(runner);
            PersistTemplateTree(runner);
            PersistTestTree(runner);
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

        private bool Validate(IReportManager reportManager)
        {
            foreach (string reportFormat in reportFormats)
            {
                IReportFormatter formatter = reportManager.GetFormatter(reportFormat);
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

        /// <summary>
        /// Removes any non-existing assemblies from the list of test assemblies.
        /// </summary>
        private void VerifyAssemblies()
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                progressMonitor.BeginTask("Verifying assembly names.", 1);

                List<string> assembliesToRemove = new List<string>();
                foreach (string assemblyName in package.AssemblyFiles)
                {
                    if (!File.Exists(assemblyName))
                    {
                        assembliesToRemove.Add(assemblyName);
                        logger.Error("Cannot find assembly: {0}", assemblyName);
                    }
                }

                // Remove invalid assemblies
                foreach (string assemblyName in assembliesToRemove)
                    package.AssemblyFiles.Remove(assemblyName);
            });
        }

        private void DisplayConfiguration()
        {
            DisplayPaths(package.AssemblyFiles, "Test Assemblies:");
            DisplayPaths(package.HintDirectories, "Hint Directories:");
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

        private void Canonicalize()
        {
            package.ApplicationBase = CanonicalizePath(package.ApplicationBase);
            CanonicalizePaths(package.AssemblyFiles);
            CanonicalizePaths(package.HintDirectories);

            CanonicalizePaths(runtimeSetup.PluginDirectories);
        }

        private static string CanonicalizePath(string path)
        {
            if (path.Length == 0)
                path = @".";
            return path == null ? null : Path.GetFullPath(path);
        }

        private static void CanonicalizePaths(IList<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
                paths[i] = CanonicalizePath(paths[i]);
        }

        private bool HasTestAssemblies()
        {
            if (package.AssemblyFiles.Count == 0)
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

        private void LoadProject(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.LoadPackage(package, progressMonitor);
            });
        }

        private void BuildTemplates(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.BuildTemplates(progressMonitor);
            });
        }

        private void BuildTests(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.BuildTests(progressMonitor);
            });
        }

        private void RunTests(ITestRunner runner)
        {
            progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
            {
                runner.Run(progressMonitor);
            });
        }

        private void PersistTemplateTree(ITestRunner runner)
        {
            if (templateModelFilename != null)
            {
                progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
                {
                    progressMonitor.BeginTask("Saving template tree.", 1);
                    progressMonitor.SetStatus(templateModelFilename);
                    SerializationUtils.SaveToXml(runner.TemplateModel, templateModelFilename);
                });
            }
        }

        private void PersistTestTree(ITestRunner runner)
        {
            if (testModelFilename != null)
            {
                progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
                {
                    progressMonitor.BeginTask("Saving test tree.", 1);
                    progressMonitor.SetStatus(testModelFilename);
                    SerializationUtils.SaveToXml(runner.TestModel, testModelFilename);
                });
            }
        }

        private void GenerateReports(IReportManager reportManager, Report report)
        {
            foreach (string reportFormat in reportFormats)
            {
                IReportFormatter formatter = reportManager.GetFormatter(reportFormat);

                string reportFileName = FileUtils.EncodeFileName(String.Format(reportNameFormat,
                    report.PackageRun.StartTime.ToShortDateString(),
                    report.PackageRun.EndTime.ToLongTimeString()));
                string extension = formatter.PreferredExtension;
                if (extension.Length != 0)
                    reportFileName = String.Concat(reportFileName, @".", extension);
                string reportPath = Path.Combine(reportDirectory, reportFileName);

                generatedReportFilenames.Add(reportFormat.ToLower(CultureInfo.InvariantCulture), reportPath);
                progressMonitorProvider.Run(delegate(IProgressMonitor progressMonitor)
                {
                    progressMonitor.BeginTask(String.Format("Generating {0} report.", reportFormat), 1);

                    formatter.Format(report, reportPath, reportFormatOptions, null,
                        new SubProgressMonitor(progressMonitor, 1));
                });
            }
        }

        private static void CheckRequiredArgument(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        private void DisposeStopWatch()
        {
            logger.InfoFormat("Stop time: {0} (Total execution time: {1:#0.000} seconds)",
                DateTime.Now.ToShortTimeString(),
                stopWatch.Elapsed.TotalSeconds);
            stopWatch = null;
        }

        private void CreateStopWatch()
        {
            stopWatch = Stopwatch.StartNew();
            logger.InfoFormat("Start time: {0}", DateTime.Now.ToShortTimeString());
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// The IDisposable interface is implemented just to be able to use this
        /// class in a using statement.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}
