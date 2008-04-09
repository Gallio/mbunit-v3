using System;
using System.Reflection;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Default implementation of a loader.
    /// </summary>
    public class DefaultLoader : ILoader
    {
        private readonly IAssemblyResolverManager assemblyResolverManager;

        /// <summary>
        /// Creates a loader.
        /// </summary>
        /// <param name="assemblyResolverManager">The assembly resolver manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyResolverManager"/>
        /// is null</exception>
        public DefaultLoader(IAssemblyResolverManager assemblyResolverManager)
        {
            if (assemblyResolverManager == null)
                throw new ArgumentNullException("assemblyResolverManager");

            this.assemblyResolverManager = assemblyResolverManager;
        }

        /// <inheritdoc />
        public IAssemblyResolverManager AssemblyResolverManager
        {
            get { return assemblyResolverManager; }
        }

        /// <inheritdoc />
        public Assembly LoadAssemblyFrom(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }
    }
}
