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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;
using Gallio.Common;

namespace Gallio.Runtime
{
    /// <summary>
    /// The runtime is instantiated within the test runner to provide a suitable
    /// hosting environment for test enumeration and execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The runtime provides services to support the Gallio test automation platform.
    /// New services are typically registered by adding them to plugin configuration
    /// files.
    /// </para>
    /// </remarks>
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
        /// Gets the runtime assembly loader.
        /// </summary>
        IAssemblyLoader AssemblyLoader { get; }

        /// <summary>
        /// Gets the condition context that is used to enable or disable plugins,
        /// components and services based on characteristics of the runtime environment.
        /// </summary>
        RuntimeConditionContext RuntimeConditionContext { get; }

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <exception cref="RuntimeException">Thrown if the runtime could not be initialized.</exception>
        void Initialize();

        /// <summary>
        /// Gets a deep copy of the runtime setup used to configure this runtime.
        /// </summary>
        /// <returns>The runtime setup.</returns>
        RuntimeSetup GetRuntimeSetup();

        /// <summary>
        /// Gets the list of all plugin assembly bindings.
        /// </summary>
        /// <returns>The assembly bindings.</returns>
        IList<AssemblyBinding> GetAllPluginAssemblyBindings();

        /// <summary>
        /// Verifies that the runtime is correctly installed.  Writes details to the log.
        /// </summary>
        /// <returns>True if the installation appears ok.</returns>
        bool VerifyInstallation();

        /// <summary>
        /// An event that is fired when a log message is sent to the runtime logger.
        /// </summary>
        event EventHandler<LogEntrySubmittedEventArgs> LogMessage;

        /// <summary>
        /// Adds a log listener that will receive log messages dispatched to the runtime logger.
        /// </summary>
        /// <param name="logger">The log listener to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        void AddLogListener(ILogger logger);

        /// <summary>
        /// Removes a log listener so that it will no longer receive log messages dispatched to the runtime logger.
        /// </summary>
        /// <param name="logger">The log listener to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        void RemoveLogListener(ILogger logger);
    }
}