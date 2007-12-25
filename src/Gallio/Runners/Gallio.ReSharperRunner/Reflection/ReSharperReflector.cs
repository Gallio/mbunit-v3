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
using Gallio.Reflection;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Abstract base class for ReSharper reflector types.
    /// </summary>
    public abstract class ReSharperReflector : IReflectionPolicy
    {
        private readonly IAssemblyResolver assemblyResolver;

        protected ReSharperReflector(IAssemblyResolver assemblyResolver)
        {
            if (assemblyResolver == null)
                throw new ArgumentNullException("assemblyResolver");

            this.assemblyResolver = assemblyResolver;
        }

        protected internal Assembly ResolveAssembly(IAssemblyInfo assembly)
        {
            try
            {
                Assembly resolvedAssembly = assemblyResolver.ResolveAssembly(assembly);
                return resolvedAssembly;
            }
            catch (Exception ex)
            {
                throw new CodeElementResolveException(assembly, ex);
            }
        }

        /// <inheritdoc />
        public abstract IAssemblyInfo LoadAssembly(AssemblyName assemblyName);
    }
}
