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
using System.Drawing;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies traits for <see cref="ITestKind" />.
    /// </summary>
    public class TestKindTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates test kind traits.
        /// </summary>
        /// <param name="name">The test kind name</param>
        /// <param name="description">The test kind description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="description" /> is null</exception>
        public TestKindTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the name of the test kind.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the test kind.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets or sets the icon for the test kind, or null if none.
        /// </summary>
        /// <remarks>
        /// The icon should be available in a 16x16 size.
        /// </remarks>
        public Icon Icon { get; set; }
    }
}
