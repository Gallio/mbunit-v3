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
        private string filter;
        private string[] reportTypes = new string[] { "html" };
        private string reportFileNameFormat = "mbunit-result-{0}{1}";
        private string reportOutputDirectory = String.Empty;
        private bool ignoreFailures = false;
        private int exitCode;

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
                new AnyFilter<ITest>()
                ))
            {
                runner.AddAssemblyFiles(Assemblies);
                runner.AddHintDirectories(hintDirectories);
                runner.AddPluginDirectories(pluginDirectories);
                ExitCode = runner.Run();
                //TODO: Maybe count ResultCode.NoTests as sucess too? 
                if (ExitCode == ResultCode.Success || IgnoreFailures)
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