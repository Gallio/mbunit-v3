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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Resolves runtime services.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Resolves a single component instance that implements a given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The component instance that implements the service.</returns>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        TService Resolve<TService>();

        /// <summary>
        /// Resolves a single component instance that implements a given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The component instance that implements the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        object Resolve(Type serviceType);

        /// <summary>
        /// Resolves all component instances that implement a given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <returns>The list of component instances that implement the service.</returns>
        /// <exception cref="RuntimeException">Thrown if a component could not be resolved.</exception>
        IList<TService> ResolveAll<TService>();

        /// <summary>
        /// Resolves all component instances that implement a given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The list of component instances that implement the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if a component could not be resolved.</exception>
        IList<object> ResolveAll(Type serviceType);

        /// <summary>
        /// Resolves a single component instance with a given component id.
        /// </summary>
        /// <param name="componentId">The component id.</param>
        /// <returns>The component instance.</returns>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/> is null.</exception>
        object ResolveByComponentId(string componentId);

        /// <summary>
        /// Resolves a handle for a single component that implements a given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TTraits">The traits type.</typeparam>
        /// <returns>The handle for the component that implements the service.</returns>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        ComponentHandle<TService, TTraits> ResolveHandle<TService, TTraits>()
            where TTraits : Traits;

        /// <summary>
        /// Resolves a handle for a single component that implements a given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The handle for the component implements the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        ComponentHandle ResolveHandle(Type serviceType);

        /// <summary>
        /// Resolves handles for all components that implement a given service type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TTraits">The traits type.</typeparam>
        /// <returns>The list of handles of components that implement the service.</returns>
        /// <exception cref="RuntimeException">Thrown if a component could not be resolved.</exception>
        IList<ComponentHandle<TService, TTraits>> ResolveAllHandles<TService, TTraits>()
            where TTraits : Traits;

        /// <summary>
        /// Resolves handles for all components that implement a given service type.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The list of handles of components that implement the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if a component could not be resolved.</exception>
        IList<ComponentHandle> ResolveAllHandles(Type serviceType);

        /// <summary>
        /// Resolves a handle for a single component with a given component id.
        /// </summary>
        /// <param name="componentId">The component id.</param>
        /// <returns>The component handle.</returns>
        /// <exception cref="RuntimeException">Thrown if no component was found
        /// or if a component could not be resolved.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/> is null.</exception>
        ComponentHandle ResolveHandleByComponentId(string componentId);

        /// <summary>
        /// Returns true if the there is a service registered with the specified type and it has not been disabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this method returns true it does not guarantee that there are any non-disabled components
        /// registered that implement the service neither does it guarantee that those components can actually
        /// be resolved without error.
        /// </para>
        /// </remarks>
        /// <param name="serviceType">The service type.</param>
        /// <returns>True if there is at least one component registered for the given service type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null.</exception>
        bool HasService(Type serviceType);

        /// <summary>
        /// Returns true if there is a component registered with the given component id and it has not been disabled.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this method returns true it does not guarantee that the component can actually be resolved
        /// without error.
        /// </para>
        /// </remarks>
        /// <param name="componentId">The component id.</param>
        /// <returns>True if there is a component registered with the given component id.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="componentId"/> is null.</exception>
        bool HasComponent(string componentId);
    }
}
