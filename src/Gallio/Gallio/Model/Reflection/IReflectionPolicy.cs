using System;
using System.Reflection;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// A reflection policy provides access to top-level reflection resources
    /// such as assemblies.
    /// </summary>
    public interface IReflectionPolicy
    {
        /// <summary>
        /// Loads an assembly by name.
        /// </summary>
        /// <param name="assemblyName">The full or partial assembly name of the assembly to load</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null</exception>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded for any reason</exception>
        IAssemblyInfo LoadAssembly(AssemblyName assemblyName);
    }
}
