// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Filters;

namespace Gallio.Model.Tree
{
    /// <summary>
    /// Common interface for elements in the test object model.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All components have a name for presentation, metadata for
    /// annotations, and a code reference to its point of definition. 
    /// </para>
    /// </remarks>
    public abstract class TestComponent : ITestDescriptor
    {
        private string name;
        private ICodeElementInfo codeElement;
        private readonly PropertyBag metadata;

        /// <summary>
        /// Initializes a test component.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="codeElement">The point of definition of the component, or null if unknown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public TestComponent(string name, ICodeElementInfo codeElement)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.name = name;
            this.codeElement = codeElement;

            metadata = new PropertyBag();
        }

        /// <summary>
        /// Gets or sets the stable unique identifier of the component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identifier must be unique across all components
        /// within a given test project.  It should also be stable so that the
        /// identifier remains valid across recompilations and code changes that
        /// do not alter the underlying declarations (insofar as is possible).
        /// </para>
        /// <para>
        /// The identifier does not refer to a specific instance of <see cref="TestComponent" />,
        /// but rather incorporates enough information so that we can unambiguously find a
        /// corresponding instance in a model that has been populated.  When we rebuild
        /// the model, assuming the code hasn't changed too much, the objects in the model
        /// will have the same identifier as before.  This allows the identifier
        /// to be saved in project files to construct lists of components.  We can also use
        /// it to refer to components remotely.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public abstract string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The name does not need to be globally unique.
        /// </para>
        /// </remarks>
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
        /// metadata entry.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method.
        /// </para>
        /// </remarks>
        /// <value>
        /// One of the <see cref="TestKinds" /> constants.
        /// </value>
        public string Kind
        {
            get { return Metadata.GetValue(MetadataKeys.TestKind); }
            set { Metadata.SetValue(MetadataKeys.TestKind, value); }
        }

        /// <summary>
        /// Gets the metadata of the component.
        /// </summary>
        /// <seealso cref="MetadataKeys"/>
        public PropertyBag Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// Gets or sets a reference to the point of definition of this test
        /// component in the code, or null if unknown.
        /// </summary>
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