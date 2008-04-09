using System;
using System.Reflection;

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Provides services to assist with loading tests and dependent resources.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Gets the assembly resolver manager used to resolve referenced assemblies.
        /// </summary>
        IAssemblyResolverManager AssemblyResolverManager { get; }

        /// <summary>
        /// Loads an assembly from the specified file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded</exception>
        Assembly LoadAssemblyFrom(string assemblyFile);
    }
}
