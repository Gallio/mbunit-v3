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

        private string[] assemblies;
        private string[] pluginDirectories;
        private string[] hintDirectories;
        private string reportFileNameFormat = "mbunit.{0}.{1}";
        private string reportOutputDirectory = "";
        private string[] reportTypes = new string[] { "html" };
        private bool haltOnFailure = false;
        private bool haltOnError = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// An array of test assemblies. This is required.
        /// </summary>
        [Required]
        public string[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        /// <summary>
        /// An array of directories that contain MbUnit plugins.
        /// </summary>
        public string[] PluginDirectories
        {
            get { return pluginDirectories; }
            set { pluginDirectories = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        /// <summary>
        /// A format string to use to generate the reports filename.
        /// </summary>
        public string ReportFileNameFormat
        {
            get { return reportFileNameFormat; }
            set { reportFileNameFormat = value; }
        }

        /// <summary>
        /// The directory where the reports will be put.
        /// </summary>
        public string ReportOutputDirectory
        {
            get { return reportOutputDirectory; }
            set { reportOutputDirectory = value; }
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
        /// Whether or not to halt on failure.
        /// </summary>
        public bool HaltOnFailure
        {
            get { return haltOnFailure; }
            set { haltOnFailure = value; }
        }

        /// <summary>
        /// Whether or not to halt on error.
        /// </summary>
        public bool HaltOnError
        {
            get { return haltOnError; }
            set { haltOnError = value; }
        } 

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <returns></returns>
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
                return !HaltOnError;
            }
        }

        #endregion

        #region Private Methods

        private bool InternalExecute()
        {
            MSBuildLogger logger = new MSBuildLogger(Log);
            using (TestRunnerHelper runner = new TestRunnerHelper
                (
                delegate { return new MSBuildProgressMonitor(logger); },
                logger,
                Verbosity.Verbose,
                new AnyFilter<ITest>()
                ))
            {
                runner.AddAssemblyFiles(Assemblies);
                runner.AddHintDirectories(hintDirectories);
                runner.AddPluginDirectories(pluginDirectories);
                DisplayTaskConfiguration();
                if (runner.Run() == ResultCode.Success)
                    return true;
            }

            return false;
        }

        private void DisplayTaskConfiguration()
        {
            DisplayVersion();
            DisplayPaths(Assemblies, "Test assemblies:");
            DisplayPaths(HintDirectories, "Hint Directories:");
            DisplayPaths(PluginDirectories, "Plugin Directories:");

            Log.LogMessage("ReportTypes: {0}", ReportTypes);
            Log.LogMessage("ReportFileNameFormat: {0}", ReportFileNameFormat);
            Log.LogMessage("ReportOutputDirectory: {0}", ReportOutputDirectory);
        }

        private void DisplayVersion()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log.LogMessage(String.Format("MbUnit MSBuild Task - Version {0}.{1} build {2}",
                                         appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        private void DisplayPaths(ICollection<string> paths, string name)
        {
            if (paths != null && paths.Count > 0)
            {
                Log.LogMessage(name);
                foreach (string assemblyPath in paths)
                    Log.LogMessage("\t{0}", assemblyPath);
            }
        }
        
        #endregion
    }
}