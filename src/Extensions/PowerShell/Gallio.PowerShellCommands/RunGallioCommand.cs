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
using System.Collections.Generic;
using System.IO;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.IO;
using Gallio.Runtime;
using Gallio.Model;
using System.Management.Automation;
using Gallio.Model.Filters;
using Gallio.PowerShellCommands.Properties;
using Gallio.Common.Reflection;
using Gallio.Runner;

namespace Gallio.PowerShellCommands
{
    /// <summary>
    /// A PowerShell Cmdlet for running Gallio.
    /// </summary>
    /// <remarks>
    /// Only the Assemblies parameter is required.
    /// </remarks>
    /// <example>
    /// <para>There are severals ways to run this cmdlet:</para>
    /// <code>
    /// # Makes the Gallio commands available
    /// Add-PSSnapIn Gallio
    /// # Runs TestAssembly1.dll
    /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll" -f Category:UnitTests -rd C:\build\reports -rf html
    /// </code>
    /// </example>
    [Cmdlet("Run", "Gallio")]
    public class RunGallioCommand : BaseCommand
    {
        #region Private Members

        private string[] assemblies;
        private string[] pluginDirectories;
        private string[] hintDirectories;

        private string applicationBaseDirectory;
        private string workingDirectory;
        private SwitchParameter shadowCopy;
        private SwitchParameter debug;

        private string[] reportTypes = EmptyArray<string>.Instance;
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string runnerType = StandardTestRunnerFactoryNames.IsolatedProcess;
        private string[] runnerExtensions = EmptyArray<string>.Instance;
        private string filter = string.Empty;
        private SwitchParameter showReports;
        private SwitchParameter doNotRun;
        private SwitchParameter ignoreAnnotations;
        private SwitchParameter noEchoResults;
        private TimeSpan? runTimeLimit;

