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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides information about a tool that extends the <see cref="PatternTestFramework" />.
    /// </summary>
    /// <seealso cref="IPatternTestFrameworkExtension"/>
    public sealed class ToolInfo
    {
        private readonly string id;
        private readonly string name;

        /// <summary>
        /// Creates a tool information object.
        /// </summary>
        /// <param name="id">The unique id of the tool</param>
        /// <param name="name">The display name of the tool</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/>
        /// is null</exception>
        public ToolInfo(string id, string name)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (name == null)
                throw new ArgumentNullException("name");

            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Gets the unique id of the tool.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the display name of the tool.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
    }
}
