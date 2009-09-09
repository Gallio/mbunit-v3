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
using EnvDTE;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides a mechanism for Shell components to register to receive certain
    /// top-level events from Visual Studio.
    /// </summary>
    public class ShellHooks
    {
        /// <summary>
        /// Hook type for <see cref="IDTCommandTarget.QueryStatus" />.
        /// </summary>
        public delegate void QueryStatusHook(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText);

        /// <summary>
        /// Hook type for <see cref="IDTCommandTarget.Exec" />.
        /// </summary>
        public delegate void ExecHook(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled);

        /// <summary>
        /// Hook event for <see cref="IDTCommandTarget.QueryStatus" />.
        /// </summary>
        public event QueryStatusHook QueryStatus;

        /// <summary>
        /// Hook event for <see cref="IDTCommandTarget.Exec" />.
        /// </summary>
        public event ExecHook Exec;

        internal void HandleQueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (QueryStatus != null)
                QueryStatus(commandName, neededText, ref statusOption, ref commandText);
        }

        internal void HandleExec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            if (Exec != null)
                Exec(commandName, executeOption, ref variantIn, ref variantOut, ref handled);
        }
    }
}
