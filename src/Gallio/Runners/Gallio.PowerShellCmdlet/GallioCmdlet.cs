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
using System.Diagnostics;
using Gallio.Collections;
using Gallio.Core.ProgressMonitoring;
using Gallio.Hosting;
using Gallio.Model;
using System.Management.Automation;
using Gallio.Model.Filters;
using Gallio.PowerShellCmdlet.Properties;
using Gallio.Runner;

namespace Gallio.PowerShellCmdlet
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
    /// # Makes the Gallio cmdlet available
    /// Add-PSSnapIn GallioCmdlet
    /// # Runs TestAssembly1.dll
    /// Run-Gallio "[Path-to-assembly1]\TestAssembly1.dll" -f Category:UnitTests
    /// -rd C:\build\reports -rf html
    /// </code>
    /// </example>
    [Cmdlet("Run", "Gallio")]
    public class GallioCmdlet : PSCmdlet
    {
        #region Private Members

        private string[] assemblies;
        private string[] pluginDirectories;
        private string[] hintDirectories;
        private string[] reportTypes = new string[] { };
        private string reportNameFormat = Resources.DefaultReportNameFormat;
        private string reportDirectory = String.Empty;
        private string filter;
        private SwitchParameter terminateOnFailure = false;
        private SwitchParameter showReports = false;

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
        /// <example>The following example shows how to specify the hint directories:
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
        /// <example>The following example shows how to specify the plugin directories:
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
        /// In the following example reports will be generated in both HTML and XML format:
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
        /// Sets whether or not to terminate on failure.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>This parameter controls the behavior of the cmdlet in case an unexpected
        /// exception occurs while running the tests. Of set
        /// </item>
        /// <item>This parameter takes the value true if present and false if not. No
        /// value has to be specified.</item> 
        /// </list> 
        /// </remarks>
        /// <example>
        /// <code>
        /// # Doesn't cause PowerShell to terminate in case an expected exception occurs
        /// Run-Gallio SomeAssembly.dll
        /// # Causes PowerShell to terminate in case an expected exception occurs
        /// Run-Gallio SomeAssembly.dll -tof
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("tof", "terminate-on-failures")]
        public SwitchParameter TerminateOnFailure
        {
            //NOTE: In the tasks runners the IgnoreFailures parameter is used instead
            //of this one. The decision of "reversing the sense" of the parameter in
            //this Cmdlet was based on the Microsoft suggestion of using SwitchParameter
            //instead of boolean types, and the fact that we want the default behavior
            //to be not to terminate on failures.
            //See http://msdn2.microsoft.com/en-us/library/ms714433.aspx for more info.
            set { terminateOnFailure = value; }
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

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            try
            {
                WriteObject(Execute());
            }
            catch (Exception ex)
            {
                ErrorRecord error = new ErrorRecord
                    (
                    ex,
                    Resources.UnexpectedErrorDuringExecution,
                    ErrorCategory.NotSpecified,
                    "GallioCmdlet"
                    );
                if (!terminateOnFailure)
                {
                    WriteError(error);
                }
                else
                {
                    ThrowTerminatingError(error);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected TestLauncherResult Execute()
        {
            PowerShellLogger logger = new PowerShellLogger(this);
            using (TestLauncher launcher = new TestLauncher())
            {
                launcher.Logger = logger;
                launcher.ProgressMonitorProvider = new LogProgressMonitorProvider(logger);
                launcher.Filter = GetFilter();
                launcher.RuntimeSetup = new RuntimeSetup();
                launcher.TestPackageConfig.EnableShadowCopy = true;
                // Please see the comments in Gallio.TDNetRunner.GallioTestRunner class
                // about the isolated test runner problem.
                launcher.TestRunnerFactory = TestRunnerFactory.CreateLocalTestRunner;

                AddAllItemSpecs(launcher.TestPackageConfig.AssemblyFiles, assemblies);
                AddAllItemSpecs(launcher.TestPackageConfig.HintDirectories, hintDirectories);
                AddAllItemSpecs(launcher.RuntimeSetup.PluginDirectories, pluginDirectories);

                if (reportDirectory != null)
                    launcher.ReportDirectory = reportDirectory;
                if (!String.IsNullOrEmpty(reportNameFormat))
                    launcher.ReportNameFormat = reportNameFormat;
                if (reportTypes != null)
                    GenericUtils.AddAll(reportTypes, launcher.ReportFormats);

                TestLauncherResult result = RunLauncher(launcher);
                ShowReportsIfAsked(result);

                return result;
            }
        }

        /// <summary>
        /// Provided so that the unit tests can skip this.
        /// </summary>
        protected virtual void ShowReportsIfAsked(TestLauncherResult result)
        {
            if (showReports.IsPresent)
            {
                foreach (string report in result.ReportDocumentPaths)
                {
                    Process p = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo(report);
                    p.StartInfo = startInfo;
                    p.Start();
                }
            }
        }

        /// <summary>
        /// Provided so that the unit tests can override test execution behavior.
        /// </summary>
        protected virtual TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            return launcher.Run();
        }

        private Filter<ITest> GetFilter()
        {
            if (String.IsNullOrEmpty(filter))
            {
                return new AnyFilter<ITest>();
            }

            return FilterUtils.ParseTestFilter(filter);
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
