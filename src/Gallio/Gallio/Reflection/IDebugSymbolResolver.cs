using System;

namespace Gallio.Reflection
{
    /// <summary>
    /// Resolves debug symbols associated with members.
    /// </summary>
    public interface IDebugSymbolResolver
    {
        /// <summary>
        /// Gets the source location of the declaration of a method, or
        /// null if not available.
        /// </summary>
        /// <param name="assemblyFile">The assembly that contains the method</param>
        /// <param name="methodToken">The method token</param>
        /// <returns>The source location, or null if unknown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyFile"/> is null</exception>
        SourceLocation GetSourceLocationForMethod(string assemblyFile, int methodToken);
    }
}