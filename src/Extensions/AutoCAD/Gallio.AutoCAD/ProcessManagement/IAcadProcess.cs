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
using Gallio.Runtime.Debugging;

namespace Gallio.AutoCAD.ProcessManagement
{
    /// <summary>
    /// Represents the AutoCAD process.
    /// </summary>
    public interface IAcadProcess : IDisposable
    {
        /// <summary>
        /// Starts the AutoCAD process and the client.
        /// </summary>
        /// <param name="ipcPortName">The IPC port name.</param>
        /// <param name="linkId">The unique id of the client/server pair.</param>
        /// <param name="gallioLoaderAssemblyPath">The path of the Gallio.Loader assembly or null if it is
        /// to be loaded from the GAC.</param>
        /// <param name="debuggerSetup">The debugger setup or null if the process shouldn't be debuggged.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ipcPortName"/> is null.</exception>
        void Start(string ipcPortName, Guid linkId, string gallioLoaderAssemblyPath, DebuggerSetup debuggerSetup);
    }
}