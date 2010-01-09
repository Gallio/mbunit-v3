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
using Gallio.VisualStudio.Shell.Core;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// Default tool window manager.
    /// </summary>
    public class DefaultToolWindowManager : IToolWindowManager
    {
        private readonly DefaultShell shell;

        /// <summary>
        /// Initializes the tool window manager.
        /// </summary>
        /// <param name="shell">The shell.</param>
        public DefaultToolWindowManager(IShell shell)
        {
            this.shell = (DefaultShell)shell;
        }

        /// <inheritdoc />
        public ToolWindow FindToolWindow(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            int internalId = GenerateWindowId(id);
            var pane = (ShellToolWindowPane)shell.ShellPackage.FindToolWindow(typeof(ShellToolWindowPane), internalId, false);
            return pane.ToolWindowContainer.ToolWindow;
        }

        /// <inheritdoc />
        public void OpenToolWindow(string id, ToolWindow window)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (window == null)
                throw new ArgumentNullException("window");

            int internalId = GenerateWindowId(id);
            var pane = (ShellToolWindowPane) shell.ShellPackage.FindToolWindow(typeof(ShellToolWindowPane), internalId, true);
            if (pane == null)
                throw new ShellException("Could not create an instance of the Shell tool window pane.");

            pane.ToolWindowContainer.ToolWindow = window;
            window.Show();
        }

        /// <inheritdoc />
        public void CloseToolWindow(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            ToolWindow window = FindToolWindow(id);
            if (window != null)
                window.Close();
        }

        private static int GenerateWindowId(string id)
        {
            return id.GetHashCode() & 0x7fffffff;
        }
    }
}
