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
using System.IO;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Maps to the <c>NETLOAD</c> command.
    /// </summary>
    public class NetLoadCommand : AcadCommand
    {
        private readonly string assemblyPath;

        /// <summary>
        /// Initializes a new <see cref="NetLoadCommand"/> object.
        /// </summary>
        public NetLoadCommand(string assemblyPath)
            : base("NETLOAD")
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");
            if (assemblyPath.Length == 0)
                throw new ArgumentException("String can't be empty.", "assemblyPath");
            if (assemblyPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException("Path contains invalid characters.", "assemblyPath");

            this.assemblyPath = assemblyPath;
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetArgumentsImpl()
        {
            yield return AssemblyPath;
        }

        /// <summary>
        /// Gets or sets the path to the assembly to load.
        /// </summary>
        public string AssemblyPath
        {
            get { return assemblyPath; }
        }
    }
}
