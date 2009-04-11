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
        /// <typeparam name="TService">The service type</typeparam>
        /// <returns>The component instance that implements the service</returns>
        /// <exception cref="Runtime.RuntimeException">Thrown if no component was found</exception>
        TService Resolve<TService>();

        /// <summary>
        /// Resolves a single component instance that implements a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The component instance that implements the service</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null</exception>
        /// <exception cref="Runtime.RuntimeException">Thrown if no component was found</exception>
        object Resolve(Type serviceType);

        /// <summary>
        /// Resolves all component instances that implement a given service type.
        /// </summary>
        /// <typeparam name="TService">The service type</typeparam>
        /// <returns>The list of component instances that implement the service</returns>
        IList<TService> ResolveAll<TService>();

        /// <summary>
        /// Resolves all component instances that implement a given service type.
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <returns>The list of component instances that implement the service</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is null</exception>
        IList<object> ResolveAll(Type serviceType);

        /// <summary>
        /// Resolves a single component instance with a given component id.
        /// </summary>
        /// <param name="componentId">The component id</param>
        /// <returns>The component instance</returns>
        object ResolveByComponentId(string componentId);
    }
}