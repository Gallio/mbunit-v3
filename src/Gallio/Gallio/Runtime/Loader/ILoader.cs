// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Provides services to assist with loading tests and dependent resources.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Gets the assembly resolver manager used to resolve referenced assemblies.
        /// </summary>
        IAssemblyResolverManager AssemblyResolverManager { get; }

        /// <summary>
        /// Loads an assembly from the specified file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded</exception>
        Assembly LoadAssemblyFrom(string assemblyFile);
    }
}
