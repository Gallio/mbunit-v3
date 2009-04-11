using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides information used to register a service.
    /// </summary>
    public class ServiceRegistration
    {
        private IPluginDescriptor plugin;
        private string serviceId;
        private TypeName serviceTypeName;
        private IHandlerFactory traitsHandlerFactory;

        /// <summary>
        /// Creates a service registration.
        /// </summary>
        /// <param name="plugin">The plugin to which the service will belong</param>
        /// <param name="serviceId">The service id</param>
        /// <param name="serviceTypeName">The service type name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="plugin"/>, <paramref name="serviceId"/>
        /// or <paramref name="serviceTypeName"/> is null</exception>
        public ServiceRegistration(IPluginDescriptor plugin, string serviceId, TypeName serviceTypeName)
        {
            Plugin = plugin;
            ServiceId = serviceId;
            ServiceTypeName = serviceTypeName;
        }

        /// <summary>
        /// Gets or sets the plugin to which the service will belong.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IPluginDescriptor Plugin
        {
            get { return plugin; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                plugin = value;
            }
        }

        /// <summary>
        /// Gets or sets the service id.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string ServiceId
        {
            get { return serviceId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the service type name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public TypeName ServiceTypeName
        {
            get { return serviceTypeName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                serviceTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the traits handler factory.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IHandlerFactory TraitsHandlerFactory
        {
            get
            {
                if (traitsHandlerFactory == null)
                    traitsHandlerFactory = new SingletonHandlerFactory();
                return traitsHandlerFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                traitsHandlerFactory = value;
            }
        }
    }
}
