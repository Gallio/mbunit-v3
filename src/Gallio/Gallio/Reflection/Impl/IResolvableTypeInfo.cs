// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// This extension of <see cref="ITypeInfo" /> is provided to enable the resolution of
    /// a type with <see cref="ReflectorResolveUtils.ResolveType" />.
    /// </summary>
    public interface IResolvableTypeInfo : ITypeInfo
    {
        /// <summary>
        /// Resolves the wrapper to its native reflection target within the scope
        /// of the specified method.
        /// </summary>
        /// <param name="methodContext">The method that is currently in scope, or null if none.
        /// This parameter is used when resolving types that are part of the signature
        /// of a generic method so that generic method arguments can be handled correctly.</param>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise returns a reflection object that represents an
        /// unresolved member which may only support a subset of the usual operations</param>
        /// <returns>The native reflection target</returns>
        /// <exception cref="ReflectionWrapperResolveException">Thrown if the target cannot be resolved</exception>
        Type Resolve(MethodInfo methodContext, bool throwOnError);
    }
}
