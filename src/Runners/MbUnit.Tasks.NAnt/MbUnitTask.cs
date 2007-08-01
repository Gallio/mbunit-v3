using System;
using System.Collections;
using System.IO;

#if false // Needs to be updated for Gallio.
namespace MbUnit.Tasks.NAnt
{    
    /// <summary>
    /// NAnt task for launching MbUnit
    /// </summary>
    [TaskName("mbunit")]
    public class MbUnitTask : Task
    {
        private FileSet[] assemblies;
        private string reportTypes = "Html";
        private string reportFileNameFormat = "mbunit-result-{0}{1}";
        private string reportOutputDirectory = null;
        private bool haltOnFailure = false;
        private DirSet assemblyPaths;

        private ReportResult result;

        [TaskAttribute("report-types")]
        public string ReportTypes
        {
            get { return reportTypes; }
            set { this.reportTypes = value; }
        }

        [BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
        public FileSet[] Assemblies
        {
            get { return this.assemblies; }
            set { this.assemblies = value; }
        }

        [BuildElement("assemblie-paths",Required =false)]
        public DirSet AssemblyPaths
        {
            get { return this.assemblyPaths; }
            set { this.assemblyPaths = value; }
        }

        [TaskAttribute("report-filename-format", Required = false)]
        public string ReportFileNameFormat
        {
            get { return reportFileNameFormat; }
            set { reportFileNameFormat = value; }
        }

        [TaskAttribute("report-output-directory", Required = false)]
        public string ReportOutputDirectory
        {
            get { return reportOutputDirectory; }
            set { reportOutputDirectory = value; }
        }

        [BooleanValidator, TaskAttribute("halt-on-failure",Required =false)]
        public bool HaltOnFailure
        {
            get { return this.haltOnFailure; }
            set { this.haltOnFailure = value; }
        }

        /// <summary>
        /// This is where the work is done
        /// </summary>
        protected override void ExecuteTask()
        {
            this.Log(Level.Info, "MbUnit {0} test runner", typeof(MbUnit.Core.Fixture).Assembly.GetName().Version);
            this.DisplayTaskConfiguration();

            if (this.Assemblies.Length == 0)
            {
                this.Log(Level.Warning, "No test assemblies, aborting task");
                return;
            }
            int count = 0;
            foreach (FileSet files in this.Assemblies)
                count += files.FileNames.Count;
            if (count == 0)
            {
                this.Log(Level.Warning, "No test assemblies found in test");
                return;
            }

            this.result = new ReportResult();
            foreach (FileSet assemblySet in this.Assemblies)
            {
                bool failureCount = ExecuteTests(assemblySet);
                if (failureCount)
                    break;
            }
            GenerateReports();

            if (result.Counter.FailureCount > 0)
                throw new BuildException("There were failing tests. Please see build log.");
        }

        private void DisplayTaskConfiguration()
        {
            this.Log(Level.Verbose, "Test assemblies:");
            foreach (FileSet assemblySet in this.Assemblies)
            {
                this.Log(Level.Verbose, "FileSet");
                foreach (string fileName in assemblySet.FileNames)
                    this.Log(Level.Verbose, "\t{0}", fileName);
            }
            this.Log(Level.Verbose, "ReportTypes: {0}", this.ReportTypes);
            this.Log(Level.Verbose, "ReportFileNameFormat: {0}", this.ReportFileNameFormat);
            this.Log(Level.Verbose, "ReportOutputDirectory: {0}", this.ReportOutputDirectory);
            this.Log(Level.Verbose, "HaltOnFailure: {0}", this.HaltOnFailure);
        }

        private void GenerateReports()
        {
            if (result == null)
                throw new InvalidOperationException("Report object is a null reference.");

            this.Log(Level.Info, "Generating reports");
            foreach (string reportType in this.ReportTypes.Split(';'))
            {
                string reportName = null;
                this.Log(Level.Verbose, "Report type: {0}", reportType);
                switch (reportType.ToLower())
                {
                    case "text":
                        reportName = TextReport.RenderToText(result, this.ReportOutputDirectory, this.ReportFileNameFormat);
                        break;
                    case "xml":
                        reportName = XmlReport.RenderToXml(result, this.ReportOutputDirectory, this.ReportFileNameFormat);
                        break;
                    case "html":
                        reportName = HtmlReport.RenderToHtml(result, this.ReportOutputDirectory, this.ReportFileNameFormat);
                        break;
                    case "dox":
                        reportName = DoxReport.RenderToDox(result, this.ReportOutputDirectory, this.ReportFileNameFormat);
                        break;
                    default:
                        this.Log(Level.Error, "Unknown report type {0}", reportType);
                        break;
                }
                if (reportName != null)
                    this.Log(Level.Info, "Created report at {0}", reportName);
            }
        }

        private bool ExecuteTests(FileSet assemblySet)
        {
            if (assemblySet.FileNames.Count == 0)
            {
                this.Log(Level.Warning, "No tests in assembly set");
                return true;
            }
            // execute
            string[] assemblieNames = new string[assemblySet.FileNames.Count];
            assemblySet.FileNames.CopyTo(assemblieNames, 0);

            // display information
            this.Log(Level.Info, "Loading {0} assemblies", assemblySet.FileNames.Count);
            foreach (string an in assemblieNames)
                this.Log(Level.Info, "\tAssemblyName: {0}", an);

            string[] dirNames = null;
            if (this.AssemblyPaths!=null)
            {
                dirNames = new string[this.AssemblyPaths.DirectoryNames.Count];
                this.AssemblyPaths.DirectoryNames.CopyTo(dirNames, 0);
            }
            try
            {
                using (
                    TestDomainDependencyGraph graph =
                    TestDomainDependencyGraph.BuildGraph(assemblieNames, dirNames, FixtureFilters.Any, false))                 
                {
                    graph.Log += new MbUnit.Core.Cons.CommandLine.ErrorReporter(graph_Log);
                    try
                    {
                        ReportResult r = graph.RunTests();
                        this.Log(Level.Info, "Finished running tests");
                        this.Log(Level.Info, "Merging results");
                        result.Merge(r);

                        return r.Counter.FailureCount == 0;
                    }
                    finally
                    {
                        graph.Log -= new MbUnit.Core.Cons.CommandLine.ErrorReporter(graph_Log);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new BuildException("Unexpected engine error while running Tests", ex);
            }
        }

        void graph_Log(string message)
        {
            this.Log(Level.Info, message);
        }
    }
}
#endif