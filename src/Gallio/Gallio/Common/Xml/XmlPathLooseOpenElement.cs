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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Concrete node representing an XML element markup in a loose path, and which is opened for further extensions.
    /// </summary>
    internal sealed class XmlPathLooseOpenElement : XmlPathLooseOpen
    {
        private readonly string name;

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the instance representing an empty root path.
        /// </summary>
        internal static readonly IXmlPathLooseOpen Root = new XmlPathLooseOpenElement();

        private XmlPathLooseOpenElement()
        {
        }

        /// <summary>
        /// Constructs a node that extends the specified parent node by targetting an child element by its name.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <param name="name">The name of the inner element.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> or <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        public XmlPathLooseOpenElement(IXmlPathLooseOpen parent, string name) 
            : base(parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Cannot be empty.", "name");

            this.name = name;
        }

        /// <inheritdoc/>
        public override IXmlPathLooseOpen Element(string name)
        {
            return new XmlPathLooseOpenElement(this, name);
        }

        /// <inheritdoc/>
        public override IXmlPathLooseClosed Attribute(string name)
        {
            return new XmlPathLooseClosedAttribute(this, name);
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

            output.Append(name);
            return output.ToString();
        }

        /// <inheritdoc/>
        public override string ToString(Fragment fragment)
        {
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IXmlPathLoose Copy(IXmlPathLoose parent)
        {
            return new XmlPathLooseOpenElement((IXmlPathLooseOpen)parent, name);
        }
    }
}