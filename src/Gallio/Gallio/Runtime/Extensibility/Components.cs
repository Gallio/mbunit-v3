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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Common.Reflection;

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
