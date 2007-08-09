using System;
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
        private string reportTypes = "html";
        private string reportFileNameFormat = "mbunit-result-{0}{1}";
        private string reportOutputDirectory = String.Empty;
        private bool haltOnFailure = false;
        private bool haltOnError = false;

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
        [TaskAttribute("report-file-name-format", Required = false)]
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
        [BooleanValidator, TaskAttribute("halt-on-failure", Required = false)]
        public bool HaltOnFailure
        {
            get { return haltOnFailure; }
            set { haltOnFailure = value; }
        }

        /// <summary>
        /// Whether or not to halt on error.
        /// </summary>
        [BooleanValidator, TaskAttribute("halt-on-error", Required = false)]
        public bool HaltOnError
        {
            get { return haltOnError; }
            set { haltOnError = value; }
        }

        #endregion

        #region Public Methods"
        /// <summary>
        /// Executes the task.
        /// </summary>
        protected override void ExecuteTask()
        {
            DisplayVersion();
            NantLogger logger = new NantLogger(this);
            using (TestRunnerHelper runner = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                Verbosity.Verbose,
                new AnyFilter<ITest>()
                ))
            {
                AddAssemblies(runner);
                AddHintDirectories(runner);
                AddPluginDirectories(runner);
                runner.Run();
            }
        }

        #endregion

        #region Private Methods

        private void DisplayVersion()
        {
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Log(Level.Info, String.Format("MbUnit NAnt Task - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build));
        }

        private void AddAssemblies(TestRunnerHelper runner)
        {
            foreach (FileSet fs in Assemblies)
            {
                runner.AddAssemblyFiles(fs.FileNames);
            }
        }

        private void AddHintDirectories(TestRunnerHelper runner)
        {
            if (HintDirectories != null)
            {
                foreach (DirSet ds in HintDirectories)
                {
                    runner.AddHintDirectories(ds.Includes);
                }
            }
        }

        private void AddPluginDirectories(TestRunnerHelper runner)
        {
            if (pluginDirectories != null)
            {
                foreach (DirSet ds in pluginDirectories)
                {
                    runner.AddPluginDirectories(ds.Includes);
                }
            }
        }

        private void DisplayTaskConfiguration()
        {
            Log(Level.Verbose, "ReportTypes: {0}", ReportTypes);
            Log(Level.Verbose, "ReportFileNameFormat: {0}", ReportFileNameFormat);
            Log(Level.Verbose, "ReportOutputDirectory: {0}", ReportOutputDirectory);
            Log(Level.Verbose, "HaltOnFailure: {0}", HaltOnFailure);
            Log(Level.Verbose, "HaltOnError: {0}", HaltOnError);
        }

        #endregion
    }
}
