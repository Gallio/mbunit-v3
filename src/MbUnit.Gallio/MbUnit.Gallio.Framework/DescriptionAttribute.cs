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
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Model;
using MbUnit.Framework.Core.Attributes;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a description with a test fixture, test method, test parameter
    /// or other test component.  The description provides useful documentation to
    /// users when browsing the tests.
    /// </summary>
    public class DescriptionAttribute : MetadataPatternAttribute
    {
        private string description;

        /// <summary>
        /// Associates a description with the test component annotated by this attribute.
        /// </summary>
        /// <param name="description">The description to associate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> is null</exception>
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
        public override void Apply(TestTemplateTreeBuilder builder, ITestComponent component)
        {
            component.Metadata.Entries.Add(MetadataConstants.DescriptionKey, description);
        }
    }
}
