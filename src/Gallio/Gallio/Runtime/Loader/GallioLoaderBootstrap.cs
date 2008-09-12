using System;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Initializes the Gallio assembly loading a runtime policies.
    /// </summary>
    /// <remarks>
    /// This type is used only by the standalone Gallio.Loader assembly using late-binding.
    /// It should not be used by any other code.
    /// </remarks>
    internal static class GallioLoaderBootstrap
    {
        public static void InstallAssemblyResolver(string runtimePath)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            AssemblyResolverBootstrap.Install(runtimePath);
        }

        public static void SetupRuntime(string runtimePath)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            if (!RuntimeAccessor.IsInitialized)
            {
                RuntimeSetup setup = new RuntimeSetup();
                setup.RuntimePath = runtimePath;
                RuntimeBootstrap.Initialize(setup, NullLogger.Instance);
            }
        }

        public static void AddHintDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            AssemblyResolverBootstrap.AssemblyResolverManager.AddHintDirectory(path);
        }
    }
}
