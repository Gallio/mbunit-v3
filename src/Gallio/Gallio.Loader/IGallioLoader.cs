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

namespace Gallio.Loader
{
    /// <summary>
    /// <para>
    /// Provides an interface for interacting with the Gallio Loader.
    /// </para>
    /// </summary>
    /// <seealso cref="GallioLoader"/>
    public interface IGallioLoader
    {
        /// <summary>
        /// Gets the Gallio runtime path.
        /// </summary>
        /// <exception cref="SafeException">Thrown if the operation could not be performed</exception>
        string RuntimePath { get; }

        /// <summary>
        /// <para>
        /// Sets up the runtime with a default runtime setup using the loader's
        /// runtime path and a null logger.  Does nothing if the runtime has
        /// already been initialized.
        /// </para>
        /// <para>
        /// If you need more control over this behavior, call RuntimeBootstrap
        /// yourself.
        /// </para>
        /// </summary>
        /// <exception cref="SafeException">Thrown if the operation could not be performed</exception>
        void SetupRuntime();

        /// <summary>
        /// Adds a hint directory to the assembly resolver.
        /// </summary>
        /// <param name="path">The path of the hint directory to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="SafeException">Thrown if the operation could not be performed</exception>
        void AddHintDirectory(string path);

        /// <summary>
        /// Resolves a runtime service.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve</typeparam>
        /// <returns>The resolved service</returns>
        /// <exception cref="SafeException">Thrown if the operation could not be performed</exception>
        T Resolve<T>();

        /// <summary>
        /// Resolves a runtime service.
        /// </summary>
        /// <param name="serviceType">The type of service to resolve</param>
        /// <returns>The resolved service</returns>
        /// <exception cref="SafeException">Thrown if the operation could not be performed</exception>
        object Resolve(Type serviceType);
    }
}
