using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class ServiceDescriptor : IServiceDescriptor
    {
        private readonly Registry registry;
        private readonly PluginDescriptor pluginDescriptor;
        private readonly string serviceId;
        private readonly TypeName serviceTypeName;
        private readonly IHandlerFactory traitsHandlerFactory;

        private Type serviceType;
        private Type traitsType;

        public ServiceDescriptor(Registry registry, ServiceRegistration serviceRegistration)
        {
            this.registry = registry;
            pluginDescriptor = (PluginDescriptor) serviceRegistration.Plugin;
            serviceId = serviceRegistration.ServiceId;
            serviceTypeName = serviceRegistration.ServiceTypeName;
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
