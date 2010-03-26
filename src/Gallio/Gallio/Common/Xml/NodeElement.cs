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
using Gallio.Common.Xml;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents an element in an XML fragment.
    /// </summary>
    public class NodeElement : NodeBase, IDiffable<NodeElement>
    {
        private readonly string name;
        private readonly NodeAttributeCollection attributes;

        /// <inheritdoc />
        public override string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the attributes of the element.
        /// </summary>
        public NodeAttributeCollection Attributes
        {
            get
            {
                return attributes;
            }
        }

        /// <summary>
        /// Constructs an XML element.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <param name="count">The total number of nodes at the same level.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="attributes">The attributes of the element.</param>
        /// <param name="children">The child nodes of the element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, <paramref name="attributes"/>, or <paramref name="children"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        public NodeElement(int index, int count, string name, IEnumerable<NodeAttribute> attributes, IEnumerable<INode> children)
            : base(NodeType.Element, index, count, children)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (name == null)
                throw new ArgumentNullException("name");
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            this.name = name;
            this.attributes = new NodeAttributeCollection(attributes);
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, IXmlPathStrict path, Options options)
        {
            var expectedComment = expected as NodeElement;

            if (expectedComment != null)
                return Diff(expectedComment, path, options);

            return new DiffSetBuilder()
                .Add(new Diff(DiffType.UnexpectedElement, path.Element(Index), DiffTargets.Actual))
                .ToDiffSet();
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeElement expected, IXmlPathStrict path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            if (!AreNamesEqual(expected.Name, options))
            {
                return new DiffSetBuilder()
                    .Add(new Diff(DiffType.MismatchedElement, path.Element(Index), DiffTargets.Both))
                    .ToDiffSet();
            }

            return new DiffSetBuilder()
                .Add(attributes.Diff(expected.Attributes, path.Element(Index), options))
                .Add(Children.Diff(expected.Children, path.Element(Index), options))
                .ToDiffSet();
        }

        private static StringComparison GetComparisonTypeForName(Options options)
        {
            return (((options & Options.IgnoreElementsNameCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        /// <inheritdoc />
        public override bool AreNamesEqual(string otherName, Options options)
        {
            return name.Equals(otherName, GetComparisonTypeForName(options));
        }

        /// <inheritdoc />
        public override int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            if (searchedPath == null)
                throw new ArgumentNullException("searchedPath");

            if (searchedPath.IsEmpty)
                return 0;

            IXmlPathLoose[] array = searchedPath.AsArray();
            var head = array[0] as XmlPathLooseOpenElement;

            if (head == null || !AreNamesEqual(head.Name, options))
                return 0;

            IXmlPathLoose trail = searchedPath.Trail();

            if (!trail.IsEmpty)
                return Attributes.CountAt(trail, expectedValue, options) + Children.CountAt(trail, expectedValue, options);

            if (expectedValue == null)
                return 1;

            if (Children.Count != 1)
                return 0;

            var content = Children[0] as NodeContent;
            return ((content != null) && content.AreValuesEqual(expectedValue, options)) ? 1 : 0;
        }
    }
}
