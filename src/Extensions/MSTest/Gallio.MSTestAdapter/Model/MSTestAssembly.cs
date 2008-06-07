// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;

namespace Gallio.MSTestAdapter.Model
{
    internal class MSTestAssembly : MSTest
    {
        /// <summary>
        /// Creates an object to represent an MSTest assembly.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public MSTestAssembly(string name, ICodeElementInfo codeElement) 
            : base(name, codeElement)
        {
        }

        /// <summary>
        /// Gets the path of the assembly.
        /// </summary>
        public string AssemblyFilePath
        {
            get { return ((IAssemblyInfo)CodeElement).Path; }
        }
    }
}
