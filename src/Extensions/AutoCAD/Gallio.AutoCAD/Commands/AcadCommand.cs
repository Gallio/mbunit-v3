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

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// The base class for commands that can be sent to the AutoCAD process.
    /// </summary>
    public abstract class AcadCommand
    {
        /// <summary>
        /// Initializes a new <see cref="AcadCommand"/> object.
        /// </summary>
        /// <param name="globalName">The global name for the command.</param>
        protected AcadCommand(string globalName)
        {
            if (String.IsNullOrEmpty(globalName))
                throw new ArgumentException("Must not be null or empty.", "globalName");
            GlobalName = globalName;
        }

        /// <summary>
        /// Gets the command's arguments.
        /// </summary>
        public IEnumerable<string> GetArguments()
        {
            var arguments = GetArgumentsImpl();
            if (arguments == null)
                throw new InvalidOperationException("Unable to get arguments.");

            return arguments;
        }

        /// <summary>
        /// Gets the command's arguments.
        /// </summary>
        /// <remarks>
        /// This method must not return null.
        /// </remarks>
        protected abstract IEnumerable<string> GetArgumentsImpl();

        /// <summary>
        /// Gets the global name for this command.
        /// </summary>
        public string GlobalName
        { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return GlobalName;
        }
    }
}
