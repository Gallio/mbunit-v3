using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.BuildTools.Tasks
{
    /// <summary>
    /// Generates an XmlSerializers assemblies for multiple types in an assembly.
    /// </summary>
    public class SGenMultipleTypes : ToolTask
    {
        public string BuildAssemblyName { get; set; }

        public string BuildAssemblyPath { get; set; }

        public string[] References { get; set; }

        public string KeyFile { get; set; }

        protected override string ToolName
        {
            get { return "Gallio.BuildTools.SGen.exe"; }
        }

        protected override string GenerateFullPathToTool()
        {
            string taskPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            return Path.Combine(taskPath, ToolName);
        }

        protected override string GenerateCommandLineCommands()
        {
            StringBuilder result = new StringBuilder();

            if (BuildAssemblyName != null)
                result.Append("-an \"").Append(BuildAssemblyName).Append("\" ");

            if (BuildAssemblyPath != null)
                result.Append("-ap \"").Append(Path.GetFullPath(NormalizePath(BuildAssemblyPath))).Append("\" ");

            foreach (string reference in References)
                result.Append("-r \"").Append(reference).Append("\" ");

            if (KeyFile != null)
                result.Append("-k \"").Append(KeyFile).Append("\" ");

            return result.ToString();
        }

        private static string NormalizePath(string path)
        {
            if (path.EndsWith("\\"))
                return path.Substring(0, path.Length - 1);
            return path;
        }

        [Output]
        public string SerializationAssembly { get; private set; }

        public override bool Execute()
        {
            string absoluteAssemblyDir = Path.GetFullPath(BuildAssemblyPath);
            string assemblyPath = Path.Combine(absoluteAssemblyDir, BuildAssemblyName);
            SerializationAssembly = Path.ChangeExtension(assemblyPath, ".XmlSerializers.dll");

            return base.Execute();
        }
    }
}
