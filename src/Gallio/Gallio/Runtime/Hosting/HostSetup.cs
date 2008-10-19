// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using Gallio.Utilities;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Specifies a collection of parameters for setting up a <see cref="IHost" />.
    /// </summary>
    [Serializable]
    [XmlRoot("hostSetup", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class HostSetup : IEquatable<HostSetup>
    {
        private string applicationBaseDirectory;
        private string workingDirectory;
        private bool shadowCopy;
        private HostConfiguration configuration;
        private ProcessorArchitecture processorArchitecture = ProcessorArchitecture.MSIL;

        /// <summary>
        /// Creates a default host setup.
        /// </summary>
        public HostSetup()
        {
        }

        /// <summary>
        /// <para>
        /// Gets or sets the relative or absolute path of the application base directory,
        /// or null to use a default value selected by the consumer.
        /// </para>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Relative paths should be canonicalized as soon as possible.
        /// See <see cref="Canonicalize" />.
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
        /// <para>
        /// Gets or sets the relative or absolute path of the working directory
        /// or null to use a default value selected by the consumer.
        /// </para>
        /// <para>
        /// If relative, the path is based on the current working directory,
        /// so a value of "" causes the current working directory to be used.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Relative paths should be canonicalized as soon as possible.
        /// See <see cref="Canonicalize" />.
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
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        [XmlAttribute("enableShadowCopy")]
        public bool ShadowCopy
        {
            get { return shadowCopy; }
            set { shadowCopy = value; }
        }

        /// <summary>
        /// Gets or sets the host configuration information.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("configuration", IsNullable=false)]
        public HostConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                    Interlocked.CompareExchange(ref configuration, new HostConfiguration(), null);
                return configuration;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                configuration = value;
            }
        }

        /// <summary>
        /// Gets or sets the processor architecture that the host should target, when supported.
        /// </summary>
        /// <value>The processor architecture, defaults to <see cref="System.Reflection.ProcessorArchitecture.MSIL" /></value>
        [XmlAttribute("processorArchitecture")]
        public ProcessorArchitecture ProcessorArchitecture
        {
            get { return processorArchitecture; }
            set { processorArchitecture = value; }
        }

        /// <summary>
        /// Creates a copy of the host setup information.
        /// </summary>
        /// <returns>The copy</returns>
        public HostSetup Copy()
        {
            HostSetup copy = new HostSetup();
            copy.applicationBaseDirectory = applicationBaseDirectory;
            copy.workingDirectory = workingDirectory;
            copy.shadowCopy = shadowCopy;
            copy.processorArchitecture = processorArchitecture;

            if (configuration != null)
                copy.configuration = configuration.Copy();

            return copy;
        }

        /// <summary>
        /// Makes all paths in this instance absolute.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory</param>
        public void Canonicalize(string baseDirectory)
        {
            applicationBaseDirectory = GetCanonicalApplicationBaseDirectory(baseDirectory);
            workingDirectory = FileUtils.CanonicalizePath(baseDirectory, workingDirectory);
        }

        /// <summary>
        /// <para>
        /// Writes a temporary configuration file for the application to disk and returns its path.
        /// The configuration file is created in the <see cref="ApplicationBaseDirectory" /> if it is not
        /// null, otherwise it is created in the current user's temp directory.
        /// </para>
        /// <para>
        /// The file should be deleted by the caller when no longer required.
        /// </para>
        /// </summary>
        /// <returns>The full path of the configuration file that was written</returns>
        /// <exception cref="IOException">Thrown if the configuration file could not be written</exception>
        public string WriteTemporaryConfigurationFile()
        {
            string path = GetTemporaryConfigurationFilePath();
            Configuration.WriteToFile(path);
            return path;
        }

        private string GetTemporaryConfigurationFilePath()
        {
            if (applicationBaseDirectory == null)
                return Path.GetTempFileName();

            for (; ; )
            {
                string path = Path.Combine(GetCanonicalApplicationBaseDirectory(null), Hash64.CreateUniqueHash() + ".tmp.config");
                if (!File.Exists(path))
                    return path;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as HostSetup);
        }

        /// <inheritdoc />
        public bool Equals(HostSetup other)
        {
            return other != null
                && applicationBaseDirectory == other.applicationBaseDirectory
                && workingDirectory == other.workingDirectory
                && shadowCopy == other.shadowCopy
                && Configuration.Equals(other.Configuration)
                && processorArchitecture == other.processorArchitecture;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (applicationBaseDirectory != null ? applicationBaseDirectory.GetHashCode() : 0)
                ^ (workingDirectory != null ? workingDirectory.GetHashCode() : 0)
                ^ Configuration.GetHashCode()
                ^ (shadowCopy.GetHashCode() << 16)
                ^ (processorArchitecture.GetHashCode() << 5);
        }

        private string GetCanonicalApplicationBaseDirectory(string baseDirectory)
        {
            return FileUtils.CanonicalizePath(baseDirectory, applicationBaseDirectory);
        }
    }
}
