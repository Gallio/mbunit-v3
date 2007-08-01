// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

namespace MbUnit.Framework
{
    /// <summary>
    /// A custom assembly resolver participates in assembly resolution when
    /// standard assembly resolution fails to load the desired assembly
    /// but before assembly load paths are considered.
    /// </summary>
    /// <seealso cref="AppDomain.AssemblyResolve"/>
    public interface IAssemblyResolver
    {
        /// <summary>
        /// Resolves the assembly with the specified name.
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly as was provided
        /// to <see cref="Assembly.Load(string)" /></param>
        /// <returns>The assembly, or null if it could not be resolved</returns>
        Assembly Resolve(string assemblyName);
    }
}
