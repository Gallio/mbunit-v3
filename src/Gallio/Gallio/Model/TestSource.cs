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
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies a list of sources that are to be explored for tests.
    /// </summary>
    public class TestSource
    {
        private readonly List<IAssemblyInfo> assemblies;
        private readonly List<ITypeInfo> types;
        private readonly List<FileInfo> files;

        /// <summary>
        /// Creates an empty test source.
        /// </summary>
        public TestSource()
        {
            assemblies = new List<IAssemblyInfo>();
            types = new List<ITypeInfo>();
            files = new List<FileInfo>();
        }

        /// <summary>
        /// Gets the read-only list of assemblies to explore.
        /// </summary>
        public IList<IAssemblyInfo> Assemblies
        {
            get { return new ReadOnlyCollection<IAssemblyInfo>(assemblies); }
        }

        /// <summary>
        /// Gets the read-only list of types to explore.
        /// </summary>
        public IList<ITypeInfo> Types
        {
            get { return new ReadOnlyCollection<ITypeInfo>(types); }
        }

        /// <summary>
        /// Gets the read-only list of files to explore.
        /// </summary>
        public IList<FileInfo> Files
        {
            get { return new ReadOnlyCollection<FileInfo>(files); }
        }

        /// <summary>
        /// Adds an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public void AddAssembly(IAssemblyInfo assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            assemblies.Add(assembly);
        }

        /// <summary>
        /// Adds a type.
        /// </summary>
        /// <param name="type">The type to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public void AddType(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            types.Add(type);
        }

        /// <summary>
        /// Adds a file.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="file"/> is null.</exception>
        public void AddFile(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException("path");

            files.Add(file);
        }
    }
}
