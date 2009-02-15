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
using Gallio.Loader;

namespace Gallio.VisualStudio.Shell
{
    /// <summary>
    /// Provides access to the global <see cref="IShell" /> for Visual Studio plugins.
    /// </summary>
    public static class ShellAccessor
    {
        private static readonly object shellLock = new object();
        private static Shell shell;

        /// <summary>
        /// Gets the shell, or null if not initialized.
        /// </summary>
        public static IShell Shell
        {
            get { return shell; }
        }

        internal static Shell GetShellInternal(bool autoInit)
        {
            lock (shellLock)
            {
                if (shell == null && autoInit)
                {
                    GallioLoader.Initialize().SetupRuntime();
                    shell = new Shell();
                }

                return shell;
            }
        }
    }
}
