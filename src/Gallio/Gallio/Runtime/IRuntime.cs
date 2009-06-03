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
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime
{
    /// <summary>
    /// <para>
    /// The runtime is instantiated within the test runner to provide a suitable
    /// hosting environment for test enumeration and execution.
    /// </para>
    /// <para>
    /// The runtime provides services to support the Gallio test automation platform.
    /// New services are typically registered by adding them to plugin configuration
    /// files.
    /// </para>
    /// </summary>
    public interface IRuntime : IDisposable
    {
        /// <summary>
        /// Gets the plugin, service and component registry.
        /// </summary>
        IRegistry Registry { get; }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Gets the resource locator.
        /// </summary>
        IResourceLocator ResourceLocator { get; }

        /// <summary>
        /// Gets the runtime logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <param name="logger">The runtime logging service.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        /// <exception cref="RuntimeException">Thrown if the runtime could not be initialized.</exception>
        void Initialize(ILogger logger);

        /// <summary>
        /// Gets a deep copy of the runtime setup used to configure this runtime.
        /// </summary>
        /// <returns>The runtime setup</returns>
        RuntimeSetup GetRuntimeSetup();

        /// <summary>
        /// Gets the list of all plugin assembly references.
        /// </summary>
        /// <returns>The assembly references</returns>
        IList<AssemblyReference> GetAllPluginAssemblyReferences();

        /// <summary>
        /// Verifies that the runtime is correctly installed.  Writes details to the log.
        /// </summary>
        /// <returns>True if the installation appears ok</returns>
        bool VerifyInstallation();
    }
}