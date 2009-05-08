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
using Gallio.Runtime.Loader;
using Gallio.Common.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A test package describes the resources that have been loaded
    /// in in preparation for enumerating and running the tests they contain.
    /// </summary>
    public sealed class TestPackage
    {
        private readonly TestPackageConfig config;
        private readonly IReflectionPolicy reflectionPolicy;
        private readonly ILoader loader;
        private readonly List<IAssemblyInfo> assemblies;

        /// <summary>
        /// Creates a test package with the specified configuration.
        /// </summary>
        /// <param name="config">The package configuration</param>
        /// <param name="reflectionPolicy">The reflection policy for the package</param>
        /// <param name="loader">The loader for the package</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="config"/>,
        /// <paramref name="reflectionPolicy"/> or <paramref name="loader"/> is null</exception>
        public TestPackage(TestPackageConfig config, IReflectionPolicy reflectionPolicy,
            ILoader loader)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");
            if (loader == null)
                throw new ArgumentNullException("loader");

            this.config = config;
            this.reflectionPolicy = reflectionPolicy;
            this.loader = loader;

            assemblies = new List<IAssemblyInfo>();
        }

        /// <summary>
        /// Gets the test package configuration.
        /// </summary>
        public TestPackageConfig Config
        {
            get { return config; }
        }

        /// <summary>
        /// Gets the reflection policy.
        /// </summary>
        public IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        /// <summary>
        /// Gets the loader for the package.
        /// </summary>
        public ILoader Loader
        {
            get { return loader; }
        }

        /// <summary>
        /// Gets the assemblies that belong to the test package.
        /// </summary>
        public IList<IAssemblyInfo> Assemblies
        {
            get { return assemblies; }
        }

        /// <summary>
        /// Adds an assembly to the test package.
        /// </summary>
        /// <param name="assembly">The assembly to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public void AddAssembly(IAssemblyInfo assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            assemblies.Add(assembly);
        }
    }
}
