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
using System.Globalization;
using System.Reflection;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Filters;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace MbUnit.Tasks.NAnt
{
    /// <summary>
    /// A NAnt task for launching MbUnit.
    /// </summary>
    [TaskName("mbunit")]
    public class MbUnitTask : Task
    {
        #region Private Members

        private FileSet[] assemblies;
        private DirSet[] hintDirectories;
        private DirSet[] pluginDirectories;
        private string filter;
        private string reportTypes = "html";
        private string reportFileNameFormat = "mbunit-result-{0}{1}";
        private string reportOutputDirectory = String.Empty;
        private bool ignoreFailures = false;
        private string resultProperty;

        #endregion

        #region Public Properties

        ///<summary>
        /// The list of test assemblies to execute. This is required.
        ///</summary>
        [BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
        public FileSet[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        [BuildElementArray("hint-directories", ElementType = typeof(DirSet))]
        public DirSet[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        /// <summary>
        /// Additional MbUnit plugin directories to search recursively.
        /// </summary>
        [BuildElementArray("plugin-directories", ElementType = typeof(DirSet))]
        public DirSet[] PluginDirectories
        {
            get { return pluginDirectories; }
            set { pluginDirectories = value; }
        }

        ///<summary>
        /// An array of report types to generate.
        ///</summary>
        [TaskAttribute("report-types")]
        public string ReportTypes
        {
            get { return reportTypes; }
            set { reportTypes = value; }
        }

        ///<summary>
        /// A format string to use to generate the reports filename.
        ///</summary>
        [TaskAttribute("report-filename-format", Required = false)]
        public string ReportFileNameFormat
        {
            get { return reportFileNameFormat; }
            set { reportFileNameFormat = value; }
        }

        ///<summary>
        /// The directory where the reports will be put.
        ///</summary>
        [TaskAttribute("report-output-directory", Required = false)]
        public string ReportOutputDirectory
        {
            get { return reportOutputDirectory; }
            set { reportOutputDirectory = value; }
        }

        ///<summary>
        /// Whether or not to halt on failure.
        ///</summary>
        [BooleanValidator, TaskAttribute("ignore-failures")]
        public bool IgnoreFailures
        {
            get { return ignoreFailures; }
            set { ignoreFailures = value; }
        }

        /// <summary>
        /// The name of a property in which the exit code of the tests execution
        /// should be stored. Only of interest if <see cref="IgnoreFailures" /> is 
        /// true.
        /// </summary>
        [TaskAttribute("result-property")]
        [StringValidator(AllowEmpty = false)]
        public string ResultProperty
        {
            get { return resultProperty; }
            set { resultProperty = value; }
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

        #endregion

        #region Public Methods"

        /// <inheritdoc />
        protected override void ExecuteTask()
        {
            DisplayVersion();
            DisplaySpecificTaskConfiguration();
            NAntLogger logger = new NAntLogger(this);
            using (TestRunnerHelper runner = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                new AnyFilter<ITest>()
                ))
            {
                AddAssemblies(runner);
                AddHintDirectories(runner);
                AddPluginDirectories(runner);
                int resultCode = runner.Run();
                ProcessResultCode(resultCode);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks the result code of the tests exceution and performs the
        /// corresponding action
        /// </summary>
        /// <param name="resultCode"></param>
        private void ProcessResultCode(int resultCode)
        {
            if (!String.IsNullOrEmpty(ResultProperty))
            {
                Properties[ResultProperty] = resultCode.ToString(CultureInfo.InvariantCulture);
            }
            if (IgnoreFailures)
            {
                //TODO: Maybe count ResultCode.NoTests as sucess too?
                if (resultCode != ResultCode.Success)
                {
                    throw new BuildException("MbUnit Tests Execution Failed");
                }
            }
        }

        private void DisplayVersion()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log(Level.Info, String.Format("MbUnit NAnt Task - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        private void AddAssemblies(TestRunnerHelper runner)
        {

            foreach (FileSet fs in Assemblies)
            {
                foreach (string f in fs.FileNames)
                    runner.Package.AssemblyFiles.Add(f);
            }
        }

        private void AddHintDirectories(TestRunnerHelper runner)
        {
            if (HintDirectories != null)
            {
                foreach (DirSet ds in HintDirectories)
                {
                    foreach (string d in ds.FileNames)
                        runner.Package.HintDirectories.Add(d);
                }
            }
        }

        private void AddPluginDirectories(TestRunnerHelper runner)
        {
            if (pluginDirectories != null)
            {
                foreach (DirSet ds in pluginDirectories)
                {
                    foreach (string d in ds.FileNames)
                        runner.RuntimeSetup.PluginDirectories.Add(d);
                }
            }
        }

        private void DisplaySpecificTaskConfiguration()
        {
            Log(Level.Verbose, "Task Configuration");
            Log(Level.Verbose, "IgnoreFailures: {0}", IgnoreFailures);
        }

        #endregion
    }
}
