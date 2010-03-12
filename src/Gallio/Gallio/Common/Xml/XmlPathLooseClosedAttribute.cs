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
    /// Concrete node representing an XML attribute markup, and that closes a loose XML path.
    /// </summary>
    internal sealed class XmlPathLooseClosedAttribute : XmlPathLooseClosed
    {
        private readonly string name;

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Constructs a node that extends the specified parent node by targetting an inner attribute by its name.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <param name="name">The name of the inner attribute.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> or <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        public XmlPathLooseClosedAttribute(IXmlPathLooseClosed parent, string name) 
            : base(parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Cannot be empty.", "name");

            this.name = name;
        }
    
        /// <summary>
        /// Formats the current path.
        /// </summary>
        /// <returns>The resulting formatted path.</returns>
        public override string ToString()
        {
            if (IsEmpty)
                return String.Empty;

            return Parent + XmlPathRoot.AttributeSeparator + name;
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
            return new XmlPathLooseClosedAttribute((IXmlPathLooseClosed)parent, name);
        }
    }
}