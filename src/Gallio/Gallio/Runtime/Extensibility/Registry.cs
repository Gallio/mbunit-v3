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
using Gallio.Common.Concurrency;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides a configurable registry of plugins, services and components.
    /// </summary>
    public class Registry : IRegistry
    {
        private readonly Plugins plugins;
        private readonly Components components;
        private readonly Services services;
        private readonly IServiceLocator serviceLocator;
        private readonly IResourceLocator resourceLocator;

        private readonly LockBox<RegistryData> dataBox;

        /// <summary>
        /// Creates an empty registry.
        /// </summary>
        public Registry()
        {
            plugins = new Plugins(this);
            components = new Components(this);
            services = new Services(this);
            serviceLocator = new RegistryServiceLocator(this);
            resourceLocator = new RegistryResourceLocator(this);

            dataBox = new LockBox<RegistryData>(new RegistryData());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public IPlugins Plugins
        {
            get { return plugins; }
        }

        /// <inheritdoc />
        public IComponents Components
        {
            get { return components; }
        }

        /// <inheritdoc />
        public IServices Services
        {
            get { return services; }
        }

        /// <inheritdoc />
        public IServiceLocator ServiceLocator
        {
            get { return serviceLocator; }
        }

        /// <inheritdoc />
        public IResourceLocator ResourceLocator
        {
            get { return resourceLocator; }
        }

        /// <inheritdoc />
        public IPluginDescriptor RegisterPlugin(PluginRegistration pluginRegistration)
        {
            if (pluginRegistration == null)
                throw new ArgumentNullException("pluginRegistration");
            if (pluginRegistration.AssemblyBindings.Contains(null))
                throw new ArgumentException("The assembly references must not contain a null reference.", "pluginRegistration");
            if (pluginRegistration.PluginDependencies.Contains(null))
                throw new ArgumentException("The plugin dependencies must not contain a null reference.", "pluginRegistration");
            if (pluginRegistration.ProbingPaths.Contains(null))
                throw new ArgumentException("The probing paths must not contain a null reference.", "pluginRegistration");

            return dataBox.Write(data =>
            {
                if (data.GetPluginById(pluginRegistration.PluginId) != null)
                    throw new ArgumentException(string.Format("There is already a plugin registered with id '{0}'.", pluginRegistration.PluginId), "pluginRegistration");

                List<IPluginDescriptor> pluginDependencies = new List<IPluginDescriptor>(pluginRegistration.PluginDependencies);

                foreach (IPluginDescriptor pluginDependency in pluginRegistration.PluginDependencies)
                {
                    if (data.GetPluginById(pluginDependency.PluginId) != pluginDependency)
                        throw new ArgumentException(
                            "One of the plugin dependencies does not belong to this registry.", "pluginRegistration");

                    foreach (IPluginDescriptor secondOrderPluginDependency in pluginDependency.PluginDependencies)
                    {
                        if (!pluginDependencies.Contains(secondOrderPluginDependency))
                            pluginDependencies.Add(secondOrderPluginDependency);
                    }
                }

                PluginDescriptor plugin = new PluginDescriptor(this, pluginRegistration, pluginDependencies);
                data.RegisterPlugin(plugin);
                return plugin;
            });
        }

        /// <inheritdoc />
        public IServiceDescriptor RegisterService(ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration == null)
                throw new ArgumentNullException("serviceRegistration");

            return dataBox.Write(data =>
            {
                if (data.GetPluginById(serviceRegistration.Plugin.PluginId) != serviceRegistration.Plugin)
                    throw new ArgumentException("The specified plugin descriptor does not belong to this registry.", "serviceRegistration");
                if (data.GetServiceById(serviceRegistration.ServiceId) != null)
                    throw new ArgumentException(string.Format("There is already a service registered with id '{0}'.", serviceRegistration.ServiceId), "serviceRegistration");
                IServiceDescriptor otherService = data.GetServiceByServiceTypeName(serviceRegistration.ServiceTypeName);
                if (otherService != null)
                    throw new ArgumentException(string.Format("There is already a service registered with type name '{0}'.  This service has id '{1}' and the other service has id '{2}'.",
                        serviceRegistration.ServiceTypeName, serviceRegistration.ServiceId, otherService.ServiceId), "serviceRegistration");

                ServiceDescriptor service = new ServiceDescriptor(this, serviceRegistration);
                data.RegisterService(service);
                return service;
            });
        }

        /// <inheritdoc />
        public IComponentDescriptor RegisterComponent(ComponentRegistration componentRegistration)
        {
            if (componentRegistration == null)
                throw new ArgumentNullException("componentRegistration");

            return dataBox.Write(data =>
            {
                if (data.GetPluginById(componentRegistration.Plugin.PluginId) != componentRegistration.Plugin)
                    throw new ArgumentException("The specified plugin descriptor does not belong to this registry.", "componentRegistration");
                if (data.GetServiceById(componentRegistration.Service.ServiceId) != componentRegistration.Service)
                    throw new ArgumentException("The specified service descriptor does not belong to this registry.", "componentRegistration");
                if (componentRegistration.ComponentTypeName == null
                    && componentRegistration.Service.DefaultComponentTypeName == null)
                    throw new ArgumentException("The specified service descriptor does not have a default component type name so the component registration must specify a component type name but it does not.", "componentRegistration");
                var servicePluginDependency = componentRegistration.Service.Plugin;
                var componentPlugin = componentRegistration.Plugin;
                if (componentPlugin != servicePluginDependency &&
                    ! componentPlugin.PluginDependencies.Contains(servicePluginDependency))
                    throw new ArgumentException(string.Format("The service belongs to a plugin that was not declared as a dependency of the plugin that provides this component.  Plugin '{0}' should declare a dependency on plugin '{1}'.",
                        componentPlugin.PluginId, servicePluginDependency.PluginId),
                        "componentRegistration");
                if (data.GetComponentById(componentRegistration.ComponentId) != null)
                    throw new ArgumentException(string.Format("There is already a component registered with id '{0}'.", componentRegistration.ComponentId), "componentRegistration");

                ComponentDescriptor component = new ComponentDescriptor(this, componentRegistration);
                data.RegisterComponent(component);
                return component;
            });
        }

        /// <summary>
        /// Disposes the registry.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        internal LockBox<RegistryData> DataBox
        {
            get { return dataBox; }
        }
    }
}
