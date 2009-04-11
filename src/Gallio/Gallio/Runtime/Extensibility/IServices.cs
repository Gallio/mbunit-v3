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
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Provides a view of registered components along with methods for resolving them.
    /// </summary>
    public interface IServices : IEnumerable<IServiceDescriptor>
    {
        /// <summary>
        /// Gets a service descriptor by its id, or null if not found.
        /// </summary>
        /// <param name="serviceId">The service id</param>
        /// <returns>The service descriptor, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceId"/> is null</exception>
        IServiceDescriptor this[string serviceId] { get; }

        /// <summary>
        /// Gets a service descriptor by its type, or null if not found.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service descriptor, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null</exception>
        IServiceDescriptor GetByServiceType(Type serviceType);

        /// <summary>
        /// Gets a service descriptor by its type name, or null if not found.
        /// </summary>
        /// <param name="serviceTypeName">The service type name</param>
        /// <returns>The service descriptor, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceTypeName"/> is null</exception>
        IServiceDescriptor GetByServiceTypeName(TypeName serviceTypeName);
    }
}
