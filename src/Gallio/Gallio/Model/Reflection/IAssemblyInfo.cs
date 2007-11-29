using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gallio.Model.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="Assembly" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IAssemblyInfo : ICodeElementInfo
    {
        /// <summary>
        /// Gets the codebase of the assembly as a local path if possible or as a Uri otherwise.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the full name of the assembly.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyName" /> of the assembly.
        /// </summary>
        /// <returns>The assembly name</returns>
        AssemblyName GetName();

        /// <summary>
        /// Gets the names of the assemblies referenced by this assembly.
        /// </summary>
        /// <returns>The names of the references assemblies</returns>
        AssemblyName[] GetReferencedAssemblies();

        /// <summary>
        /// Gets the public types exported by the assembly.
        /// </summary>
        /// <returns>The exported types</returns>
        ITypeInfo[] GetExportedTypes();

        /// <summary>
        /// Gets a public type by name, or null if not found.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The type</returns>
        ITypeInfo GetType(string typeName);

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <returns>The native reflection target</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved</exception>
        Assembly Resolve();
    }
}
