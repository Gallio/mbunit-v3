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
