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
    /// A custom MSBuild Task that allows to run MbUnit from MSBuild.
    /// </summary>
    /// <remarks>
    /// In order for MSBuild to find this task, the MbUnit.Tasks.MSBuild.dll has to be loaded with
    /// the UsingTask task:
    /// <code>
    /// <![CDATA[
    /// <UsingTask AssemblyFile="[Path-to-assembly]\MbUnit.Tasks.MSBuild.dll" TaskName="MbUnit" />
    /// ]]>
    /// </code>
    /// The AssemblyFile attribute must be set to the path where the MbUnit.Tasks.MSBuild.dll assembly resides,
    /// and the TaskName attribute <strong>must</strong> be set to "MbUnit", otherwise MSBuild won't load the task.
    /// </remarks>
    /// <example>
    /// The following code is an example build file that shows how to load the task, specify the test assemblies
    /// and set some of the task's properties:
    /// <code>
    /// <![CDATA[
    /// <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    ///     <!-- This is need by MSBuild to locate the MbUnit task -->
    ///     <UsingTask AssemblyFile="[Path-to-assembly]\MbUnit.Tasks.MSBuild.dll" TaskName="MbUnit" />
    ///     <!-- Specify the tests assemblies -->
    ///     <ItemGroup>
    ///         <TestAssemblies Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
    ///         <TestAssemblies Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
    ///     </ItemGroup>
    ///     <Target Name="MyTarget">
    ///         <MbUnit IgnoreFailures="true" Filter="Type=PassingTests" Assemblies="@(TestAssemblies)">
    ///             <!-- This tells MSBuild to store the output value of the task's ExitCode property
    ///                  into the project's MbUnit.ExitCode property -->
    ///             <Output TaskParameter="ExitCode" PropertyName="MbUnit.ExitCode"/>
    ///         </MbUnit>
    ///         <Error Text="Tests execution failed" Condition="'$(MbUnit.ExitCode)' != 0" />
    ///     </Target>
    /// </Project>
    /// ]]>
    /// </code>
    /// </example>
    public class MbUnit : Task
    {
        /// Internal comment. In the MSBuild the class name is also the custom task
        /// name, so we named this class "MbUnit" to make things easier to the user.

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
        /// The list of relative or absolute paths of test assembly files to execute. This is required.
        /// </summary>
        /// <example>The following example shows how to specify the test assemblies (for a more complete example
        /// please see the <see cref="MbUnit"/> task documentation):
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <TestAssemblies Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
        ///     <TestAssemblies Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <MbUnit Assemblies="@(TestAssemblies)" />
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
        /// <!-- -->
        /// <ItemGroup>
        ///     <HintDirectories Include="[Path-to-test-hint-directory-1]/" />
        ///     <HintDirectories Include="[Path-to-test-hint-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <MbUnit HintDirectories="@(HintDirectories)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public ITaskItem[] HintDirectories
        {
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional MbUnit plugin directories to search recursively.
        /// </summary>
        /// <example>The following example shows how to specify the plugins directories:
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <PluginDirectories Include="[Path-to-test-plugin-directory-1]/" />
        ///     <PluginDirectories Include="[Path-to-test-plugin-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <MbUnit PluginDirectories="@(PluginDirectories)" />
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
        /// <list>
        /// <item>The types supported "out of the box" are: Html, Html-Inline, Text, XHtml,
        /// XHtml-Inline, Xml, and Xml-Inline, but more types could be available as plugins.</item>
        /// <item>This property is not case sentitive.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// In the following example reports will be generated in both HTML and XML format.
        /// <code>
        /// <![CDATA[
        /// <Target Name="MyTarget">
        ///     <MbUnit ReportTypes="html;xml" />
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
        /// The default format string is mbunit-{0}-{1}.
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
        /// Sets the filter to apply in the format "property=value;property=value;..."
        /// If left empty the "Any" filter will be applied.
        /// </summary>
        public string Filter
        {
            set { filter = value; }
        }

        /// <summary>
        /// Sets whether or not to halt on failure.
        /// </summary>
        public bool IgnoreFailures
        {
            set { ignoreFailures = value; }
        }

        /// <summary>
        /// Gets the exit code of the tests execution.
        /// </summary>
        /// <remarks>
        /// This property is only meaningful when the IgnoreFailures property is set to true.
        /// </remarks>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's ExitCode output property will
        ///           be made available as a property called MbUnitExitCode in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="ExitCode" PropertyName="MbUnitExitCode"/>
        /// </MbUnit>
        /// <!-- After the exit code be retrieved and used like this: -->
        /// <Error Text="The tests execution failed" Condition="'$(MbUnitExitCode)' != 0" />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's TestCount output property will
        ///           be made available as a property called MbUnitTestCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="TestCount" PropertyName="MbUnitTestCount" />
        /// </MbUnit>
        /// <!-- After execution the number of tests run can be retrieved like this: -->
        /// <Message Text="$(MbUnitTestCount) tests were run." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's PassCount output property will
        ///           be made available as a property called MbUnitPassCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="TestCount" PropertyName="MbUnitPassCount" />
        /// </MbUnit>
        /// <!-- After execution the number of passed tests can be retrieved like this: -->
        /// <Message Text="$(MbUnitPassCount) tests passed." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's FailCount output property will
        ///           be made available as a property called MbUnitFailCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="FailCount" PropertyName="MbUnitFailCount" />
        /// </MbUnit>
        /// <!-- After execution the number of failed tests can be retrieved like this: -->
        /// <Message Text="$(MbUnitFailCount) tests passed." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's IgnoreCount output property will
        ///           be made available as a property called MbUnitIgnoreCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="IgnoreCount" PropertyName="MbUnitIgnoreCount" />
        /// </MbUnit>
        /// <!-- After execution the number of ignored tests can be retrieved like this: -->
        /// <Message Text="$(MbUnitIgnoreCount) tests were ignored." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's InconclusiveCount output property will
        ///           be made available as a property called MbUnitInconclusiveCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="InconclusiveCount" PropertyName="MbUnitInconclusiveCount" />
        /// </MbUnit>
        /// <!-- After execution the number of inconclusive tests can be retrieved like this: -->
        /// <Message Text="$(MbUnitInconclusiveCount) tests were inconclusive." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's RunCount output property will
        ///           be made available as a property called MbUnitRunCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="RunCount" PropertyName="MbUnitRunCount" />
        /// </MbUnit>
        /// <!-- After execution the number of tests run can be retrieved like this: -->
        /// <Message Text="$(MbUnitRunCount) tests were run." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's SkipCount output property will
        ///           be made available as a property called MbUnitSkipCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="SkipCount" PropertyName="MbUnitSkipCount" />
        /// </MbUnit>
        /// <!-- After execution the number of skipped tests can be retrieved like this: -->
        /// <Message Text="$(MbUnitSkipCount) tests were skipped." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's Duration output property will
        ///           be made available as a property called MbUnitDuration in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="Duration" PropertyName="MbUnitDuration" />
        /// </MbUnit>
        /// <!-- After execution the duration can be retrieved like this: -->
        /// <Message Text="The tests took $(MbUnitDuration)s to execute." />
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
        /// MbUnit tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <MbUnit>
        ///      <!-- This tells MSBuild that the task's AssertionCount output property will
        ///           be made available as a property called MbUnitAssertionCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="AssertionCount" PropertyName="MbUnitAssertionCount" />
        /// </MbUnit>
        /// <!-- After execution the number of assertions can be retrieved like this: -->
        /// <Message Text="$(MbUnitAssertionCount) assertions were evaluated." />
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
                Log.LogError(Resources.UnexpectedFailureDuringMbUnitExecution);
                Log.LogErrorFromException(ex, true);
                return ignoreFailures;
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

                if (reportDirectory != null)
                    launcher.ReportDirectory = reportDirectory;
                if (reportNameFormat != null)
                    launcher.ReportNameFormat = reportNameFormat;

                if (reportTypes != null)
                    GenericUtils.AddAll(reportTypes, launcher.ReportFormats);

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
