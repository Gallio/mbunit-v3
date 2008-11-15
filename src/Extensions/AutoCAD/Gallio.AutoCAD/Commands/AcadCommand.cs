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
using System.Collections.Generic;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// The base class for commands that can be sent to the AutoCAD process.
    /// </summary>
    internal abstract class AcadCommand
    {
        private readonly string globalName;

        /// <summary>
        /// Initializes a new <see cref="AcadCommand"/> object.
        /// </summary>
        /// <param name="globalName">The global name for the command.</param>
        protected AcadCommand(string globalName)
        {
            if (String.IsNullOrEmpty(globalName))
                throw new ArgumentException("Must not be null or empty.", "globalName");
            this.globalName = globalName;
        }

        /// <summary>Gets the command's arguments.</summary>
        public abstract IEnumerable<string> Arguments
        {
            get;
        }

        /// <summary>Gets the global name for this command.</summary>
        public string GlobalName
        {
            get { return globalName; }
        }

        /// <summary><c>true</c> if this command should be sent asynchonously; otherwise, <c>false</c>.</summary>
        public bool SendAsynchronously
        {
            get;
            set;
        }
    }
}
