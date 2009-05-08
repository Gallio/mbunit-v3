// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gallio.Common.Reflection
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
    public interface IAssemblyInfo : ICodeElementInfo, IEquatable<IAssemblyInfo>
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
        IList<AssemblyName> GetReferencedAssemblies();

        /// <summary>
        /// Gets the public types exported by the assembly.
        /// </summary>
        /// <returns>The exported types</returns>
        IList<ITypeInfo> GetExportedTypes();

        /// <summary>
        /// Gets all of the types in an assembly.
        /// </summary>
        /// <returns>The types</returns>
        IList<ITypeInfo> GetTypes();

        /// <summary>
        /// Gets a public type by name, or null if not found.
        /// </summary>
        /// <param name="typeName">The type name</param>
        /// <returns>The type</returns>
        ITypeInfo GetType(string typeName);

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns null</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the target cannot be resolved</exception>
        Assembly Resolve(bool throwOnError);
    }
}
