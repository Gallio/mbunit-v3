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

namespace Gallio.VisualStudio.Toolkit
{
    /// <summary>
    /// A component is an object that manages some resource on behalf of an add-in.
    /// </summary>
    public abstract class Component
    {
        private readonly Shell shell;

        /// <summary>
        /// Creates a component.
        /// </summary>
        /// <param name="shell">The shell that owns the component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shell"/> is null</exception>
        public Component(Shell shell)
        {
            if (shell == null)
                throw new ArgumentNullException("shell");

            this.shell = shell;
        }

        /// <summary>
        /// Gets the shell that owns the component.
        /// </summary>
        public Shell Shell
        {
            get { return shell; }
        }
    }
}
