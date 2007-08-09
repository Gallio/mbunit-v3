using System;
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
        private string[] reportTypes = new string[] { "html" };
        private string reportFileNameFormat = "mbunit-result-{0}{1}";
        private string reportOutputDirectory = String.Empty;
        private bool haltOnFailure = false;
        private bool haltOnError = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of test assemblies to execute. This is required.
        /// </summary>
        [Required]
        public string[] Assemblies
        {
            get { return assemblies; }
            set { assemblies = value; }
        }

        /// <summary>
        /// The list of directories used for loading assemblies and other dependent resources.
        /// </summary>
        public string[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        /// <summary>
        ///  Additional MbUnit plugin directories to search recursively.
        /// </summary>
        public string[] PluginDirectories
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
        /// Executes the task.
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
            DisplayVersion();
            MSBuildLogger logger = new MSBuildLogger(Log);
            using (TestRunnerHelper runner = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                Verbosity.Verbose,
                new AnyFilter<ITest>()
                ))
            {
                runner.AddAssemblyFiles(Assemblies);
                runner.AddHintDirectories(hintDirectories);
                runner.AddPluginDirectories(pluginDirectories);
                if (runner.Run() == ResultCode.Success)
                    return true;
            }

            return false;
        }

        private void DisplayVersion()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log.LogMessage(String.Format("MbUnit MSBuild Task - Version {0}.{1} build {2}",
                                         appVersion.Major, appVersion.Minor, appVersion.Build));
        }
        
        #endregion
    }
}