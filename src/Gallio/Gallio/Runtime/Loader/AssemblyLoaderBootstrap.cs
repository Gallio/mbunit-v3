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

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Bootstraps a globally reachable assembly loader within the Gallio installation path.
    /// </summary>
    /// <remarks>
    /// <para>
    /// May be used by clients to ensure that Gallio assemblies can be resolved assuming we
    /// were able to load the main assembly and access the bootstrap.
    /// </para>
    /// </remarks>
    public static class AssemblyLoaderBootstrap
    {
        private static readonly object syncRoot = new object();
        private static IAssemblyLoader assemblyLoader;

        /// <summary>
        /// Installs a global assembly resolver given the specified runtime path.
        /// </summary>
        /// <param name="runtimePath">The Gallio runtime path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimePath"/> is null.</exception>
        public static void Install(string runtimePath)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            lock (syncRoot)
            {
                if (assemblyLoader != null)
                    return;

                assemblyLoader = new DefaultAssemblyLoader();
                assemblyLoader.AddHintDirectory(runtimePath);
            }
        }

        /// <summary>
        /// Gets the bootstrapped assembly loader.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the bootstrap assembly loader has not been initialized.</exception>
        public static IAssemblyLoader AssemblyLoader
        {
            get
            {
                lock (syncRoot)
                {
                    if (assemblyLoader == null)
                        throw new InvalidOperationException("The bootstrap assembly loader has not been initialized.");

                    return assemblyLoader;
                }
            }
        }
    }
}
