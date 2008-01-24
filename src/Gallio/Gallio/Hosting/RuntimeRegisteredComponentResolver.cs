using System;
using System.Collections.Generic;

namespace Gallio.Hosting
{
    /// <summary>
    /// A default implementation of <see cref="IRegisteredComponentResolver{T}" />
    /// based on <see cref="IRuntime" />.
    /// </summary>
    public class RuntimeRegisteredComponentResolver<T> : IRegisteredComponentResolver<T>
        where T : class, IRegisteredComponent
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates a test runner manager.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public RuntimeRegisteredComponentResolver(IRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;
        }

        /// <inheritdoc />
        public IList<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach (T component in runtime.ResolveAll<T>())
                names.Add(component.Name);

            return names;
        }

        /// <inheritdoc />
        public T Resolve(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (T component in runtime.ResolveAll<T>())
            {
                if (String.Equals(name, component.Name, StringComparison.CurrentCultureIgnoreCase))
                    return component;
            }

            return null;
        }
    }
}
