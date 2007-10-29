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
using MbUnit.Attributes;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates the name of the type under test with a test fixture, test method,
    /// test parameter or other test component.  The type under test helps to describe
    /// which type is primarily being exercised by the test so that we can quickly
    /// identify which tests to run after making changes to a given type.
    /// </summary>
    /// <remarks>
    /// This attribute can be repeated multiple times if there are multiple types.
    /// </remarks>
    public class TestsOnAttribute : MetadataPatternAttribute
    {
        private string typeName;

        /// <summary>
        /// Associates the type under test with the test component annotated by this attribute.
        /// </summary>
        /// <param name="type">The type under test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public TestsOnAttribute(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.typeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Associates the full name or assembly qualified name of the type under test
        /// with the test component annotated by this attribute.
        /// </summary>
        /// <param name="typeName">The name of the type under test as obtained by <see cref="Type.FullName" />
        /// or <see cref="Type.AssemblyQualifiedName" /></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="typeName"/> is null</exception>
        public TestsOnAttribute(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");

            this.typeName = typeName;
        }

        /// <summary>
        /// Gets or sets the full name or assembly qualified name of the type under test.
        /// </summary>
        /// <value>
        /// The name of the type under test as obtained by <see cref="Type.FullName" />
        /// or <see cref="Type.AssemblyQualifiedName" />.
        /// </value>
        public string TypeName
        {
            get { return typeName; }
        }

        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, ITemplateComponent component)
        {
            component.Metadata.Add(MetadataKeys.TestsOn, typeName);
        }
    }
}