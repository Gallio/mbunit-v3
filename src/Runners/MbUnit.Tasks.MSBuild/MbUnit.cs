using System;
using System.IO;
using System.Reflection;
using System.Collections;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#if false // Needs to be updated for Gallio.
namespace MbUnit.Tasks.MSBuild
{
    public class MbUnit : Task
    {
        private string[] assemblies;
        private string[] assemblyPaths;
        private string reportFileNameFormat = "mbunit.{0}.{1}";
        private string reportOutputDirectory = "";
        private string[] reportTypes = new string[] { "html" };
        private bool haltOnFailure= false;
        private bool haltOnError = false;

        private ReportResult result = null;


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

        private bool InternalExecute()
        {
            this.Log.LogMessage("MbUnit {0} test runner",
                typeof(Fixture).Assembly.GetName().Version);

            this.DisplayTaskConfiguration();
            // check data
            this.VerifyData();

            // create new report
            this.result = new ReportResult();

            // load and execute
            using (
                    TestDomainDependencyGraph graph =
                    TestDomainDependencyGraph.BuildGraph(this.Assemblies, this.AssemblyPaths, FixtureFilters.Any, false))
            {
                graph.Log+=new ErrorReporter(graph_Log);
                ReportResult r = graph.RunTests();
                graph.Log -= new ErrorReporter(graph_Log);
                result.Merge(r);
            }
            
            this.GenerateReports();

            return result.Counter.FailureCount == 0;
        }

        private void DisplayTaskConfiguration()
        {
            this.Log.LogMessage("Test assemblies:");
            foreach (string fileName in this.Assemblies)
                this.Log.LogMessage("\t{0}", fileName);
            if (this.AssemblyPaths != null && this.AssemblyPaths.Length > 0)
            {
                this.Log.LogMessage("Assembly paths:");
                foreach (string assemblyPath in this.AssemblyPaths)
                    this.Log.LogMessage("\t{0}", assemblyPath);
            }
            this.Log.LogMessage("ReportTypes: {0}", this.ReportTypes);
            this.Log.LogMessage("ReportFileNameFormat: {0}", this.ReportFileNameFormat);
            this.Log.LogMessage("ReportOutputDirectory: {0}", this.ReportOutputDirectory);
        }

        private void VerifyData()
        {
            this.CheckAssemblyNames();
            this.CheckReportTypes();
        }

        private void CheckAssemblyNames()
        {
            this.Log.LogMessage("Verifying assembly names");
            foreach (string assemblyName in this.Assemblies)
            {
                if (!File.Exists(assemblyName))
                {
                    this.Log.LogError("Test assembly {0} cannot be found", assemblyName);
                    this.Log.LogError("Full name: {0}", Path.GetFullPath(assemblyName));
                }
            }
        }

        private void GenerateReports()
        {
            this.Log.LogMessage("Generating reports");
            foreach (string reportType in this.ReportTypes)
            {
                RenderReport(reportType);
            }
        }

        void graph_Log(string message)
        {
            Log.LogMessage(message);
        }

        private void RenderReport(string reportType)
        {
            this.Log.LogMessage("Render report {0}", reportType);
            string outputFileName = null;
            switch (reportType.ToLower())
            {
                case "text":
                    outputFileName=TextReport.RenderToText(
                        result, 
                        this.ReportOutputDirectory, 
                        this.ReportFileNameFormat
                        );
                    break;
                case "html":
                    outputFileName=HtmlReport.RenderToHtml(
                        result,
                        this.ReportOutputDirectory,
                        this.ReportFileNameFormat);
                    break;
                case "xml":
                    outputFileName = XmlReport.RenderToXml(
                        result,
                        this.ReportOutputDirectory,
                        this.ReportFileNameFormat);
                    break;
                case "dox":
                    outputFileName=DoxReport.RenderToDox(
                        result,
                        this.ReportOutputDirectory,
                        this.ReportFileNameFormat);
                    break;
                default:
                    this.Log.LogError("Report type {0} not recognized.", reportType);
                    return;
            }

            this.Log.LogMessage("Generated {0} report at {1}",
                reportType,
                outputFileName);
        }

        private void CheckReportTypes()
        {
            this.Log.LogMessage("Verifying report names");
            if (this.reportTypes != null && this.reportTypes.Length != 0)
                return;

            this.Log.LogWarning(
                "No ReportTypes attribute is specified. Default formatter will be used"
                );
            this.Log.LogMessage("Available types: Text,Html,Xml,Dox -> ReportTypes=\"Text;Html\"");
            this.reportTypes = new string[] { "Html" };
        }

    }
}
#endif