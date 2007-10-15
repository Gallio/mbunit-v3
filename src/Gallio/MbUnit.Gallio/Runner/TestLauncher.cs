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
using MbUnit.Core.ConsoleSupport;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Runner.Reports;
using MbUnit.Runner.Monitors;
using MbUnit.Hosting;
using MbUnit.Model;
using MbUnit.Model.Filters;
using MbUnit.Model.Serialization;

namespace MbUnit.Runner
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
    /// <see cref="Runtime" /> environment, set the <see cref="TestRunnerFactory" /> to
    /// create a <see cref="ITestRunner" /> other than the default <see cref="StandaloneTestRunner" />.
    /// </para>
    /// </summary>
    /// <todo>
    /// More validation of arguments up front.  Particularly report formats.
    /// </todo>
    public class TestLauncher : IDisposable
    {
        #region Private Members

        private TestPackage testPackage;
        private RuntimeSetup runtimeSetup;

        private readonly List<string> reportFormats;
        private readonly NameValueCollection reportFormatOptions;

        private IProgressMonitorProvider progressMonitorProvider;
        private ILogger logger;

        private Filter<ITest> filter;

        private TestRunnerFactory testRunnerFactory;
        private bool echoResults;
        private string templateModelFilename;
        private string testModelFilename;

        private string reportDirectory;
        private string reportNameFormat;

        private readonly ReportMonitor reportMonitor;
        private readonly List<ITestRunnerMonitor> customMonitors;

        #endregion

        /// <summary>
        /// Creates a launcher with default options and no test assemblies specified.
        /// </summary>
        public TestLauncher()
        {
            runtimeSetup = new RuntimeSetup();
            testPackage = new TestPackage();

            reportDirectory = @"";
            reportNameFormat = @"mbunit-{0}-{1}";
            reportFormats = new List<string>();
            reportFormatOptions = new NameValueCollection();

            progressMonitorProvider = NullProgressMonitorProvider.Instance;
            logger = NullLogger.Instance;

            filter = new AnyFilter<ITest>();

            reportMonitor = new ReportMonitor();
            customMonitors = new List<ITestRunnerMonitor>();
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
        /// Gets or sets the test filter to apply.
        /// </para>
        /// <para>
        /// The default filter is an instance of <see cref="AnyFilter{T}" />.
        /// </para>
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
        /// Gets or sets the test package.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TestPackage TestPackage
        {
            get { return testPackage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testPackage = value;
            }
        }

        /// <summary>
        /// Gets or sets the runtime setup.
        /// </summary>
        /// <remarks>
        /// The value of this property is only meaningful if <see cref="TestRunnerFactory" />
        /// is <c>null</c>.
        /// </remarks>
        /// <seealso cref="TestRunnerFactory"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public RuntimeSetup RuntimeSetup
        {
            get { return runtimeSetup; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                runtimeSetup = value;
            }
        }

        /// <summary>
        /// <para>
        /// Specifies a factory to use when constructing the <see cref="ITestRunner" /> at
        /// test execution time.
        /// </para>
        /// <para>
        /// The default value is <c>null</c> which indicates that the launcher should create
        /// a <see cref="StandaloneTestRunner" /> to automatically initialize and shutdown the
        /// runtime using the provided <see cref="RuntimeSetup" /> during test execution.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Runtimes cannot be nested.  So if the value of this property is <c>null</c> and
        /// the runtime has already been initialized, then an exception will be thrown when
        /// running the tests because the <see cref="StandaloneTestRunner" /> used by default
        /// will attempt to reinitialize the runtime unnecessarily.
        /// </remarks>
        /// <seealso cref="RuntimeSetup"/>
        public TestRunnerFactory TestRunnerFactory
        {
            get { return testRunnerFactory; }
            set { testRunnerFactory = value; }
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
        /// Gets or sets the name of a file to which the template model
        /// should be persisted, or <c>null</c> if none.
        /// </para>
        /// <para>
        /// The default value is <c>null</c>.
        /// </para>
        /// </summary>
        public string TemplateModelFilename
        {
            get { return templateModelFilename; }
            set { templateModelFilename = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets name of a file to which the test model should be persisted,
        /// or <c>null</c> if none.
        /// </para>
        /// <para>
        /// The default value is <c>null</c>.
        /// </para>
        /// </summary>
        public string TestModelFilename
        {
            get { return testModelFilename; }
            set { testModelFilename = value; }
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
        /// The default value is <c>"mbunit-{0}-{1}"</c>.
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
        /// Gets the test runner's report monitor.
        /// </para>
        /// <para>
        /// The report monitor fires events during test execution to allow test results
        /// to be retrieved incrementally as the test executes.
        /// </para>
        /// </summary>
        public ReportMonitor ReportMonitor
        {
            get { return reportMonitor; }
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
            get { return customMonitors; }
        }

        #region Public Methods

        /// <summary>
        /// Runs a project.
        /// </summary>
        /// <returns>An integer representing the result of the execution.</returns>
        public TestLauncherResult Run()
        {
            reportMonitor.ResetReport();
            TestLauncherResult result = new TestLauncherResult(reportMonitor.Report);

            if (!PrepareToRun(result))
                return result;

            using (ITestRunner runner = CreateRunner())
            {
                IReportManager reportManager = Runtime.Instance.Resolve<IReportManager>();
                if (!InitializeRunner(result, runner, reportManager))
                    return result;

                Stopwatch stopWatch = Stopwatch.StartNew();
                logger.InfoFormat("Start time: {0}", DateTime.Now.ToShortTimeString());

                // Run initial phases.
                if (!RunInitialPhases(result, runner))
                    return result;

                // Run tests.
                try
                {
                    RunTests(runner);

                    if (reportMonitor.Report.PackageRun.Statistics.FailureCount > 0)
                        result.SetResultCode(ResultCode.Failure);
                    else if (reportMonitor.Report.PackageRun.Statistics.TestCount == 0)
                        result.SetResultCode(ResultCode.NoTests);
                }
                catch (OperationCanceledException)
                {
                    result.SetResultCode(ResultCode.Canceled);
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

                return result;
            }
        }

        #endregion

        #region Private Methods

        private bool PrepareToRun(TestLauncherResult result)
        {
            Canonicalize();
            DisplayConfiguration();

            VerifyAssemblies();

            if (!HasTestAssemblies())
            {
                result.SetResultCode(ResultCode.NoTests);
                return false;
            }

            return true;
        }

        private ITestRunner CreateRunner()
        {
            if (testRunnerFactory != null)
                return testRunnerFactory();

            return new StandaloneTestRunner(runtimeSetup);
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
                LoadProject(runner);
                BuildTemplates(runner);
                BuildTests(runner);
                PersistTemplateTree(runner);
                PersistTestTree(runner);
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
                foreach (string assemblyName in testPackage.AssemblyFiles)
                {
                    if (!File.Exists(assemblyName))
                    {
                        assembliesToRemove.Add(assemblyName);
                        logger.Error("Cannot find assembly: {0}", assemblyName);
                    }
                }

                // Remove invalid assemblies
                foreach (string assemblyName in assembliesToRemove)
                    testPackage.AssemblyFiles.Remove(assemblyName);
            });
        }

        private void DisplayConfiguration()
        {
            DisplayPaths(testPackage.AssemblyFiles, "Test Assemblies:");
            DisplayPaths(testPackage.HintDirectories, "Hint Directories:");
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
            testPackage.ApplicationBase = CanonicalizePath(testPackage.ApplicationBase);
            CanonicalizePaths(testPackage.AssemblyFiles);
            CanonicalizePaths(testPackage.HintDirectories);

            CanonicalizePaths(runtimeSetup.PluginDirectories);
        }

        private static string CanonicalizePath(string path)
        {
            if (path == null)
                return null;

            return path.Length == 0 ? Environment.CurrentDirectory : Path.GetFullPath(path);
        }

        private static void CanonicalizePaths(IList<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
                paths[i] = CanonicalizePath(paths[i]);
        }

        private bool HasTestAssemblies()
        {
            if (testPackage.AssemblyFiles.Count == 0)
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
                runner.LoadPackage(testPackage, progressMonitor);
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

        private void GenerateReports(TestLauncherResult result, IReportManager reportManager)
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
                        new SubProgressMonitor(progressMonitor, 1));
                }

                // Save the full paths of the documents.
                foreach (string reportDocumentPath in reportWriter.ReportDocumentPaths)
                    result.AddReportDocumentPath(Path.Combine(reportDirectory, reportDocumentPath));
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
