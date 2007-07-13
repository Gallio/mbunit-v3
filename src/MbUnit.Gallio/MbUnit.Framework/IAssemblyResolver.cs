using System;
using System.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// A custom assembly resolver participates in assembly resolution when
    /// standard assembly resolution fails to load the desired assembly
    /// but before assembly load paths are considered.
    /// </summary>
    /// <seealso cref="AppDomain.AssemblyResolve"/>
    public interface IAssemblyResolver
    {
        /// <summary>
        /// Resolves the assembly with the specified name.
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly as was provided
        /// to <see cref="Assembly.Load(string)" /></param>
        /// <returns>The assembly, or null if it could not be resolved</returns>
        Assembly Resolve(string assemblyName);
    }
}
