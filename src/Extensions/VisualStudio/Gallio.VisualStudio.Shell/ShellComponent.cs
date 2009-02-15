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

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Abstract base class for a component that requires a reference to the <see cref="IShell" />.
    /// </summary>
    public abstract class ShellComponent : IShellComponent
    {
        private readonly IShell shell;

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="shell">The associated shell</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shell"/> is null</exception>
        public ShellComponent(IShell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");

            this.shell = shell;
        }

        /// <summary>
        /// Gets the shell associated with the component.
        /// </summary>
        public IShell Shell
        {
            get { return shell; }
        }
    }
}
