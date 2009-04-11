using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    internal sealed class Components : IComponents
    {
        private readonly Registry registry;

        public Components(Registry registry)
        {
            this.registry = registry;
        }

        public IComponentDescriptor this[string componentId]
        {
            get
            {
                if (componentId == null)
                    throw new ArgumentNullException("componentId");

                return registry.DataBox.Read(data => data.GetComponentById(componentId));
            }
        }

        public IList<IComponentDescriptor> FindByServiceId(string serviceId)
        {
            if (serviceId == null)
                throw new ArgumentNullException("serviceId");

            return registry.DataBox.Read(data => data.FindComponentsByServiceId(serviceId));
        }

        public IList<IComponentDescriptor> FindByServiceType(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return FindByServiceTypeNameImpl(new TypeName(serviceType));
        }

        public IList<IComponentDescriptor> FindByServiceTypeName(TypeName serviceTypeName)
        {
            if (serviceTypeName == null)
                throw new ArgumentNullException("serviceTypeName");

            return FindByServiceTypeNameImpl(serviceTypeName);
        }

        private IList<IComponentDescriptor> FindByServiceTypeNameImpl(TypeName serviceTypeName)
        {
            return registry.DataBox.Read(data => data.FindComponentsByServiceTypeName(serviceTypeName));
        }

        public IEnumerator<IComponentDescriptor> GetEnumerator()
        {
            return registry.DataBox.Read(data => data.GetComponents()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
