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
using System.Reflection;
using MbUnit.Collections;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Hosting;
using MbUnit.Runner;
using MbUnit.Model.Filters;
using MbUnit.Model;
using MbUnit.Runner.Reports;
using MbUnit.Tasks.MSBuild.Properties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger = Castle.Core.Logging.ILogger;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// A MSBuild Task implementation that allows to run MbUnit.
    /// </summary>
    /// <example>
    /// </example>
    public class MbUnit : Task
    {
        /// Internal comment. In the MSBuild the class name is also the custom task
        /// name, so we named this class "MbUnit" to be more friendly to the user.

        #region Private Members

        private ITaskItem[] assemblies;
        private ITaskItem[] pluginDirectories;
        private ITaskItem[] hintDirectories;
        private string filter;
        private string[] reportTypes = new string[] { };
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private bool ignoreFailures = false;
        private int exitCode;
        private int testCount;
        private int passCount;
        private int failureCount;
        private int ignoreCount;
        private int inconclusiveCount;
        private int runCount;
        private int skipCount;
        private double duration;
        private int assertCount;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of test assemblies to execute. This is required.
        /// </summary>
        [Required]
        public ITaskItem[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        public ITaskItem[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        /// <summary>
        ///  Additional MbUnit plugin directories to search recursively.
        /// </summary>
        public ITaskItem[] PluginDirectories
        {
            get { return pluginDirectories; }
            set { pluginDirectories = value; }
        }

        /// <summary>
        /// An array of report types to generate.
        /// </summary>
        public string[] ReportTypes
        {
            get { return reportTypes; }
            set { reportTypes = value; }
        }

        /// <summary>
        /// A format string to use to generate the reports filename.
        /// </summary>
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set { reportNameFormat = value; }
        }

        /// <summary>
        /// The directory where the reports will be put.
        /// </summary>
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set { reportDirectory = value; }
        }

        /// <summary>
        /// The filter to apply in the format "property=value;property=value;..."
        /// If left empty the "Any" filter will be applied.
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        /// <summary>
        /// Whether or not to halt on failure.
        /// </summary>
        public bool IgnoreFailures
        {
            get { return ignoreFailures; }
            set { ignoreFailures = value; }
        }

        /// <summary>
        /// The exit code of the tests execution.
        /// </summary>
        [Output]
        public int ExitCode
        {
            get { return exitCode; }
            set { exitCode = value; }
        }

        /// <summary>
        /// The total number of test cases.
        /// </summary>
        [Output]
        public int TestCount
        {
            get { return testCount; }
            set { testCount = value; }
        }

        /// <summary>
        /// The total number of test cases that were run and passed.
        /// </summary>
        [Output]
        public int PassCount
        {
            get { return passCount; }
            set { passCount = value; }
        }

        /// <summary>
        /// The total number of test cases that were run and failed.
        /// </summary>
        [Output]
        public int FailureCount
        {
            get { return failureCount; }
            set { failureCount = value; }
        }

        /// <summary>
        /// The total number of test cases that did not run because they were ignored.
        /// </summary>
        [Output]
        public int IgnoreCount
        {
            get { return ignoreCount; }
            set { ignoreCount = value; }
        }

        /// <summary>
        /// The total number of test cases that ran and were inconclusive.
        /// </summary>
        [Output]
        public int InconclusiveCount
        {
            get { return inconclusiveCount; }
            set { inconclusiveCount = value; }
        }

        /// <summary>
        /// The total number of test cases that were run.
        /// </summary>
        [Output]
        public int RunCount
        {
            get { return runCount; }
            set { runCount = value; }
        }

        /// <summary>
        /// The total number of test cases that did not run because they were skipped.
        /// </summary>
        [Output]
        public int SkipCount
        {
            get { return skipCount; }
            set { skipCount = value; }
        }

        /// <summary>
        /// Duration of the tests execution in seconds.
        /// </summary>
        [Output]
        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// Number of assertions evaluated.
        /// </summary>
        [Output]
        public int AssertCount
        {
            get { return assertCount; }
            set { assertCount = value; }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override bool Execute()
        {
            try
            {
                return InternalExecute();
            }
            catch (Exception ex)
            {
                Log.LogError(Resources.UnexpectedFailureDuringMbUnitExecution);
                Log.LogErrorFromException(ex, true);
                return IgnoreFailures;
            }
        }

        #endregion

        #region Private Methods

        private bool InternalExecute()
        {
            DisplayVersion();

            MSBuildLogger logger = new MSBuildLogger(Log);

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
                launcher.Filter = GetFilter();
                launcher.RuntimeSetup = new RuntimeSetup();

                AddAllItemSpecs(launcher.TestPackage.AssemblyFiles, assemblies);
                AddAllItemSpecs(launcher.TestPackage.HintDirectories, hintDirectories);
                AddAllItemSpecs(launcher.RuntimeSetup.PluginDirectories, pluginDirectories);

                if (ReportDirectory != null)
                    launcher.ReportDirectory = ReportDirectory;
                if (ReportNameFormat != null)
                    launcher.ReportNameFormat = ReportNameFormat;

                if (ReportTypes != null)
                    GenericUtils.AddAll(ReportTypes, launcher.ReportFormats);

                TestLauncherResult result = RunLauncher(launcher);
                exitCode = result.ResultCode;

                LogResultSummary(logger, result);
                PopulateStatistics(result);

                if (ExitCode == ResultCode.Success ||
                    ExitCode == ResultCode.NoTests ||
                    IgnoreFailures)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Provided so that the unit tests can override test execution behavior.
        /// </summary>
        protected virtual TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            return launcher.Run();
        }

        private void PopulateStatistics(TestLauncherResult result)
        {
            PackageRunStatistics stats = result.Statistics;
            testCount = stats.TestCount;
            passCount = stats.PassCount;
            failureCount = stats.FailureCount;
            ignoreCount = stats.IgnoreCount;
            inconclusiveCount = stats.InconclusiveCount;
            runCount = stats.RunCount;
            skipCount = stats.SkipCount;
            duration = stats.Duration;
            assertCount = stats.AssertCount;
        }

        private static void LogResultSummary(ILogger logger, TestLauncherResult result)
        {
            switch (result.ResultCode)
            {
                case ResultCode.Success:
                    logger.Info(result.ResultSummary);
                    break;

                case ResultCode.Failure:
                    logger.Error(result.ResultSummary);
                    break;
            }
        }

        private Filter<ITest> GetFilter()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return new AnyFilter<ITest>();
            }
            return FilterParser.ParseFilterList<ITest>(filter);
        }

        private void DisplayVersion()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log.LogMessage(String.Format(Resources.TaskNameAndVersion,
                                         appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        private static void AddAllItemSpecs(ICollection<string> collection, IEnumerable<ITaskItem> items)
        {
            if (items != null)
            {
                foreach (ITaskItem item in items)
                    collection.Add(item.ItemSpec);
            }
        }

        #endregion
    }
}
