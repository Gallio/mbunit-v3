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
using System.Text;
using EnvDTE;
using Gallio.Runtime.Debugging;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides control over Visual Studio.
    /// </summary>
    public interface IVisualStudio
    {
        /// <summary>
        /// Gets the version of Visual Studio represented by this object.
        /// </summary>
        VisualStudioVersion Version { get; }

        /// <summary>
        /// Returns true if the instance of Visual Studio was launched by our code, or
        /// false if it had been previously running.
        /// </summary>
        bool WasLaunched { get; }

        /// <summary>
        /// Runs a block of code with the Visual Studio DTE.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The action runs in an STA thread context with appropriate <see cref="ComRetryMessageFilter" /> installed
        /// to guard against COM timeouts.
        /// </para>
        /// </remarks>
        /// <param name="action">The action to run.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        /// <exception cref="VisualStudioException">Thrown if the call into Visual Studio failed.</exception>
        void Call(Action<DTE> action);

        /// <summary>
        /// Makes Visual Studio the foreground window.
        /// </summary>
        void BringToFront();

        /// <summary>
        /// Quits Visual Studio.
        /// </summary>
        void Quit();

        /// <summary>
        /// Gets the associated Visual Studio debugger.
        /// </summary>
        /// <param name="debuggerSetup">The debugger setup options.</param>
        /// <returns>The debugger.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="debuggerSetup"/> is null.</exception>
        IDebugger GetDebugger(DebuggerSetup debuggerSetup);
    }
}
