using Gallio.Runtime;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Extensibility;
using Gallio.Common.Reflection;
using System.IO;
using System;

namespace Gallio.Icarus.Runtime
{
    internal class IcarusRuntime : DefaultRuntime
    {
        private readonly RuntimeSetup runtimeSetup;

        public IcarusRuntime(IRegistry registry, IPluginLoader pluginLoader,
            IAssemblyResolverManager assemblyResolverManager, RuntimeSetup runtimeSetup)
            : base(registry, pluginLoader, assemblyResolverManager, runtimeSetup)
        {
            this.runtimeSetup = runtimeSetup;
        }

        public void RegisterComponent(string serviceId, Type serviceType, object component)
        {
            const string pluginId = "Gallio.Icarus";
            var pluginDescriptor = Registry.Plugins[pluginId] ?? Registry.RegisterPlugin(new PluginRegistration(pluginId, 
                new TypeName(typeof(DefaultPlugin)), new DirectoryInfo(runtimeSetup.RuntimePath))
            {
                TraitsProperties = { { "Name", "Gallio Icarus Test Runner" } }
            });

            var serviceDescriptor = Registry.Services[serviceId] ?? Registry.RegisterService(new ServiceRegistration(pluginDescriptor, 
                serviceId, new TypeName(serviceType)));

            Registry.RegisterComponent(new ComponentRegistration(pluginDescriptor, serviceDescriptor, 
                serviceId, new TypeName(component.GetType()))
            {
                ComponentHandlerFactory = new InstanceHandlerFactory(component)
            });
        }
    }
}
