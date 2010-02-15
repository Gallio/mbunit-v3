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
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Text;
using Gallio.Runtime;
using Gallio.Model;
using System.Management.Automation;
using Gallio.Model.Filters;
using Gallio.PowerShellCommands.Properties;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using Gallio.Runtime.Debugging;

namespace Gallio.PowerShellCommands
{
    /// <summary>
    /// A PowerShell Cmdlet for running Gallio.
    /// </summary>
    /// <remarks>
    /// Only the <see cref="Files"/> parameter is required.
    /// </remarks>
    /// <example>
    /// <para>There are severals ways to run this cmdlet:</para>
    /// <code><![CDATA[
    /// # Makes the Gallio commands available
    /// Add-PSSnapIn Gallio
    /// 
    /// # Runs a few assemblies and scripts.
    /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll","[Path-to-assembly2]\TestAssembly2.dll","[Path-to-test-script1]/TestScript1_spec.rb","[Path-to-test-script2]/TestScript2.xml" -f Category:UnitTests -rd C:\build\reports -rf html -ra zip
    /// ]]></code>
    /// </example>
    [Cmdlet("Run", "Gallio")]
    public class RunGallioCommand : BaseCommand
    {
        private TimeSpan? runTimeLimit;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RunGallioCommand()
        {
            ReportFormatterProperties = EmptyArray<string>.Instance;
            RunnerProperties = EmptyArray<string>.Instance;
            RunnerExtensions = EmptyArray<string>.Instance;
            Filter = string.Empty;
            ReportTypes = EmptyArray<string>.Instance;
        }

        /// <summary>
        /// The list of relative or absolute paths of test files, projects and assemblies to execute.
        /// Wildcards may be used.  This is required.
        /// </summary>
        /// <example>
        /// <para>There are severals ways to pass the test files to the cmdlet:</para>
        /// <code><![CDATA[
        /// # Runs TestAssembly1.dll
        /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll"
        /// 
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll","[Path-to-assembly2]\TestAssembly2.dll"
        /// 
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// $assemblies = "[Path-to-assembly1]\TestAssembly1.dll","[Path-to-assembly2]\TestAssembly2.dll"
        /// Run-Gallio $assemblies
        /// 
        /// # Runs TestAssembly1.dll, TestAssembly2.dll, TestScript1_spec.rb, and TestScript2.xml
        /// $assembly1 = "[Path-to-assembly1]\TestAssembly1.dll"
        /// $assembly2 = "[Path-to-assembly2]\TestAssembly2.dll"
        /// $script1 = "[Path-to-test-script1]\TestScript1_spec.rb"
        /// $script2 = "[Path-to-test-script2]/TestScript2.xml"
        /// $files = $assembly1,$assembly2,$script1,$script2
        /// Run-Gallio $files
        /// 
        /// # If you don't specify the test files, PowerShell will prompt you for the names:
        /// PS C:\Documents and Settings\jhi> Run-Gallio
        ///
        /// cmdlet Run-Gallio at command pipeline position
        /// Supply values for the following parameters:
        /// Files[0]:
        /// ]]></code>
        /// </example>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true), ValidateNotNullOrEmpty, ValidateCount(1, 99999)]
        public string[] Files
        {
            private get;
            set;
        }

