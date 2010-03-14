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
using System.Text;

namespace Gallio.Common.Xml.Paths
{
    /// <summary>
    /// Concrete node representing an XML element node in a strict path, and which is opened for further extensions.
    /// </summary>
    internal sealed class XmlPathStrictElement : XmlPathStrict
    {
        /// <summary>
        /// Gets the instance representing an empty root path.
        /// </summary>
        internal static readonly IXmlPathStrict Root = new XmlPathStrictElement();

        private XmlPathStrictElement()
        {
        }

        /// <summary>
        /// Constructs a node that extends the specified parent node by targetting an child element by its index.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <param name="index">The index of the child element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative.</exception>
        public XmlPathStrictElement(IXmlPathStrict parent, int index) 
            : base(parent, index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
        }

        /// <inheritdoc/>
        public override IXmlPathStrict Element(int index)
        {
            return new XmlPathStrictElement(this, index);
        }

        /// <inheritdoc/>
        public override IXmlPathStrict Attribute(int index)
        {
            return new XmlPathStrictAttribute(this, index);
        }

        /// <inheritdoc/>
        public override IXmlPathStrict Declaration()
        {
            return new XmlPathStrictDeclaration(this);
        }

        /// <summary>
        /// Formats the current path.
        /// </summary>
        /// <returns>The resulting formatted path.</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return XmlPathRoot.ElementSeparator;

            var o = Parent.ToString();
            var output = new StringBuilder(o);

            if (!o.EndsWith(XmlPathRoot.ElementSeparator))
                output.Append(XmlPathRoot.ElementSeparator);

            output.Append(Index);
            return output.ToString();
        }
    }
}