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
    /// Describes a service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptor is used to query declarative information about the service
    /// and to load associated code and resources.
    /// </para>
    /// </remarks>
    public interface IServiceDescriptor
    {
        /// <summary>
        /// Gets the descriptor of the plugin that provides the service.
        /// </summary>
        IPluginDescriptor Plugin { get; }

        /// <summary>
        /// Gets the service's id.
        /// </summary>
        string ServiceId { get; }

        /// <summary>
        /// Gets the service type name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The service type is usually an interface but it may also be an abstract class.
        /// </para>
        /// </remarks>
        TypeName ServiceTypeName { get; }

        /// <summary>
        /// Gets the default component type name for this service, or null if there is no default component type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default type must be a concrete class.
        /// </para>
        /// </remarks>
        TypeName DefaultComponentTypeName { get; }

        /// <summary>
        /// Gets the traits handler factory.
        /// </summary>
        IHandlerFactory TraitsHandlerFactory { get; }

        /// <summary>
        /// Returns true if the plugin that provides the service is disabled.
        /// </summary>
        bool IsDisabled { get; }

        /// <summary>
        /// Gets the reason the service was disabled.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsDisabled" /> is false.</exception>
        string DisabledReason { get; }

        /// <summary>
        /// Resolves the service type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The service type is usually an interface but it may also be an abstract class.
        /// </para>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The service type</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution.</exception>
        Type ResolveServiceType();

        /// <summary>
        /// Resolves the traits type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The traits type is a subclass of <see cref="Traits"/>.
        /// </para>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// <para>
        /// This method effectively performs two lookups.  First it resolves the service
        /// type then it searches for the associated <see cref="TraitsAttribute" /> on
        /// the service.  Consequently it is not possible to obtain the traits type
        /// name without loading the assembly in which the service and traits have been defined.
        /// </para>
        /// </remarks>
        /// <returns>The traits type</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution.</exception>
        Type ResolveTraitsType();
    }
}
