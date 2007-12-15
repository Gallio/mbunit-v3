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
using System.Collections.Generic;
using System.Text;
using Gallio.Model.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A test package describes the resources that have been loaded
    /// in in preparation for enumerating and running the tests they contain.
    /// </summary>
    public sealed class TestPackage
    {
        private readonly TestPackageConfig config;
        private readonly List<IAssemblyInfo> assemblies;

        /// <summary>
        /// Creates a test package with the specified configuration.
        /// </summary>
        /// <param name="config">The package configuration</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="config"/> is null</exception>
        public TestPackage(TestPackageConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            this.config = config;

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
