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
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides a view of registered components along with methods for resolving them.
    /// </summary>
    public interface IComponents : IEnumerable<IComponentDescriptor>
    {
        /// <summary>
        /// Gets a component descriptor by its id, or null if not found.
        /// </summary>
        /// <param name="componentId">The component id.</param>
        /// <returns>The component descriptor, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/> is null.</exception>
        IComponentDescriptor this[string componentId] { get; }

        /// <summary>
        /// Gets descriptors for all components that implement a service with a given id.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        /// <returns>The list of component descriptors.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceId"/> is null.</exception>
        IList<IComponentDescriptor> FindByServiceId(string serviceId);

        /// <summary>
        /// Gets descriptors for all components that implement a service of a given type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The list of component descriptors.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        IList<IComponentDescriptor> FindByServiceType(Type serviceType);

        /// <summary>
        /// Gets descriptors for all components that implement a service of a given type by name.
        /// </summary>
        /// <param name="serviceTypeName">The service type name.</param>
        /// <returns>The list of component descriptors.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceTypeName"/> is null.</exception>
        IList<IComponentDescriptor> FindByServiceTypeName(TypeName serviceTypeName);
    }
}
