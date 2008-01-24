// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.MSBuildTasks.Properties;
using Gallio.Collections;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger=Castle.Core.Logging.ILogger;

namespace Gallio.MSBuildTasks
{
    /// <summary>
    /// An MSBuild task that provides support for running Gallio tests.
    /// </summary>
    /// <remarks>
    /// In order for MSBuild to find this task, the Gallio.MSBuildTasks.dll has to be loaded with
    /// the UsingTask directive:
    /// <code>
    /// <![CDATA[
    /// <UsingTask AssemblyFile="[Path-to-assembly]\Gallio.MSBuildTasks.dll" TaskName="Gallio" />
    /// ]]>
    /// </code>
    /// The AssemblyFile attribute must be set to the path where the Gallio.MSBuildTasks.dll assembly resides,
    /// and the TaskName attribute <strong>must</strong> be set to "Gallio", otherwise MSBuild won't load the task.
    /// </remarks>
    /// <example>
    /// The following code is an example build file that shows how to load the task, specify the test assemblies
    /// and set some of the task's properties:
    /// <code>
    /// <![CDATA[
    /// <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    ///     <!-- This is needed by MSBuild to locate the Gallio task -->
    ///     <UsingTask AssemblyFile="[Path-to-assembly]\Gallio.MSBuildTasks.dll" TaskName="Gallio" />
    ///     <!-- Specify the tests assemblies -->
    ///     <ItemGroup>
    ///         <TestAssemblies Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
    ///         <TestAssemblies Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
    ///     </ItemGroup>
    ///     <Target Name="RunTests">
    ///         <Gallio IgnoreFailures="true" Filter="Type=SomeFixture" Assemblies="@(TestAssemblies)">
    ///             <!-- This tells MSBuild to store the output value of the task's ExitCode property
    ///                  into the project's ExitCode property -->
    ///             <Output TaskParameter="ExitCode" PropertyName="ExitCode"/>
    ///         </Gallio>
    ///         <Error Text="Tests execution failed" Condition="'$(ExitCode)' != 0" />
    ///     </Target>
    /// </Project>
    /// ]]>
    /// </code>
    /// </example>
    public class Gallio : Task
    {
        /// Internal comment. In the MSBuild the class name is also the custom task
        /// name, so we named this class "Gallio" to make things easier to the user.

        #region Private Members

