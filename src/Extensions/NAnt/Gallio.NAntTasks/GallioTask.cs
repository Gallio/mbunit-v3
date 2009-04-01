// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.IO;
using System.Reflection;
using Gallio.Runtime;
using Gallio.NAntTasks.Properties;
using Gallio.Collections;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Utilities;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Gallio.NAntTasks
{
    /// <summary>
    /// A NAnt task that provides support for running Gallio tests.
    /// </summary>
    /// <remarks>
    /// In order for NAnt to find this task, either the Gallio.NAntTasks.dll assembly needs
    /// to be put in NAnt's bin folder, or it must be loaded with the loadtasks directive:
    /// <code>
    /// <![CDATA[
    ///    <loadtasks assembly="[pathtoassembly]\Gallio.NAntTasks.dll" />
    /// ]]>
    /// </code>
    /// </remarks>
    /// <example>
    /// The following code is an example build file that shows how to load the task, specify the test assemblies
    /// and set some of the task's properties:
    /// <code>
    /// <![CDATA[
    ///    <?xml version="1.0" ?>
    ///    <project name="TestProject" default="RunTests">
    ///    <!-- This is needed by NAnt to locate the Gallio task -->
    ///    <loadtasks assembly="[pathtoassembly]\Gallio.NAntTasks.dll" />
    ///    <target name="RunTests">
    ///     <gallio result-property="ExitCode" failonerror="false" filter="Type=SomeFixture" >
    ///      <assemblies>
    ///        <!-- Specify the tests assemblies -->
    ///        <include name="[Path-to-test-assembly1]/TestAssembly1.dll" />
    ///        <include name="[Path-to-test-assembly2]/TestAssembly2.dll" />
    ///      </assemblies>
    ///     </gallio>
    ///     <fail if="${ExitCode != '0'}" >The return code should have been 0!</fail>
    ///    </target>
    ///
    ///    </project>
    /// ]]>
    /// </code>
    /// </example>
    [TaskName(@"gallio")]
    public class GallioTask : Task
    {
        #region Private Members

        private FileSet[] assemblies;
        private DirSet[] pluginDirectories;
        private DirSet[] hintDirectories;

        private string applicationBaseDirectory;
        private string workingDirectory;
        private bool shadowCopy;
        private bool debug;

        private string filter = string.Empty;
        private string reportTypes = string.Empty;
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string resultProperty;
        private string statisticsPropertiesPrefix;
        private bool showReports;
        private string runnerType = StandardTestRunnerFactoryNames.IsolatedProcess;
        private ArgumentCollection runnerExtensions = new ArgumentCollection();
        private bool doNotRun;
        private bool ignoreAnnotations;
        private bool echoResults = true;
        private TimeSpan? runTimeLimit;

        private ArgumentCollection runnerProperties = new ArgumentCollection();
        private ArgumentCollection reportFormatterProperties = new ArgumentCollection();

        #endregion

        #region Public Properties

        ///<summary>
        /// The list of test assemblies to execute. This is required.
        ///</summary>
        ///<example>The following example shows how to specify the test assemblies (for a more complete example
        /// please see the <see cref="GallioTask"/> task documentation):
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <assemblies>
        ///         <!-- Specify the tests assemblies -->
        ///         <include name="[Path-to-test-assembly1]/TestAssembly1.dll" />
        ///         <include name="[Path-to-test-assembly2]/TestAssembly2.dll" />
        ///     </assemblies>
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
        public FileSet[] Assemblies
        {
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        /// <example>The following example shows how to specify the hint directories:
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <hint-directories>
        ///         <include name="C:\SomeFolder\AnotherFolder" />
        ///         <include name="../somefolder" />
        ///     </hint-directories>
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("hint-directories", ElementType = typeof(DirSet))]
        public DirSet[] HintDirectories
        {
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>The following example shows how to specify the plugins directories:
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <plugin-directories>
        ///         <include name="C:\SomeFolder\AnotherFolder" />
        ///         <include name="../somefolder" />
        ///     </plugin-directories>
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("plugin-directories", ElementType = typeof(DirSet))]
        public DirSet[] PluginDirectories
        {
            set { pluginDirectories = value; }
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
        [TaskAttribute("application-base-directory")]
        public string ApplicationBaseDirectory
        {
            set { applicationBaseDirectory = value; }
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
        [TaskAttribute("working-directory")]
        public string WorkingDirectory
        {
            set { workingDirectory = value; }
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
        [TaskAttribute("shadow-copy")]
        public bool ShadowCopy
        {
            set { shadowCopy = value; }
        }

        /// <summary>
        /// <para>
        /// Attaches the debugger to the test process when set to true.
        /// </para>
        /// <para>
        /// The default is false.
        /// </para>
        /// </summary>
        [TaskAttribute("debug")]
        public bool Debug
        {
            set { debug = value; }
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
        /// <code>
        /// <![CDATA[
        /// <gallio report-types="html;xml">
        ///     <!-- More options -->
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [TaskAttribute("report-types")]
        public string ReportTypes
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
        [TaskAttribute("report-name-format", Required = false)]
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
        [TaskAttribute("report-directory", Required = false)]
        public string ReportDirectory
        {
            set { reportDirectory = value; }
        }

        /// <summary>
        /// Sets whether to show generated reports in a window using the default system application
        /// registered to the report file type.
        /// </summary>
        [TaskAttribute("show-reports", Required = false)]
        public bool ShowReports
        {
            set { showReports = value; }
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
            set { runnerType = value; }
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
        /// <example>
        /// The following example runs tests using a custom logger extension:
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <runner-extension value="FancyLogger,MyExtensions.dll;ColorOutput,FancyIndenting" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("runner-extension")]
        public ArgumentCollection RunnerExtensions
        {
            get { return runnerExtensions; }
        }

        /// <summary>
        /// Specifies option property key/value pairs for the test runner.
        /// </summary>
        /// <example>
        /// The following example specifies some extra NCover arguments.
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <runner-property value="NCoverArguments='//eas Gallio'" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("runner-property")]
        public ArgumentCollection RunnerProperties
        {
            get { return runnerProperties; }
        }

        /// <summary>
        /// Specifies option property key/value pairs for the report formatter.
        /// </summary>
        /// <example>
        /// The following example changes the default attachment content disposition for the reports.
        /// <code>
        /// <![CDATA[
        /// <gallio>
        ///     <report-formatter-property value="AttachmentContentDisposition=Absent" />
        ///     <!-- More options -->
        /// </gallio>
        /// ]]>
        /// </code>
        /// </example>
        [BuildElementArray("report-formatter-property")]
        public ArgumentCollection ReportFormatterProperties
        {
            get { return reportFormatterProperties; }
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        [TaskAttribute("do-not-run", Required = false)]
        public bool DoNotRun
        {
            set { doNotRun = value; }
        }

        /// <summary>
        /// <para>
        /// Sets whether to ignore annotations when determining the result code.
        /// If false (default), then error annotations, usually indicative of broken tests, will cause
        /// a failure result to be generated.
        /// </para>
        /// </summary>
        [TaskAttribute("ignore-annotations", Required = false)]
        public bool IgnoreAnnotations
        {
            set { ignoreAnnotations = value; }
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  If this option is set
        /// to true, the default, test results are echoed to the console
        /// in varying detail depending on the current verbosity level.  Otherwise
        /// only the final summary statistics are displayed.
        /// </summary>
        [TaskAttribute("echo-results", Required = false)]
        public bool EchoResults
        {
            set { echoResults = value; }
        }

        /// <summary>
        /// Sets the maximum amount of time (in seconds) the tests can run 
        /// before they are canceled. The default is an infinite time to run. 
        /// </summary>
        [TaskAttribute("run-time-limit", Required = false)]
        public double RunTimeLimit
        {
            set { runTimeLimit = TimeSpan.FromSeconds(value); }
        }

        /// <summary>
        /// Sets the name of a NAnt property in which the exit code of the tests execution
        /// should be stored.
        /// </summary>
        /// <remarks>
        /// Only of interest if FailOnError is set to false.
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// <target name="RunTests">
        ///     <gallio result-property="ExitCode" failonerror="false">
        ///         <!-- Include test assemblies -->
        ///     </gallio>
        ///     <fail if="${ExitCode != 0}" >The return code should have been 0!</fail>
        /// </target>
        /// ]]>
        /// </code>
        /// </example>
        [TaskAttribute("result-property")]
        [StringValidator(AllowEmpty = false)]
        public string ResultProperty
        {
            set { resultProperty = value; }
        }

        /// <summary>
        /// Sets the prefix that will be used for the statistics result properties.
        /// </summary>
        /// <remarks>
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
        /// </remarks>
        /// <example>The following example shows how to use the result-properties-prefix property :
        /// <code>
        /// <![CDATA[
        /// <target name="RunTests">
        ///     <gallio statistics-properties-prefix="gallio.">
        ///         <assemblies>
        ///             <include name="SomeAssembly.dll" />
        ///         </assemblies>
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
        /// ]]>
        /// </code>
        /// </example>
        [TaskAttribute("statistics-properties-prefix")]
        [StringValidator(AllowEmpty = false)]
        public string StatisticsPropertiesPrefix
        {
            set { statisticsPropertiesPrefix = value; }
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
            set { filter = value; }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        protected override void ExecuteTask()
        {
            InternalExecute();
        }

        internal void InternalExecute()
        {
            // We don't catch exceptions here because NAnt takes care of that job,
            // and decides whether to let them through based on the value of the
            // FailOnError
            TaskLogger logger = new TaskLogger(this);

            DisplayVersion();

            TestLauncher launcher = new TestLauncher();
            launcher.Logger = logger;
            launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
            launcher.TestExecutionOptions.FilterSet = GetFilterSet();
            launcher.ShowReports = showReports;
            launcher.DoNotRun = doNotRun;
            launcher.IgnoreAnnotations = ignoreAnnotations;
            launcher.EchoResults = echoResults;
            launcher.RunTimeLimit = runTimeLimit;

            launcher.TestRunnerFactoryName = runnerType;
            if (runnerExtensions != null)
            {
                foreach (Argument arg in runnerExtensions)
                    launcher.TestRunnerExtensionSpecifications.Add(arg.Value);
            }

            launcher.RuntimeSetup = new RuntimeSetup();

            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(GallioTask).Assembly));

            launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory = applicationBaseDirectory;
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = workingDirectory;
            launcher.TestPackageConfig.HostSetup.ShadowCopy = shadowCopy;
            launcher.TestPackageConfig.HostSetup.Debug = debug;

            foreach (Argument option in reportFormatterProperties)
                launcher.ReportFormatterOptions.Properties.Add(StringUtils.ParseKeyValuePair(option.Value));

            foreach (Argument option in runnerProperties)
                launcher.TestRunnerOptions.Properties.Add(StringUtils.ParseKeyValuePair(option.Value));

            AddAssemblies(launcher);
            AddHintDirectories(launcher);
            AddPluginDirectories(launcher);

            if (reportDirectory != null)
                launcher.ReportDirectory = reportDirectory;
            if (!String.IsNullOrEmpty(reportNameFormat))
                launcher.ReportNameFormat = reportNameFormat;
            if (reportTypes != null)
                GenericUtils.AddAll(reportTypes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                    launcher.ReportFormats);

            TestLauncherResult result = RunLauncher(launcher);

            SetResultProperty(result.ResultCode);
            PopulateStatistics(result);

            if (FailOnError)
            {
                if (result.ResultCode != ResultCode.Success && result.ResultCode != ResultCode.NoTests)
                {
                    // The only way to make the task fail is to throw an exception
                    throw new BuildException(Resources.TestExecutionFailed);
                }
            }
        }

        #endregion

        #region Private Methods

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

            Properties[statisticsPropertiesPrefix + @"TestCount"] = stats.TestCount.ToString();
            Properties[statisticsPropertiesPrefix + @"StepCount"] = stats.StepCount.ToString();
            Properties[statisticsPropertiesPrefix + @"PassedCount"] = stats.PassedCount.ToString();
            Properties[statisticsPropertiesPrefix + @"FailedCount"] = stats.FailedCount.ToString();
            Properties[statisticsPropertiesPrefix + @"InconclusiveCount"] = stats.InconclusiveCount.ToString();
            Properties[statisticsPropertiesPrefix + @"RunCount"] = stats.RunCount.ToString();
            Properties[statisticsPropertiesPrefix + @"SkippedCount"] = stats.SkippedCount.ToString();
            Properties[statisticsPropertiesPrefix + @"Duration"] = stats.Duration.ToString();
            Properties[statisticsPropertiesPrefix + @"AssertCount"] = stats.AssertCount.ToString();
        }

        private FilterSet<ITest> GetFilterSet()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return FilterSet<ITest>.Empty;
            }

            return FilterUtils.ParseTestFilterSet(filter);
        }

        /// <summary>
        /// Checks the result code of the tests execution and performs the
        /// corresponding action.
        /// </summary>
        /// <param name="resultCode">The result code returned by the Run method of the
        /// TestRunnerHelper class.</param>
        private void SetResultProperty(IConvertible resultCode)
        {
            if (!String.IsNullOrEmpty(resultProperty))
            {
                Properties[resultProperty] = resultCode.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void DisplayVersion()
        {
            Version appVersion = AssemblyUtils.GetApplicationVersion(Assembly.GetExecutingAssembly());

            Log(Level.Info, String.Format(Resources.TaskNameAndVersion,
                appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision));
        }

        private void AddAssemblies(TestLauncher launcher)
        {
            if (assemblies != null)
            {
                foreach (FileSet fs in assemblies)
                {
                    foreach (string f in fs.FileNames)
                        launcher.TestPackageConfig.AssemblyFiles.Add(f);
                }
            }
        }

        private void AddHintDirectories(TestLauncher launcher)
        {
            if (hintDirectories != null)
            {
                foreach (DirSet ds in hintDirectories)
                {
                    foreach (string d in ds.DirectoryNames)
                        launcher.TestPackageConfig.HintDirectories.Add(d);
                }
            }
        }

        private void AddPluginDirectories(TestLauncher launcher)
        {
            if (pluginDirectories != null)
            {
                foreach (DirSet ds in pluginDirectories)
                {
                    foreach (string d in ds.DirectoryNames)
                        launcher.RuntimeSetup.PluginDirectories.Add(d);
                }
            }
        }

        #endregion
    }
}
