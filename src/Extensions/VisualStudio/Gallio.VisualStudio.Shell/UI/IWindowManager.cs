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
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// Manages Gallio tool windows and editors.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Finds the shell tool window with the specified window id.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <returns>The tool window, or null if none found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        ShellToolWindow FindToolWindow(string id);

        /// <summary>
        /// Opens a tool window.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <param name="window">The tool window to open</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="window"/> is null</exception>
        void OpenToolWindow(string id, ShellToolWindow window);

        /// <summary>
        /// Closes the shell tool window with the specified window id.
        /// </summary>
        /// <param name="id">The window id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is null</exception>
        void CloseToolWindow(string id);
    }
}
