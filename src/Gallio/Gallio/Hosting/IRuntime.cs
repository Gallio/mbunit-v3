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
using Castle.Core;

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// The runtime is instantiated within the test runner to provide a suitable
    /// hosting environment for test enumeration and execution.
    /// </para>
    /// <para>
    /// The runtime provides a number of services to the MbUnit test framework.
    /// New services can be registered by registering them in the inversion of control
    /// container provided by the MbUnit core.
    /// </para>
    /// </summary>
    public interface IRuntime : IDisposable, IInitializable
    {
        /// <summary>
        /// Resolves a reference to a component that implements the specified service.
        /// </summary>
        /// <param name="service">The service type</param>
        /// <returns>A component that implements the service</returns>
        /// <exception cref="Exception">Thrown if the service could not be resolved</exception>
        object Resolve(Type service);

        /// <summary>
        /// Resolves a reference to a component that implements the specified service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>A component that implements the service</returns>
        T Resolve<T>();

        /// <summary>
        /// Resolves references to all components that implement the specified service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>An array of components that implement the service</returns>
        T[] ResolveAll<T>();

        /// <summary>
        /// Maps a Uri to a local path.
        /// </summary>
        /// <remarks>
        /// Recognizes plugin-relative paths of the form "plugin://Some.Plugin.Name/foo.txt"
        /// and Uri's in the "file" scheme.
        /// </remarks>
        /// <param name="uri">The uri to map</param>
        /// <returns>The local path</returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="uri"/>
        /// cannot be mapped to a local path</exception>
        string MapUriToLocalPath(Uri uri);

        /// <summary>
        /// Gets a deep copy of the runtime setup used to configure this runtime.
        /// </summary>
        /// <returns>The runtime setup</returns>
        RuntimeSetup GetRuntimeSetup();
    }
}
