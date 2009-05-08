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
using Gallio.Common.Collections;
using Gallio.Common.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestComponent" />.
    /// </summary>
    public abstract class BaseTestComponent : ITestComponent
    {
        private string name;
        private ICodeElementInfo codeElement;
        private readonly PropertyBag metadata;

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

            metadata = new PropertyBag();
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

        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataKeys.TestKind" />
        /// metadata entry.  (This is a convenience method.)
        /// </summary>
        /// <value>
        /// One of the <see cref="TestKinds" /> constants.
        /// </value>
        public string Kind
        {
            get { return Metadata.GetValue(MetadataKeys.TestKind); }
            set { Metadata.SetValue(MetadataKeys.TestKind, value); }
        }

        /// <inheritdoc />
        public PropertyBag Metadata
        {
            get { return metadata; }
        }

        /// <inheritdoc />
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
            set { codeElement = value; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return name;
        }
    }
}
