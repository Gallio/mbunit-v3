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
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// A MSBuild Task implementation that allows to run MbUnit.
    /// </summary>
    public class MbUnit : Task
    {
        #region Private Members

        private ITaskItem[] assemblies;
        private ITaskItem[] pluginDirectories;
        private ITaskItem[] hintDirectories;
        private string filter;
        private string[] reportTypes = new string[] { };
        private string reportNameFormat = "mbunit-result-{0}{1}";
        private string reportDirectory = String.Empty;
        private bool ignoreFailures = false;
        private int exitCode;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of test assemblies to execute. This is required.
        /// </summary>
        [Required]
        public ITaskItem[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        public ITaskItem[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        /// <summary>
        ///  Additional MbUnit plugin directories to search recursively.
        /// </summary>
        public ITaskItem[] PluginDirectories
        {
            get { return pluginDirectories; }
            set { pluginDirectories = value; }
        }

        /// <summary>
        /// An array of report types to generate.
        /// </summary>
        public string[] ReportTypes
        {
            get { return reportTypes; }
            set { reportTypes = value; }
        }

        /// <summary>
        /// A format string to use to generate the reports filename.
        /// </summary>
        public string ReportNameFormat
        {
            get { return reportNameFormat; }
            set { reportNameFormat = value; }
        }

        /// <summary>
        /// The directory where the reports will be put.
        /// </summary>
        public string ReportDirectory
        {
            get { return reportDirectory; }
            set { reportDirectory = value; }
        }       

        /// <summary>
        /// Whether or not to halt on failure.
        /// </summary>
        public bool IgnoreFailures
        {
            get { return ignoreFailures; }
            set { ignoreFailures = value; }
        }

        /// <summary>
        /// The exit code of the tests execution.
        /// </summary>
        [Output]
        public int ExitCode
        {
            get { return exitCode; }
            set { exitCode = value; }
        } 

        #endregion

        /// <summary>
        /// The filter to apply in the format "property=value;property=value;..."
        /// If left empty the "Any" filter will be applied.
        /// </summary>
        public string Filter
        {
            get { return filter; }
            set { filter = value; }
        }

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
                Log.LogError("Unexpected failure during MbUnit execution");
                Log.LogErrorFromException(ex,true);
                return IgnoreFailures;
            }
        }

        #endregion

        #region Private Methods

        private bool InternalExecute()
        {
            DisplayVersion();
            MSBuildLogger logger = new MSBuildLogger(Log);
            using (TestRunnerHelper runner = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                GetFilter()
                ))
            {
                AddAllItemSpecs(runner.Package.AssemblyFiles, assemblies);
                AddAllItemSpecs(runner.Package.HintDirectories, hintDirectories);
                AddAllItemSpecs(runner.RuntimeSetup.PluginDirectories, pluginDirectories);
                                
                runner.ReportDirectory = ReportDirectory;
                runner.ReportNameFormat = ReportNameFormat;
                runner.ReportFormats.AddRange(ReportTypes);

                //TODO: Check whether this makes sense for this runner
                //runner.TemplateModelFilename = SaveTemplateTree;
                //runner.TestModelFilename = SaveTestTree;

                ExitCode = runner.Run();

                //TODO: Maybe count ResultCode.NoTests as sucess too? 
                if (ExitCode == ResultCode.Success || IgnoreFailures)
                    return true;
            }

            return false;
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
            Log.LogMessage(String.Format("MbUnit MSBuild Task - Version {0}.{1} build {2}",
                                         appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        private static void AddAllItemSpecs(IList<string> collection, IList<ITaskItem> items)
        {
            //TODO: Check for nothing in the TestPackage and RuntimeSetup classes?
            if (items != null)
            {
                foreach (ITaskItem item in items)
                    collection.Add(item.ItemSpec);
            }
        }
        
        #endregion
    }
}
