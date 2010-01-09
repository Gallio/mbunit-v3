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
    /// Associates a category name with a test fixture, test method, test parameter
    /// or other test component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The category name can be used to classify tests and build test suites of related tests.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = true, Inherited = true)]
    public class CategoryAttribute : MetadataPatternAttribute
    {
        private readonly string category;

        /// <summary>
        /// Associates a category with the test component annotated by this attribute.
        /// </summary>
        /// <param name="category">The name of the category to associate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="category"/> is null.</exception>
        public CategoryAttribute(string category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            this.category = category;
        }

        /// <summary>
        /// Gets the name of the category.
        /// </summary>
        public string Category
        {
            get { return category; }
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            yield return new KeyValuePair<string, string>(MetadataKeys.Category, category);
        }
    }
}