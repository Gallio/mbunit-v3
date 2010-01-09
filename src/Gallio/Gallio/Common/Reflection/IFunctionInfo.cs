// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A <see cref="MethodBase" /> reflection wrapper.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </remarks>
    public interface IFunctionInfo : IMemberInfo, IEquatable<IFunctionInfo>
    {
        /// <summary>
        /// Gets the method attributes.
        /// </summary>
        MethodAttributes MethodAttributes { get; }

        /// <summary>
        /// Gets the calling conventions for this method.
        /// </summary>
        CallingConventions CallingConvention { get; }

        /// <summary>
        /// Returns true if the method is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Returns true if the method is final.
        /// </summary>
        bool IsFinal { get; }

        /// <summary>
        /// Returns true if the method is static.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Gets a value indicating whether the method is virtual.
        /// </summary>
        bool IsVirtual { get; }

        /// <summary>
        /// Gets a value indicating whether this method can be called by other classes in the same assembly.
        /// </summary>
        bool IsAssembly { get; }

        /// <summary>
        /// Gets a value indicating whether access to this method is restricted to members of the class and members of its derived classes.
        /// </summary>
        bool IsFamily { get; }

        /// <summary>
        /// Gets a value indicating whether this method can be called by derived classes if they are in the same assembly.
        /// </summary>
        bool IsFamilyAndAssembly { get; }

        /// <summary>
        /// Gets a value indicating whether this method can be called by derived classes, wherever they are, and by all classes in the same assembly.
        /// </summary>
        bool IsFamilyOrAssembly { get; }

        /// <summary>
        /// Gets a value indicating whether this member is private.
        /// </summary>
        bool IsPrivate { get; }

        /// <summary>
        /// Gets a value indicating whether this is a public method.
        /// </summary>
        bool IsPublic { get; }

        /// <summary>
        /// Gets a value indicating whether only a member of the same kind with exactly the same signature is hidden in the derived class.
        /// </summary>
        bool IsHideBySig { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        /// <returns>The parameters.</returns>
        IList<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations.</param>
        /// <returns>The native reflection target.</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the target cannot be resolved.</exception>
        /// <seealso cref="Reflector.IsUnresolved(MemberInfo)"/>
        new MethodBase Resolve(bool throwOnError);
    }
}