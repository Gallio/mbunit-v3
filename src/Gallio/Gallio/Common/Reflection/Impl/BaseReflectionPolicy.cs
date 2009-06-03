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

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A base reflection policy implementation that performs argument validation.
    /// </summary>
    public abstract class BaseReflectionPolicy : IReflectionPolicy
    {
        /// <inheritdoc />
        public IAssemblyInfo LoadAssembly(AssemblyName assemblyName)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            return LoadAssemblyImpl(assemblyName);
        }

        /// <inheritdoc />
        public IAssemblyInfo LoadAssemblyFrom(string assemblyFile)
        {
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            return LoadAssemblyFromImpl(assemblyFile);
        }

        /// <summary>
        /// Loads an assembly.
        /// </summary>
        /// <param name="assemblyName">The assembly name, not null.</param>
        /// <returns>The loaded assembly wrapper.</returns>
        /// <exception cref="Exception">Any exception may be thrown if the loading fails.</exception>
        protected abstract IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName);

        /// <summary>
        /// Loads an assembly from a file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file path, not null.</param>
        /// <returns>The loaded assembly wrapper.</returns>
        /// <exception cref="Exception">Any exception may be thrown if the loading fails.</exception>
        protected abstract IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile);
    }
}
