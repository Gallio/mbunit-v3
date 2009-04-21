using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Collections;
using Gallio.Concurrency;
using Gallio.Reflection;

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

        private readonly LockBox<RegistryData> dataBox;

        /// <summary>
        /// Creates an empty registry.
        /// </summary>
        public Registry()
        {
            plugins = new Plugins(this);
            components = new Components(this);
            services = new Services(this);

            dataBox = new LockBox<RegistryData>(new RegistryData());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
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
        public IPluginDescriptor RegisterPlugin(PluginRegistration pluginRegistration)
        {
            if (pluginRegistration == null)
                throw new ArgumentNullException("pluginRegistration");
            if (pluginRegistration.AssemblyReferences.Contains(null))
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

        /// <inheritdoc />
        public TService Resolve<TService>()
        {
            return (TService)ResolveImpl(new TypeName(typeof(TService)));
        }

        /// <inheritdoc />
        public object Resolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return ResolveImpl(new TypeName(serviceType));
        }

        /// <inheritdoc />
        public IList<TService> ResolveAll<TService>()
        {
            return ResolveAllImpl<TService>(new TypeName(typeof(TService)));
        }

        /// <inheritdoc />
        public IList<object> ResolveAll(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return ResolveAllImpl<object>(new TypeName(serviceType));
        }

        /// <inheritdoc />
        public object ResolveByComponentId(string componentId)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");

            ComponentDescriptor descriptor = dataBox.Read(data => data.GetComponentById(componentId));
            if (descriptor == null)
                throw new RuntimeException(string.Format("Could not resolve component with id '{0}' because it does not appear to be registered.", componentId));

            return descriptor.ResolveComponent();
        }

        /// <inheritdoc />
        public bool CanResolve(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            IList<IComponentDescriptor> descriptors = dataBox.Read(data => data.FindComponentsByServiceTypeName(new TypeName(serviceType)));
            return descriptors.Count != 0;
        }

        /// <inheritdoc />
        public bool CanResolveByComponentId(string componentId)
        {
            if (componentId == null)
                throw new ArgumentNullException("componentId");

            ComponentDescriptor descriptor = dataBox.Read(data => data.GetComponentById(componentId));
            return descriptor != null;
        }

        private object ResolveImpl(TypeName serviceTypeName)
        {
            IList<IComponentDescriptor> descriptors = dataBox.Read(data => data.FindComponentsByServiceTypeName(serviceTypeName));
            if (descriptors.Count == 0)
                throw new RuntimeException(string.Format("Could not resolve component for service type '{0}' because there do not appear to be any components registered for that service type.", serviceTypeName));

            if (descriptors.Count > 1)
                throw new RuntimeException(string.Format("Could not resolve component for service type '{0}' because there are {1} of them registered so the request is ambiguous.", serviceTypeName, descriptors.Count));

            return descriptors[0].ResolveComponent();
        }

        /// <summary>
        /// Disposes the registry.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        private IList<TService> ResolveAllImpl<TService>(TypeName serviceTypeName)
        {
            IList<IComponentDescriptor> descriptors = dataBox.Read(data => data.FindComponentsByServiceTypeName(serviceTypeName));
            return GenericUtils.ConvertAllToArray(descriptors, descriptor => (TService) descriptor.ResolveComponent());
        }

        internal LockBox<RegistryData> DataBox
        {
            get { return dataBox; }
        }
    }
}
