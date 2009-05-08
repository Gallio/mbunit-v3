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
using System.IO;

namespace Gallio.Common.IO
{
    /// <summary>
    /// Sets <see cref="Environment.CurrentDirectory" /> when created, then
    /// restores it when disposed.
    /// </summary>
    public class CurrentDirectorySwitcher : IDisposable
    {
        private string oldDirectory;

        /// <summary>
        /// Saves the current directory then changes it to the specified value.
        /// </summary>
        /// <param name="directory">The new directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="directory"/> is null</exception>
        /// <exception cref="IOException">Thrown if the current directory could not be set</exception>
        public CurrentDirectorySwitcher(string directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            oldDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = directory;
        }

        /// <summary>
        /// Resets the current directory to its original saved value.
        /// </summary>
        /// <exception cref="IOException">Thrown if the current directory could not be reset</exception>
        public void Dispose()
        {
            if (oldDirectory != null)
            {
                Environment.CurrentDirectory = oldDirectory;
                oldDirectory = null;
            }
        }
    }
}