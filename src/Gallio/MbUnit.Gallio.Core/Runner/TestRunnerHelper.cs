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
using System.IO;
using System.Diagnostics;
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
        private readonly ProgressMonitorCreator progressMonitorCreator;
        private readonly RuntimeSetup runtimeSetup;
        private readonly LevelFilteredLogger logger;
        private readonly Filter<ITest> filter;

        private string templateModelFilename;
        private string testModelFilename;

        private string reportDirectory = "";
        private string reportNameFormat = "mbunit-{0}-{1}";
        private List<string> reportFormats;
        private NameValueCollection reportFormatOptions;

        private Stopwatch stopWatch;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the TestRunnerHelper class.
        /// </summary>
        /// <param name="progressMonitorCreator">A delegate to a rutine that will be
        /// called to create progress monitors</param>
        /// <param name="logger">A LevelFilteredLogger instance to log messages to.</param>
        /// <param name="filter">The filter to apply to the tests.</param>
        public TestRunnerHelper(
            ProgressMonitorCreator progressMonitorCreator,
            LevelFilteredLogger logger,
            Filter<ITest> filter)
        {
            CheckRequiredArgument(progressMonitorCreator, "progressMonitorCreator");
            CheckRequiredArgument(filter, "filter");
            CheckRequiredArgument(logger, "logger");

            runtimeSetup = new RuntimeSetup();
            package = new TestPackage();

            this.progressMonitorCreator = progressMonitorCreator;
            this.filter = filter;
            this.logger = logger;

            reportFormats = new List<string>();
            reportFormatOptions = new NameValueCollection();
        }

        /// <summary>
        /// Initializes a new instance of the TestRunnerHelper class.
        /// </summary>
        /// <param name="progressMonitorCreator">A delegate to a rutine that will be
        /// called to create progress monitors</param>
        /// <param name="logger">A LevelFilteredLogger instance to log messages to.</param>
        /// <param name="filter">A string representation of the filter to apply to
        /// the tests.</param>
        public TestRunnerHelper(
            ProgressMonitorCreator progressMonitorCreator,
            LevelFilteredLogger logger,
            string filter)
            : this(progressMonitorCreator,
                   logger,
                   FilterParser.ParseFilterList<ITest>(filter))
        {
        }

        #endregion

        #region Public Delegates

        /// <summary>
        /// Defines a method that is able to create IProgressMonitor objects
        /// </summary>
        /// <returns></returns>
        public delegate IProgressMonitor ProgressMonitorCreator();

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs a project
        /// </summary>
        /// <returns>An integer representing the result of the execution.</returns>
        public int Run()
        {
            Canonicalize();

            using (AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup))
            {
                IReportManager reportManager = runner.Runtime.Resolve<IReportManager>();

                DisplayConfiguration();
                CreateStopWatch();

                if (!Validate(reportManager))
                    return ResultCode.InvalidArguments;

                VerifyAssemblies();
                if (!HasTestAssemblies())
                    return ResultCode.NoTests;

                StringWriter debugWriter = null;
                if (logger.IsDebugEnabled)
                {
                    debugWriter = new StringWriter();
                    new DebugMonitor(debugWriter).Attach(runner);
                }

                ReportMonitor reportMonitor = new ReportMonitor();
                reportMonitor.Attach(runner);

                // Run the initial phases.
                try
                {
                    ApplyFilter(runner);
                    LoadProject(runner);
                    BuildTemplates(runner);
                    BuildTests(runner);
                    PersistTemplateTree(runner);
                    PersistTestTree(runner);
                }
                catch (OperationCanceledException)
                {
                    return ResultCode.Canceled;
                }
                finally
                {
                    ConsoleCancelHandler.IsCanceled = false;
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
                finally
                {
                    ConsoleCancelHandler.IsCanceled = false;
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
                finally
                {
                    ConsoleCancelHandler.IsCanceled = false;
                }

                logger.Info(reportMonitor.Report.PackageRun.Statistics.FormatTestCaseResultSummary());
                DisposeStopWatch();

                // Make sure we write out debug log messages.
                if (!runCanceled && debugWriter != null)
                    logger.Debug(debugWriter.ToString());

                if (reportMonitor.Report.PackageRun.Statistics.FailureCount > 0)
                    return ResultCode.Failure;
                if (runCanceled)
                    return ResultCode.Canceled;

                return ResultCode.Success;
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
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                progressMonitor.BeginTask("Verifying assembly names.", 1);

                List<string> assembliesToRemove = new List<string>();
                foreach (string assemblyName in package.AssemblyFiles)
                {
                    if (!File.Exists(assemblyName))
                    {
                        assembliesToRemove.Add(assemblyName);
                        logger.Error("Test assembly {0} cannot be found", assemblyName);
                        logger.Error("Full name: {0}", Path.GetFullPath(assemblyName));
                    }
                }

                // Remove invalid assemblies
                foreach (string assemblyName in assembliesToRemove)
                    package.AssemblyFiles.Remove(assemblyName);
            }
        }

        private void DisplayConfiguration()
        {
            DisplayPaths(package.AssemblyFiles, "Test assemblies:");
            DisplayPaths(package.HintDirectories, "Hint Directories:");
            DisplayPaths(runtimeSetup.PluginDirectories, "Plugin Directories:");
        }

        private void DisplayPaths(ICollection<string> paths, string name)
        {
            if (paths != null && paths.Count > 0)
            {
                logger.Info(name);
                foreach (string path in paths)
                    logger.Info("\t{0}", path);
            }
        }

        #endregion

        #region Private Methods

        private void Canonicalize()
        {
            package.ApplicationBase = CanonicalizePath(package.ApplicationBase);
            CanonicalizePaths(package.AssemblyFiles);
            CanonicalizePaths(package.HintDirectories);

            CanonicalizePaths(runtimeSetup.PluginDirectories);
        }

        private static string CanonicalizePath(string path)
        {
            if (path == "")
                path = ".";
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
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                progressMonitor.ThrowIfCanceled();
                runner.LoadPackage(package, progressMonitor);
            }
        }

        private void BuildTemplates(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                progressMonitor.ThrowIfCanceled();
                runner.BuildTemplates(progressMonitor);
            }
        }

        private void BuildTests(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                progressMonitor.ThrowIfCanceled();
                runner.BuildTests(progressMonitor);
            }
        }

        private void RunTests(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                progressMonitor.ThrowIfCanceled();
                runner.Run(progressMonitor);
            }
        }

        private void PersistTemplateTree(ITestRunner runner)
        {
            if (templateModelFilename != null)
            {
                using (IProgressMonitor progressMonitor = progressMonitorCreator())
                {
                    progressMonitor.ThrowIfCanceled();
                    progressMonitor.BeginTask("Saving template tree.", 1);
                    progressMonitor.SetStatus(templateModelFilename);
                    SerializationUtils.SaveToXml(runner.TemplateModel, templateModelFilename);
                }
            }
        }

        private void PersistTestTree(ITestRunner runner)
        {
            if (testModelFilename != null)
            {
                using (IProgressMonitor progressMonitor = progressMonitorCreator())
                {
                    progressMonitor.ThrowIfCanceled();
                    progressMonitor.BeginTask("Saving test tree.", 1);
                    progressMonitor.SetStatus(testModelFilename);
                    SerializationUtils.SaveToXml(runner.TestModel, testModelFilename);
                }
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
                    reportFileName = String.Concat(reportFileName, ".", extension);
                string reportPath = Path.Combine(reportDirectory, reportFileName);

                using (IProgressMonitor progressMonitor = progressMonitorCreator())
                {
                    progressMonitor.ThrowIfCanceled();
                    progressMonitor.BeginTask("Generating " + reportFormat + " report.", 1);

                    formatter.Format(report, reportPath, reportFormatOptions, null,
                        new SubProgressMonitor(progressMonitor, 1));
                }
            }
        }

        private static void CheckRequiredArgument(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        private void DisposeStopWatch()
        {
            logger.InfoFormat("\nStop time: {0}", DateTime.Now.ToShortTimeString());
            logger.InfoFormat("Total execution time: {0:#0.000}s", stopWatch.Elapsed.TotalSeconds);
            stopWatch = null;
        }

        private void CreateStopWatch()
        {
            stopWatch = Stopwatch.StartNew();
            logger.InfoFormat("\nStart time: {0}\n", DateTime.Now.ToShortTimeString());
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
