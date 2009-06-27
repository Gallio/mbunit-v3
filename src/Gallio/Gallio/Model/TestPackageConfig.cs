// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Runtime.Hosting;

namespace Gallio.Model
{
    /// <summary>
    /// A test package specifies the test files, assemblies, and configuration options
    /// that govern test execution.
    /// </summary>
    [Serializable]
    [XmlRoot("testPackageConfig", Namespace=XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class TestPackageConfig
    {
        private readonly List<string> hintDirectories;
        private readonly List<string> files;
        private readonly List<string> excludedFrameworkIds;
        private bool shadowCopy;
        private bool debug;
        private string applicationBaseDirectory;
        private string workingDirectory;
        private string runtimeVersion;

        /// <summary>
        /// Creates an empty package.
        /// </summary>
        public TestPackageConfig()
        {
            hintDirectories = new List<string>();
            files = new List<string>();
            excludedFrameworkIds = new List<string>();
        }

        /// <summary>
        /// Gets the list of relative or absolute paths of test files.
        /// </summary>
        [XmlArray("files", IsNullable = false)]
        [XmlArrayItem("file", typeof(string), IsNullable = false)]
        public List<string> Files
        {
            get { return files; }
        }

        /// <summary>
        /// Gets the list of hint directories used to resolve test assemblies and other files.
        /// </summary>
        [XmlArray("hintDirectories", IsNullable=false)]
        [XmlArrayItem("hintDirectory", typeof(string), IsNullable=false)]
        public List<string> HintDirectories
        {
            get { return hintDirectories; }
        }

        /// <summary>
        /// Gets the list of test framework IDs that are to be excluded from the test
        /// exploration process.
        /// </summary>
        [XmlArray("excludedFrameworkIds", IsNullable = false)]
        [XmlArrayItem("excludedFrameworkId", typeof(string), IsNullable = false)]
        public List<string> ExcludedFrameworkIds
        {
            get { return excludedFrameworkIds; }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        /// <value>True if shadow copying is enabled.  Default is <c>false</c>.</value>
        [XmlAttribute("enableShadowCopy")]
        public bool ShadowCopy
        {
            get { return shadowCopy; }
            set { shadowCopy = value; }
        }

        /// <summary>
        /// Gets or sets whether to attach the debugger to the host.
        /// </summary>
        /// <value>True if a debugger should be attached to the host.  Default is <c>false</c>.</value>
        [XmlAttribute("debug")]
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory,
        /// or null to use the directory containing each group of test files or some other
        /// suitable default chosen by the test framework.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// Relative paths should be canonicalized as soon as possible.
        /// See <see cref="Canonicalize" />.
        /// </para>
        /// </remarks>
        /// <value>
        /// The application base directory.  Default is <c>null</c>.
        /// </value>
        [XmlAttribute("applicationBaseDirectory")]
        public string ApplicationBaseDirectory
        {
            get { return applicationBaseDirectory; }
            set { applicationBaseDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the working directory
        /// or null to use the directory containing each group of test files or some other
        /// suitable default chosen by the test framework.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// <para>
        /// Relative paths should be canonicalized as soon as possible.
        /// See <see cref="Canonicalize" />.
        /// </para>
        /// </remarks>
        /// <value>
        /// The working directory.  Default is <c>null</c>.
        /// </value>
        [XmlAttribute("workingDirectory")]
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set { workingDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the .Net runtime version, or null to auto-detect.
        /// </summary>
        /// <value>The runtime version, eg. "v2.0.50727".  Default is <c>null</c>.</value>
        [XmlAttribute("runtimeVersion")]
        public string RuntimeVersion
        {
            get { return runtimeVersion; }
            set { runtimeVersion = value; }
        }

        /// <summary>
        /// Creates a host setup based on the package properties.
        /// </summary>
        /// <returns>The host setup</returns>
        public HostSetup CreateHostSetup()
        {
            var hostSetup = new HostSetup
            {
                ConfigurationFileLocation = ConfigurationFileLocation.AppBase,
                ProcessorArchitecture = ProcessorArchitecture.None,
                Debug = debug,
                ShadowCopy = shadowCopy,
                ApplicationBaseDirectory = applicationBaseDirectory,
                WorkingDirectory = workingDirectory,
                RuntimeVersion = runtimeVersion
            };

            return hostSetup;
        }

        /// <summary>
        /// Creates a copy of the test package.
        /// </summary>
        /// <returns>The copy.</returns>
        public TestPackageConfig Copy()
        {
            TestPackageConfig copy = new TestPackageConfig();

            copy.files.AddRange(files);
            copy.hintDirectories.AddRange(hintDirectories);
            copy.excludedFrameworkIds.AddRange(excludedFrameworkIds);
            copy.shadowCopy = shadowCopy;
            copy.debug = debug;
            copy.applicationBaseDirectory = applicationBaseDirectory;
            copy.workingDirectory = workingDirectory;
            copy.runtimeVersion = runtimeVersion;

            return copy;
        }

        /// <summary>
        /// Makes all paths in this instance absolute.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory.</param>
        public void Canonicalize(string baseDirectory)
        {
            FileUtils.CanonicalizePaths(baseDirectory, files);
            FileUtils.CanonicalizePaths(baseDirectory, hintDirectories);
            applicationBaseDirectory = FileUtils.CanonicalizePath(baseDirectory, applicationBaseDirectory);
            workingDirectory = FileUtils.CanonicalizePath(baseDirectory, workingDirectory);
        }

        /// <summary>
        /// Returns true if the framework with the specified id should be used to explore
        /// the contents of the test package.
        /// </summary>
        /// <param name="frameworkId">The framework id.</param>
        /// <returns>True if the framework is requested.</returns>
        public bool IsFrameworkRequested(string frameworkId)
        {
            return ! excludedFrameworkIds.Contains(frameworkId);
        }
    }
}