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

namespace Gallio.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="PropertyInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IPropertyInfo : IMemberInfo, ISlotInfo, IEquatable<IPropertyInfo>
    {
        /// <summary>
        /// Gets the property attributes.
        /// </summary>
        PropertyAttributes PropertyAttributes { get; }

        /// <summary>
        /// Returns true if the property can be read.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Returns true if the property can be written.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets the get method of the property, or null if none.
        /// </summary>
        /// <returns>The get method, or null if none</returns>
        IMethodInfo GetMethod { get; }

        /// <summary>
        /// Gets the set method of the property, or null if none.
        /// </summary>
        /// <returns>The set method, or null if none</returns>
        IMethodInfo SetMethod { get; }

        /// <summary>
        /// Gets the list of index parameters for an indexer property, or
        /// an empty array if none.
        /// </summary>
        IList<IParameterInfo> IndexParameters { get; }

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the target cannot be resolved</exception>
        new PropertyInfo Resolve(bool throwOnError);
    }
}
