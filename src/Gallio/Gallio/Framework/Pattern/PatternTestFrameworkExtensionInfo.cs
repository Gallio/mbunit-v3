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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides information about a particular pattern test framework extension.
    /// </summary>
    public sealed class PatternTestFrameworkExtensionInfo
    {
        private readonly string id;
        private readonly string name;

        /// <summary>
        /// Creates an information object.
        /// </summary>
        /// <param name="id">The unique id of the extension</param>
        /// <param name="name">The display name of the extension</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/>
        /// is null</exception>
        public PatternTestFrameworkExtensionInfo(string id, string name)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (name == null)
                throw new ArgumentNullException("name");

            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Gets the unique id of the extension.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the display name of the extension.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the extension's test kind which is used to decorate the test
        /// framework test when a test assembly only contains tests that belong to one
        /// particular framework.  May be null if none.
        /// </summary>
        public string FrameworkKind { get; set; }
    }
}
