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
using System.Reflection;
using Gallio.Common.Reflection.Impl;
using Mono.Cecil;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// This assembly loader loads assemblies externally for reflection only.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation ensures that the assembly files do not remain locked on disk and are not
    /// cached in memory outside of the scope of the loader.  Consequently the Garbage Collector will
    /// automatically reclaim the loader and its contents when they become unreferenced.
    /// </para>
    /// <para>
    /// In contrast, that of the standard .Net reflection-only load context
    /// (<seealso cref="Assembly.ReflectionOnlyLoad(string)"/>) does not allow assemblies to be reclaimed
    /// until the <see cref="AppDomain" /> is unloaded.
    /// </para>
    /// <para>
    /// The loader considers all assemblies in the specified search path as well as any that
    /// can be located using the current AppDomain's assembly resolvers.
    /// </para>
    /// </remarks>
    public class ReflectionOnlyAssemblyLoader
    {
        private readonly CecilReflectionPolicy policy;

        /// <summary>
        /// Creates an assembly loader.
        /// </summary>
        public ReflectionOnlyAssemblyLoader()
        {
            policy = new CecilReflectionPolicy();
        }

        /// <summary>
        /// Adds a hint directory to search for loading assemblies.
        /// </summary>
        /// <param name="path">The search directory to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null.</exception>
        public void AddHintDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            policy.AddHintDirectory(path);
        }

        /// <summary>
        /// Gets the reflection policy used to load and access the assemblies.
        /// </summary>
        public IReflectionPolicy ReflectionPolicy
        {
            get { return policy; }
        }
    }
}
