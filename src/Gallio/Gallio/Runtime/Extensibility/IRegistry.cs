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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A registry of plugins, services and components.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The registry is used to enumerate and resolve plugins, services and components
    /// in the system.  There is typically one registry singleton in any given process
    /// which holds references to all other components as required.
    /// </para>
    /// <para>
    /// At this time, the components managed by the runtime all have a singleton
    /// lifecycle.  This behavior may change in the future to permit the construction
    /// of transient or context-dependent components.
    /// </para>
    /// </remarks>
    public interface IRegistry : IDisposable
    {
        /// <summary>
        /// Gets a view of all registered plugins.
        /// </summary>
        IPlugins Plugins { get; }

        /// <summary>
        /// Gets a view of all registered components.
        /// </summary>
        IComponents Components { get; }

        /// <summary>
        /// Gets a view of all registered services.
        /// </summary>
        IServices Services { get; }

        /// <summary>
        /// Gets a service locator based on the contents of the registry.
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Gets a resource locator based on the contents of the registry.
        /// </summary>
        IResourceLocator ResourceLocator { get; }

        /// <summary>
        /// Registers a plugin and returns its descriptor.
        /// </summary>
        /// <param name="pluginRegistration">The plugin registration.</param>
        /// <returns>The new plugin descriptor.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pluginRegistration"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if there is already a plugin registered with the same id
        /// or if the registration contains errors.</exception>
        IPluginDescriptor RegisterPlugin(PluginRegistration pluginRegistration);

        /// <summary>
        /// Registers a service and returns its descriptor.
        /// </summary>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <returns>The new service descriptor.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceRegistration"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if there is already a service registered with the same id or type name
        /// or if the plugin does not belong to this registry.</exception>
        IServiceDescriptor RegisterService(ServiceRegistration serviceRegistration);

        /// <summary>
        /// Registers a component and returns its descriptor.
        /// </summary>
        /// <param name="componentRegistration">The component registration.</param>
        /// <returns>The new component descriptor.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentRegistration"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if there is already a component registered with the same id
        /// or if the plugin or service does not belong to this registry or if the component implements a service
        /// provided by a plugin that was not listed as a plugin dependency of the component's plugin.</exception>
        IComponentDescriptor RegisterComponent(ComponentRegistration componentRegistration);
    }
}
