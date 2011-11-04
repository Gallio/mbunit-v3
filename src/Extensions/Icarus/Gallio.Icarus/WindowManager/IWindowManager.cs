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

using System.Drawing;
using System.Windows.Forms;
using Gallio.Common;

namespace Gallio.Icarus.WindowManager
{
    /// <summary>
    /// Window manager for application shell.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// The menu manager.
        /// </summary>
        IMenuManager MenuManager { get; }

    	/// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        /// <param name="content">The content to add to the window.</param>
        /// <param name="caption">The caption to give the window.</param>
        /// <returns>The added window.</returns>
        Window Add(string identifier, Control content, string caption);
        
        /// <summary>
        /// Adds a new window.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        /// <param name="content">The content to add to the window.</param>
        /// <param name="caption">The caption to give the window.</param>
        /// <param name="icon">The icon to use for the window.</param>
        /// <returns>The added window.</returns>
        Window Add(string identifier, Control content, string caption, Icon icon);
        
        /// <summary>
        /// Gets an existing window, or null if it cannot be found.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        /// <returns>A window, or null.</returns>
        Window Get(string identifier);
        
        /// <summary>
        /// Activates a window.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        void Show(string identifier);

        /// <summary>
        /// Activates a window.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        /// <param name="location">The location to display the window.</param>
        void Show(string identifier, Location location);
        
        /// <summary>
        /// Registers an action to create the window when required.
        /// </summary>
        /// <param name="identifer">The unique id of the window.</param>
        /// <param name="action">An action that creates the window.</param>
        void Register(string identifer, Action action);

        /// <summary>
        /// Registers an action to create the window when required.
        /// </summary>
        /// <param name="identifer">The unique id of the window.</param>
        /// <param name="action">An action that creates the window.</param>
        /// <param name="defaultLocation">The default location to display the window.</param>
        void Register(string identifer, Action action, Location defaultLocation);

        /// <summary>
        /// Removes a window.
        /// </summary>
        /// <param name="identifier">The unique id of the window.</param>
        void Remove(string identifier);

        /// <summary>
        /// 
        /// </summary>
        void ShowDefaults();

		/// <summary>
		/// Show a modal dialog.
		/// </summary>
		/// <param name="form">The form to display.</param>
		/// <returns>A dialog result.</returns>
    	DialogResult ShowDialog(Form form);
    }
}
