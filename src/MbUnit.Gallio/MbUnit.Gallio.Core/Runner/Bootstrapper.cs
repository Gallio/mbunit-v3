using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Services.Runtime;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// The bootstrapper configures a newly created AppDomain and prepares
    /// it for test execution.
    /// </summary>
    public sealed class Bootstrapper : LongLivingMarshalByRefObject
    {
        private WindsorRuntime runtime;

        /// <summary>
        /// Initializes the AppDomain's runtime environment.
        /// </summary>
        /// <param name="pluginDirectories">Additional plugin directories to search
        /// beyond the default one containing the MbUnit core</param>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been initialized</exception>
        public void Initialize(IList<string> pluginDirectories)
        {
            if (runtime != null)
                throw new InvalidOperationException("The runtime has already been initialized.");

            DefaultAssemblyResolverManager assemblyResolverManager = new DefaultAssemblyResolverManager();
            assemblyResolverManager.AddMbUnitDirectories();

            runtime = new WindsorRuntime(assemblyResolverManager);
            foreach (string pluginDirectory in pluginDirectories)
                runtime.PluginDirectories.Add(pluginDirectory);

            runtime.Initialize();
        }

        /// <summary>
        /// Creates a remotely accessible test domain within the AppDomain.
        /// </summary>
        /// <returns>The test domain</returns>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has not been initialized</exception>
        public ITestDomain CreateTestDomain()
        {
            if (runtime == null)
                throw new InvalidOperationException("The runtime has not been initialized.");

            return new LocalTestDomain(runtime);
        }

        /// <summary>
        /// Shuts down the bootstrapped environment.
        /// </summary>
        public void Shutdown()
        {
            if (runtime != null)
            {
                runtime.Dispose();
                runtime = null;
            }
        }
    }
}
