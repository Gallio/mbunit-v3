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
using System.Windows.Forms;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// Abstract base class for components embedded in a <see cref="GallioToolWindowControl"/>.
    /// </summary>
    public abstract class GallioToolWindowControl : UserControl, IShellComponent
    {
        /// <summary>
        /// Gets the shell associated with the component.
        /// </summary>
        public IShell Shell
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor.
        /// (For the designer only)
        /// </summary>
        protected GallioToolWindowControl()
        {
        }

        /// <summary>
        /// Constructs a user control.
        /// </summary>
        /// <param name="shell">The shell associated with the component.</param>
        protected GallioToolWindowControl(IShell shell)
        {
            if (shell == null)
            {
                throw new ArgumentNullException("shell");
            }

            this.Shell = shell;
        }
    }
}
