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
using System.IO;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides information used to register a plugin.
    /// </summary>
    public class PluginRegistration
    {
        private string pluginId;
        private TypeName pluginTypeName;
        private DirectoryInfo baseDirectory;
        private PropertySet pluginProperties;
        private PropertySet traitsProperties;
        private IHandlerFactory pluginHandlerFactory;
        private IList<AssemblyReference> assemblyReferences;
        private IList<IPluginDescriptor> pluginDependencies;
        private IList<string> probingPaths;

        /// <summary>
        /// Creates a plugin registration.
        /// </summary>
        /// <param name="pluginId">The plugin id</param>
        /// <param name="pluginTypeName">The plugin type name</param>
        /// <param name="baseDirectory">The plugin base directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <aramref name="pluginId"/>,
        /// <paramref name="pluginTypeName"/> or <paramref name="baseDirectory"/> is null</exception>
        public PluginRegistration(string pluginId, TypeName pluginTypeName, DirectoryInfo baseDirectory)
        {
            PluginId = pluginId;
            PluginTypeName = pluginTypeName;
            BaseDirectory = baseDirectory;
        }

        /// <summary>
        /// Gets or sets the plugin id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string PluginId
        {
            get { return pluginId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginId = value;
            }
        }

        /// <summary>
        /// Gets or sets the plugin type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TypeName PluginTypeName
        {
            get { return pluginTypeName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the base directory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public DirectoryInfo BaseDirectory
        {
            get { return baseDirectory; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                baseDirectory = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of plugin assembly references.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IList<AssemblyReference> AssemblyReferences
        {
            get
            {
                if (assemblyReferences == null)
                    assemblyReferences = new List<AssemblyReference>();
                return assemblyReferences;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                assemblyReferences = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of probing paths in which to
        /// attempt to locate referenced assemblies.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IList<string> ProbingPaths
        {
            get
            {
                if (probingPaths == null)
                    probingPaths = new List<string>();
                return probingPaths;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                probingPaths = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of plugin dependencies.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IList<IPluginDescriptor> PluginDependencies
        {
            get
            {
                if (pluginDependencies == null)
                    pluginDependencies = new List<IPluginDescriptor>();
                return pluginDependencies;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginDependencies = value;
            }
        }

        /// <summary>
        /// Gets or sets the plugin properties.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public PropertySet PluginProperties
        {
            get
            {
                if (pluginProperties == null)
                    pluginProperties = new PropertySet();
                return pluginProperties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the traits properties.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public PropertySet TraitsProperties
        {
            get
            {
                if (traitsProperties == null)
                    traitsProperties = new PropertySet();
                return traitsProperties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                traitsProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the plugin handler factory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IHandlerFactory PluginHandlerFactory
        {
            get
            {
                if (pluginHandlerFactory == null)
                    pluginHandlerFactory = new SingletonHandlerFactory();
                return pluginHandlerFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                pluginHandlerFactory = value;
            }
        }
    }
}