        /// <summary>
        /// The list of directories used for loading referenced assemblies and other dependent resources.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the hint directories:</para>
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -hd C:\SomeFolder
        /// ]]></code>
        /// <para>See the <see cref="Files"/> property for more ways of passing list of parameters to the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("hd")]
        public string[] HintDirectories
        {
            private get;
            set;
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the plugin directories:</para>
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -pd C:\SomeFolder
        /// ]]></code>
        /// <para>See the <see cref="Files"/> property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("pd")]
        public string[] PluginDirectories
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("abd")]
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("wd")]
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("sc")]
        public SwitchParameter? ShadowCopy
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("d")]
        public SwitchParameter? DebugTests
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the version of the .Net runtime to use for running tests.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For the CLR, this must be the name of one of the framework directories in <c>%SystemRoot%\Microsoft.Net\Framework.</c>  eg. 'v2.0.50727'.
        /// </para>
        /// <para>
        /// The default is null which uses the most recent installed and supported framework.
        /// </para>
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("rv")]
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
        /// <para>In the following example reports will be generated in both HTML and XML format:</para>
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -rt "html","text"
        /// ]]></code>
        /// <para>See the <see cref="Files"/> property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("rt", "report-types")]
        public string[] ReportTypes
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
        /// </para>
        /// <para>
        /// The default format string is <c>test-report-{0}-{1}</c>.
        /// </para>
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("rnf", "report-name-format")]
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("rd", "report-directory")]
        public string ReportDirectory
        {
            private get;
            set;
        }

        /// <summary>
        /// Specifies to enclose the resulting reports into a compressed archive file (zip).
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("ra", "report-archive")]
        public string ReportArchive
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("f")]
        public string Filter
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to open the generated reports once execution has finished.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>This parameter takes the value true if present and false if not. No
        /// value has to be specified.</item>
        /// <item>
        /// The reports are opened in a window using the default system application
        /// registered to the report file type.
        /// </item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// # Doesn't show the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll
        /// 
        /// # Shows the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll -sr
        /// ]]></code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("sr", "show-reports")]
        public SwitchParameter ShowReports
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the type of test runner to use.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The types supported "out of the box" are: <c>Local</c>, <c>IsolatedAppDomain</c>
        /// and <c>IsolatedProcess</c> (default), but more types could be available as plugins.</item>
        /// <item>The runner types are not case sensitive.</item>
        /// </list>
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("runner-type")]
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
        /// The value must be in the form <c>'[Namespace.]Type,Assembly[;Parameters]'</c> .
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example runs tests using a custom logger extension:
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -runner-extension 'FancyLogger,MyExtensions.dll;ColorOutput,FancyIndenting'
        /// ]]></code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("runner-extension")]
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
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -runner-property "NCoverArguments='//eas Gallio'"
        /// ]]></code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("runner-property")]
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
        /// <code><![CDATA[
        /// Run-Gallio SomeAssembly.dll -report-formatter-property "AttachmentContentDisposition=Absent"
        /// ]]></code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("report-formatter-property")]
        public string[] ReportFormatterProperties
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// This option may be used to produce a report that contains test metadata for consumption by other tools.
        /// </para>
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("dnr", "do-not-run")]
        public SwitchParameter DoNotRun
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
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("ia", "ignore-annotations")]
        public SwitchParameter IgnoreAnnotations
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this option is specified only the final summary statistics are displayed.  Otherwise test results are echoed to the
        /// console in varying detail depending on the current verbosity level.
        /// </para>
        /// </remarks>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("ne", "no-echo-results")]
        public SwitchParameter NoEchoResults
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the maximum amount of time (in seconds) the tests can run 
        /// before they are canceled. The default is an infinite time to run. 
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true), Alias("rtl", "run-time-limit")]
        public double RunTimeLimit
        {
            set
            {
                runTimeLimit = TimeSpan.FromSeconds(value);
            }
        }

        /// <exclude />
        protected override void EndProcessing()
        {
            try
            {
                WriteObject(ExecuteWithMessagePump());
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, Resources.UnexpectedErrorDuringExecution, ErrorCategory.NotSpecified, null));
            }
        }

        internal TestLauncherResult ExecuteWithMessagePump()
        {
            TestLauncherResult result = null;

            RunWithMessagePump(delegate
            {
                result = ExecuteWithCurrentDirectory();
            });

            if (result == null)
                throw new InvalidOperationException("The task finished without returning a result.");

            return result;
        }

        internal TestLauncherResult ExecuteWithCurrentDirectory()
        {
            if (SessionState != null)
            {
                // FIXME: Will this throw an exception if the current path is within a virtual file system?
                string resolvedDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
                using (new CurrentDirectorySwitcher(resolvedDirectory))
                    return Execute();
            }

            return Execute();
        }
        
        internal TestLauncherResult Execute()
        {
            var launcher = new TestLauncher
            {
                Logger = Logger,
                ProgressMonitorProvider = ProgressMonitorProvider,
                TestExecutionOptions = { FilterSet = GetFilterSet() },
                ShowReports = ShowReports.IsPresent,
                DoNotRun = DoNotRun.IsPresent,
                IgnoreAnnotations = IgnoreAnnotations.IsPresent,
                EchoResults = !NoEchoResults.IsPresent,
                RunTimeLimit = runTimeLimit
            };

            if (RunnerType != null)
                launcher.TestProject.TestRunnerFactoryName = RunnerType;
            if (RunnerExtensions != null)
                GenericCollectionUtils.ForEach(RunnerExtensions, x => launcher.TestProject.AddTestRunnerExtensionSpecification(x));

            launcher.RuntimeSetup = new RuntimeSetup();

            // Set the installation path explicitly to the path of the Gallio cmdlet task assembly
            // since otherwise we will look at the path of PowerShell.exe.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(RunGallioCommand).Assembly));

            if (ApplicationBaseDirectory != null)
                launcher.TestProject.TestPackage.ApplicationBaseDirectory = new DirectoryInfo(ApplicationBaseDirectory);
            if (WorkingDirectory != null)
                launcher.TestProject.TestPackage.WorkingDirectory = new DirectoryInfo(WorkingDirectory);
            if (ShadowCopy.HasValue)
                launcher.TestProject.TestPackage.ShadowCopy = ShadowCopy.Value.IsPresent;
            if (DebugTests.HasValue && DebugTests.Value.IsPresent)
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

            ForEachItem(Files, launcher.AddFilePattern);
            ForEachItem(HintDirectories, x => launcher.TestProject.TestPackage.AddHintDirectory(new DirectoryInfo(x)));
            ForEachItem(PluginDirectories, x => launcher.RuntimeSetup.AddPluginDirectory(x));

            if (ReportDirectory != null)
                launcher.TestProject.ReportDirectory = ReportDirectory;
            if (ReportNameFormat != null)
                launcher.TestProject.ReportNameFormat = ReportNameFormat;
            if (ReportTypes != null)
                GenericCollectionUtils.ForEach(ReportTypes, launcher.AddReportFormat);
            launcher.TestProject.ReportArchive = Runner.Reports.ReportArchive.Parse(ReportArchive); 
            TestLauncherResult result = RunLauncher(launcher);
            return result;
        }

        /// <exclude />
        /// <summary>
        /// Provided so that the unit tests can override test execution behavior.
        /// </summary>
        protected virtual TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            return launcher.Run();
        }

        private FilterSet<ITestDescriptor> GetFilterSet()
        {
            if (String.IsNullOrEmpty(Filter))
            {
                return FilterSet<ITestDescriptor>.Empty;
            }

            return FilterUtils.ParseTestFilterSet(Filter);
        }

        private static void ForEachItem(IEnumerable<string> items, Action<string> action)
        {
            if (items != null)
            {
                foreach (string item in items)
                    action(item);
            }
        }
    }
}
