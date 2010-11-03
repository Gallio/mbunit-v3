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
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.UI.Events;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Runtime
{
    /// <summary>
    /// Auto-registration of services and components (in the Icarus assembly).
    /// The class name is an homage to Jeremy Miller and StructureMap.
    /// </summary>
    public class DefaultConventionScanner
    {
        private readonly IRegistry registry;
        private readonly string pluginId;

        public DefaultConventionScanner(IRegistry registry, string pluginId)
        {
            this.registry = registry;
            this.pluginId = pluginId;
        }

        public void Scan(Assembly assembly)
        {
            var plugin = registry.Plugins[pluginId];

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
                if (false == RelevantInterface(interfaceType, assembly))
                    continue;

                if (IsOpenGenericType(interfaceType))
                    continue;

                if (ComponentAlreadyRegisteredForService(interfaceType, typeName))
                    continue;

                var serviceDescriptor = GetServiceDescriptor(interfaceType, plugin);

                RegisterComponent(interfaceType, componentId, typeName, 
                    plugin, serviceDescriptor);
            }
        }

        private static bool RelevantInterface(Type interfaceType, Assembly assembly)
        {
            if (interfaceType.Assembly == assembly)
                return true;

            if (interfaceType == typeof(ICommand))
                return true;

            if (IsAnEventHandler(interfaceType))
                return true;

            return false;
        }

        private static bool IsOpenGenericType(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.ContainsGenericParameters;
        }

        private void RegisterComponent(Type interfaceType, string componentId, TypeName typeName, 
            IPluginDescriptor plugin, IServiceDescriptor serviceDescriptor)
        {
            ComponentRegistration componentRegistration;
            if (IsAnEventHandler(interfaceType))
            {
                componentRegistration = RegisterEventHandlerProxy(interfaceType, plugin, 
                    serviceDescriptor, componentId);
            }
            else
            {
                componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                    componentId, typeName);
            }
            registry.RegisterComponent(componentRegistration);
        }

        private static ComponentRegistration RegisterEventHandlerProxy(Type interfaceType, IPluginDescriptor plugin, 
            IServiceDescriptor serviceDescriptor, string componentId)
        {
            var proxyType = typeof(EventHandlerProxy<>).MakeGenericType(interfaceType.GetGenericArguments());
            var typeName = new TypeName(proxyType).ConvertToPartialAssemblyName();
            
            var componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                Guid.NewGuid().ToString(), typeName);
            
            componentRegistration.ComponentProperties.Add("target", string.Format("${{{0}}}", componentId));
            
            return componentRegistration;
        }

        private static bool IsAnEventHandler(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(Handles<>);
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
