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
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides information used to register a component.
    /// </summary>
    public class ComponentRegistration
    {
        private IPluginDescriptor plugin;
        private IServiceDescriptor service;
        private string componentId;
        private TypeName componentTypeName;
        private PropertySet componentProperties;
        private PropertySet traitsProperties;
        private IHandlerFactory componentHandlerFactory;

        /// <summary>
        /// Creates a component registration.
        /// </summary>
        /// <param name="plugin">The plugin to which the component will belong</param>
        /// <param name="service">The service implemented by the component</param>
        /// <param name="componentId">The component id</param>
        /// <param name="componentTypeName">The component type name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="plugin"/>, <paramref name="service"/>,
        /// <paramref name="componentId"/> or <paramref name="componentTypeName"/> is null</exception>
        public ComponentRegistration(IPluginDescriptor plugin, IServiceDescriptor service, string componentId, TypeName componentTypeName)
        {
            Plugin = plugin;
            Service = service;
            ComponentId = componentId;
            ComponentTypeName = componentTypeName;
        }

        /// <summary>
        /// Gets or sets the plugin to which the component will belong.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IPluginDescriptor Plugin
        {
            get { return plugin; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                plugin = value;
            }
        }

        /// <summary>
        /// Gets or sets the service implemented by the component.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IServiceDescriptor Service
        {
            get { return service; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                service = value;
            }
        }

        /// <summary>
        /// Gets or sets the component id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string ComponentId
        {
            get { return componentId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentId = value;
            }
        }

        /// <summary>
        /// Gets or sets the component type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TypeName ComponentTypeName
        {
            get { return componentTypeName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the component properties.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public PropertySet ComponentProperties
        {
            get
            {
                if (componentProperties == null)
                    componentProperties = new PropertySet();
                return componentProperties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentProperties = value;
            }
        }

        /// <summary>
        /// Gets or sets the component handler factory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IHandlerFactory ComponentHandlerFactory
        {
            get
            {
                if (componentHandlerFactory == null)
                    componentHandlerFactory = new SingletonHandlerFactory();
                return componentHandlerFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                componentHandlerFactory = value;
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
    }
}
