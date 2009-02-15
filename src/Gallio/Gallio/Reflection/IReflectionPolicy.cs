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
using System.Reflection;

namespace Gallio.Reflection
{
    /// <summary>
    /// A reflection policy provides access to top-level reflection resources
    /// such as assemblies.
    /// </summary>
    public interface IReflectionPolicy
    {
        /// <summary>
        /// Loads an assembly by name.
        /// </summary>
        /// <param name="assemblyName">The full or partial assembly name of the assembly to load</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null</exception>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded for any reason</exception>
        IAssemblyInfo LoadAssembly(AssemblyName assemblyName);

        /// <summary>
        /// Loads an assembly from a file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file path</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyFile"/> is null</exception>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded for any reason</exception>
        IAssemblyInfo LoadAssemblyFrom(string assemblyFile);
    }
}
