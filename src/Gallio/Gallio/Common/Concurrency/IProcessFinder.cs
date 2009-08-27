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

using System.Diagnostics;

namespace Gallio.Common.Concurrency
{
    /// <summary>
    /// Wraps the static process query methods on <see cref="Process"/> to allow testing.
    /// </summary>
    public interface IProcessFinder
    {
        /// <summary>
        /// Creates an array of <see cref="IProcess"/> objects for all
        /// processes on the local compuer that share the specified name.
        /// </summary>
        /// <param name="processName">The friendly name of he process.</param>
        /// <returns>An array of <see cref="IProcess"/> objects.</returns>
        /// <seealso cref="Process.GetProcessesByName(string)"/>
        IProcess[] GetProcessesByName(string processName);
    }
}
