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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// Describes the runtime configuration of a <see cref="IHost" />.
    /// </summary>
    [Serializable]
    [XmlRoot("hostConfiguration", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class HostConfiguration : IEquatable<HostConfiguration>
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
        [XmlElement("configurationXml", IsNullable = false)]
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
        [XmlAttribute("legacyUnhandledExceptionPolicyEnabled")]
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
        [XmlAttribute("assertUiEnabled")]
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
        [XmlAttribute("remotingCustomErrorsEnabled")]
        public bool RemotingCustomErrorsEnabled
        {
            get { return remotingCustomErrorsEnabled; }
            set { remotingCustomErrorsEnabled = value; }
        }

        /// <summary>
        /// Gets a mutable list of assembly qualifications.
        /// </summary>
        [XmlArray("assemblyQualifications", IsNullable = false)]
        [XmlArrayItem("assemblyQualification", typeof(AssemblyQualification), IsNullable = false)]
        public List<AssemblyQualification> AssemblyQualifications
        {
            get { return assemblyQualifications; }
        }

        /// <summary>
        /// Gets a mutable list of assembly dependencies.
        /// </summary>
        [XmlArray("assemblyDependencies", IsNullable = false)]
        [XmlArrayItem("assemblyDependency", typeof(AssemblyDependency), IsNullable = false)]
        public List<AssemblyDependency> AssemblyDependencies
        {
            get { return assemblyDependencies; }
        }

        /// <summary>
        /// Gets a mutable list of supported runtime versions in order of preference.
        /// When the list is empty, the runtime version used to build the application is used.
        /// Otherwise one of the supported runtimes in the list is used.
        /// </summary>
        [XmlArray("supportedRuntimeVersions", IsNullable=false)]
        [XmlArrayItem("supportedRuntimeVersion", typeof(string), IsNullable = false)]
        public List<string> SupportedRuntimeVersions
        {
            get { return supportedRuntimeVersions; }
        }

        /// <summary>
        /// Adds a binding to the configuration for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="bindingRedirect">True if a catch-all binding redirect should be used to
        /// ensure that this exact version of the assembly is loaded no matter which version
        /// was originally requested</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public void AddAssemblyBinding(Assembly assembly, bool bindingRedirect)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            AddAssemblyBinding(assembly.GetName(), assembly.CodeBase, bindingRedirect);
        }

        /// <summary>
        /// Adds a binding to the configuration for the specified assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name</param>
        /// <param name="codeBase">The assembly code base Uri</param>
        /// <param name="bindingRedirect">True if a catch-all binding redirect should be used to
        /// ensure that this exact version of the assembly is loaded no matter which version
        /// was originally requested</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/>
        /// or <paramref name="codeBase"/> is null</exception>
        public void AddAssemblyBinding(AssemblyName assemblyName, string codeBase, bool bindingRedirect)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            if (codeBase == null)
                throw new ArgumentNullException("codeBase");

            assemblyQualifications.Add(new AssemblyQualification(assemblyName.Name, assemblyName.FullName));

            AssemblyDependency assemblyDependency = new AssemblyDependency(assemblyName.Name);

            if (assemblyName.GetPublicKeyToken() != null)
                assemblyDependency.AssemblyPublicKeyToken = ToHex(assemblyName.GetPublicKeyToken());

            if (bindingRedirect)
                assemblyDependency.BindingRedirects.Add(new AssemblyBindingRedirect(
                    @"0.0.0.0-65535.65535.65535.65535",
                    assemblyName.Version.ToString()));

            assemblyDependency.CodeBases.Add(new AssemblyCodeBase(assemblyName.Version.ToString(), codeBase));

            assemblyDependencies.Add(assemblyDependency);
        }

        /// <summary>
        /// Overlays the <see cref="ConfigurationXml" /> with the additional configuration
        /// entries necessary to enable the features described by this instance and returns
        /// the combined Xml configuration.
        /// </summary>
        /// <returns>The combined Xml configuration</returns>
        public override string ToString()
        {
            XmlDocument document = GenerateXmlDocument();
            return document.InnerXml;
        }

        /// <summary>
        /// Overlays the <see cref="ConfigurationXml" /> with the additional configuration
        /// entries necessary to enable the features described by this instance and writes
        /// the combined Xml configuration.
        /// </summary>
        /// <param name="textWriter">The TextWriter where the Xml configuration will be written to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="textWriter"/> is null.</exception>
        public void WriteTo(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            XmlDocument document = GenerateXmlDocument();

            XmlWriterSettings settings = new XmlWriterSettings();
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
        /// Creates a copy of the host configuration information.
        /// </summary>
        /// <returns>The copy</returns>
        public HostConfiguration Copy()
        {
            HostConfiguration copy = new HostConfiguration();

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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as HostConfiguration);
        }

        /// <inheritdoc />
        public bool Equals(HostConfiguration other)
        {
            return other != null
                && legacyUnhandledExceptionPolicyEnabled == other.legacyUnhandledExceptionPolicyEnabled
                && assertUiEnabled == other.assertUiEnabled
                && remotingCustomErrorsEnabled == other.remotingCustomErrorsEnabled
                && configurationXml == other.configurationXml
                && GenericUtils.ElementsEqualOrderIndependent(assemblyQualifications, other.assemblyQualifications)
                && GenericUtils.ElementsEqualOrderIndependent(assemblyDependencies, other.assemblyDependencies)
                && GenericUtils.ElementsEqualOrderIndependent(supportedRuntimeVersions, other.supportedRuntimeVersions);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return legacyUnhandledExceptionPolicyEnabled.GetHashCode()
                ^ assertUiEnabled.GetHashCode()
                ^ remotingCustomErrorsEnabled.GetHashCode()
                ^ (configurationXml != null ? configurationXml.GetHashCode() : 0);
        }

        private XmlDocument GenerateXmlDocument()
        {
            XmlDocument document = new XmlDocument();
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
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class AssemblyQualification : IEquatable<AssemblyQualification>
        {
            private string partialName;
            private string fullName;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private AssemblyQualification()
            {
            }

            /// <summary>
            /// Creates an assembly name qualification.
            /// </summary>
            /// <param name="partialName">The assembly partial name to qualify</param>
            /// <param name="fullName">The assembly full name to use</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="partialName"/>
            /// or <paramref name="fullName"/> is null</exception>
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
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("partialName")]
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
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("fullName")]
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
            /// <returns>The copy</returns>
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

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as AssemblyQualification);
            }

            /// <inheritdoc />
            public bool Equals(AssemblyQualification other)
            {
                return other != null
                    && partialName == other.partialName
                    && fullName == other.fullName;
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return partialName.GetHashCode()
                    ^ fullName.GetHashCode();
            }
        }

        /// <summary>
        /// Describes a dependent assembly configuration entry that optionally
        /// specifies the codebase, a publisher policy and binding redirects.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class AssemblyDependency : IEquatable<AssemblyDependency>
        {
            private readonly List<AssemblyBindingRedirect> bindingRedirects;
            private readonly List<AssemblyCodeBase> codeBases;

            private string assemblyName;
            private string assemblyPublicKeyToken;
            private string assemblyCulture;
            private string assemblyProcessorArchitecture;
            private bool applyPublisherPolicy = true;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private AssemblyDependency()
            {
                bindingRedirects = new List<AssemblyBindingRedirect>();
                codeBases = new List<AssemblyCodeBase>();
            }

            /// <summary>
            /// Creates an assembly dependency.
            /// </summary>
            /// <param name="assemblyName">The assembly name</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null</exception>
            public AssemblyDependency(string assemblyName)
                : this()
            {
                if (assemblyName == null)
                    throw new ArgumentNullException("assemblyName");

                this.assemblyName = assemblyName;
            }

            /// <summary>
            /// Gets or sets assembly name.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("assemblyName")]
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
            [XmlAttribute("assemblyPublicKeyToken")]
            public string AssemblyPublicKeyToken
            {
                get { return assemblyPublicKeyToken; }
                set { assemblyPublicKeyToken = value; }
            }

            /// <summary>
            /// Gets or sets the assembly culture, or null if none.
            /// </summary>
            [XmlAttribute("assemblyCulture")]
            public string AssemblyCulture
            {
                get { return assemblyCulture; }
                set { assemblyCulture = value; }
            }

            /// <summary>
            /// Gets or sets the assembly processor architecture, or null if none.
            /// </summary>
            [XmlAttribute("assemblyProcessorArchitecture")]
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
            /// Gets a mutable list of assembly binding redirect elements.
            /// </summary>
            [XmlArray("bindingRedirects", IsNullable = false)]
            [XmlArrayItem("bindingRedirect", typeof(AssemblyBindingRedirect), IsNullable = false)]
            public List<AssemblyBindingRedirect> BindingRedirects
            {
                get { return bindingRedirects; }
            }

            /// <summary>
            /// Gets a mutable list of assembly code base elements.
            /// </summary>
            [XmlArray("codeBases", IsNullable = false)]
            [XmlArrayItem("codeBase", typeof(AssemblyCodeBase), IsNullable = false)]
            public List<AssemblyCodeBase> CodeBases
            {
                get { return codeBases; }
            }

            /// <summary>
            /// Creates a copy of the assembly dependency information.
            /// </summary>
            /// <returns>The copy</returns>
            public AssemblyDependency Copy()
            {
                AssemblyDependency copy = new AssemblyDependency();

                copy.applyPublisherPolicy = applyPublisherPolicy;
                copy.assemblyCulture = assemblyCulture;
                copy.assemblyName = assemblyName;
                copy.assemblyProcessorArchitecture = assemblyProcessorArchitecture;
                copy.assemblyPublicKeyToken = assemblyPublicKeyToken;

                foreach (AssemblyBindingRedirect bindingRedirect in bindingRedirects)
                    copy.bindingRedirects.Add(bindingRedirect.Copy());
                foreach (AssemblyCodeBase codeBase in codeBases)
                    copy.codeBases.Add(codeBase.Copy());

                return copy;
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as AssemblyDependency);
            }

            /// <inheritdoc />
            public bool Equals(AssemblyDependency other)
            {
                return other != null
                    && applyPublisherPolicy == other.applyPublisherPolicy
                    && assemblyCulture == other.assemblyCulture
                    && assemblyName == other.assemblyName
                    && assemblyProcessorArchitecture == other.assemblyProcessorArchitecture
                    && assemblyPublicKeyToken == other.assemblyPublicKeyToken
                    && GenericUtils.ElementsEqualOrderIndependent(bindingRedirects, other.bindingRedirects)
                    && GenericUtils.ElementsEqualOrderIndependent(codeBases, other.codeBases);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return applyPublisherPolicy.GetHashCode()
                    ^ (assemblyName != null ? assemblyName.GetHashCode() : 0);
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

                foreach (AssemblyBindingRedirect bindingRedirect in bindingRedirects)
                    bindingRedirect.AddConfigurationElement(dependentAssemblyElement);

                foreach (AssemblyCodeBase codeBase in codeBases)
                    codeBase.AddConfigurationElement(dependentAssemblyElement);
            }
        }

        /// <summary>
        /// Describes an assembly binding redirection from an old version range to
        /// a new version.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class AssemblyBindingRedirect : IEquatable<AssemblyBindingRedirect>
        {
            private string oldVersionRange;
            private string newVersion;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private AssemblyBindingRedirect()
            {
            }

            /// <summary>
            /// Creates an assembly binding redirect.
            /// </summary>
            /// <param name="oldVersionRange">The range of old versions to redirect, specified
            /// either as a single version such as "1.2.3.4" or as a range such as "1.2.3.4-10.11.12.13"</param>
            /// <param name="newVersion">The new version to which the binding should be redirected
            /// such as "1.2.3.4"</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="oldVersionRange"/>
            /// or <paramref name="newVersion"/> is null</exception>
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
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("oldVersionRange")]
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
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("newVersion")]
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
            /// <returns>The copy</returns>
            public AssemblyBindingRedirect Copy()
            {
                return new AssemblyBindingRedirect(oldVersionRange, newVersion);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as AssemblyBindingRedirect);
            }

            /// <inheritdoc />
            public bool Equals(AssemblyBindingRedirect other)
            {
                return other != null
                    && oldVersionRange == other.oldVersionRange
                    && newVersion == other.newVersion;
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return oldVersionRange.GetHashCode()
                    ^ newVersion.GetHashCode();
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
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public class AssemblyCodeBase : IEquatable<AssemblyCodeBase>
        {
            private string version;
            private string uri;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private AssemblyCodeBase()
            {
            }

            /// <summary>
            /// Creates an assembly code base element.
            /// </summary>
            /// <param name="version">The assembly version to which this element applies
            /// such as "1.2.3.4"</param>
            /// <param name="uri">The Uri that specifies the location of the assembly</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="version"/>
            /// or <paramref name="uri"/> is null</exception>
            public AssemblyCodeBase(string version, string uri)
            {
                if (version == null)
                    throw new ArgumentNullException("version");
                if (uri == null)
                    throw new ArgumentNullException("uri");

                this.version = version;
                this.uri = uri;
            }

            /// <summary>
            /// Gets or sets the assembly version to which this element applies
            /// such as "1.2.3.4".
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("version")]
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
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("uri")]
            public string Uri
            {
                get { return uri; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    uri = value;
                }
            }

            /// <summary>
            /// Creates a copy of the assembly binding redirect information.
            /// </summary>
            /// <returns>The copy</returns>
            public AssemblyCodeBase Copy()
            {
                return new AssemblyCodeBase(version, uri);
            }

            /// <inheritdoc />
            public override bool Equals(object obj)
            {
                return Equals(obj as AssemblyCodeBase);
            }

            /// <inheritdoc />
            public bool Equals(AssemblyCodeBase other)
            {
                return other != null
                    && version == other.version
                    && uri == other.uri;
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return version.GetHashCode()
                    ^ uri.GetHashCode();
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
