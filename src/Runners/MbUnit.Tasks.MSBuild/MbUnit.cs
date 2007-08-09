using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MbUnit.Core.Runner;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// MSBuild Task implementation for MbUnit
    /// </summary>
    public class MbUnit : Task
    {
        #region Private Members

        private string[] assemblies;
        private string[] assemblyPaths;
        private string[] pluginDirectories;
        private string[] hintDirectories;
        private string reportFileNameFormat = "mbunit.{0}.{1}";
        private string reportOutputDirectory = "";
        private string[] reportTypes = new string[] { "html" };
        private bool haltOnFailure = false;
        private bool haltOnError = false;
        //private ReportResult result = null; 

        #endregion

        #region Public Properties

        public string ReportFileNameFormat
        {
            get { return this.reportFileNameFormat; }
            set { this.reportFileNameFormat = value; }
        }

        public string ReportOutputDirectory
        {
            get { return this.reportOutputDirectory; }
            set { this.reportOutputDirectory = value; }
        }

        public string[] ReportTypes
        {
            get { return this.reportTypes; }
            set { this.reportTypes = value; }
        }

        [Required]
        public string[] Assemblies
        {
            get { return this.assemblies; }
            set { this.assemblies = value; }
        }

        public string[] AssemblyPaths
        {
            get { return this.assemblyPaths; }
            set { this.assemblyPaths = value; }
        }

        public string[] PluginDirectories
        {
            get { return pluginDirectories; }
            set { pluginDirectories = value; }
        }

        public string[] HintDirectories
        {
            get { return hintDirectories; }
            set { hintDirectories = value; }
        }

        public bool HaltOnFailure
        {
            get { return this.haltOnFailure; }
            set { this.haltOnFailure = value; }
        }

        public bool HaltOnError
        {
            get { return this.haltOnError; }
            set { this.haltOnError = value; }
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
                return this.InternalExecute();
            }
            catch (Exception ex)
            {
                this.Log.LogError("Unexpected failure during MbUnit execution");
                this.Log.LogErrorFromException(ex,true);
                return !this.HaltOnError;
            }
        }

        #endregion

        #region Private Methods

        private bool InternalExecute()
        {
            VerifyData();

            DisplayTaskConfiguration();

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
                if (hintDirectories != null) runner.AddHintDirectories(hintDirectories);
                if (pluginDirectories != null) runner.AddPluginDirectories(pluginDirectories);
                if (runner.Run() == ResultCode.Success)
                    return true;
            }

            return false;
            //this.Log.LogMessage("MbUnit {0} test runner",
            //    typeof(Fixture).Assembly.GetName().Version);

            //this.DisplayTaskConfiguration();
            //// check data
            //this.VerifyData();

            //// create new report
            //this.result = new ReportResult();

            //// load and execute
            //using (
            //        TestDomainDependencyGraph graph =
            //        TestDomainDependencyGraph.BuildGraph(this.Assemblies, this.AssemblyPaths, FixtureFilters.Any, false))
            //{
            //    graph.Log+=new ErrorReporter(graph_Log);
            //    ReportResult r = graph.RunTests();
            //    graph.Log -= new ErrorReporter(graph_Log);
            //    result.Merge(r);
            //}
            
            //this.GenerateReports();

            //return result.Counter.FailureCount == 0;
        }

        private void DisplayTaskConfiguration()
        {
            DisplayVersion();
            DisplayPaths(Assemblies, "Valid Test assemblies:");
            DisplayPaths(AssemblyPaths, "Assembly Paths:");
            DisplayPaths(HintDirectories, "Hint Directories:");
            DisplayPaths(PluginDirectories, "Plugin Directories:");
         
            //this.Log.LogMessage("ReportTypes: {0}", this.ReportTypes);
            //this.Log.LogMessage("ReportFileNameFormat: {0}", this.ReportFileNameFormat);
            //this.Log.LogMessage("ReportOutputDirectory: {0}", this.ReportOutputDirectory);
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
                this.Log.LogMessage(name);
                foreach (string assemblyPath in paths)
                    this.Log.LogMessage("\t{0}", assemblyPath);
            }
        }

        private void VerifyData()
        {
            this.CheckAssemblyNames();
            //this.CheckReportTypes();
        }

        private void CheckAssemblyNames()
        {
            List<string> assembliesToRemove = new List<string>();
            List<string> validAssemblies = new List<string>();

            Log.LogMessage("Verifying assembly names");
            validAssemblies.AddRange(Assemblies);
            foreach (string assemblyName in this.Assemblies)
            {
                if (!File.Exists(assemblyName))
                {
                    assembliesToRemove.Add(assemblyName);
                    this.Log.LogError("Test assembly {0} cannot be found", assemblyName);
                    this.Log.LogError("Full name: {0}", Path.GetFullPath(assemblyName));
                }
            }

            // Remove invalid assemblies
            foreach(string assemblyName in assembliesToRemove)
            {
                validAssemblies.Remove(assemblyName);
            }

            Assemblies = validAssemblies.ToArray();
        }
        
        #endregion

        #region Old Code

        //private void GenerateReports()
        //{
        //    this.Log.LogMessage("Generating reports");
        //    foreach (string reportType in this.ReportTypes)
        //    {
        //        RenderReport(reportType);
        //    }
        //}

        //void graph_Log(string message)
        //{
        //    Log.LogMessage(message);
        //}

        //private void RenderReport(string reportType)
        //{
        //    this.Log.LogMessage("Render report {0}", reportType);
        //    string outputFileName = null;
        //    switch (reportType.ToLower())
        //    {
        //        case "text":
        //            outputFileName=TextReport.RenderToText(
        //                result, 
        //                this.ReportOutputDirectory, 
        //                this.ReportFileNameFormat
        //                );
        //            break;
        //        case "html":
        //            outputFileName=HtmlReport.RenderToHtml(
        //                result,
        //                this.ReportOutputDirectory,
        //                this.ReportFileNameFormat);
        //            break;
        //        case "xml":
        //            outputFileName = XmlReport.RenderToXml(
        //                result,
        //                this.ReportOutputDirectory,
        //                this.ReportFileNameFormat);
        //            break;
        //        case "dox":
        //            outputFileName=DoxReport.RenderToDox(
        //                result,
        //                this.ReportOutputDirectory,
        //                this.ReportFileNameFormat);
        //            break;
        //        default:
        //            this.Log.LogError("Report type {0} not recognized.", reportType);
        //            return;
        //    }

        //    this.Log.LogMessage("Generated {0} report at {1}",
        //        reportType,
        //        outputFileName);
        //}

        //private void CheckReportTypes()
        //{
        //    this.Log.LogMessage("Verifying report names");
        //    if (this.reportTypes != null && this.reportTypes.Length != 0)
        //        return;

        //    this.Log.LogWarning(
        //        "No ReportTypes attribute is specified. Default formatter will be used"
        //        );
        //    this.Log.LogMessage("Available types: Text,Html,Xml,Dox -> ReportTypes=\"Text;Html\"");
        //    this.reportTypes = new string[] { "Html" };
        //} 

        #endregion

    }
}