        private string[] runnerProperties = EmptyArray<string>.Instance;
        private string[] reportFormatterProperties = EmptyArray<string>.Instance;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of relative or absolute paths of test assembly files to execute.
        /// This is required.
        /// </summary>
        /// <example>
        /// <para>There are severals ways to pass the test assemblies names to the
        /// cmdlet:</para>
        /// <code>
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
        /// # Runs TestAssembly1.dll and TestAssembly2.dll
        /// $assembly1 = "[Path-to-assembly1]\TestAssembly1.dll"
        /// $assembly2 = "[Path-to-assembly2]\TestAssembly2.dll"
        /// $assemblies = $assembly1,$assembly2
        /// Run-Gallio $assemblies
        /// 
        /// # If you don't specify the test assemblies, PowerShell will prompt you for the names:
        /// PS C:\Documents and Settings\jhi> Run-Gallio
        ///
        /// cmdlet Run-Gallio at command pipeline position
        /// Supply values for the following parameters:
        /// Assemblies[0]:
        /// </code>
        /// </example>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true
            )]
        [ValidateNotNullOrEmpty]
        [ValidateCount(1, 99999)]
        public string[] Assemblies
        {
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the hint directories:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -hd C:\SomeFolder
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("hd")]
        public string[] HintDirectories
        {
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional Gallio plugin directories to search recursively.
        /// </summary>
        /// <example>
        /// <para>The following example shows how to specify the plugin directories:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -pd C:\SomeFolder
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("pd")]
        public string[] PluginDirectories
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("abd")]
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("wd")]
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("sc")]
        public SwitchParameter ShadowCopy
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("d")]
        public bool DebugTests
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
        /// <para>In the following example reports will be generated in both HTML and XML format:</para>
        /// <code>
        /// Run-Gallio SomeAssembly.dll -rt "html","text"
        /// </code>
        /// <para>See the Assemblies property for more ways of passing list of parameters to
        /// the cmdlet.</para>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rt", "report-types")]
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rnf", "report-name-format")]
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rd", "report-directory")]
        public string ReportDirectory
        {
            set { reportDirectory = value; }
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("f")]
        public string Filter
        {
            set { filter = value; }
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
        /// <code>
        /// # Doesn't show the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll
        /// # Shows the reports once execution has finished
        /// Run-Gallio SomeAssembly.dll -sr
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("sr", "show-reports")]
        public SwitchParameter ShowReports
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("runner-type")]
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
        /// Run-Gallio SomeAssembly.dll -runner-extension 'FancyLogger,MyExtensions.dll;ColorOutput,FancyIndenting'
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("runner-extension")]
        public string[] RunnerExtensions
        {
            set { runnerExtensions = value; }
        }

        /// <summary>
        /// Specifies option property key/value pairs for the test runner.
        /// </summary>
        /// <example>
        /// The following example specifies some extra NCover arguments.
        /// <code>
        /// Run-Gallio SomeAssembly.dll -runner-property "NCoverArguments='//eas Gallio'"
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("runner-property")]
        public string[] RunnerProperties
        {
            set { runnerProperties = value; }
        }

        /// <summary>
        /// Specifies option property key/value pairs for the report formatter.
        /// </summary>
        /// <example>
        /// The following example changes the default attachment content disposition for the reports.
        /// <code>
        /// Run-Gallio SomeAssembly.dll -report-formatter-property "AttachmentContentDisposition=Absent"
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("report-formatter-property")]
        public string[] ReportFormatterProperties
        {
            set { reportFormatterProperties = value; }
        }

        /// <summary>
        /// Sets whether to load the tests but not run them.  This option may be used to produce a
        /// report that contains test metadata for consumption by other tools.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("dnr", "do-not-run")]
        public SwitchParameter DoNotRun
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
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("ia", "ignore-annotations")]
        public SwitchParameter IgnoreAnnotations
        {
            set { ignoreAnnotations = value; }
        }

        /// <summary>
        /// Sets whether to echo results to the screen as tests finish.  If this option is specified
        /// only the final summary statistics are displayed.  Otherwise test results are echoed to the
        /// console in varying detail depending on the current verbosity level.
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("ne", "no-echo-results")]
        public SwitchParameter NoEchoResults
        {
            set { noEchoResults = value; }
        }

        /// <summary>
        /// Sets the maximum amount of time (in seconds) the tests can run 
        /// before they are canceled. The default is an infinite time to run. 
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("rtl", "run-time-limit")]
        public double RunTimeLimit
        {
            set { runTimeLimit = TimeSpan.FromSeconds(value); }
        }

        #endregion

        #region Protected Methods

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

        #endregion

        #region Private Methods

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
                // FIXME: Will this throw an exception if the current path is
                //        within a virtual file system?
                string resolvedDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
                using (new CurrentDirectorySwitcher(resolvedDirectory))
                    return Execute();
            }

            return Execute();
        }
        
        internal TestLauncherResult Execute()
        {
            TestLauncher launcher = new TestLauncher();
            launcher.Logger = Logger;
            launcher.ProgressMonitorProvider = ProgressMonitorProvider;
            launcher.TestExecutionOptions.FilterSet = GetFilterSet();
            launcher.ShowReports = showReports.IsPresent;
            launcher.DoNotRun = doNotRun.IsPresent;
            launcher.IgnoreAnnotations = ignoreAnnotations.IsPresent;
            launcher.EchoResults = !noEchoResults.IsPresent;
            launcher.RunTimeLimit = runTimeLimit;

            launcher.TestRunnerFactoryName = runnerType;
            if (runnerExtensions != null)
                GenericCollectionUtils.AddAll(runnerExtensions, launcher.TestRunnerExtensionSpecifications);

            launcher.RuntimeSetup = new RuntimeSetup();

            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            launcher.RuntimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(RunGallioCommand).Assembly));

            launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory = applicationBaseDirectory;
            launcher.TestPackageConfig.HostSetup.WorkingDirectory = workingDirectory;
            launcher.TestPackageConfig.HostSetup.ShadowCopy = shadowCopy.IsPresent;
            launcher.TestPackageConfig.HostSetup.Debug = debug.IsPresent;

            foreach (string option in reportFormatterProperties)
                launcher.ReportFormatterOptions.Properties.Add(StringUtils.ParseKeyValuePair(option));

            foreach (string option in runnerProperties)
                launcher.TestRunnerOptions.Properties.Add(StringUtils.ParseKeyValuePair(option));

            AddAllItemSpecs(launcher.TestPackageConfig.AssemblyFiles, assemblies);
            AddAllItemSpecs(launcher.TestPackageConfig.HintDirectories, hintDirectories);
            AddAllItemSpecs(launcher.RuntimeSetup.PluginDirectories, pluginDirectories);

            if (reportDirectory != null)
                launcher.ReportDirectory = reportDirectory;
            if (!String.IsNullOrEmpty(reportNameFormat))
                launcher.ReportNameFormat = reportNameFormat;
            if (reportTypes != null)
                GenericCollectionUtils.AddAll(reportTypes, launcher.ReportFormats);

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

        private FilterSet<ITest> GetFilterSet()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return FilterSet<ITest>.Empty;
            }

            return FilterUtils.ParseTestFilterSet(filter);
        }

        private static void AddAllItemSpecs(ICollection<string> collection, IEnumerable<string> items)
        {
            if (items != null)
            {
                foreach (string item in items)
                    collection.Add(item);
            }
        }

        #endregion
    }
}
