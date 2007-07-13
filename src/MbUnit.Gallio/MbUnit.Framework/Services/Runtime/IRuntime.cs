using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Services.Runtime
{
    /// <summary>
    /// <para>
    /// The runtime is instantiated within the test runner to provide a suitable
    /// hosting environment for test enumeration and execution.
    /// </para>
    /// <para>
    /// The runtime provides a number of services to the MbUnit test framework.
    /// New services can be registered by registering them in the inversion of control
    /// container provided by the MbUnit core.
    /// </para>
    /// </summary>
    public interface IRuntime
    {
        /// <summary>
        /// Resolves a reference to a component that implements the specified service.
        /// </summary>
        /// <param name="service">The service type</param>
        /// <returns>A component that implements the service</returns>
        /// <exception cref="Exception">Thrown if the service could not be resolved</exception>
        object Resolve(Type service);

        /// <summary>
        /// Resolves a reference to a component that implements the specified service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>A component that implements the service</returns>
        T Resolve<T>();
    }
}
