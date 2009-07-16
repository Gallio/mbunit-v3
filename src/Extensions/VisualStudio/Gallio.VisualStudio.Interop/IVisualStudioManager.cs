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
using EnvDTE;
using Gallio.Runtime.Logging;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides support for finding and launching instances of VisualStudio.
    /// </summary>
    public interface IVisualStudioManager
    {
        /// <summary>
        /// Gets the active instance of Visual Studio, or spawns one if requested.
        /// </summary>
        /// <param name="version">The version of Visual Studio to find, or <see cref="VisualStudioVersion.Any"/> for any version.</param>
        /// <param name="launchIfNoActiveInstance">If true, launches an instance if there is no active Visual Studio yet.</param>
        /// <param name="logger">The logger for writing progress and failure messages.</param>
        /// <returns>The Visual Studio instance, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        IVisualStudio GetVisualStudio(VisualStudioVersion version, bool launchIfNoActiveInstance, ILogger logger);

        /// <summary>
        /// Launches the latest installed version of Visual Studio.
        /// </summary>
        /// <param name="version">The version of Visual Studio to launch, or <see cref="VisualStudioVersion.Any"/> for the most recent version.</param>
        /// <param name="logger">The logger for writing progress and failure messages.</param>
        /// <returns>The Visual Studio instance, or null on failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null.</exception>
        IVisualStudio LaunchVisualStudio(VisualStudioVersion version, ILogger logger);
    }
}
