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
using System.Collections.Generic;
using System.Reflection;

namespace Gallio.Reflection
{
    /// <summary>
    /// <para>
    /// A <see cref="MethodBase" /> reflection wrapper.
    /// </para>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </summary>
    public interface IFunctionInfo : IMemberInfo, IEquatable<IFunctionInfo>
    {
        /// <summary>
        /// Gets the method attributes.
        /// </summary>
        MethodAttributes MethodAttributes { get; }

        /// <summary>
        /// Returns true if the method is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Returns true if the method is public.
        /// </summary>
        bool IsPublic { get; }

        /// <summary>
        /// Returns true if the method is static.
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        /// <returns>The parameters</returns>
        IList<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Gets the generic parameters of the function.
        /// </summary>
        /// <returns>The generic parameters</returns>
        IList<IGenericParameterInfo> GenericParameters { get; }

        /// <summary>
        /// Resolves the wrapper to its native reflection target.
        /// </summary>
        /// <returns>The native reflection target</returns>
        /// <exception cref="CodeElementResolveException">Thrown if the target cannot be resolved</exception>
        new MethodBase Resolve();
    }
}