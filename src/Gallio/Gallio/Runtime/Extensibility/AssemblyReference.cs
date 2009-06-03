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
using System.Reflection;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Describes an assembly referenced by a plugin.
    /// </summary>
    public class AssemblyReference
    {
        private readonly AssemblyName assemblyName;
        private readonly Uri codeBase;

        /// <summary>
        /// Creates an assembly reference object.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="codeBase">The assembly codebase, or null if unknown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/>
        /// is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="codeBase"/>
        /// is not null and is not an absolute Uri.</exception>
        public AssemblyReference(AssemblyName assemblyName, Uri codeBase)
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");
            if (codeBase != null && !codeBase.IsAbsoluteUri)
                throw new ArgumentException("CodeBase must be an absolute Uri.", "codeBase");

            this.assemblyName = assemblyName;
            this.codeBase = codeBase;
        }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        /// <summary>
        /// Gets the assembly codebase, or null if unknown.
        /// </summary>
        public Uri CodeBase
        {
            get { return codeBase; }
        }
    }
}
