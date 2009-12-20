using System;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Icarus.Events;
using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Runtime
{
    /// <summary>
    /// Auto-registration of services and components (in the Icarus assembly).
    /// The class name is an homage to Jeremy Miller and StructureMap.
    /// </summary>
    internal class DefaultConventionScanner
    {
        private readonly IRegistry registry;
        private const string IcarusPluginId = "Gallio.Icarus";

        public DefaultConventionScanner(IRegistry registry)
        {
            this.registry = registry;
        }

        public void Scan()
        {
            var plugin = registry.Plugins[IcarusPluginId];

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
            var componentId = Guid.NewGuid().ToString();

            foreach (var interfaceType in type.GetInterfaces())
            {
                // only register services in the Icarus assembly
                if (interfaceType.Assembly != assembly)
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
                var proxyType = typeof(EventHandlerProxy<>).MakeGenericType(interfaceType.GetGenericArguments());
                typeName = new TypeName(proxyType).ConvertToPartialAssemblyName();
                componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                    Guid.NewGuid().ToString(), typeName);
                componentRegistration.ComponentProperties.Add("target", string.Format("${{{0}}}", componentId));
            }
            else
            {
                componentRegistration = new ComponentRegistration(plugin, serviceDescriptor, 
                    componentId, typeName);
            }
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
