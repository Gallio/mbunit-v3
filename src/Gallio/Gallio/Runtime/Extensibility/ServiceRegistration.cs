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
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides information used to register a service.
    /// </summary>
    public class ServiceRegistration
    {
        private IPluginDescriptor plugin;
        private string serviceId;
        private TypeName serviceTypeName;
        private TypeName defaultComponentTypeName;
        private IHandlerFactory traitsHandlerFactory;

        /// <summary>
        /// Creates a service registration.
        /// </summary>
        /// <param name="plugin">The plugin to which the service will belong.</param>
        /// <param name="serviceId">The service id.</param>
        /// <param name="serviceTypeName">The service type name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="plugin"/>, <paramref name="serviceId"/>
        /// or <paramref name="serviceTypeName"/> is null.</exception>
        public ServiceRegistration(IPluginDescriptor plugin, string serviceId, TypeName serviceTypeName)
        {
            Plugin = plugin;
            ServiceId = serviceId;
            ServiceTypeName = serviceTypeName;
        }

        /// <summary>
        /// Gets or sets the plugin to which the service will belong.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
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
        /// Gets or sets the service id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string ServiceId
        {
            get { return serviceId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the service type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TypeName ServiceTypeName
        {
            get { return serviceTypeName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the default component type name, or null if none.
        /// </summary>
        public TypeName DefaultComponentTypeName
        {
            get { return defaultComponentTypeName; }
            set { defaultComponentTypeName = value; }
        }

        /// <summary>
        /// Gets or sets the traits handler factory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public IHandlerFactory TraitsHandlerFactory
        {
            get
            {
                if (traitsHandlerFactory == null)
                    traitsHandlerFactory = new SingletonHandlerFactory();
                return traitsHandlerFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                traitsHandlerFactory = value;
            }
        }
    }
}
