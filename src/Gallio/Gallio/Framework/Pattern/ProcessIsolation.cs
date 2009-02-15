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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// The process isolation object saves global process-level context
    /// information when it is created and restores it when it is disposed.
    /// </para>
    /// <para>
    /// The following context information is saved and restored:
    /// <list type="bullet">
    /// <item>The current working directory</item>
    /// </list>
    /// </para>
    /// </summary>
    public sealed class ProcessIsolation : IDisposable
    {
        private bool restored;
        private string currentDirectory;

        /// <summary>
        /// Saves global process state to be subsequently restored when disposed.
        /// </summary>
        public ProcessIsolation()
        {
            Save();
        }

        /// <summary>
        /// Restores the saved process state.
        /// </summary>
        public void Dispose()
        {
            if (!restored)
            {
                Restore();
                restored = true;
            }
        }

        private void Save()
        {
            currentDirectory = Environment.CurrentDirectory;
        }

        private void Restore()
        {
            Environment.CurrentDirectory = currentDirectory;
        }
    }
}
