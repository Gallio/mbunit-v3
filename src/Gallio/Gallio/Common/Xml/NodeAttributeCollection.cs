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
using Gallio.Common.Collections;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// An immutable collection of XML attributes.
    /// </summary>
    public class NodeAttributeCollection : NodeCollection<NodeAttribute>, IDiffableCollection<NodeAttributeCollection, NodeAttribute>
    {
        /// <summary>
        /// An empty collection singleton instance.
        /// </summary>
        public readonly static NodeAttributeCollection Empty = new NodeAttributeCollection(EmptyArray<NodeAttribute>.Instance);

        /// <summary>
        /// Constructs a collection of XML attributes from the specified enumeration.
        /// </summary>
        /// <param name="attributes">An enumeration of attributes.</param>
        public NodeAttributeCollection(IEnumerable<NodeAttribute> attributes)
            : base(attributes)
        {
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeAttributeCollection expected, IXmlPathStrict path, IXmlPathStrict pathExpected, Options options)
        {
            return DiffEngineFactory.ForAttributes(expected, this, path, pathExpected, options).Diff();
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

            return Exists(attribute => attribute.AreNamesEqual(searchedAttributeName, options)
                && ((expectedAttributeValue == null) || attribute.AreValuesEqual(expectedAttributeValue, options)));
        }
    }
}
