using System;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Bootstraps a globally reachable assembly resolver manager within the
    /// Gallio installation path.  May be used by clients to ensure that Gallio
    /// assemblies can be resolved assuming we were able to load the main
    /// assembly and access the bootstrap.
    /// </summary>
    /// <remarks>
    /// This type is used by the standalone Gallio.Loader assembly.
    /// </remarks>
    public static class AssemblyResolverBootstrap
    {
        private static readonly object syncRoot = new object();
        private static IAssemblyResolverManager assemblyResolverManager;

        /// <summary>
        /// Initializes a global assembly resolver given the specified installation path.
        /// </summary>
        /// <param name="installationPath">The Gallio installation path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="installationPath"/> is null</exception>
        public static void Initialize(string installationPath)
        {
            if (installationPath == null)
                throw new ArgumentNullException("installationPath");

            lock (syncRoot)
            {
                if (assemblyResolverManager != null)
                    return;

                assemblyResolverManager = new DefaultAssemblyResolverManager();
                assemblyResolverManager.AddHintDirectory(installationPath);
            }
        }

        /// <summary>
        /// Gets the bootstrapped assembly resolver manager.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the bootstrap resolver has not been initialized</exception>
        public static IAssemblyResolverManager AssemblyResolverManager
        {
            get
            {
                lock (syncRoot)
                {
                    if (assemblyResolverManager == null)
                        throw new InvalidOperationException("The bootstrap assembly resolver has not been initialized.");

                    return assemblyResolverManager;
                }
            }
        }
    }
}
