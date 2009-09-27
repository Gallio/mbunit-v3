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
using System.Diagnostics;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// Wrapper for <see cref="Process"/> to allow testing.
    /// </summary>
    /// <seealso cref="Process"/>
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// Checks whether a module with the specified file name is
        /// loaded in the process.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>true if the module is loaded; otherwise, false.</returns>
        bool IsModuleLoaded(string fileName);

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <seealso cref="Process.Kill"/>
        void Kill();

        /// <summary>
        /// Discards any information about the associated process
        /// that has been cached inside the process object.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Causes the <see cref="IProcess"/> object to wait indefinitely for
        /// the associated process to enter an idle state. This overload applies
        /// only to processes with a user interface and, therefore, a message loop.
        /// </summary>
        /// <returns>true if the associated process has reached an idle state.</returns>
        bool WaitForInputIdle();

        /// <summary>
        /// Causes the <see cref="IProcess"/> object to wait the specified number
        /// of milliseconds for the associated process to enter an idle state. This
        /// overload applies only to processes with a user interface and, therefore,
        /// a message loop.
        /// </summary>
        /// <param name="milliseconds">
        /// A value of 1 to <see cref="int.MaxValue"/> that specifies the amount of
        /// time, in milliseconds, to wait for the associated process to become idle.
        /// A value of 0 specifies an immediate return, and a value of -1 specifies
        /// an infinite wait.
        /// </param>
        /// <returns>
        /// true if the associated process has reached an idle state; otherwise, false.
        /// </returns>
        bool WaitForInputIdle(int milliseconds);

        /// <summary>
        /// Gets a value indicating whether the associated process has been terminated.
        /// </summary>
        bool HasExited
        { get; }

        /// <summary>
        /// Gets or sets the properties to passed to the <see cref="Process.Start()"/>
        /// method of the underlying <see cref="Process"/>.
        /// </summary>
        ProcessStartInfo StartInfo
        { get; set; }

        /// <summary>
        /// Gets the unique identifier for the associated process.
        /// </summary>
        int Id
        { get; }
    }
}
