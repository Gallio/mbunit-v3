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
using System.IO;
using System.Reflection;
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Runtime.Debugging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Specifies a collection of parameters for setting up a <see cref="IHost" />.
    /// </summary>
    [Serializable]
    public sealed class HostSetup
    {
        private string applicationBaseDirectory;
        private string workingDirectory;
        private bool shadowCopy;
        private DebuggerSetup debuggerSetup;
        private string runtimeVersion;
        private bool elevated;
        private ConfigurationFileLocation configurationFileLocation = ConfigurationFileLocation.Temp;
        private HostConfiguration configuration;
        private ProcessorArchitecture processorArchitecture = ProcessorArchitecture.MSIL;
        private readonly PropertySet properties;

        /// <summary>
        /// Creates a default host setup.
        /// </summary>
        public HostSetup()
        {
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets a read-only collection of configuration properties for the host.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the application base directory,
        /// or null to use a default value selected by the consumer.
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
        public string ApplicationBaseDirectory
        {
            get { return applicationBaseDirectory; }
            set { applicationBaseDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the relative or absolute path of the working directory
        /// or null to use a default value selected by the consumer.
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
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set { workingDirectory = value; }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        /// <value>True if shadow copying is enabled.  Default is <c>false</c>.</value>
        public bool ShadowCopy
        {
            get { return shadowCopy; }
            set { shadowCopy = value; }
        }

        /// <summary>
        /// Gets or sets the debugger setup options, or null if not debugging.
        /// </summary>
        /// <value>The debugger setup options.  Default is <c>null</c>.</value>
        public DebuggerSetup DebuggerSetup
        {
            get { return debuggerSetup; }
            set { debuggerSetup = value; }
        }

        /// <summary>
        /// Gets or sets the .Net runtime version of the host, or null to auto-detect.
        /// </summary>
        /// <value>The runtime version, eg. "v2.0.50727".  Default is <c>null</c>.</value>
        public string RuntimeVersion
        {
            get { return runtimeVersion; }
            set { runtimeVersion = value; }
        }

        /// <summary>
        /// Gets or sets whether the host should run with elevated privileges.
        /// </summary>
        /// <value>True if the host should have elevated privileges.  Default is <c>false</c>.</value>
        public bool Elevated
        {
            get { return elevated; }
            set { elevated = value; }
        }

        /// <summary>
        /// Gets or sets where the host should write out the configuration file for the hosted components.
        /// </summary>
        /// <value>The configuration file location.  Default is <see cref="Hosting.ConfigurationFileLocation.Temp" />.</value>
        public ConfigurationFileLocation ConfigurationFileLocation
        {
            get { return configurationFileLocation; }
            set { configurationFileLocation = value; }
        }

        /// <summary>
        /// Gets or sets the host configuration information.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
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
        /// <value>The processor architecture.  Default is <see cref="System.Reflection.ProcessorArchitecture.MSIL" /></value>
        public ProcessorArchitecture ProcessorArchitecture
        {
            get { return processorArchitecture; }
            set { processorArchitecture = value; }
        }

        /// <summary>
        /// Clears the collection of properties.
        /// </summary>
        public void ClearProperties()
        {
            properties.Clear();
        }

        /// <summary>
        /// Adds a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="key"/> is already in the property set.</exception>
        public void AddProperty(string key, string value)
        {
            properties.Add(key, value); // note: implicitly checks arguments
        }

        /// <summary>
        /// Removes a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        public void RemoveProperty(string key)
        {
            properties.Remove(key); // note: implicitly checks arguments
        }

        /// <summary>
        /// Creates a copy of the host setup information.
        /// </summary>
        /// <returns>The copy.</returns>
        public HostSetup Copy()
        {
            HostSetup copy = new HostSetup();
            copy.applicationBaseDirectory = applicationBaseDirectory;
            copy.workingDirectory = workingDirectory;
            copy.shadowCopy = shadowCopy;
            if (debuggerSetup != null)
                copy.debuggerSetup = debuggerSetup.Copy();
            copy.processorArchitecture = processorArchitecture;
            copy.runtimeVersion = runtimeVersion;
            copy.elevated = elevated;
            copy.configurationFileLocation = configurationFileLocation;
            copy.properties.AddAll(properties);

            if (configuration != null)
                copy.configuration = configuration.Copy();

            return copy;
        }

        /// <summary>
        /// Makes all paths in this instance absolute.
        /// </summary>
        /// <param name="baseDirectory">The base directory for resolving relative paths,
        /// or null to use the current directory.</param>
        public void Canonicalize(string baseDirectory)
        {
            applicationBaseDirectory = GetCanonicalApplicationBaseDirectory(baseDirectory);
            workingDirectory = FileUtils.CanonicalizePath(baseDirectory, workingDirectory);
        }

        /// <summary>
        /// Writes a temporary configuration file for the application to disk and returns its path
        /// based on <see cref="ConfigurationFileLocation" />.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// The file is created with a unique name each time.
        /// </para>
        /// <para>
        /// The file should be deleted by the caller when no longer required.
        /// </para>
        /// </remarks>
        /// <returns>The full path of the configuration file that was written, or null if no file was written.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ApplicationBaseDirectory"/>
        /// is <c>null</c> but <see cref="ConfigurationFileLocation" /> is <see cref="Hosting.ConfigurationFileLocation.AppBase" />.</exception>
        /// <exception cref="IOException">Thrown if the configuration file could not be written.</exception>
        public string WriteTemporaryConfigurationFile()
        {
            string path = GetTemporaryConfigurationFilePath();
            if (path != null)
                Configuration.WriteToFile(path);
            return path;
        }

        private string GetTemporaryConfigurationFilePath()
        {
            switch (ConfigurationFileLocation)
            {
                case ConfigurationFileLocation.None:
                    return null;

                case ConfigurationFileLocation.Temp:
                    return SpecialPathPolicy.For("Hosting").CreateTempFileWithUniqueName().FullName;

                case ConfigurationFileLocation.AppBase:
                    if (applicationBaseDirectory == null)
                        throw new InvalidOperationException("The configuration file was to be written to the application base directory but none was specified in the host setup.");

                    for (; ; )
                    {
                        string path = Path.Combine(GetCanonicalApplicationBaseDirectory(null), Hash64.CreateUniqueHash() + ".tmp.config");
                        if (!File.Exists(path))
                            return path;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetCanonicalApplicationBaseDirectory(string baseDirectory)
        {
            return FileUtils.CanonicalizePath(baseDirectory, applicationBaseDirectory);
        }
    }
}
