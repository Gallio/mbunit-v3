// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Default implementation of a loader.
    /// </summary>
    public class DefaultLoader : ILoader
    {
        private readonly IAssemblyResolverManager assemblyResolverManager;

        /// <summary>
        /// Creates a loader.
        /// </summary>
        /// <param name="assemblyResolverManager">The assembly resolver manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyResolverManager"/>
        /// is null</exception>
        public DefaultLoader(IAssemblyResolverManager assemblyResolverManager)
        {
            if (assemblyResolverManager == null)
                throw new ArgumentNullException("assemblyResolverManager");

            this.assemblyResolverManager = assemblyResolverManager;
        }

        /// <inheritdoc />
        public IAssemblyResolverManager AssemblyResolverManager
        {
            get { return assemblyResolverManager; }
        }

        /// <inheritdoc />
        public Assembly LoadAssemblyFrom(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }
    }
}
