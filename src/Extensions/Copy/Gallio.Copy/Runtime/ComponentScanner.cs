﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/

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
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Copy.Runtime
{
    /// <summary>
    /// Auto-registration of services and components (in the Icarus assembly).
    /// The class name is an homage to Jeremy Miller and StructureMap.
    /// </summary>
    internal class ComponentScanner
    {
        private readonly IRegistry registry;
        private const string PluginId = "Gallio.Copy";

        public ComponentScanner(IRegistry registry)
        {
            this.registry = registry;
        }

        public void Scan()
        {
            var plugin = registry.Plugins[PluginId];

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                var typeName = new TypeName(type).ConvertToPartialAssemblyName();

                RegisterComponentForServices(assembly, type, typeName, plugin);
            }
        }

        private void RegisterComponentForServices(Assembly assembly, Type type, 
            TypeName typeName, IPluginDescriptor plugin)
        {
            var componentId = type.FullName;

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.Assembly != assembly && interfaceType != typeof(ICommand))
                    continue;

                if (IsOpenGenericType(interfaceType))
                    continue;

                if (ComponentAlreadyRegisteredForService(interfaceType, typeName))
                    continue;

                var serviceDescriptor = GetServiceDescriptor(interfaceType, plugin);

                RegisterComponent(componentId, typeName,
                                  plugin, serviceDescriptor);
            }
        }

        private static bool IsOpenGenericType(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.ContainsGenericParameters;
        }

        private void RegisterComponent(string componentId, TypeName typeName, 
            IPluginDescriptor plugin, IServiceDescriptor serviceDescriptor)
        {
            var componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                componentId, typeName);
            registry.RegisterComponent(componentRegistration);
        }

        private IServiceDescriptor GetServiceDescriptor(Type interfaceType, IPluginDescriptor plugin)
        {
            var serviceDescriptor = registry.Services.GetByServiceType(interfaceType);

            if (serviceDescriptor == null)
            {
                var serviceRegistration = new ServiceRegistration(plugin, interfaceType.FullName,
                                                                  new TypeName(interfaceType));
                serviceDescriptor = registry.RegisterService(serviceRegistration);
            }

            return serviceDescriptor;
        }

        private bool ComponentAlreadyRegisteredForService(Type interfaceType, TypeName typeName)
        {
            var componentDescriptors = registry.Components.FindByServiceType(interfaceType);
            foreach (var componentDescriptor in componentDescriptors)
            {
                if (componentDescriptor.ComponentTypeName.Equals(typeName))
                    return true;
            }
            return false;
        }
    }
}

