using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a test project in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="TestProject"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class TestProjectInfo
    {
        private string applicationBase;
        private List<string> hintDirectories;
        private List<string> assemblyFiles;
        private bool enableShadowCopy;

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory.
        /// If relative, the path is based on the application base of the test runner,
        /// so a value of "" causes the test runner's application base to be used.
        /// </summary>
        public string ApplicationBase
        {
            get { return applicationBase; }
            set { applicationBase = value; }
        }

        /// <summary>
        /// Gets or sets the list of hint directories used to resolve test assemblies and other files.
        /// </summary>
        [XmlArray("hintDirectories", IsNullable=false)]
        [XmlArrayItem("hintDirectory", IsNullable=false)]
        public string[] HintDirectories
        {
            get { return hintDirectories.ToArray(); }
            set { hintDirectories = new List<string>(value); }
        }

        /// <summary>
        /// Gets or sets the relative or absolute paths of test assembly files.
        /// </summary>
        [XmlArray("assemblyFiles", IsNullable = false)]
        [XmlArrayItem("assemblyFile", IsNullable = false)]
        public string[] AssemblyFiles
        {
            get { return assemblyFiles.ToArray(); }
            set { assemblyFiles = new List<string>(value); }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        public bool EnableShadowCopy
        {
            get { return enableShadowCopy; }
            set { enableShadowCopy = value; }
        }

        /// <summary>
        /// Adds a path to use for resolving test assemblies and other files.
        /// </summary>
        /// <param name="hintDirectory">The path to add</param>
        public void AddHintDirectory(string hintDirectory)
        {
            hintDirectories.Add(hintDirectory);
        }

        /// <summary>
        /// Adds a relative or absolute path of a test assembly.
        /// </summary>
        /// <param name="assemblyFile">The path of the assembly file to add</param>
        public void AddAssemblyFile(string assemblyFile)
        {
            assemblyFiles.Add(assemblyFile);
        }

        /// <summary>
        /// Creates an empty but fully initialized project.
        /// </summary>
        /// <returns>The project</returns>
        public static TestProjectInfo Create()
        {
            TestProjectInfo project = new TestProjectInfo();
            project.applicationBase = "";
            project.hintDirectories = new List<string>();
            project.assemblyFiles = new List<string>();
            return project;
        }
    }
}
