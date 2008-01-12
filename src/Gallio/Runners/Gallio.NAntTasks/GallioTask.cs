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

// Copyright 2008 MbUnit Project - http://www.gallio.com/
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
using System.Globalization;
using System.Reflection;
using Gallio.NAntTasks.Properties;
using Gallio.Collections;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Reports;
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
        private string filter;
        private string reportTypes = @"";
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string resultProperty;
        private string resultPropertiesPrefix;
        private bool showReports;
        private bool doNotRun;
        private bool echoResults = true;

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
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        [TaskAttribute("do-not-run", Required = false)]
        public bool DoNotRun
        {
            set { doNotRun = value; }
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
        /// <item><term>FailureCount</term><description>Gets the total number of test cases that were run and failed.</description></item>
        /// <item><term>IgnoreCount</term><description>Gets the total number of test cases that did not run because they were ignored.</description></item>
        /// <item><term>InconclusiveCount</term><description>Gets the total number of test cases that ran and were inconclusive.</description></item>
        /// <item><term>PassCount</term><description>Gets the total number of test cases that were run and passed.</description></item>
        /// <item><term>RunCount</term><description>Gets the total number of test cases that were run.</description></item>
        /// <item><term>SkipCount</term><description>Gets the total number of test cases that did not run because they were skipped.</description></item>
        /// <item><term>TestCount</term><description>Gets the total number of test cases.</description></item>
        /// </list>
        /// </remarks>
        /// <example>The following example shows how to use the result-properties-prefix property :
        /// <code>
        /// <![CDATA[
        /// <target name="RunTests">
        ///     <gallio result-properties-prefix="gallio.">
        ///         <assemblies>
        ///             <include name="SomeAssembly.dll" />
        ///         </assemblies>
        ///     </gallio>
        ///     <echo message="AssertCount = ${gallio.AssertCount}" />
        ///     <echo message="FailureCount = ${gallio.FailureCount}" />
        ///     <echo message="IgnoreCount = ${gallio.IgnoreCount}" />
        ///     <echo message="InconclusiveCount = ${gallio.InconclusiveCount}" />
        ///     <echo message="PassCount = ${gallio.PassCount}" />
        ///     <echo message="RunCount = ${gallio.RunCount}" />
        ///     <echo message="SkipCount = ${gallio.SkipCount}" />
        ///     <echo message="TestCount = ${gallio.TestCount}" />
        /// </target>
        /// ]]>
        /// </code>
        /// </example>
        [TaskAttribute("result-properties-prefix")]
        [StringValidator(AllowEmpty = false)]
        public string ResultPropertiesPrefix
        {
            set { resultPropertiesPrefix = value; }
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
            // We don't catch exceptions here because NAnt takes care of that job,
            // and decides whether to let them through based on the value of the
            // FailOnError
            TaskLogger logger = new TaskLogger(this);

            DisplayVersion();

            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
                launcher.Filter = GetFilter();
                launcher.ShowReports = showReports;
                launcher.DoNotRun = doNotRun;
                launcher.EchoResults = echoResults;
                launcher.RuntimeSetup = new RuntimeSetup();

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
            PackageRunStatistics stats = result.Statistics;

            Properties[resultPropertiesPrefix + @"TestCount"] = stats.TestCount.ToString();
            Properties[resultPropertiesPrefix + @"PassCount"] = stats.PassCount.ToString();
            Properties[resultPropertiesPrefix + @"FailureCount"] = stats.FailureCount.ToString();
            Properties[resultPropertiesPrefix + @"IgnoreCount"] = stats.IgnoreCount.ToString();
            Properties[resultPropertiesPrefix + @"InconclusiveCount"] = stats.InconclusiveCount.ToString();
            Properties[resultPropertiesPrefix + @"RunCount"] = stats.RunCount.ToString();
            Properties[resultPropertiesPrefix + @"SkipCount"] = stats.SkipCount.ToString();
            Properties[resultPropertiesPrefix + @"Duration"] = stats.Duration.ToString();
            Properties[resultPropertiesPrefix + @"AssertCount"] = stats.AssertCount.ToString();
        }

        private Filter<ITest> GetFilter()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return new AnyFilter<ITest>();
            }

            return FilterUtils.ParseTestFilter(filter);
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
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

            Log(Level.Info, String.Format(Resources.TaskNameAndVersion,
                appVersion.Major, appVersion.Minor, appVersion.Build));
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