        private ITaskItem[] assemblies;
        private ITaskItem[] pluginDirectories;
        private ITaskItem[] hintDirectories;
        private string filter = "*";
        private string[] reportTypes = new string[] { };
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string runnerType = StandardTestRunnerFactoryNames.IsolatedAppDomain;
        private bool ignoreFailures;
        private bool showReports;
        private bool doNotRun;
        private bool echoResults = true;
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
        /// The list of relative or absolute paths of test assembly files to execute. This is required.
        /// </summary>
        /// <example>The following example shows how to specify the test assemblies (for a more complete example
        /// please see the <see cref="Gallio"/> task documentation):
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <TestAssemblies Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
        ///     <TestAssemblies Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio Assemblies="@(TestAssemblies)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        [Required]
        public ITaskItem[] Assemblies
        {
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        /// <example>The following example shows how to specify the hint directories:
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <HintDirectories Include="[Path-to-test-hint-directory-1]/" />
        ///     <HintDirectories Include="[Path-to-test-hint-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio HintDirectories="@(HintDirectories)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public ITaskItem[] HintDirectories
        {
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>The following example shows how to specify the plugins directories:
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <PluginDirectories Include="[Path-to-test-plugin-directory-1]/" />
        ///     <PluginDirectories Include="[Path-to-test-plugin-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio PluginDirectories="@(PluginDirectories)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public ITaskItem[] PluginDirectories
        {
            set { pluginDirectories = value; }
        }

        /// <summary>
        /// A list of the types of reports to generate, separated by semicolons. 
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: Html, Html-Inline, Text, XHtml,
        /// XHtml-Inline, Xml, and Xml-Inline, but more types could be available as plugins.</item>
        /// <item>The report types are not case sensitive.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// In the following example reports will be generated in both HTML and XML format:
        /// <code>
        /// <![CDATA[
        /// <Target Name="MyTarget">
        ///     <Gallio ReportTypes="html;xml" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public string[] ReportTypes
        {
            set { reportTypes = value; }
        }

        /// <summary>
        /// Sets the format string to use to generate the reports filenames.
        /// </summary>
        /// <remarks>
        /// Any occurence of {0} will be replaced by the date, and any occurrence of {1} by the time.
        /// The default format string is test-report-{0}-{1}.
        /// </remarks>
        public string ReportNameFormat
        {
            set { reportNameFormat = value; }
        }

        /// <summary>
        /// Sets the name of the directory where the reports will be put.
        /// </summary>
        /// <remarks>
        /// The directory will be created if it doesn't exist. Existing files will be overwrited.
        /// </remarks>
        public string ReportDirectory
        {
            set { reportDirectory = value; }
        }

        /// <summary>
        /// Sets the type of test runner to use.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: LocalAppDomain, IsolatedAppDomain (default)
        /// and IsolatedProcess, but more types could be available as plugins.</item>
        /// <item>The runner types are not case sensitive.</item>
        /// </list>
        /// </remarks>
        public string RunnerType
        {
            set { runnerType = value; }
        }

        /// <summary>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/summary/*' />
        /// </summary>
        /// <remarks>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/remarks/*' />
        /// </remarks>
        /// <example>
        /// <include file='../../../Gallio/docs/FilterSyntax.xml' path='doc/example/*' />
        /// </example>
        public string Filter
        {
            set { filter = value; }
        }

        /// <summary>
        /// Sets whether test failures will be ignored and allow the build to proceed.
        /// When set to <c>false</c>, test failures will cause the build to fail.
        /// </summary>
        public bool IgnoreFailures
        {
            set { ignoreFailures = value; }
        }

        /// <summary>
        /// Sets whether to show generated reports in a window using the default system application
        /// registered to the report file type.
        /// </summary>
        public bool ShowReports
        {
            set { showReports = value; }
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        public bool DoNotRun
        {
            set { doNotRun = value; }
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  If this option is set
        /// to true, the default, test results are echoed to the console
        /// in varying detail depending on the current verbosity level.  Otherwise
        /// only final summary statistics are displayed.
        /// </summary>
        public bool EchoResults
        {
            set { echoResults = value; }
        }

        /// <summary>
        /// Gets the exit code of the tests execution.
        /// </summary>
        /// <remarks>
        /// This property is only meaningful when the IgnoreFailures property is set to true.
        /// </remarks>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's ExitCode output property will
        ///           be made available as a property called ExitCode in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="ExitCode" PropertyName="ExitCode"/>
        /// </Gallio>
        /// <!-- After the exit code be retrieved and used like this: -->
        /// <Error Text="The tests execution failed" Condition="'$(ExitCode)' != 0" />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int ExitCode
        {
            get { return exitCode; }
        }

        /// <summary>
        /// Gets the total number of test cases.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's TestCount output property will
        ///           be made available as a property called TestCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="TestCount" PropertyName="TestCount" />
        /// </Gallio>
        /// <!-- After execution the number of tests run can be retrieved like this: -->
        /// <Message Text="$(TestCount) tests were run." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int TestCount
        {
            get { return testCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that were run and passed.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's PassCount output property will
        ///           be made available as a property called PassCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="TestCount" PropertyName="PassCount" />
        /// </Gallio>
        /// <!-- After execution the number of passed tests can be retrieved like this: -->
        /// <Message Text="$(PassCount) tests passed." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int PassCount
        {
            get { return passCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that were run and failed.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's FailCount output property will
        ///           be made available as a property called FailCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="FailCount" PropertyName="FailCount" />
        /// </Gallio>
        /// <!-- After execution the number of failed tests can be retrieved like this: -->
        /// <Message Text="$(FailCount) tests passed." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int FailureCount
        {
            get { return failureCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that did not run because they were ignored.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's IgnoreCount output property will
        ///           be made available as a property called IgnoreCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="IgnoreCount" PropertyName="IgnoreCount" />
        /// </Gallio>
        /// <!-- After execution the number of ignored tests can be retrieved like this: -->
        /// <Message Text="$(IgnoreCount) tests were ignored." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int IgnoreCount
        {
            get { return ignoreCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that ran and were inconclusive.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's InconclusiveCount output property will
        ///           be made available as a property called InconclusiveCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="InconclusiveCount" PropertyName="InconclusiveCount" />
        /// </Gallio>
        /// <!-- After execution the number of inconclusive tests can be retrieved like this: -->
        /// <Message Text="$(InconclusiveCount) tests were inconclusive." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int InconclusiveCount
        {
            get { return inconclusiveCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that were run.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's RunCount output property will
        ///           be made available as a property called RunCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="RunCount" PropertyName="RunCount" />
        /// </Gallio>
        /// <!-- After execution the number of tests run can be retrieved like this: -->
        /// <Message Text="$(RunCount) tests were run." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int RunCount
        {
            get { return runCount; }
        }

        /// <summary>
        /// Gets the total number of test cases that did not run because they were skipped.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's SkipCount output property will
        ///           be made available as a property called SkipCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="SkipCount" PropertyName="SkipCount" />
        /// </Gallio>
        /// <!-- After execution the number of skipped tests can be retrieved like this: -->
        /// <Message Text="$(SkipCount) tests were skipped." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int SkipCount
        {
            get { return skipCount; }
        }

        /// <summary>
        /// Gets the duration of the tests execution in seconds.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's Duration output property will
        ///           be made available as a property called Duration in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="Duration" PropertyName="Duration" />
        /// </Gallio>
        /// <!-- After execution the duration can be retrieved like this: -->
        /// <Message Text="The tests took $(Duration)s to execute." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public double Duration
        {
            get { return duration; }
        }

        /// <summary>
        /// Gets the number of assertions evaluated.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's AssertionCount output property will
        ///           be made available as a property called AssertionCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="AssertionCount" PropertyName="AssertionCount" />
        /// </Gallio>
        /// <!-- After execution the number of assertions can be retrieved like this: -->
        /// <Message Text="$(AssertionCount) assertions were evaluated." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int AssertCount
        {
            get { return assertCount; }
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
                Log.LogError(Resources.UnexpectedErrorDuringExecution);
                Log.LogErrorFromException(ex, true, true, null);
                return ignoreFailures;
            }
        }

        #endregion

        #region Private Methods

        private bool InternalExecute()
        {
            DisplayVersion();

            TaskLogger logger = new TaskLogger(Log);

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
                launcher.Filter = GetFilter();
                launcher.ShowReports = showReports;
                launcher.DoNotRun = doNotRun;
                launcher.RuntimeSetup = new RuntimeSetup();

                if (echoResults)
                    launcher.CustomMonitors.Add(new TaskTestRunnerMonitor(Log, launcher.ReportMonitor));

                AddAllItemSpecs(launcher.TestPackageConfig.AssemblyFiles, assemblies);
                AddAllItemSpecs(launcher.TestPackageConfig.HintDirectories, hintDirectories);
                AddAllItemSpecs(launcher.RuntimeSetup.PluginDirectories, pluginDirectories);

                if (reportDirectory != null)
                    launcher.ReportDirectory = reportDirectory;
                if (!String.IsNullOrEmpty(reportNameFormat))
                    launcher.ReportNameFormat = reportNameFormat;
                if (reportTypes != null)
                    GenericUtils.AddAll(reportTypes, launcher.ReportFormats);

                launcher.TestRunnerFactoryName = runnerType;

                TestLauncherResult result = RunLauncher(launcher);
                exitCode = result.ResultCode;

                LogResultSummary(logger, result);
                PopulateStatistics(result);

                if (ExitCode == ResultCode.Success ||
                    ExitCode == ResultCode.NoTests ||
                    ignoreFailures)
                    return true;
            }

            return false;
        }

        /// <exclude />
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

            return FilterUtils.ParseTestFilter(filter);
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
