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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// A test package specifies the options used by a test runner to load tests
    /// into memory for execution.  The package may contain multiple test assemblies
    /// that are to be loaded together for test execution.  It can also be serialized as
    /// XML or using .Net remoting for persistence and remote operation.
    /// </summary>
    /// <remarks author="jeff">
    /// Someday a test package might allow other kinds of resources to be specified
    /// such as individual test scripts, archives, dependent files, environmental
    /// configuration settings, etc...
    /// </remarks>
    [Serializable]
    [XmlRoot("package", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class TestPackage
    {
        private string applicationBase;
        private List<string> hintDirectories;
        private List<string> assemblyFiles;
        private bool enableShadowCopy;

        /// <summary>
        /// Creates an empty package.
        /// </summary>
        public TestPackage()
        {
            applicationBase = "";
            hintDirectories = new List<string>();
            assemblyFiles = new List<string>();
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory.
        /// If relative, the path is based on the application base of the test runner,
        /// so a value of "" causes the test runner's application base to be used.
        /// </summary>
        [XmlArrayItem("applicationBase")]
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
        [XmlAttribute("enableShadowCopy")]
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
    }
}