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
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents a single attribute in an XML element.
    /// </summary>
    public class NodeAttribute : IDiffable<NodeAttribute>, INode
    {
        private readonly int index;
        private readonly string name;
        private readonly string value;
        private readonly int count;

        /// <summary>
        /// Gets the index of the attribute.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the value of the attribute.
        /// </summary>
        public string Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Constructs an attribute.
        /// </summary>
        /// <param name="index">The index of the attribute.</param>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="count">The number of attributes in the parent collection.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative.</exception>
        public NodeAttribute(int index, string name, string value, int count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to zero.");
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Must be greater than or equal to zero.");
            if (count <= index)
                throw new ArgumentOutOfRangeException("count", "Must be greater than the index.");

            this.index = index;
            this.name = name;
            this.value = value;
            this.count = count;
        }

        /// <inheritdoc />
        public DiffSet Diff(INode expected, IXmlPathStrict path, Options options)
        {
            return Diff((NodeAttribute) expected, path, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeAttribute expected, IXmlPathStrict path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            var builder = new DiffSetBuilder();

            if (!AreNamesEqual(expected.Name, options))
            {
                builder.Add(new Diff("Unexpected attribute found.", path.Attribute(index), DiffTargets.Actual));
            }
            else if (!value.Equals(expected.Value, GetComparisonTypeForValue(options)))
            {
                builder.Add(new Diff("Unexpected attribute value found.", path.Attribute(index), DiffTargets.Both));
            }

            return builder.ToDiffSet();
        }

        private static StringComparison GetComparisonTypeForName(Options options)
        {
            return (((options & Options.IgnoreAttributesNameCase) != 0) 
                ? StringComparison.CurrentCultureIgnoreCase 
                : StringComparison.CurrentCulture);
        }

        private static StringComparison GetComparisonTypeForValue(Options options)
        {
            return (((options & Options.IgnoreAttributesValueCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        /// <inheritdoc />
        public bool AreNamesEqual(string otherName, Options options)
        {
            return name.Equals(otherName, GetComparisonTypeForName(options));
        }

        /// <summary>
        /// Determines whether the value of the current attribute is equal to the specified value, 
        /// by respecting the specified equality options.
        /// </summary>
        /// <param name="otherValue">The value to compare.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>True if the values are equal; false otherwise.</returns>
        public bool AreValuesEqual(string otherValue, Options options)
        {
            return Value.Equals(otherValue, GetComparisonTypeForValue(options));
        }

        /// <inheritdoc />
        public NodeCollection Children
        {
            get
            {
                return NodeCollection.Empty;
            }
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public void Aggregate(XmlPathFormatAggregator aggregator)
        {
            aggregator.HangAttribute(name, value, index == 0, index == count - 1);
        }

        /// <inheritdoc />
        public int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            if (searchedPath == null)
                throw new ArgumentNullException("searchedPath");

            if (searchedPath.IsEmpty)
                return 0;

            IXmlPathLoose[] array = searchedPath.AsArray();
            var head = array[0] as XmlPathLooseClosedAttribute;

            if (head == null || 
                !AreNamesEqual(head.Name, options) ||
                ((expectedValue != null) && !AreValuesEqual(expectedValue, options)))
                return 0;

            return 1;
        }
    }
}
