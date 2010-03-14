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
    /// Concrete node representing an XML declaration node in a strict path, and which is closed to further extensions.
    /// </summary>
    internal sealed class XmlPathStrictDeclaration : XmlPathStrict
    {
        /// <summary>
        /// Constructs a node that extends the specified root node by targetting the declaration node.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="parent"/> is not the root.</exception>
        public XmlPathStrictDeclaration(IXmlPathStrict parent) 
            : base(parent, -1)
        {
            if (!parent.IsEmpty)
                throw new InvalidOperationException("A declaration node can only extend the root path.");
        }

        /// <summary>
        /// Formats the current path.
        /// </summary>
        /// <returns>The resulting formatted path.</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return String.Empty;

            var o = Parent.ToString();
            var output = new StringBuilder(o);

            if (!o.EndsWith(XmlPathRoot.ElementSeparator))
                output.Append(XmlPathRoot.ElementSeparator);

            output.Append("-1");
            return output.ToString();
        }

        /// <inheritdoc/>
        public override IXmlPathStrict Attribute(int index)
        {
            return new XmlPathStrictAttribute(this, index);
        }
    }
}