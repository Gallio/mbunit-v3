using System;

namespace MbUnit.Framework.Services.Runtime
{
    /// <summary>
    /// Resolves assemblies using hint paths and custom resolvers.
    /// </summary>
    public interface IAssemblyResolverManager : IDisposable
    {
        /// <summary>
        /// The event raised when standard assembly resolution fails.
        /// </summary>
        event ResolveEventHandler AssemblyResolve; 

        /// <summary>
        /// Adds an assembly load hint directory to search when standard assembly
        /// resolution fails.
        /// </summary>
        /// <param name="hintDirectory">The hint directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hintDirectory"/> is null</exception>
        void AddHintDirectory(string hintDirectory);

        /// <summary>
        /// Adds the directory that contains the specified file as an assembly load hint
        /// directory to search when standard assembly resolution fails.
        /// </summary>
        /// <param name="file">The file in the hint directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is null</exception>
        void AddHintDirectoryContainingFile(string file);

        /// <summary>
        /// Adds the MbUnit assembly directories to the hint directory list.
        /// </summary>
        void AddMbUnitDirectories();
    }
}