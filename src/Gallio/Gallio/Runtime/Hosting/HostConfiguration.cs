// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Describes the runtime configuration of a <see cref="IHost" />.
    /// </summary>
    [Serializable]
    public sealed class HostConfiguration
    {
        private readonly List<AssemblyQualification> assemblyQualifications;
        private readonly List<AssemblyDependency> assemblyDependencies;
        private readonly List<string> supportedRuntimeVersions;

        private string configurationXml;
        private bool legacyUnhandledExceptionPolicyEnabled = true;
        private bool assertUiEnabled;
        private bool remotingCustomErrorsEnabled;

        /// <summary>
        /// Creates a default host configuration.
        /// </summary>
        public HostConfiguration()
        {
            assemblyQualifications = new List<AssemblyQualification>();
            assemblyDependencies = new List<AssemblyDependency>();
            supportedRuntimeVersions = new List<string>();
        }

        /// <summary>
        /// Gets or sets the primary Xml configuration data, or null if none.
        /// </summary>
        /// <value>
        /// The default value is <c>null</c>.
        /// </value>
        public string ConfigurationXml
        {
            get { return configurationXml; }
            set { configurationXml = value; }
        }

        /// <summary>
        /// Gets or sets whether the legacy unhandled exception policy is enabled.
        /// </summary>
        /// <value>
        /// The default value is <c>true</c> which prevents the application from terminating
        /// abruptly when an unhandled exception occurs.
        /// </value>
        public bool LegacyUnhandledExceptionPolicyEnabled
        {
            get { return legacyUnhandledExceptionPolicyEnabled; }
            set { legacyUnhandledExceptionPolicyEnabled = value; }
        }

        /// <summary>
        /// Gets or sets whether the <see cref="Debug.Assert(bool)" /> failure dialog is enabled.
        /// </summary>
        /// <value>
        /// The default value is <c>false</c> which prevents the assertion dialog from appearing
        /// when an assertion fails.
        /// </value>
        public bool AssertUiEnabled
        {
            get { return assertUiEnabled; }
            set { assertUiEnabled = value; }
        }

        /// <summary>
        /// Gets or sets whether remoting exceptions are substituted with custom errors instead
        /// of being passed through to the remote client.
        /// </summary>
        /// <value>
        /// The default value is <c>false</c> which ensures that the remote client receives
        /// all exception details.
        /// </value>
        public bool RemotingCustomErrorsEnabled
        {
            get { return remotingCustomErrorsEnabled; }
            set { remotingCustomErrorsEnabled = value; }
        }

        /// <summary>
        /// Gets a read-only list of assembly qualifications.
        /// </summary>
        public IList<AssemblyQualification> AssemblyQualifications
        {
            get { return new ReadOnlyCollection<AssemblyQualification>(assemblyQualifications); }
        }

        /// <summary>
        /// Gets a read-only list of assembly dependencies.
        /// </summary>
        public IList<AssemblyDependency> AssemblyDependencies
        {
            get { return new ReadOnlyCollection<AssemblyDependency>(assemblyDependencies); }
        }

        /// <summary>
        /// Gets a read-only list of supported runtime versions in order of preference.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When the list is empty, the runtime version used to build the application is used.
        /// Otherwise one of the supported runtimes in the list is used.
        /// </para>
        /// <para>
        /// The default value is an empty list.  Each entry should start with a "v".  eg. "v2.0.50727".
        /// </para>
        /// <para>
        /// Note that the host factory will use the <see cref="HostSetup.RuntimeVersion" /> property
        /// to determine the actual runtime used for hosting.  The purpose of this property is to
        /// facilitate writing configuration files with supported runtime version information keeping
        /// in mind that it can be ignored.
        /// </para>
        /// </remarks>
        public IList<string> SupportedRuntimeVersions
        {
            get { return new ReadOnlyCollection<string>(supportedRuntimeVersions); }
        }

        /// <summary>
        /// Clears the list of supported runtime versions.
        /// </summary>
        public void ClearSupportedRuntimeVersions()
        {
            supportedRuntimeVersions.Clear();
        }

        /// <summary>
        /// Adds a supported runtime version number if it is not already in the configuration.
        /// </summary>
        /// <param name="version">The version number, eg. "v2.0.50727".</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/> is null.</exception>
        public void AddSupportedRuntimeVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            if (! supportedRuntimeVersions.Contains(version))
                supportedRuntimeVersions.Add(version);
        }

        /// <summary>
        /// Removes a supported runtime version number.
        /// </summary>
        /// <param name="version">The version number, eg. "v2.0.50727".</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/> is null.</exception>
        public void RemoveSupportedRuntimeVersion(string version)
        {
            if (version == null)
                throw new ArgumentNullException("version");

            supportedRuntimeVersions.Remove(version);
        }

        /// <summary>
        /// Adds a binding to the configuration for the specified assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Depending on how the binding is configured, this method will add an assembly dependency
        /// record and possibly an assembly qualification record.
        /// </para>
        /// </remarks>
        /// <param name="assemblyBinding">The assembly binding.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyBinding"/> is null.</exception>
        public void AddAssemblyBinding(AssemblyBinding assemblyBinding)
        {
            if (assemblyBinding == null)
                throw new ArgumentNullException("assemblyBinding");

            AssemblyName assemblyName = assemblyBinding.AssemblyName;
            byte[] publicKeyTokenBytes = assemblyName.GetPublicKeyToken();
            string publicKeyToken = publicKeyTokenBytes != null && publicKeyTokenBytes.Length != 0 ? ToHex(publicKeyTokenBytes) : null;

            if (assemblyBinding.QualifyPartialName && publicKeyToken != null)
                AddAssemblyQualification(assemblyName.Name, assemblyName.FullName);

            AssemblyDependency assemblyDependency = AddAssemblyDependency(
                assemblyName.Name,
                publicKeyToken,
                AssemblyUtils.GetAssemblyNameCulture(assemblyName),
                GetProcessorArchitectureName(assemblyName.ProcessorArchitecture));

            foreach (AssemblyBinding.BindingRedirect bindingRedirect in assemblyBinding.BindingRedirects)
            {
                assemblyDependency.AddAssemblyBindingRedirect(bindingRedirect.OldVersion, assemblyName.Version.ToString());
            }

            // Note: If unsigned assembly appears outside of appbase then we get an exception:
            //       "The private assembly was located outside the appbase directory."
            if (assemblyBinding.CodeBase != null && publicKeyToken != null)
                assemblyDependency.AddAssemblyCodeBase(assemblyName.Version.ToString(), assemblyBinding.CodeBase.ToString());

            assemblyDependency.ApplyPublisherPolicy = assemblyBinding.ApplyPublisherPolicy;
        }

        private static string GetProcessorArchitectureName(ProcessorArchitecture architecture)
        {
            switch (architecture)
            {
                case ProcessorArchitecture.None:
                    return null;

                case ProcessorArchitecture.MSIL:
                    return "msil";

                case ProcessorArchitecture.X86:
                    return "x86";

                case ProcessorArchitecture.IA64:
                    return "ia64";

                case ProcessorArchitecture.Amd64:
                    return "amd64";

                default:
                    throw new ArgumentOutOfRangeException("architecture");
            }
        }

        /// <summary>
        /// Clears the list of assembly dependencies.
        /// </summary>
        public void ClearAssemblyDependencies()
        {
            assemblyDependencies.Clear();
        }

        /// <summary>
        /// Adds an assembly dependency if it is not already in the configuration.
        /// </summary>
        /// <param name="assemblyDependency">The assembly dependency to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyDependency"/> is null.</exception>
        public void AddAssemblyDependency(AssemblyDependency assemblyDependency)
        {
            if (assemblyDependency == null)
                throw new ArgumentNullException("assemblyDependency");

            if (!assemblyDependencies.Contains(assemblyDependency))
                assemblyDependencies.Add(assemblyDependency);
        }

        /// <summary>
        /// Adds an assembly dependency element if a suitable one does not already exist.
        /// </summary>
        /// <param name="name">The assembly name.</param>
        /// <param name="publicKeyToken">The assembly public key token, or null if none.</param>
        /// <param name="culture">The assembly culture.</param>
        /// <param name="architecture">The assembly processor architecture, or null if none.</param>
        /// <returns>The assembly dependency element.</returns>
        public AssemblyDependency AddAssemblyDependency(string name, string publicKeyToken, string culture, string architecture)
        {
            if (publicKeyToken != null && publicKeyToken.Length == 0)
                throw new InvalidOperationException();

            AssemblyDependency assemblyDependency = assemblyDependencies.Find(x =>
                x.AssemblyName == name && x.AssemblyPublicKeyToken == publicKeyToken
                    && x.AssemblyCulture == culture && x.AssemblyProcessorArchitecture == architecture);
            if (assemblyDependency == null)
            {
                assemblyDependency = new AssemblyDependency(name)
                {
                    AssemblyPublicKeyToken = publicKeyToken,
                    AssemblyCulture = culture,
                    AssemblyProcessorArchitecture = architecture
                };

                assemblyDependencies.Add(assemblyDependency);
            }

            return assemblyDependency;
        }

        /// <summary>
        /// Removes an assembly dependency.
        /// </summary>
        /// <param name="assemblyDependency">The assembly dependency to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyDependency"/> is null.</exception>
        public void RemoveAssemblyDependency(AssemblyDependency assemblyDependency)
        {
            if (assemblyDependency == null)
                throw new ArgumentNullException("assemblyDependency");

            assemblyDependencies.Remove(assemblyDependency);
        }

        /// <summary>
        /// Clears the list of assembly qualifications.
        /// </summary>
        public void ClearAssemblyQualifications()
        {
            assemblyQualifications.Clear();
        }

        /// <summary>
        /// Adds an assembly qualification if it is not already in the configuration.
        /// </summary>
        /// <param name="assemblyQualification">The assembly qualification to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyQualification"/> is null.</exception>
        public void AddAssemblyQualification(AssemblyQualification assemblyQualification)
        {
            if (assemblyQualification == null)
                throw new ArgumentNullException("assemblyQualification");

            if (!assemblyQualifications.Contains(assemblyQualification))
                assemblyQualifications.Add(assemblyQualification);
        }

        /// <summary>
        /// Adds an assembly qualification element if a suitable one does not already exist.
        /// </summary>
        /// <param name="partialName">The partial name to quality.</param>
        /// <param name="fullName">The full name.</param>
        /// <returns>The assembly qualification element.</returns>
        /// <exception cref="InvalidOperationException">Thrown if an assembly qualification already
        /// exists for the same partial name but with a different full name.</exception>
        public AssemblyQualification AddAssemblyQualification(string partialName, string fullName)
        {
            AssemblyQualification assemblyQualification = assemblyQualifications.Find(x => x.PartialName == partialName);
            if (assemblyQualification != null)
            {
                if (assemblyQualification.FullName != fullName)
                    throw new InvalidOperationException(String.Format("The configuration already contains an AssemblyQualification with PartialName='{0}' and FullName='{1}' but the requested qualification has FullName='{2}'.",
                        partialName, assemblyQualification.FullName, fullName));
            }
            else
            {
                assemblyQualifications.Add(new AssemblyQualification(partialName, fullName));
            }

            return assemblyQualification;
        }

        /// <summary>
        /// Removes an assembly qualification.
        /// </summary>
        /// <param name="assemblyQualification">The assembly qualification to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyQualification"/> is null.</exception>
        public void RemoveAssemblyQualification(AssemblyQualification assemblyQualification)
        {
            if (assemblyQualification == null)
                throw new ArgumentNullException("assemblyQualification");

            assemblyQualifications.Remove(assemblyQualification);
        }

        /// <summary>
        /// Overlays the <see cref="ConfigurationXml" /> with the additional configuration
        /// entries necessary to enable the features described by this instance and returns
        /// the combined Xml configuration.
        /// </summary>
        /// <returns>The combined Xml configuration.</returns>
        public override string ToString()
        {
            XmlDocument document = GenerateXmlDocument();
            return document.InnerXml;
        }

        /// <summary>
        /// Overlays the <see cref="ConfigurationXml" /> with the additional configuration
        /// entries necessary to enable the features described by this instance and writes
        /// the combined Xml configuration to a <see cref="TextWriter" />.
        /// </summary>
        /// <param name="textWriter">The TextWriter where the Xml configuration will be written to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="textWriter"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the configuration could not be written.</exception>
        public void WriteTo(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            XmlDocument document = GenerateXmlDocument();

            var settings = new XmlWriterSettings();
            // This setting is important! For some reason, if the user settings section
            // is not indented and it contains more than one setting, the CLR won't parse
            // them correctly and will throw a System.Configuration.ConfigurationErrorsException
            // with the following message: "Unrecognized element 'setting'"
            settings.Indent = true;
            settings.CloseOutput = false;
            settings.CheckCharacters = false;
            settings.Encoding = textWriter.Encoding;
            using (XmlWriter writer = XmlWriter.Create(textWriter, settings))
                document.Save(writer);
        }

        /// <summary>
        /// Overlays the <see cref="ConfigurationXml" /> with the additional configuration
        /// entries necessary to enable the features described by this instance and writes
        /// the combined Xml configuration to a file with the given path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        /// <exception cref="IOException">Thrown if the configuration could not be written.</exception>
        public void WriteToFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            Encoding encoding = new UTF8Encoding(false);
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
                WriteTo(writer);
        }

        /// <summary>
        /// Creates a copy of the host configuration information.
        /// </summary>
        /// <returns>The copy.</returns>
        public HostConfiguration Copy()
        {
            var copy = new HostConfiguration();

            copy.assertUiEnabled = assertUiEnabled;
            copy.configurationXml = configurationXml;
            copy.legacyUnhandledExceptionPolicyEnabled = legacyUnhandledExceptionPolicyEnabled;
            copy.remotingCustomErrorsEnabled = remotingCustomErrorsEnabled;
            copy.supportedRuntimeVersions.AddRange(supportedRuntimeVersions);

            foreach (AssemblyDependency dependency in assemblyDependencies)
                copy.assemblyDependencies.Add(dependency.Copy());
            foreach (AssemblyQualification qualification in assemblyQualifications)
                copy.assemblyQualifications.Add(qualification.Copy());

            return copy;
        }

        private XmlDocument GenerateXmlDocument()
        {
            var document = new XmlDocument();
            document.LoadXml(configurationXml ?? @"<configuration/>");

            XmlElement rootElement = document.DocumentElement;
            ConfigureLegacyUnhandledExceptionPolicy(rootElement);
            ConfigureAssertUi(rootElement);
            ConfigureRemotingCustomErrors(rootElement);
            ConfigureAssemblyBindings(rootElement);
            ConfigureSupportedRuntimes(rootElement);
            return document;
        }

        private void ConfigureLegacyUnhandledExceptionPolicy(XmlElement rootElement)
        {
            XmlElement sectionElement = GetOrCreateChildElement(rootElement, "runtime", null);
            XmlElement configElement = GetOrCreateChildElement(sectionElement, "legacyUnhandledExceptionPolicy", null);

            configElement.SetAttribute("enabled", legacyUnhandledExceptionPolicyEnabled ? "1" : "0");
        }

        private void ConfigureAssertUi(XmlElement rootElement)
        {
            XmlElement sectionElement = GetOrCreateChildElement(rootElement, "system.diagnostics", null);
            XmlElement configElement = GetOrCreateChildElement(sectionElement, "assert", null);

            configElement.SetAttribute("assertuienabled", assertUiEnabled ? "true" : "false");
        }

        private void ConfigureRemotingCustomErrors(XmlElement rootElement)
        {
            XmlElement sectionElement = GetOrCreateChildElement(rootElement, "system.runtime.remoting", null);
            XmlElement configElement = GetOrCreateChildElement(sectionElement, "customErrors", null);

            configElement.SetAttribute("mode", remotingCustomErrorsEnabled ? "on" : "off");
        }

        private void ConfigureAssemblyBindings(XmlElement rootElement)
        {
            if (assemblyDependencies.Count == 0 && assemblyQualifications.Count == 0)
                return;

            XmlElement sectionElement = GetOrCreateChildElement(rootElement, "runtime", null);

            XmlElement bindingElement = GetOrCreateChildElement(sectionElement, "assemblyBinding", "urn:schemas-microsoft-com:asm.v1");

            foreach (AssemblyQualification assemblyQualification in assemblyQualifications)
                assemblyQualification.AddConfigurationElement(bindingElement);

            foreach (AssemblyDependency assemblyDependency in assemblyDependencies)
                assemblyDependency.AddConfigurationElement(bindingElement);
        }

        private void ConfigureSupportedRuntimes(XmlElement rootElement)
        {
            if (supportedRuntimeVersions.Count == 0)
                return;

            XmlElement sectionElement = GetOrCreateChildElement(rootElement, "startup", null);

            foreach (string version in supportedRuntimeVersions)
            {
                XmlElement entry = CreateChildElement(sectionElement, "supportedRuntime", null);
                entry.SetAttribute("version", version);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return builder.ToString();
        }

        private static XmlElement GetOrCreateChildElement(XmlElement parent, string name, string namespaceUri)
        {
            XmlNodeList nodeList = parent.GetElementsByTagName(name, namespaceUri ?? parent.NamespaceURI);
            if (nodeList.Count != 0)
                return (XmlElement) nodeList[0];

            return CreateChildElement(parent, name, namespaceUri);
        }

        private static XmlElement CreateChildElement(XmlElement parent, string name, string namespaceUri)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(name, namespaceUri ?? parent.NamespaceURI);
            parent.AppendChild(element);
            return element;
        }

        /// <summary>
        /// Describes an assembly name qualification configuration entry that
        /// maps an assembly partial name to its full name.
        /// </summary>
        [Serializable]
        public sealed class AssemblyQualification
        {
            private string partialName;
            private string fullName;

            /// <summary>
            /// Creates an assembly name qualification.
            /// </summary>
            /// <param name="partialName">The assembly partial name to qualify.</param>
            /// <param name="fullName">The assembly full name to use.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="partialName"/>
            /// or <paramref name="fullName"/> is null.</exception>
            public AssemblyQualification(string partialName, string fullName)
            {
                if (partialName == null)
                    throw new ArgumentNullException("partialName");
                if (fullName == null)
                    throw new ArgumentNullException("fullName");

                this.partialName = partialName;
                this.fullName = fullName;
            }

            /// <summary>
            /// Gets or sets the assembly partial name to qualify.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string PartialName
            {
                get { return partialName; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    partialName = value;
                }
            }

            /// <summary>
            /// Gets or sets the assembly full name to use.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string FullName
            {
                get { return fullName; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    fullName = value;
                }
            }

            /// <summary>
            /// Creates a copy of the assembly qualification information.
            /// </summary>
            /// <returns>The copy.</returns>
            public AssemblyQualification Copy()
            {
                return new AssemblyQualification(partialName, fullName);
            }

            internal void AddConfigurationElement(XmlElement parent)
            {
                XmlElement qualifyAssemblyElement = CreateChildElement(parent, "qualifyAssembly", null);
                qualifyAssemblyElement.SetAttribute("partialName", partialName);
                qualifyAssemblyElement.SetAttribute("fullName", fullName);
            }
        }

        /// <summary>
        /// Describes a dependent assembly configuration entry that optionally
        /// specifies the codebase, a publisher policy and binding redirects.
        /// </summary>
        [Serializable]
        public sealed class AssemblyDependency
        {
            private readonly List<AssemblyBindingRedirect> assemblyBindingRedirects;
            private readonly List<AssemblyCodeBase> assemblyCodeBases;

            private string assemblyName;
            private string assemblyPublicKeyToken;
            private string assemblyCulture;
            private string assemblyProcessorArchitecture;
            private bool applyPublisherPolicy = true;

            /// <summary>
            /// Creates an assembly dependency.
            /// </summary>
            /// <param name="assemblyName">The assembly name.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null.</exception>
            public AssemblyDependency(string assemblyName)
            {
                if (assemblyName == null)
                    throw new ArgumentNullException("assemblyName");

                this.assemblyName = assemblyName;

                assemblyBindingRedirects = new List<AssemblyBindingRedirect>();
                assemblyCodeBases = new List<AssemblyCodeBase>();
            }

            /// <summary>
            /// Gets or sets assembly name.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string AssemblyName
            {
                get { return assemblyName; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    assemblyName = value;
                }
            }

            /// <summary>
            /// Gets or sets the assembly public key token, or null if none.
            /// </summary>
            public string AssemblyPublicKeyToken
            {
                get { return assemblyPublicKeyToken; }
                set { assemblyPublicKeyToken = value; }
            }

            /// <summary>
            /// Gets or sets the assembly culture, or null if none.
            /// </summary>
            public string AssemblyCulture
            {
                get { return assemblyCulture; }
                set { assemblyCulture = value; }
            }

            /// <summary>
            /// Gets or sets the assembly processor architecture, or null if none.
            /// </summary>
            public string AssemblyProcessorArchitecture
            {
                get { return assemblyProcessorArchitecture; }
                set { assemblyProcessorArchitecture = value; }
            }

            /// <summary>
            /// Gets or sets whether to apply the publisher policy for this assembly.
            /// </summary>
            /// <value>
            /// The default value is true.
            /// </value>
            public bool ApplyPublisherPolicy
            {
                get { return applyPublisherPolicy; }
                set { applyPublisherPolicy = value; }
            }

            /// <summary>
            /// Gets a read-only list of assembly binding redirect elements.
            /// </summary>
            public IList<AssemblyBindingRedirect> AssemblyBindingRedirects
            {
                get { return new ReadOnlyCollection<AssemblyBindingRedirect>(assemblyBindingRedirects); }
            }

            /// <summary>
            /// Gets a mutable list of assembly code base elements.
            /// </summary>
            public IList<AssemblyCodeBase> AssemblyCodeBases
            {
                get { return new ReadOnlyCollection<AssemblyCodeBase>(assemblyCodeBases); }
            }

            /// <summary>
            /// Creates a copy of the assembly dependency information.
            /// </summary>
            /// <returns>The copy.</returns>
            public AssemblyDependency Copy()
            {
                AssemblyDependency copy = new AssemblyDependency(assemblyName);

                copy.applyPublisherPolicy = applyPublisherPolicy;
                copy.assemblyCulture = assemblyCulture;
                copy.assemblyProcessorArchitecture = assemblyProcessorArchitecture;
                copy.assemblyPublicKeyToken = assemblyPublicKeyToken;

                foreach (AssemblyBindingRedirect bindingRedirect in assemblyBindingRedirects)
                    copy.assemblyBindingRedirects.Add(bindingRedirect.Copy());
                foreach (AssemblyCodeBase codeBase in assemblyCodeBases)
                    copy.assemblyCodeBases.Add(codeBase.Copy());

                return copy;
            }

            internal void AddConfigurationElement(XmlElement parent)
            {
                XmlElement dependentAssemblyElement = CreateChildElement(parent, "dependentAssembly", null);

                XmlElement assemblyIdentityElement = CreateChildElement(dependentAssemblyElement, "assemblyIdentity", null);
                assemblyIdentityElement.SetAttribute("name", assemblyName);

                if (assemblyPublicKeyToken != null)
                    assemblyIdentityElement.SetAttribute("publicKeyToken", assemblyPublicKeyToken);

                if (assemblyCulture != null)
                    assemblyIdentityElement.SetAttribute("culture", assemblyCulture);

                if (assemblyProcessorArchitecture != null)
                    assemblyIdentityElement.SetAttribute("processorArchitecture", assemblyProcessorArchitecture);

                if (!applyPublisherPolicy)
                {
                    XmlElement publisherPolicyElement = CreateChildElement(dependentAssemblyElement, "publisherPolicy", null);
                    publisherPolicyElement.SetAttribute("apply", "no");
                }

                foreach (AssemblyBindingRedirect bindingRedirect in assemblyBindingRedirects)
                    bindingRedirect.AddConfigurationElement(dependentAssemblyElement);

                foreach (AssemblyCodeBase codeBase in assemblyCodeBases)
                    codeBase.AddConfigurationElement(dependentAssemblyElement);
            }

            /// <summary>
            /// Clears the list of assembly binding redirects.
            /// </summary>
            public void ClearAssemblyBindingRedirects()
            {
                assemblyBindingRedirects.Clear();
            }

            /// <summary>
            /// Adds an assembly binding redirect if it is not already in the configuration.
            /// </summary>
            /// <param name="assemblyBindingRedirect">The assembly binding redirect to add.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyBindingRedirect"/> is null.</exception>
            public void AddAssemblyBindingRedirect(AssemblyBindingRedirect assemblyBindingRedirect)
            {
                if (assemblyBindingRedirect == null)
                    throw new ArgumentNullException("assemblyBindingRedirect");

                if (!assemblyBindingRedirects.Contains(assemblyBindingRedirect))
                    assemblyBindingRedirects.Add(assemblyBindingRedirect);
            }

            /// <summary>
            /// Adds an assembly binding redirect element if a suitable one is not already present.
            /// </summary>
            /// <param name="oldVersionRange">Th old version range.</param>
            /// <param name="newVersion">The new version for redirection.</param>
            /// <returns>The binding redirect element.</returns>
            public AssemblyBindingRedirect AddAssemblyBindingRedirect(string oldVersionRange, string newVersion)
            {
                AssemblyBindingRedirect assemblyBindingRedirect = assemblyBindingRedirects.Find(x =>
                    x.OldVersionRange == oldVersionRange && x.NewVersion == newVersion);
                if (assemblyBindingRedirect == null)
                {
                    assemblyBindingRedirect = new AssemblyBindingRedirect(oldVersionRange, newVersion);
                    assemblyBindingRedirects.Add(assemblyBindingRedirect);
                }

                return assemblyBindingRedirect;
            }

            /// <summary>
            /// Removes an assembly binding redirect.
            /// </summary>
            /// <param name="assemblyBindingRedirect">The assembly binding redirect to remove.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyBindingRedirect"/> is null.</exception>
            public void RemoveAssemblyBindingRedirect(AssemblyBindingRedirect assemblyBindingRedirect)
            {
                if (assemblyBindingRedirect == null)
                    throw new ArgumentNullException("assemblyBindingRedirect");

                assemblyBindingRedirects.Remove(assemblyBindingRedirect);
            }

            /// <summary>
            /// Clears the list of assembly codebases.
            /// </summary>
            public void ClearAssemblyCodeBases()
            {
                assemblyCodeBases.Clear();
            }

            /// <summary>
            /// Adds an assembly codebase if it is not already in the configuration.
            /// </summary>
            /// <param name="assemblyCodeBase">The assembly codebase to add.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyCodeBase"/> is null.</exception>
            public void AddAssemblyCodeBase(AssemblyCodeBase assemblyCodeBase)
            {
                if (assemblyCodeBase == null)
                    throw new ArgumentNullException("assemblyCodeBase");

                if (!assemblyCodeBases.Contains(assemblyCodeBase))
                    assemblyCodeBases.Add(assemblyCodeBase);
            }

            /// <summary>
            /// Removes an assembly codebase.
            /// </summary>
            /// <param name="assemblyCodeBase">The assembly codebase to remove.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyCodeBase"/> is null.</exception>
            public void RemoveAssemblyCodeBase(AssemblyCodeBase assemblyCodeBase)
            {
                if (assemblyCodeBase == null)
                    throw new ArgumentNullException("assemblyCodeBase");

                assemblyCodeBases.Remove(assemblyCodeBase);
            }

            /// <summary>
            /// Adds an assembly code-base element if a suitable one is not already present.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If another codebase element exists with the same version but perhaps
            /// a different Uri, then we do not add a new codebase since this most
            /// likely indicates that the same assembly can be loaded from multiple locations.
            /// </para>
            /// </remarks>
            /// <param name="version">The assembly version.</param>
            /// <param name="uri">The code base uri.</param>
            /// <returns>The code base element.</returns>
            public AssemblyCodeBase AddAssemblyCodeBase(string version, string uri)
            {
                AssemblyCodeBase assemblyCodeBase = assemblyCodeBases.Find(x => x.Version == version);
                if (assemblyCodeBase == null)
                {
                    assemblyCodeBase = new AssemblyCodeBase(version, uri);
                    assemblyCodeBases.Add(assemblyCodeBase);
                }

                return assemblyCodeBase;
            }
        }

        /// <summary>
        /// Describes an assembly binding redirection from an old version range to
        /// a new version.
        /// </summary>
        [Serializable]
        public sealed class AssemblyBindingRedirect
        {
            private string oldVersionRange;
            private string newVersion;

            /// <summary>
            /// Creates an assembly binding redirect.
            /// </summary>
            /// <param name="oldVersionRange">The range of old versions to redirect, specified
            /// either as a single version such as "1.2.3.4" or as a range such as "1.2.3.4-10.11.12.13".</param>
            /// <param name="newVersion">The new version to which the binding should be redirected
            /// such as "1.2.3.4".</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="oldVersionRange"/>
            /// or <paramref name="newVersion"/> is null.</exception>
            public AssemblyBindingRedirect(string oldVersionRange, string newVersion)
            {
                if (oldVersionRange == null)
                    throw new ArgumentNullException("oldVersionRange");
                if (newVersion == null)
                    throw new ArgumentNullException("newVersion");

                this.oldVersionRange = oldVersionRange;
                this.newVersion = newVersion;
            }

            /// <summary>
            /// Gets or sets the range of old versions to redirect, specified
            /// either as a single version such as "1.2.3.4" or as a range such as "1.2.3.4-10.11.12.13"
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string OldVersionRange
            {
                get { return oldVersionRange; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    oldVersionRange = value;
                }
            }

            /// <summary>
            /// Gets or sets the new version to which the binding should be redirected
            /// such as "1.2.3.4"
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string NewVersion
            {
                get { return newVersion; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    newVersion = value;
                }
            }

            /// <summary>
            /// Creates a copy of the assembly binding redirect information.
            /// </summary>
            /// <returns>The copy.</returns>
            public AssemblyBindingRedirect Copy()
            {
                return new AssemblyBindingRedirect(oldVersionRange, newVersion);
            }

            internal void AddConfigurationElement(XmlElement parent)
            {
                XmlElement bindingRedirectElement = CreateChildElement(parent, "bindingRedirect", null);
                bindingRedirectElement.SetAttribute("oldVersion", oldVersionRange);
                bindingRedirectElement.SetAttribute("newVersion", newVersion);
            }
        }

        /// <summary>
        /// Describes the location of the codebase of a particular assembly version.
        /// </summary>
        [Serializable]
        public class AssemblyCodeBase
        {
            private string version;
            private string uri;

            /// <summary>
            /// Creates an assembly code base element.
            /// </summary>
            /// <param name="version">The assembly version to which this element applies
            /// such as "1.2.3.4"</param>
            /// <param name="uri">The Uri that specifies the location of the assembly.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/>
            /// or <paramref name="uri"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="uri"/> is not an absolute Uri.</exception>
            /// <exception cref="UriFormatException">Thrown if <paramref name="uri"/> is not a well-formed Uri.</exception>
            public AssemblyCodeBase(string version, string uri)
            {
                if (version == null)
                    throw new ArgumentNullException("version");
                if (uri == null)
                    throw new ArgumentNullException("uri");

                this.version = version;
                Uri = uri;
            }

            /// <summary>
            /// Gets or sets the assembly version to which this element applies
            /// such as "1.2.3.4".
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string Version
            {
                get { return version; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    version = value;
                }
            }

            /// <summary>
            /// Gets or sets the Uri that specifies the location of the assembly.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not an absolute Uri.</exception>
            /// <exception cref="UriFormatException">Thrown if <paramref name="value"/> is not a well-formed Uri.</exception>
            public string Uri
            {
                get { return uri; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");

                    Uri uriObj = new Uri(value);
                    if (!uriObj.IsAbsoluteUri)
                        throw new ArgumentException(String.Format("The codebase Uri must be a valid absolute Uri but '{0}' was used.", value));
                    uri = uriObj.ToString();
                }
            }

            /// <summary>
            /// Creates a copy of the assembly binding redirect information.
            /// </summary>
            /// <returns>The copy.</returns>
            public AssemblyCodeBase Copy()
            {
                return new AssemblyCodeBase(version, uri);
            }

            internal void AddConfigurationElement(XmlElement parent)
            {
                XmlElement codeBaseElement = CreateChildElement(parent, "codeBase", null);
                codeBaseElement.SetAttribute("version", version);
                codeBaseElement.SetAttribute("href", uri);
            }
        }
    }
}
