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
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestComponent" />.
    /// </summary>
    public abstract class BaseTestComponent : ITestComponent
    {
        private string name;
        private ICodeElementInfo codeElement;
        private readonly MetadataMap metadata;

        /// <summary>
        /// Initializes a test component.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition of the component, or null if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public BaseTestComponent(string name, ICodeElementInfo codeElement)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.name = name;
            this.codeElement = codeElement;

            metadata = new MetadataMap();
        }

        /// <inheritdoc />
        public abstract string Id { get; }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                name = value;
            }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                codeElement = value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return name;
        }
    }
}
