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
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// An immutable collection of XML attributes.
    /// </summary>
    public class AttributeCollection : IDiffableCollection<AttributeCollection, Attribute>
    {
        private readonly List<Attribute> attributes;

        /// <summary>
        /// An empty collection singleton instance.
        /// </summary>
        public readonly static AttributeCollection Empty = new AttributeCollection();

        private AttributeCollection()
        {
            this.attributes = new List<Attribute>();
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return attributes.Count;
            }
        }

        /// <inheritdoc />
        public Attribute this[int index]
        {
            get
            {
                return attributes[index];
            }
        }

        /// <summary>
        /// Constructs a collection of XML attributes from the specified enumeration.
        /// </summary>
        /// <param name="attributes">An enumeration of attributes.</param>
        public AttributeCollection(IEnumerable<Attribute> attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            this.attributes = new List<Attribute>(attributes);
        }

        /// <inheritdoc />
        public IEnumerator<Attribute> GetEnumerator()
        {
            return attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var attribute in attributes)
            {
                yield return attribute;
            }
        }

        /// <inheritdoc />
        public DiffSet Diff(AttributeCollection expected, IXmlPathStrict path, Options options)
        {
            return DiffEngineFactory.ForAttributes(expected, this, path, options).Diff();
        }

        /// <inheritdoc />
        public int FindIndex(Predicate<int> predicate)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (predicate(i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the collection contains an attribute with the specified name, and optionally the specified value.
        /// </summary>
        /// <param name="searchedAttributeName">The name of the searched attribute.</param>
        /// <param name="expectedAttributeValue">The expected value found in the searched attribute, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>True is such an attribute exists; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedAttributeName"/> is null.</exception>
        public bool Contains(string searchedAttributeName, string expectedAttributeValue, Options options)
        {
            if (searchedAttributeName == null)
                throw new ArgumentNullException("searchedAttributeName");

            return attributes.Exists(attribute => attribute.AreNamesEqual(searchedAttributeName, options)
                && ((expectedAttributeValue == null) || attribute.AreValuesEqual(expectedAttributeValue, options)));
        }

        /// <summary>
        /// Returns the number of times the searched markup was found at the specified path.
        /// </summary>
        /// <param name="searchedPath">The path of the searched markup.</param>
        /// <param name="expectedValue">The expected value found in the searched markup, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>The number of matching items found; zero if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedPath"/> is null.</exception>
        public int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            int count = 0;

            foreach (Attribute attribute in attributes)
            {
                count += attribute.CountAt(searchedPath, expectedValue, options);
            }

            return count;
        }
    }
}
