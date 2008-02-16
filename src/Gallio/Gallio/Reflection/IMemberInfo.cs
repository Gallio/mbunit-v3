// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;

namespace Gallio.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="MemberInfo" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IMemberInfo : ICodeElementInfo, IEquatable<IMemberInfo>
    {
        /// <summary>
        /// Gets the declaring type of the member.  If the member not a type or is a nested type,
        /// returns its containing type.  If it is a generic parameter, returns the type
        /// to which it (or its declaring method) belongs.  Otherwise returns null.
        /// </summary>
        /// <seealso cref="IGenericParameterInfo.DeclaringMethod"/>
        ITypeInfo DeclaringType { get; }

        /// <summary>
        /// Gets the member to its underlying native reflection type.
        /// </summary>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations</param>
        /// <returns>The underlying native reflection type</returns>
        /// <exception cref="CodeElementResolveException">Thrown if the target cannot be resolved</exception>
        MemberInfo Resolve(bool throwOnError);
    }
}
