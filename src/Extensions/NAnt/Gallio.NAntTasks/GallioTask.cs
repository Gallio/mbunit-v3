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
using System.Globalization;
using System.IO;
using System.Reflection;
using Gallio.Common.Policies;
using Gallio.Common.Text;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime;
using Gallio.NAntTasks.Properties;
using Gallio.Common.Collections;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Gallio.NAntTasks
{
    /// <summary>
    /// A NAnt task that provides support for running Gallio tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In order for NAnt to find this task, either the Gallio.NAntTasks.dll assembly needs
    /// to be put in NAnt's bin folder, or it must be loaded with the loadtasks directive:
    /// <code><![CDATA[
    ///    <loadtasks assembly="[pathtoassembly]\Gallio.NAntTasks.dll" />
    /// ]]></code>
    /// </para>
    /// </remarks>
    /// <example>
    /// The following code is an example build file that shows how to load the task, specify the test files
    /// and assemblies and set some of the task's properties:
    /// <code><![CDATA[
    ///    <?xml version="1.0" ?>
    ///    <project name="TestProject" default="RunTests">
    ///    <!-- This is needed by NAnt to locate the Gallio task -->
    ///    <loadtasks assembly="[pathtoassembly]\Gallio.NAntTasks.dll" />
    ///    <target name="RunTests">
    ///     <gallio result-property="ExitCode" failonerror="false" filter="Type=SomeFixture" >
    ///      <files>
    ///        <!-- Specify the tests files and assemblies -->
    ///        <include name="[Path-to-test-assembly1]/TestAssembly1.dll" />
    ///        <include name="[Path-to-test-assembly2]/TestAssembly2.dll" />
    ///        <include name="[Path-to-test-script1]/TestScript1_spec.rb" />
    ///        <include name="[Path-to-test-script2]/TestScript2.xml" />
    ///      </files>
    ///     </gallio>
    ///     <fail if="${ExitCode != '0'}" >The return code should have been 0!</fail>
    ///    </target>
    ///
    ///    </project>
    /// ]]></code>
    /// </example>
    [TaskName(@"gallio")]
    public class GallioTask : Task
    {
        private TimeSpan? runTimeLimit;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GallioTask()
        {
            Verbosity = Verbosity.Normal;
            Filter = string.Empty;
            EchoResults = true;
            ReportFormatterProperties = new ArgumentCollection();
            RunnerProperties = new ArgumentCollection();
            RunnerExtensions = new ArgumentCollection();
            ReportTypes = string.Empty;
        }

        /// <summary>
        /// The list of test files, projects and assemblies to execute. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// Wildcards may be used.
        /// </para>
        /// <para>
        /// This is required.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to specify the test files, projects and assemblies
        /// (for a more complete example please see the <see cref="GallioTask"/> task documentation):
        /// <code><![CDATA[
        /// <gallio>
        ///     <files>
        ///        <!-- Specify the tests files, projects and assemblies -->
        ///        <include name="[Path-to-test-assembly1]/TestAssembly1.dll" />
        ///        <include name="[Path-to-test-assembly2]/TestAssembly2.dll" />
        ///        <include name="[Path-to-test-script1]/TestScript1_spec.rb" />
        ///        <include name="[Path-to-test-script2]/TestScript2.xml" />
        ///     </files>
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("files", Required = true, ElementType = typeof(FileSet))]
        public FileSet[] Files
        {
            private get;
            set;
        }

        /// <summary>
        /// The list of directories used for loading referenced assemblies and other dependent resources.
        /// </summary>
        /// <example>
        /// The following example shows how to specify the hint directories:
        /// <code><![CDATA[
        /// <gallio>
        ///     <hint-directories>
        ///         <include name="C:\SomeFolder\AnotherFolder" />
        ///         <include name="../somefolder" />
        ///     </hint-directories>
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("hint-directories", ElementType = typeof(DirSet))]
        public DirSet[] HintDirectories
        {
            private get;
            set;
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>
        /// The following example shows how to specify the plugins directories:
        /// <code><![CDATA[
        /// <gallio>
        ///     <plugin-directories>
        ///         <include name="C:\SomeFolder\AnotherFolder" />
        ///         <include name="../somefolder" />
        ///     </plugin-directories>
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("plugin-directories", ElementType = typeof(DirSet))]
        public DirSet[] PluginDirectories
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory,
        /// or null to use a default value selected by the consumer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// The default is null.
        /// </para>
        /// </remarks>
        [TaskAttribute("application-base-directory")]
        public string ApplicationBaseDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the working directory
        /// or null to use a default value selected by the consumer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// The default is null.
        /// </para>
        /// </remarks>
        [TaskAttribute("working-directory")]
        public string WorkingDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// Enables shadow copying when set to true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Shadow copying allows the original assemblies to be modified while the tests are running.
        /// However, shadow copying may occasionally cause some tests to fail if they depend on their original location.
        /// </para>
        /// <para>
        /// The default is false.
        /// </para>
        /// </remarks>
        [TaskAttribute("shadow-copy")]
        public bool? ShadowCopy
        {
            private get;
            set;
        }

        /// <summary>
        /// Attaches the debugger to the test process when set to true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default is false.
        /// </para>
        /// </remarks>
        [TaskAttribute("debug")]
        public bool? Debug
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the .Net runtime to use for running tests.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For the CLR, this must be the name of one of the framework directories in %SystemRoot%\Microsoft.Net\Framework.  eg. 'v2.0.50727'.
        /// </para>
        /// <para>
        /// The default is null which uses the most recent installed and supported framework.
        /// </para>
        /// </remarks>
        [TaskAttribute("runtime-version")]
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
        /// <item>The types supported "out of the box" are: Html, Html-Inline, Text, XHtml,
        /// XHtml-Inline, Xml, and Xml-Inline, but more types could be available as plugins.</item>
        /// <item>The report types are not case sensitives.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// In the following example reports will be generated in both HTML and XML format.
        /// <code><![CDATA[
        /// <gallio report-types="html;xml">
        ///     <!-- More options -->
        /// </gallio>
        /// ]]></code>
        /// </example>
        [TaskAttribute("report-types")]
        public string ReportTypes
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
        /// <gallio report-archive="zip">
        ///     <!-- More options -->
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [TaskAttribute("report-archive", Required = false)]
        public string ReportArchive
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the format string to use to generate the reports filenames.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Any occurence of {0} will be replaced by the date, and any occurrence of {1} by the time.
        /// The default format string is test-report-{0}-{1}.
        /// </para>
        /// </remarks>
        [TaskAttribute("report-name-format", Required = false)]
        public string ReportNameFormat
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the name of the directory where the reports will be put.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The directory will be created if it doesn't exist. Existing files will be overwritten.
        /// The default report directory is "Reports".
        /// </para>
        /// </remarks>
        [TaskAttribute("report-directory", Required = false)]
        public string ReportDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to show generated reports in a window using the default system application
        /// registered to the report file type.
        /// </summary>
        [TaskAttribute("show-reports", Required = false)]
        public bool ShowReports
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
        [TaskAttribute("runner-type", Required = false)]
        public string RunnerType
        {
            private get;
            set;
        }

        /// <summary>
        /// Specifies the type, assembly, and parameters of custom test runner
        /// extensions to use during the test run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value must be in the form '[Namespace.]Type,Assembly[;Parameters]'.
        /// </para>
        /// <para>
        /// eg. 'FancyLogger,MyCustomExtensions.dll;SomeParameters'
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example runs tests using a custom logger extension:
        /// <code><![CDATA[
        /// <gallio>
        ///     <runner-extension value="FancyLogger,MyExtensions.dll;ColorOutput,FancyIndenting" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("runner-extension")]
        public ArgumentCollection RunnerExtensions
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies option property key/value pairs for the test runner.
        /// </summary>
        /// <example>
        /// The following example specifies some extra NCover arguments.
        /// <code><![CDATA[
        /// <gallio>
        ///     <runner-property value="NCoverArguments='//eas Gallio'" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("runner-property")]
        public ArgumentCollection RunnerProperties
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies option property key/value pairs for the report formatter.
        /// </summary>
        /// <example>
        /// The following example changes the default attachment content disposition for the reports.
        /// <code><![CDATA[
        /// <gallio>
        ///     <report-formatter-property value="AttachmentContentDisposition=Absent" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]></code>
        /// </example>
        [BuildElementArray("report-formatter-property")]
        public ArgumentCollection ReportFormatterProperties
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </para>
        /// </remarks>
        [TaskAttribute("do-not-run", Required = false)]
        public bool DoNotRun
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to ignore annotations when determining the result code.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If false (default), then error annotations, usually indicative of broken tests, will cause
        /// a failure result to be generated.
        /// </para>
        /// </remarks>
        [TaskAttribute("ignore-annotations", Required = false)]
        public bool IgnoreAnnotations
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this option is set to true, the default, test results are echoed to the console
        /// in varying detail depending on the current verbosity level.  Otherwise
        /// only the final summary statistics are displayed.
        /// </para>
        /// </remarks>
        [TaskAttribute("echo-results", Required = false)]
        public bool EchoResults
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the maximum amount of time (in seconds) the tests can run 
        /// before they are canceled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default is an infinite time to run. 
        /// </para>
        /// </remarks>
        [TaskAttribute("run-time-limit", Required = false)]
        public double RunTimeLimit
        {
            set
            {
                runTimeLimit = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// Sets the name of a NAnt property in which the exit code of the tests execution
        /// should be stored.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Only of interest if FailOnError is set to false.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// <target name="RunTests">
        ///     <gallio result-property="ExitCode" failonerror="false">
        ///         <!-- Include test assemblies -->
        ///     </gallio>
        ///     <fail if="${ExitCode != 0}" >The return code should have been 0!</fail>
        /// </target>
        /// ]]></code>
        /// </example>
        [TaskAttribute("result-property"), StringValidator(AllowEmpty = false)]
        public string ResultProperty
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the prefix that will be used for the statistics result properties.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The following properties are available:
        /// <list type="bullet">
        /// <item><term>AssertCount</term><description>Gets the number of assertions evaluated.</description></item>
        /// <item><term>FailedCount</term><description>Gets the total number of test cases that were run and failed.</description></item>
        /// <item><term>InconclusiveCount</term><description>Gets the total number of test cases that ran and were inconclusive.</description></item>
        /// <item><term>PassedCount</term><description>Gets the total number of test cases that were run and passed.</description></item>
        /// <item><term>SkippedCount</term><description>Gets the total number of test cases that did not run because they were skipped.</description></item>
        /// <item><term>RunCount</term><description>Gets the total number of test cases that were run.</description></item>
        /// <item><term>TestCount</term><description>Gets the total number of test cases.</description></item>
        /// <item><term>StepCount</term><description>Gets the total number of test steps.</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to use the result-properties-prefix property:
        /// <code><![CDATA[
        /// <target name="RunTests">
        ///     <gallio statistics-properties-prefix="gallio.">
        ///         <files>
        ///             <include name="SomeAssembly.dll" />
        ///         </files>
        ///     </gallio>
        ///     <echo message="AssertCount = ${gallio.AssertCount}" />
        ///     <echo message="FailedCount = ${gallio.FailedCount}" />
        ///     <echo message="InconclusiveCount = ${gallio.InconclusiveCount}" />
        ///     <echo message="PassedCount = ${gallio.PassedCount}" />
        ///     <echo message="SkippedCount = ${gallio.SkippedCount}" />
        ///     <echo message="RunCount = ${gallio.RunCount}" />
        ///     <echo message="TestCount = ${gallio.TestCount}" />
        ///     <echo message="StepCount = ${gallio.StepCount}" />
        /// </target>
        /// ]]></code>
        /// </example>
        [TaskAttribute("statistics-properties-prefix"), StringValidator(AllowEmpty = false)]
        public string StatisticsPropertiesPrefix
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
        [TaskAttribute("filter")]
        public string Filter
        {
            private get;
            set;
        }

        /// <summary>
        /// Controls the level of information logged.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default is "Normal".
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// <target name="RunTests">
        ///     <gallio verbosity="Quiet" failonerror="false">
        ///         <!-- Include test files -->
        ///     </gallio>
        ///     <fail if="${ExitCode != 0}" >The return code should have been 0!</fail>
        /// </target>
        /// ]]></code>
        /// </example>
        [TaskAttribute("verbosity")]
        public Verbosity Verbosity
        {
            private get;
            set;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        protected override void ExecuteTask()
        {
            InternalExecute();
        }

        internal void InternalExecute()
        {
            // We don't catch exceptions here because NAnt takes care of that job,
            // and decides whether to let them through based on the value of the
            // FailOnError
            var logger = CreateLogger();
            DisplayVersion();
            var launcher = new TestLauncher();
            launcher.Logger = logger;
            launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
            launcher.TestExecutionOptions.FilterSet = GetFilterSet();
            launcher.ShowReports = ShowReports;
            launcher.DoNotRun = DoNotRun;
            launcher.IgnoreAnnotations = IgnoreAnnotations;
            launcher.EchoResults = EchoResults;
            launcher.RunTimeLimit = runTimeLimit;

            if (RunnerType != null)
            {
                launcher.TestProject.TestRunnerFactoryName = RunnerType;
            }

            if (RunnerExtensions != null)
            {
                foreach (Argument arg in RunnerExtensions)
                {
                    launcher.TestProject.AddTestRunnerExtensionSpecification(arg.Value);
                }
            }

            launcher.RuntimeSetup = new RuntimeSetup();

            // Set the installation path explicitly to the path of the NAnt task assembly
            // since otherwise we will look at the path of NAnt.exe.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(GallioTask).Assembly));

            if (ApplicationBaseDirectory != null)
                launcher.TestProject.TestPackage.ApplicationBaseDirectory = new DirectoryInfo(ApplicationBaseDirectory);
            if (WorkingDirectory != null)
                launcher.TestProject.TestPackage.WorkingDirectory = new DirectoryInfo(WorkingDirectory);
            if (ShadowCopy.HasValue)
                launcher.TestProject.TestPackage.ShadowCopy = ShadowCopy.Value;
            if (Debug.HasValue && Debug.Value)
                launcher.TestProject.TestPackage.DebuggerSetup = new DebuggerSetup();
            if (RuntimeVersion != null)
                launcher.TestProject.TestPackage.RuntimeVersion = RuntimeVersion;

            foreach (Argument option in ReportFormatterProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option.Value);
                launcher.ReportFormatterOptions.AddProperty(pair.Key, pair.Value);
            }

            foreach (Argument option in RunnerProperties)
            {
                KeyValuePair<string, string> pair = StringUtils.ParseKeyValuePair(option.Value);
                launcher.TestRunnerOptions.AddProperty(pair.Key, pair.Value);
            }

            AddAssemblies(launcher);
            AddHintDirectories(launcher);
            AddPluginDirectories(launcher);

            if (ReportDirectory != null)
                launcher.TestProject.ReportDirectory = ReportDirectory;
            if (ReportNameFormat != null)
                launcher.TestProject.ReportNameFormat = ReportNameFormat;
            if (ReportArchive != null)
                launcher.TestProject.ReportArchive = Runner.Reports.ReportArchive.Parse(ReportArchive);

            if (ReportTypes != null)
            {
                string[] typeNames = ReportTypes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                GenericCollectionUtils.ForEach(typeNames, launcher.AddReportFormat);
            }

            TestLauncherResult result = RunLauncher(launcher);
            SetResultProperty(result.ResultCode);
            PopulateStatistics(result);

            if (!FailOnError)
                return;

            if (result.ResultCode != ResultCode.Success && result.ResultCode != ResultCode.NoTests)
            {
                // The only way to make the task fail is to throw an exception
                throw new BuildException(Resources.TestExecutionFailed);
            }
        }

        private ILogger CreateLogger()
        {
            var logger = new TaskLogger(this);
            return new FilteredLogger(logger, Verbosity);
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
            Properties[StatisticsPropertiesPrefix + @"TestCount"] = stats.TestCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"StepCount"] = stats.StepCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"PassedCount"] = stats.PassedCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"FailedCount"] = stats.FailedCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"InconclusiveCount"] = stats.InconclusiveCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"RunCount"] = stats.RunCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"SkippedCount"] = stats.SkippedCount.ToString();
            Properties[StatisticsPropertiesPrefix + @"Duration"] = stats.Duration.ToString();
            Properties[StatisticsPropertiesPrefix + @"AssertCount"] = stats.AssertCount.ToString();
        }

        private FilterSet<ITestDescriptor> GetFilterSet()
        {
            if (String.IsNullOrEmpty(Filter))
            {
                return FilterSet<ITestDescriptor>.Empty;
            }

            return FilterUtils.ParseTestFilterSet(Filter);
        }

        /// <summary>
        /// Checks the result code of the tests execution and performs the
        /// corresponding action.
        /// </summary>
        /// <param name="resultCode">The result code returned by the Run method of the
        /// TestRunnerHelper class.</param>
        private void SetResultProperty(IConvertible resultCode)
        {
            if (!String.IsNullOrEmpty(ResultProperty))
            {
                Properties[ResultProperty] = resultCode.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void DisplayVersion()
        {
            string versionLabel = VersionPolicy.GetVersionLabel(Assembly.GetExecutingAssembly());
            Log(Level.Info, String.Format(Resources.TaskNameAndVersion, versionLabel));
        }

        private void AddAssemblies(TestLauncher launcher)
        {
            if (Files != null)
            {
                foreach (FileSet fs in Files)
                {
                    foreach (string f in fs.FileNames)
                    {
                        launcher.AddFilePattern(f);
                    }
                }
            }
        }

        private void AddHintDirectories(TestLauncher launcher)
        {
            if (HintDirectories != null)
            {
                foreach (DirSet ds in HintDirectories)
                {
                    foreach (string d in ds.DirectoryNames)
                    {
                        launcher.TestProject.TestPackage.AddHintDirectory(new DirectoryInfo(d));
                    }
                }
            }
        }

        private void AddPluginDirectories(TestLauncher launcher)
        {
            if (PluginDirectories != null)
            {
                foreach (DirSet ds in PluginDirectories)
                {
                    foreach (string d in ds.DirectoryNames)
                    {
                        launcher.RuntimeSetup.AddPluginDirectory(d);
                    }
                }
            }
        }
    }
}
