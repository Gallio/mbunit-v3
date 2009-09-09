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
using System.Linq;
using System.Text;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// A tool window pane provides the chrome for a tool window.
    /// </summary>
    public interface IToolWindowPane
    {
        /// <summary>
        /// Gets or sets the window caption.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        string Caption { get; set; }

        /// <summary>
        /// Closes the tool window.
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the tool window.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the tool window.
        /// </summary>
        void Hide();
    }
}
