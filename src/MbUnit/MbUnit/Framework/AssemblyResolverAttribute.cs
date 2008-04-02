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
using Gallio.Framework.Pattern;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Registers a custom assembly resolver.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestAssembly, AllowMultiple=true)]
    public sealed class AssemblyResolverAttribute : TestAssemblyInitializationAttribute
    {
        private readonly Type assemblyResolverType;

        /// <summary>
        /// Registers a custom assembly resolver.
        /// </summary>
        /// <param name="assemblyResolverType">The assembly resolver type, must
        /// implement <see cref="IAssemblyResolver" /></param>
        public AssemblyResolverAttribute(Type assemblyResolverType)
        {
            if (assemblyResolverType == null)
                throw new ArgumentNullException("assemblyResolverType");
            if (!typeof(IAssemblyResolver).IsAssignableFrom(assemblyResolverType))
                throw new ArgumentException("The assembly resolver type must be assignable from " + typeof(IAssemblyResolver).Name, "assemblyResolverType");

            this.assemblyResolverType = assemblyResolverType;
        }

        /// <summary>
        /// Gets the assembly resolver type.
        /// </summary>
        public Type AssemblyResolverType
        {
            get { return assemblyResolverType; }
        }

        /// <inheritdoc />
        public override void Initialize(PatternEvaluationScope frameworkScope, IAssemblyInfo assembly)
        {
            try
            {
                IAssemblyResolver resolver = (IAssemblyResolver)Activator.CreateInstance(assemblyResolverType);
                Loader.AssemblyResolverManager.AddAssemblyResolver(resolver);
            }
            catch (Exception ex)
            {
                throw new PatternUsageErrorException(String.Format("Failed to create custom assembly resolver type '{0}'.", assemblyResolverType), ex);
            }
        }
    }
}
