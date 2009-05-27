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
using System.IO;
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class ServiceDescriptor : IServiceDescriptor
    {
        private readonly Registry registry;
        private readonly PluginDescriptor pluginDescriptor;
        private readonly string serviceId;
        private readonly TypeName serviceTypeName;
        private readonly TypeName defaultComponentTypeName;
        private readonly IHandlerFactory traitsHandlerFactory;

        private Type serviceType;
        private Type traitsType;

        public ServiceDescriptor(Registry registry, ServiceRegistration serviceRegistration)
        {
            this.registry = registry;
            pluginDescriptor = (PluginDescriptor) serviceRegistration.Plugin;
            serviceId = serviceRegistration.ServiceId;
            serviceTypeName = serviceRegistration.ServiceTypeName;
            defaultComponentTypeName = serviceRegistration.DefaultComponentTypeName;
            traitsHandlerFactory = serviceRegistration.TraitsHandlerFactory;
        }

        public IPluginDescriptor Plugin
        {
            get { return pluginDescriptor; }
        }

        public string ServiceId
        {
            get { return serviceId; }
        }

        public TypeName ServiceTypeName
        {
            get { return serviceTypeName; }
        }

        public TypeName DefaultComponentTypeName
        {
            get { return defaultComponentTypeName; }
        }

        public IHandlerFactory TraitsHandlerFactory
        {
            get { return traitsHandlerFactory; }
        }

        public bool IsDisabled
        {
            get { return pluginDescriptor.IsDisabled; }
        }

        public string DisabledReason
        {
            get
            {
                if (pluginDescriptor.IsDisabled)
                    return string.Format("The plugin that provides this service was disabled.  Reason: {0}", pluginDescriptor.DisabledReason);

                throw new InvalidOperationException("The service has not been disabled.");
            }
        }


        public Type ResolveServiceType()
        {
            if (serviceType == null)
            {
                try
                {
                    serviceType = serviceTypeName.Resolve();
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the service type of service '{0}'.", serviceId), ex);
                }
            }

            return serviceType;
        }

        public Type ResolveTraitsType()
        {
            if (traitsType == null)
            {
                Type serviceType = ResolveServiceType();

                try
                {
                    var attribs = (TraitsAttribute[])serviceType.GetCustomAttributes(typeof(TraitsAttribute), true);
                    traitsType = attribs.Length == 0 ? typeof(Traits) : attribs[0].TraitsType;
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not resolve the traits type of service '{0}'.", serviceId), ex);
                }
            }

            return traitsType;
        }
    }
}
