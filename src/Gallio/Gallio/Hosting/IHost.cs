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
using System.Text;
using Castle.Core.Logging;

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// A host represents an environment that may be used to perform various
    /// services in isolation.
    /// </para>
    /// <para>
    /// For example, a host might provide the ability to run code in an isolated
    /// <see cref="AppDomain" /> of the current process, or it might run code
    /// in an isolated process, or connect to an existing remote process.
    /// </para>
    /// </summary>
    public interface IHost : IDisposable
    {
        /// <summary>
        /// Creates an instance of a remote object of the specified type
        /// using the type's default constructor.
        /// </summary>
        /// <param name="assemblyName">The name of assembly that contains the type</param>
        /// <param name="typeName">The full name of the type</param>
        /// <returns>The instance</returns>
        object Activate(string assemblyName, string typeName);

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup</param>
        /// <param name="logger">The logger</param>
        void InitializeRuntime(RuntimeSetup runtimeSetup, ILogger logger);

        /// <summary>
        /// Shuts down the runtime.
        /// </summary>
        void ShutdownRuntime();
    }
}
