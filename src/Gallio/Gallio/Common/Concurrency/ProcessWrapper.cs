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
using System.Diagnostics;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// Default implementation of <see cref="IProcess"/> using <see cref="Process"/>.
    /// </summary>
    public class ProcessWrapper : IProcess
    {
        private readonly Process process;

        /// <summary>
        /// Creates a new process wrapper.
        /// </summary>
        /// <param name="process">A process.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="process"/> is null.</exception>
        public ProcessWrapper(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");
            this.process = process;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the <see cref="ProcessWrapper"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                process.Dispose();
            }
        }

        /// <inheritdoc/>
        public bool IsModuleLoaded(string fileName)
        {
            foreach (ProcessModule module in process.Modules)
            {
                if (module.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void Kill()
        {
            process.Kill();
        }

        /// <inheritdoc/>
        public void Refresh()
        {
            process.Refresh();
        }

        /// <inheritdoc/>
        public bool WaitForInputIdle()
        {
            return process.WaitForInputIdle();
        }

        /// <inheritdoc/>
        public bool WaitForInputIdle(int milliseconds)
        {
            return process.WaitForInputIdle(milliseconds);
        }

        /// <inheritdoc/>
        public bool HasExited
        {
            get { return process.HasExited; }
        }

        /// <inheritdoc/>
        public ProcessStartInfo StartInfo
        {
            get { return process.StartInfo; }
            set { process.StartInfo = value; }
        }

        /// <inheritdoc/>
        public int Id
        {
            get { return process.Id; }
        }
    }
}
