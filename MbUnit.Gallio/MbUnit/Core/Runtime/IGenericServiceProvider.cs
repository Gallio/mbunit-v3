using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Runtime
{
    /// <summary>
    /// The generic service provider interface extends the service provider
    /// interface with an overload of <see cref="IServiceProvider.GetService" />
    /// that uses a type parameter.
    /// </summary>
    public interface IGenericServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Gets a service of the specified type.
        /// </summary>
        /// <remarks>
        /// This is functionally equivalent to calling <see cref="IServiceProvider.GetService" />
        /// with the specified type then casting the resulting object.
        /// </remarks>
        /// <typeparam name="T">The type of service to obtain</typeparam>
        /// <returns>An instance of a component that implements that service</returns>
        T GetService<T>();
    }
}
