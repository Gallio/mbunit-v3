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
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.Security
{
    /// <summary>
    /// Provides privilege elevation services to perform tasks that require administrative rights.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service interacts with the OS to determine whether the current user has administrative
    /// rights and to prompt for elevation when required.  For example, on Windows Vista this may
    /// result in the appearance of a UAC dialog.
    /// </para>
    /// <para>
    /// Windows does not support privilege elevation in process.  Consequently the requested tasks
    /// may be performed in a separate process when required.  Parameters and results are
    /// serialized and communicated with the foreign process via .Net serialization.
    /// </para>
    /// <para>
    /// Since acquiring an elevation context may require prompting the user, they should
    /// be used sparingly and in a reasonably coarse-grained manner (without compromising
    /// security by running everything in an elevation context).
    /// </para>
    /// </remarks>
    public interface IElevationManager
    {
        /// <summary>
        /// Returns true if the user of the current process has elevated privileges.
        /// </summary>
        bool HasElevatedPrivileges { get; }

        /// <summary>
        /// Tries to acquire a context for performing elevated commands.
        /// </summary>
        /// <param name="reason">Specifies the reason an elevation is required.  The
        /// reason will be displayed to the user to explain the purpose of the request.</param>
        /// <param name="context">Returns the context on success, null otherwise</param>
        /// <returns>True if the context was acquired, false if forbidden by
        /// the user or by the operating system</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reason"/> is null</exception>
        bool TryAcquireElevationContext(string reason, out IElevationContext context);
    }
}
