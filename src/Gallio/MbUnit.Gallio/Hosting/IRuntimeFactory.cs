using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core;

namespace MbUnit.Hosting
{
    /// <summary>
    /// Creates an instance of <see cref="IRuntime" /> given
    /// the runtime setup options.
    /// </summary>
    public interface IRuntimeFactory
    {
        /// <summary>
        /// Creates a <see cref="IRuntime" /> given setup options.
        /// </summary>
        /// <remarks>
        /// The runtime's <see cref="IInitializable.Initialize" /> method
        /// will be called on the instance returned to perform any additional
        /// deferred initialization once the global runtime <see cref="Runtime.Instance" />
        /// property has been set.
        /// </remarks>
        /// <param name="setup">The runtime setup options, never null</param>
        /// <returns>The runtime</returns>
        IRuntime CreateRuntime(RuntimeSetup setup);
    }
}
