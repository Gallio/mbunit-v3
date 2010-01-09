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
using System.Collections.Generic;
using Gallio.Model;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a description with a test fixture, test method, test parameter
    /// or other test component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The description provides documentation to users when browsing the tests.
    /// However, it is not the only way.  If the test has associated XML documentation
    /// comments and the compiler is generating an XML documentation file for the
    /// test assembly, then MbUnit will automatically import the documentation and
    /// make it available to the user as metadata.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = true, Inherited = true)]
    public class DescriptionAttribute : MetadataPatternAttribute
    {
        private readonly string description;

        /// <summary>
        /// Associates a description with the test component annotated by this attribute.
        /// </summary>
        /// <param name="description">The description to associate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> is null.</exception>
        public DescriptionAttribute(string description)
        {
            if (description == null)
                throw new ArgumentNullException("description");

            this.description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            yield return new KeyValuePair<string, string>(MetadataKeys.Description, description);
        }
    }
}
