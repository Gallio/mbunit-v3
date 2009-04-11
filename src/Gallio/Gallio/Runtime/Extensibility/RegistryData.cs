using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;

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
                plugins = new ReadOnlyCollection<IPluginDescriptor>(GenericUtils.ToArray(pluginsByPluginId.Values));

            return plugins;
        }

        public IList<IServiceDescriptor> GetServices()
        {
            if (services == null)
                services = new ReadOnlyCollection<IServiceDescriptor>(GenericUtils.ToArray(servicesByServiceId.Values));

            return services;
        }

        public IList<IComponentDescriptor> GetComponents()
        {
            if (components == null)
                components = new ReadOnlyCollection<IComponentDescriptor>(GenericUtils.ToArray(componentsByComponentId.Values));

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
                return new ReadOnlyCollection<IComponentDescriptor>(GenericUtils.ConvertAllToArray(components, d => d));

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
