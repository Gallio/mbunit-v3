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
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Stores the data managed by the registry so that it can be safely guarded by a reader/writer lock.
    /// </summary>
    internal sealed class RegistryData
    {
        private readonly Dictionary<string, PluginDescriptor> pluginsByPluginId;
        private readonly Dictionary<string, ServiceDescriptor> servicesByServiceId;
        private readonly Dictionary<TypeName, ServiceDescriptor> servicesByServiceTypeName;
        private readonly Dictionary<string, ComponentDescriptor> componentsByComponentId;
        private readonly MultiMap<string, ComponentDescriptor> componentsByServiceId;

        private ReadOnlyCollection<IPluginDescriptor> plugins;
        private ReadOnlyCollection<IServiceDescriptor> services;
        private ReadOnlyCollection<IComponentDescriptor> components;

        public RegistryData()
        {
            pluginsByPluginId = new Dictionary<string, PluginDescriptor>();
            servicesByServiceId = new Dictionary<string, ServiceDescriptor>();
            servicesByServiceTypeName = new Dictionary<TypeName, ServiceDescriptor>();
            componentsByComponentId = new Dictionary<string, ComponentDescriptor>();
            componentsByServiceId = new MultiMap<string, ComponentDescriptor>();
        }

        public PluginDescriptor GetPluginById(string id)
        {
            PluginDescriptor plugin;
            pluginsByPluginId.TryGetValue(id, out plugin);
            return plugin;
        }

        public ServiceDescriptor GetServiceById(string id)
        {
            ServiceDescriptor service;
            servicesByServiceId.TryGetValue(id, out service);
            return service;
        }

        public ComponentDescriptor GetComponentById(string id)
        {
            ComponentDescriptor component;
            componentsByComponentId.TryGetValue(id, out component);
            return component;
        }

        public IList<IPluginDescriptor> GetPlugins()
        {
            if (plugins == null)
                plugins = new ReadOnlyCollection<IPluginDescriptor>(GenericCollectionUtils.ToArray(pluginsByPluginId.Values));

            return plugins;
        }

        public IList<IServiceDescriptor> GetServices()
        {
            if (services == null)
                services = new ReadOnlyCollection<IServiceDescriptor>(GenericCollectionUtils.ToArray(servicesByServiceId.Values));

            return services;
        }

        public IList<IComponentDescriptor> GetComponents()
        {
            if (components == null)
                components = new ReadOnlyCollection<IComponentDescriptor>(GenericCollectionUtils.ToArray(componentsByComponentId.Values));

            return components;
        }

        public ServiceDescriptor GetServiceByServiceTypeName(TypeName serviceTypeName)
        {
            ServiceDescriptor service;
            if (servicesByServiceTypeName.TryGetValue(serviceTypeName, out service))
                return service;

            if (! serviceTypeName.HasPartialAssemblyName)
            {
                serviceTypeName = serviceTypeName.ConvertToPartialAssemblyName();
                if (servicesByServiceTypeName.TryGetValue(serviceTypeName, out service))
                    return service;
            }

            return null;
        }

        public IList<IComponentDescriptor> FindComponentsByServiceId(string serviceId)
        {
            IList<ComponentDescriptor> components;
            if (componentsByServiceId.TryGetValue(serviceId, out components))
                return new ReadOnlyCollection<IComponentDescriptor>(GenericCollectionUtils.ConvertAllToArray(components, d => d));

            return EmptyArray<IComponentDescriptor>.Instance;
        }

        public IList<IComponentDescriptor> FindComponentsByServiceTypeName(TypeName serviceTypeName)
        {
            ServiceDescriptor service = GetServiceByServiceTypeName(serviceTypeName);
            if (service == null)
                return EmptyArray<IComponentDescriptor>.Instance;

            return FindComponentsByServiceId(service.ServiceId);
        }

        public void RegisterPlugin(PluginDescriptor plugin)
        {
            plugins = null;
            pluginsByPluginId.Add(plugin.PluginId, plugin);
        }

        public void RegisterService(ServiceDescriptor service)
        {
            services = null;
            servicesByServiceId.Add(service.ServiceId, service);
            servicesByServiceTypeName.Add(service.ServiceTypeName, service);

            if (! service.ServiceTypeName.HasPartialAssemblyName)
                servicesByServiceTypeName.Add(service.ServiceTypeName.ConvertToPartialAssemblyName(), service);
        }

        public void RegisterComponent(ComponentDescriptor component)
        {
            components = null;
            componentsByComponentId.Add(component.ComponentId, component);
            componentsByServiceId.Add(component.Service.ServiceId, component);
        }
    }
}
