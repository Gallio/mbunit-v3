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
using System.IO;
using System.Reflection;
using Gallio.Common.Policies;
using Gallio.Common.Text;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.MSBuildTasks.Properties;
using Gallio.Common.Collections;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using ILogger=Gallio.Runtime.Logging.ILogger;

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
    /// The following code is an example build file that shows how to load the task, specify the test files
    /// and assemblies and set some of the task's properties:
    /// <code>
    /// <![CDATA[
    /// <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    ///     <!-- This is needed by MSBuild to locate the Gallio task -->
    ///     <UsingTask AssemblyFile="[Path-to-assembly]\Gallio.MSBuildTasks.dll" TaskName="Gallio" />
    ///     <!-- Specify the test files and assemblies -->
    ///     <ItemGroup>
    ///       <TestFile Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
    ///       <TestFile Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
    ///       <TestFile Include="[Path-to-test-script1]/TestScript1_spec.rb" />
    ///       <TestFile Include="[Path-to-test-script2]/TestScript2.xml" />
    ///     </ItemGroup>
    ///     <Target Name="RunTests">
    ///         <Gallio IgnoreFailures="true" Filter="Type=SomeFixture" Files="@(TestFile)">
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
        private TimeSpan? runTimeLimit;
        private Verbosity verbosity = Runtime.Logging.Verbosity.Normal;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Gallio()
        {
            EchoResults = true;
            Filter = string.Empty;
            ReportFormatterProperties = EmptyArray<string>.Instance;
            RunnerProperties = EmptyArray<string>.Instance;
            RunnerExtensions = EmptyArray<string>.Instance;
            ReportTypes = EmptyArray<string>.Instance;
        }

        /// <summary>
        /// The list of relative or absolute paths of test files, projects and assemblies to execute.
        /// Wildcards may be used.  This is required.
        /// </summary>
        /// <example>The following example shows how to specify the test files, projects and assemblies
        /// (for a more complete example please see the <see cref="Gallio"/> task documentation):
        /// <code>
        /// <![CDATA[
        /// <!-- Specify the test files, projects and assemblies -->
        /// <ItemGroup>
        ///     <TestFile Include="[Path-to-test-assembly1]/TestAssembly1.dll" />
        ///     <TestFile Include="[Path-to-test-assembly2]/TestAssembly2.dll" />
        ///     <TestFile Include="[Path-to-test-script1]/TestScript1_spec.rb" />
        ///     <TestFile Include="[Path-to-test-script2]/TestScript2.xml" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio Files="@(TestFile)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        [Required]
        public ITaskItem[] Files
        {
            private get;
            set;
        }

        /// <summary>
        /// The list of directories used for loading referenced assemblies and other dependent resources.
        /// </summary>
        /// <example>The following example shows how to specify the hint directories:
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <HintDirectory Include="[Path-to-test-hint-directory-1]/" />
        ///     <HintDirectory Include="[Path-to-test-hint-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio HintDirectories="@(HintDirectory)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public ITaskItem[] HintDirectories
        {
            private get;
            set;
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>The following example shows how to specify the plugins directories:
        /// <code>
        /// <![CDATA[
        /// <ItemGroup>
        ///     <PluginDirectory Include="[Path-to-test-plugin-directory-1]/" />
        ///     <PluginDirectory Include="[Path-to-test-plugin-directory-2]/" />
        /// </ItemGroup>
        /// <Target Name="MyTarget">
        ///     <Gallio PluginDirectories="@(PluginDirectory)" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public ITaskItem[] PluginDirectories
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Gets or sets the relative or absolute path of the application base directory,
        /// or null to use a default value selected by the consumer.
        /// </para>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// The default is null.
        /// </para>
        /// </summary>
        public ITaskItem ApplicationBaseDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Gets or sets the relative or absolute path of the working directory
        /// or null to use a default value selected by the consumer.
        /// </para>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// The default is null.
        /// </para>
        /// </summary>
        public ITaskItem WorkingDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Enables shadow copying when set to true.
        /// </para>
        /// <para>
        /// Shadow copying allows the original assemblies to be modified while the tests are running.
        /// However, shadow copying may occasionally cause some tests to fail if they depend on their original location.
        /// </para>
        /// <para>
        /// The default is false.
        /// </para>
        /// </summary>
        public bool ShadowCopy
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Attaches the debugger to the test process when set to true.
        /// </para>
        /// <para>
        /// The default is false.
        /// </para>
        /// </summary>
        public bool Debug
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Gets or sets the version of the .Net runtime to use for running tests.
        /// </para>
        /// <para>
        /// For the CLR, this must be the name of one of the framework directories in %SystemRoot%\Microsoft.Net\Framework.  eg. 'v2.0.50727'.
        /// </para>
        /// <para>
        /// The default is null which uses the most recent installed and supported framework.
        /// </para>
        /// </summary>
        public string RuntimeVersion
        {
            private get;
            set;
        }

        /// <summary>
        /// A list of the types of reports to generate, separated by semicolons. 
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: Html, Html-Condensed, Text, Text-Condendes, XHtml,
        /// XHtml-Condensed, MHtml, MHtml-CondensedXml, and Xml-Inline, but more types could be available as plugins.</item>
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
            private get;
            set;
        }

        /// <summary>
        /// Sets the report archive mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The supported modes are:
        /// <list type="bullet">
        /// <item>Normal (default)</item>
        /// <item>Zip</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// In the following example, reports will be enclosed in a zip file:
        /// <code>
        /// <![CDATA[
        /// <Target Name="MyTarget">
        ///     <Gallio ReportArchive="zip" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public string ReportArchive
        {
            private get;
            set;
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
            private get;
            set;
        }

        /// <summary>
        /// Sets the name of the directory where the reports will be put.
        /// </summary>
        /// <remarks>
        /// The directory will be created if it doesn't exist.  Existing files will be overwritten.
        /// The default report directory is "Reports".
        /// </remarks>
        public ITaskItem ReportDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the type of test runner to use.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: Local, IsolatedAppDomain
        /// and IsolatedProcess (default), but more types could be available as plugins.</item>
        /// <item>The runner types are not case sensitive.</item>
        /// </list>
        /// </remarks>
        public string RunnerType
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Specifies the type, assembly, and parameters of custom test runner
        /// extensions to use during the test run in the form:
        /// '[Namespace.]Type,Assembly[;Parameters]'.
        /// </para>
        /// <para>
        /// eg. 'FancyLogger,MyCustomExtensions.dll;SomeParameters'
        /// </para>
        /// </summary>
        /// <remarks>
        /// Since semicolons are used to delimit multiple property values in MSBuild,
        /// it may be necessary to escape semicolons that appear as part of test
        /// runner extension specifications to ensure MSBuild does not misinterpret them.
        /// An escaped semicolon may be written as "%3B" in the build file.
        /// </remarks>
        /// <example>
        /// The following example runs tests using a custom logger extension:
        /// <code>
        /// <![CDATA[
        /// <Target Name="MyTarget">
        ///     <Gallio Files="MyTestAssembly.dll" RunnerExtensions="FancyLogger,MyExtensions.dll%3BColorOutput,FancyIndenting" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public string[] RunnerExtensions
        {
            private get;
            set;
        }

        /// <summary>
        /// Specifies option property key/value pairs for the test runner.
        /// </summary>
        /// <example>
        /// The following example specifies some extra NCover arguments.
        /// <code>
        /// <![CDATA[
        /// <gallio>
        /// <Target Name="MyTarget">
        ///     <Gallio Files="MyTestAssembly.dll" RunnerExtensions="NCoverArguments='//eas Gallio'" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public string[] RunnerProperties
        {
            private get;
            set;
        }

        /// <summary>
        /// Specifies option property key/value pairs for the report formatter.
        /// </summary>
        /// <example>
        /// The following example changes the default attachment content disposition for the reports.
        /// <code>
        /// <![CDATA[
        /// <Target Name="MyTarget">
        ///     <Gallio Files="MyTestAssembly.dll" RunnerExtensions="AttachmentContentDisposition=Absent" />
        /// </Target>
        /// ]]>
        /// </code>
        /// </example>
        public string[] ReportFormatterProperties
        {
            private get;
            set;
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
            private get;
            set;
        }

        /// <summary>
        /// Sets whether test failures will be ignored and allow the build to proceed.
        /// When set to <c>false</c>, test failures will cause the build to fail.
        /// </summary>
        public bool IgnoreFailures
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to show generated reports in a window using the default system application
        /// registered to the report file type.
        /// </summary>
        public bool ShowReports
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        public bool DoNotRun
        {
            private get;
            set;
        }

        /// <summary>
        /// <para>
        /// Sets whether to ignore annotations when determining the result code.
        /// If false (default), then error annotations, usually indicative of broken tests, will cause
        /// a failure result to be generated.
        /// </para>
        /// </summary>
        public bool IgnoreAnnotations
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  If this option is set
        /// to true, the default, test results are echoed to the console
        /// in varying detail depending on the current verbosity level.  Otherwise
        /// only final summary statistics are displayed.
        /// </summary>
        public bool EchoResults
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the maximum amount of time (in seconds) the tests can run
        /// before they are canceled. The default is an infinite time to run.
        /// </summary>
        public double RunTimeLimit
        {
            set
            {
                runTimeLimit = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// The verbosity to use when logging.  The default is "Normal".
        /// </summary>
        public string Verbosity
        {
            set
            {
                verbosity = (Verbosity)Enum.Parse(typeof(Verbosity), value);
            }
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
            get;
            private set;
        }

        /// <summary>
        /// Gets the total number of test cases run.
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
        /// <!-- After execution the number of test cases run can be retrieved like this: -->
        /// <Message Text="$(TestCount) test cases were run." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int TestCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total number of test steps run.
        /// </summary>
        /// <example>
        /// To use this property, you need to include an Output tag within the
        /// Gallio tag to specify a name to reference it:
        /// <code>
        /// <![CDATA[
        /// <Gallio>
        ///      <!-- This tells MSBuild that the task's StepCount output property will
        ///           be made available as a property called StepCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="StepCount" PropertyName="StepCount" />
        /// </Gallio>
        /// <!-- After execution the number of test steps run can be retrieved like this: -->
        /// <Message Text="$(StepCount) test steps were run." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int StepCount
        {
            get;
            private set;
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
        ///      <!-- This tells MSBuild that the task's PassedCount output property will
        ///           be made available as a property called PassedCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="PassedCount" PropertyName="PassedCount" />
        /// </Gallio>
        /// <!-- After execution the number of passed tests can be retrieved like this: -->
        /// <Message Text="$(PassedCount) tests passed." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int PassedCount
        {
            get;
            private set;
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
        ///      <!-- This tells MSBuild that the task's FailedCount output property will
        ///           be made available as a property called FailedCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="FailedCount" PropertyName="FailedCount" />
        /// </Gallio>
        /// <!-- After execution the number of failed tests can be retrieved like this: -->
        /// <Message Text="$(FailedCount) tests passed." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int FailedCount
        {
            get;
            private set;
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
            get;
            private set;
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
            get;
            private set;
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
        ///      <!-- This tells MSBuild that the task's SkippedCount output property will
        ///           be made available as a property called SkippedCount in the project
        ///           after the tests have been run: -->
        ///     <Output TaskParameter="SkippedCount" PropertyName="SkippedCount" />
        /// </Gallio>
        /// <!-- After execution the number of skipped tests can be retrieved like this: -->
        /// <Message Text="$(SkippedCount) tests were skipped." />
        /// ]]>
        /// </code>
        /// </example>
        [Output]
        public int SkippedCount
        {
            get;
            private set;
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
            get;
            private set;
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
            get;
            private set;
        }

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
                return false;
            }
        }

        internal bool InternalExecute()
        {
            DisplayVersion();
            var logger = new FilteredLogger(new TaskLogger(Log), verbosity);
            var launcher = new TestLauncher();
            launcher.Logger = logger;
            launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
            launcher.TestExecutionOptions.FilterSet = GetFilterSet();
            launcher.ShowReports = ShowReports;
            launcher.DoNotRun = DoNotRun;
            launcher.IgnoreAnnotations = IgnoreAnnotations;
            launcher.RunTimeLimit = runTimeLimit;
            launcher.RuntimeSetup = new RuntimeSetup();

            // Set the installation path explicitly to the path of the MSBuild task assembly
            // since otherwise we will look at the path of MSBuild.exe.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(Gallio).Assembly));

            if (EchoResults)
                launcher.TestProject.AddTestRunnerExtension(new TaskLogExtension(Log));
            if (ApplicationBaseDirectory != null)
                launcher.TestProject.TestPackage.ApplicationBaseDirectory = new DirectoryInfo(ApplicationBaseDirectory.ItemSpec);
            if (WorkingDirectory != null)
                launcher.TestProject.TestPackage.WorkingDirectory = new DirectoryInfo(WorkingDirectory.ItemSpec);
            launcher.TestProject.TestPackage.ShadowCopy = ShadowCopy;
            if (Debug)
                launcher.TestProject.TestPackage.DebuggerSetup = new DebuggerSetup();
            if (RuntimeVersion != null)
                launcher.TestProject.TestPackage.RuntimeVersion = RuntimeVersion;

            foreach (string option in ReportFormatterProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option);
                launcher.ReportFormatterOptions.AddProperty(pair.Key, pair.Value);
            }

            foreach (string option in RunnerProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option);
                launcher.TestRunnerOptions.AddProperty(pair.Key, pair.Value);
            }

            ForEachItemSpec(Files, launcher.AddFilePattern);
            ForEachItemSpec(HintDirectories, x => launcher.TestProject.TestPackage.AddHintDirectory(new DirectoryInfo(x)));
            ForEachItemSpec(PluginDirectories, x => launcher.RuntimeSetup.AddPluginDirectory(x));

            if (ReportDirectory != null)
                launcher.TestProject.ReportDirectory = ReportDirectory.ItemSpec;
            if (ReportNameFormat != null)
                launcher.TestProject.ReportNameFormat = ReportNameFormat;
            if (ReportTypes != null)
                GenericCollectionUtils.ForEach(ReportTypes, launcher.AddReportFormat);
            if (ReportArchive != null)
                launcher.TestProject.ReportArchive = Runner.Reports.ReportArchive.Parse(ReportArchive);
            if (RunnerType != null)
                launcher.TestProject.TestRunnerFactoryName = RunnerType;
            if (RunnerExtensions != null)
                GenericCollectionUtils.ForEach(RunnerExtensions, x => launcher.TestProject.AddTestRunnerExtensionSpecification(x));

            TestLauncherResult result = RunLauncher(launcher);
            ExitCode = result.ResultCode;
            LogResultSummary(logger, result);
            PopulateStatistics(result);
            return ExitCode == ResultCode.Success 
                || ExitCode == ResultCode.NoTests 
                || IgnoreFailures;
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
            Statistics stats = result.Statistics;
            AssertCount = stats.AssertCount;
            Duration = stats.Duration;
            FailedCount = stats.FailedCount;
            InconclusiveCount = stats.InconclusiveCount;
            PassedCount = stats.PassedCount;
            SkippedCount = stats.SkippedCount;
            RunCount = stats.RunCount;
            StepCount = stats.StepCount;
            TestCount = stats.TestCount;
        }

        private static void LogResultSummary(ILogger logger, TestLauncherResult result)
        {
            switch (result.ResultCode)
            {
                case ResultCode.Success:
                    logger.Log(LogSeverity.Info, result.ResultSummary);
                    break;

                case ResultCode.Failure:
                    logger.Log(LogSeverity.Error, result.ResultSummary);
                    break;
            }
        }

        private FilterSet<ITestDescriptor> GetFilterSet()
        {
            if (String.IsNullOrEmpty(Filter))
            {
                return FilterSet<ITestDescriptor>.Empty;
            }

            return FilterUtils.ParseTestFilterSet(Filter);
        }

        private void DisplayVersion()
        {
            string versionLabel = VersionPolicy.GetVersionLabel(Assembly.GetExecutingAssembly());
            Log.LogMessage(String.Format(Resources.TaskNameAndVersion, versionLabel));
        }

        private static void ForEachItemSpec(IEnumerable<ITaskItem> items, Action<string> action)
        {
            if (items != null)
            {
                foreach (ITaskItem item in items)
                    action(item.ItemSpec);
            }
        }
    }
}
