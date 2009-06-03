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
    /// A <see cref="MethodInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IMethodInfo : IFunctionInfo
    {
        /// <summary>
        /// Returns true if the method is a generic method.
        /// If so, the <see cref="GenericArguments" /> list will be non-empty.
        /// </summary>
        bool IsGenericMethod { get; }

        /// <summary>
        /// Returns true if the method is a generic method definition.
        /// </summary>
        bool IsGenericMethodDefinition { get; }

        /// <summary>
        /// Returns true if the method contains unbound generic parameters.
        /// If so, the <see cref="GenericArguments" /> list will contain one
        /// or more <see cref="IGenericParameterInfo" /> objects.
        /// </summary>
        bool ContainsGenericParameters { get; }

        /// <summary>
        /// Gets the generic arguments of the method.
        /// The list may contain <see cref="IGenericParameterInfo"/> objects when
        /// no type has yet been bound to a certain generic parameter slots.
        /// </summary>
        /// <returns>The generic arguments</returns>
        IList<ITypeInfo> GenericArguments { get; }

        /// <summary>
        /// Gets the generic method definition of this method, or null if the method is not generic.
        /// </summary>
        IMethodInfo GenericMethodDefinition { get; }

        /// <summary>
        /// Gets the method return type.
        /// </summary>
        ITypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the method return parameter object that contains information
        /// about the return value and its attributes.
        /// </summary>
        IParameterInfo ReturnParameter { get; }

        /// <summary>
        /// Makes a generic method instantiation.
        /// </summary>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <returns>The generic method instantiation</returns>
        IMethodInfo MakeGenericMethod(IList<ITypeInfo> genericArguments);
        
        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations.</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="NotSupportedException">Thrown if the target cannot be resolved.</exception>
        new MethodInfo Resolve(bool throwOnError);
    }
}
