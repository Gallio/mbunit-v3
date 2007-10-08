// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Event arguments for <see cref="ITestHarness.AssemblyAdded" />.
    /// </summary>
    [Serializable]
    public class AssemblyAddedEventArgs : EventArgs
    {
        private Assembly assembly;

        /// <summary>
        /// Creates event arguments.
        /// </summary>
        /// <param name="assembly">The assembly that was added</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public AssemblyAddedEventArgs(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly that was added.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }
    }
}
