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
using Gallio.Collections;
using Gallio.Reflection;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Describes a component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The descriptor is used to query declarative information about the component
    /// and to load associated code and resources.
    /// </para>
    /// </remarks>
    public interface IComponentDescriptor
    {
        /// <summary>
        /// Gets the descriptor of the plugin that provides the component.
        /// </summary>
        IPluginDescriptor Plugin { get; }

        /// <summary>
        /// Gets the descriptor of the service implemented by the component.
        /// </summary>
        IServiceDescriptor Service { get; }

        /// <summary>
        /// Gets the component's id.
        /// </summary>
        string ComponentId { get; }

        /// <summary>
        /// Gets the component type name.
        /// </summary>
        TypeName ComponentTypeName { get; }

        /// <summary>
        /// Gets the component handler factory.
        /// </summary>
        IHandlerFactory ComponentHandlerFactory { get; }

        /// <summary>
        /// Gets the parameter properties.
        /// </summary>
        PropertySet ComponentProperties { get; }

        /// <summary>
        /// Gets the traits properties.
        /// </summary>
        PropertySet TraitsProperties { get; }

        /// <summary>
        /// Returns true if the plugin that provides the component is disabled
        /// or if the service implemented by the component is disabled.
        /// </summary>
        bool IsDisabled { get; }

        /// <summary>
        /// Gets the reason the component was disabled.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IsDisabled" /> is false</exception>
        string DisabledReason { get; }

        /// <summary>
        /// Resolves the component type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The component type</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        Type ResolveComponentType();

        /// <summary>
        /// Resolves the component handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The component handler</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        IHandler ResolveComponentHandler();

        /// <summary>
        /// Resolves the component instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The component instance</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        object ResolveComponent();

        /// <summary>
        /// Resolves the traits handler.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// </remarks>
        /// <returns>The traits handler</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        IHandler ResolveTraitsHandler();

        /// <summary>
        /// Resolves the component traits.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may cause plugin resources to be loaded.  The caller
        /// should therefore assume that the operation is potentially time-consuming
        /// and may fail.
        /// </para>
        /// <para>
        /// The specific subclass of <see cref="Traits" /> returned by this method is
        /// detemined by the service's traits type as obtained by <see cref="IServiceDescriptor.ResolveTraitsType" />.
        /// </para>
        /// </remarks>
        /// <returns>The component traits</returns>
        /// <exception cref="RuntimeException">Thrown if an error occurs during resolution</exception>
        Traits ResolveTraits();
    }
}
