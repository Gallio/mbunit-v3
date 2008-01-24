using System;
using System.Collections.Generic;

namespace Gallio.Hosting
{
    /// <summary>
    /// A registered service resolver looks up <see cref="IRegisteredComponent"/>
    /// components by name.
    /// </summary>
    public interface IRegisteredComponentResolver<T>
        where T : class, IRegisteredComponent
    {
        /// <summary>
        /// Gets the names of all registered components.
        /// </summary>
        /// <returns>The list of registered component names</returns>
        IList<string> GetNames();

        /// <summary>
        /// Resolves a registered component by name.
        /// </summary>
        /// <param name="name">The name of the registered component, matched case-insensitively</param>
        /// <returns>The test runner factory, or null if none exist with the specified name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        T Resolve(string name);
    }
}
