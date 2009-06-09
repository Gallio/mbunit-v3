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

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// A progress monitor task cookie represents a task in progress.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// When it is disposed, the corresponding <see cref="IProgressMonitor" />'s 
    /// <see cref="IProgressMonitor.Done" /> method is called.
    /// </para>
    /// </remarks>
    public struct ProgressMonitorTaskCookie : IDisposable
    {
        private readonly IProgressMonitor progressMonitor;

        /// <summary>
        /// Creates an object representing a task in progress.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null.</exception>
        public ProgressMonitorTaskCookie(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            this.progressMonitor = progressMonitor;
        }

        /// <summary>
        /// Marks the task as finished.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Equivalent to calling <see cref="IProgressMonitor.Done" /> on the associated <see cref="IProgressMonitor"/>.
        /// This method is provded as a convenience for use with the C# using statement.
        /// </para>
        /// </remarks>
        public void Dispose()
        {
            progressMonitor.Done();
        }
    }
}