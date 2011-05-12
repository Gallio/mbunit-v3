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

namespace Gallio.Common.Xml.Paths
{
    /// <summary>
    /// Concrete node representing an XML attribute node, and that closes a strict XML path.
    /// </summary>
    internal sealed class XmlPathStrictAttribute : XmlPathStrict
    {
        /// <summary>
        /// Constructs a node that extends the specified parent node by targetting an inner attribute by its index.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <param name="index">The index of the inner attribute.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative.</exception>
        public XmlPathStrictAttribute(IXmlPathStrict parent, int index)
            : base(parent, index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (parent.IsEmpty)
                throw new InvalidOperationException("Cannot extend the root node with an attribute.");
        }

        /// <summary>
        /// Formats the current path.
        /// </summary>
        /// <returns>The resulting formatted path.</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return String.Empty;

            return Parent + XmlPathRoot.AttributeSeparator + Index;
        }

        /// <inheritdoc/>
        public override INode FindInParent(INode parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            var element = parent as NodeElement;

            if (element != null)
                return element.Attributes[Index];

            return ((NodeDeclaration)parent).Attributes[Index];
        }
    }
}