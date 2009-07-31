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
using Gallio.Runtime.Debugging;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Creates <see cref="IAcadProcess"/> objects.
    /// </summary>
    public interface IAcadProcessFactory
    {
        /// <summary>
        /// Creates a new AutoCAD process.
        /// </summary>
        /// <param name="debuggerSetup">The debugger setup options, or null if not debugging.</param>
        /// <returns>The new process.</returns>
        IAcadProcess CreateProcess(DebuggerSetup debuggerSetup);

        /// <summary>
        /// Gets or sets the path to <c>acad.exe</c>.
        /// </summary>
        string AcadExePath { get; set; }

        /// <summary>
        /// Set to <c>true</c> to have Gallio attach to an existing
        /// AutoCAD process; otherwise, set to false to always create
        /// new AutoCAD instances.
        /// </summary>
        bool AttachToExistingProcess { get; set; }
    }
}
