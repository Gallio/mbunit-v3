using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class Services : IServices
    {
        private readonly Registry registry;

        public Services(Registry registry)
        {
            this.registry = registry;
        }

        public IServiceDescriptor this[string serviceId]
        {
            get
            {
                if (serviceId == null)
                    throw new ArgumentNullException("serviceId");

                return registry.DataBox.Read(data => data.GetServiceById(serviceId));
            }
        }

        public IServiceDescriptor GetByServiceType(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return GetByServiceTypeNameImpl(new TypeName(serviceType));
        }

        public IServiceDescriptor GetByServiceTypeName(TypeName serviceTypeName)
        {
            if (serviceTypeName == null)
                throw new ArgumentNullException("serviceTypeName");

            return GetByServiceTypeNameImpl(serviceTypeName);
        }

        private IServiceDescriptor GetByServiceTypeNameImpl(TypeName serviceTypeName)
        {
            return registry.DataBox.Read(data => data.GetServiceByServiceTypeName(serviceTypeName));
        }

        public IEnumerator<IServiceDescriptor> GetEnumerator()
        {
            return registry.DataBox.Read(data => data.GetServices()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
