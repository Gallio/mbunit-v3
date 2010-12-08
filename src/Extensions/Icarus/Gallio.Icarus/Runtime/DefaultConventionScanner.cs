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
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.UI.Events;

namespace Gallio.Icarus.Runtime
{
    /// <summary>
    /// Auto-registration of services and components (in the Icarus assembly).
    /// The class name is an homage to Jeremy Miller and StructureMap.
    /// </summary>
    public class DefaultConventionScanner : IPluginScanner
    {
        private readonly IRegistry registry;

        public DefaultConventionScanner(IRegistry registry)
        {
            this.registry = registry;
        }

        public void Scan(string pluginId, Assembly assembly)
        {
            var plugin = registry.Plugins[pluginId];

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;
                
                RegisterComponentForServices(type, plugin);
            }
        }

        private void RegisterComponentForServices(Type type, IPluginDescriptor plugin)
        {
            var componentId = type.FullName;

            var interfaceTypes = GetDirectInterfaces(type);
            var typeName = new TypeName(type).ConvertToPartialAssemblyName();

            RegisterFirstInterface(interfaceTypes, plugin, componentId, typeName);
            RegisterEventHandlers(interfaceTypes, plugin, componentId, typeName);
        }

        private void RegisterFirstInterface(IEnumerable<Type> interfaceTypes, IPluginDescriptor plugin, 
            string componentId, TypeName typeName)
        {
            foreach (var interfaceType in interfaceTypes)
            {
                if (IsOpenGenericType(interfaceType))
                    return;

                if (ComponentIsAlreadyRegisteredForService(interfaceType, typeName))
                    return;

                var serviceDescriptor = GetServiceDescriptor(interfaceType, plugin);
                RegisterComponent(componentId, typeName, plugin, serviceDescriptor);
                
                return;
            }
        }

        private static IEnumerable<Type> GetDirectInterfaces(Type type)
        {
            var interfaceTypes = type.GetInterfaces();

            var baseInterfaceTypes = new List<Type>();
            var baseType = type.BaseType;
            
            if (baseType != null) // i.e. type != Object
                baseInterfaceTypes.AddRange(baseType.GetInterfaces());

            foreach (var interfaceType in interfaceTypes)
            {
                if (baseInterfaceTypes.Contains(interfaceType) == false)
                    yield return interfaceType;
            }
        }

        private void RegisterEventHandlers(IEnumerable<Type> interfaceTypes, IPluginDescriptor plugin, 
            string componentId, TypeName typeName)
        {
            foreach (var interfaceType in interfaceTypes)
            {
                if (IsAnEventHandler(interfaceType) == false)
                    continue;

                if (IsOpenGenericType(interfaceType))
                    continue;

                if (ComponentIsAlreadyRegisteredForService(interfaceType, typeName))
                    continue;

                var serviceDescriptor = GetServiceDescriptor(interfaceType, plugin);
                RegisterEventHandlerProxy(interfaceType, plugin, serviceDescriptor, componentId);
            }
        }

        private static bool IsOpenGenericType(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.ContainsGenericParameters;
        }

        private void RegisterComponent(string componentId, TypeName typeName, IPluginDescriptor plugin, 
            IServiceDescriptor serviceDescriptor)
        {
            var componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                componentId, typeName);

            registry.RegisterComponent(componentRegistration);
        }

        private void RegisterEventHandlerProxy(Type interfaceType, IPluginDescriptor plugin, 
            IServiceDescriptor serviceDescriptor, string componentId)
        {
            var proxyType = typeof(EventHandlerProxy<>).MakeGenericType(interfaceType.GetGenericArguments());
            var typeName = new TypeName(proxyType).ConvertToPartialAssemblyName();
            
            var componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                Guid.NewGuid().ToString(), typeName);
            
            componentRegistration.ComponentProperties.Add("target", string.Format("${{{0}}}", componentId));

            registry.RegisterComponent(componentRegistration);
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

        private bool ComponentIsAlreadyRegisteredForService(Type interfaceType, TypeName typeName)
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
