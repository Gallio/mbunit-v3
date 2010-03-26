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
using Gallio.Common.Collections;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents a textual content inside an element
    /// </summary>
    public class NodeContent : NodeBase, IDiffable<NodeContent>
    {
        private readonly string text;

        /// <summary>
        /// Gets the textual content of the comment tag.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
        }

        /// <summary>
        /// Constructs textual content inside an element.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <param name="count">The total number of nodes at the same level.</param>
        /// <param name="text">The literal text content.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public NodeContent(int index, int count, string text)
            : base(NodeType.Content, index, count, EmptyArray<INode>.Instance)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (text == null)
                throw new ArgumentNullException("text");
            
            this.text = text;
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, IXmlPathStrict path, Options options)
        {
            var expectedContent = expected as NodeContent;

            if (expectedContent != null)
                return Diff(expectedContent, path, options);

            return new DiffSetBuilder()
                .Add(new Diff(DiffType.UnexpectedContent, path.Element(Index), DiffTargets.Actual))
                .ToDiffSet();
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeContent expected, IXmlPathStrict path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            if (AreValuesEqual(expected.Text, options))
            {
                return DiffSet.Empty;
            }

            return new DiffSetBuilder()
                .Add(new Diff(DiffType.MismatchedContent, path.Element(Index), DiffTargets.Both))
                .ToDiffSet();
        }

        private static StringComparison GetComparisonType(Options options)
        {
            return (((options & Options.IgnoreElementsValueCase) != 0)
                ? StringComparison.CurrentCultureIgnoreCase
                : StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Determines whether the text value is equal to the specified text value, 
        /// by respecting the specified equality options.
        /// </summary>
        /// <param name="otherText">The text value to compare.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>True if the values are equal; false otherwise.</returns>
        public bool AreValuesEqual(string otherText, Options options)
        {
            return Text.Equals(otherText, GetComparisonType(options));
        }
    }
}
