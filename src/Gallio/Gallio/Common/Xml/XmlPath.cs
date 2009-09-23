// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents the immutable location of an element or an attribute in an XML fragment. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The path is closed (i.e. it cannot be extended to the location of a child element.)
    /// </para>
    /// </remarks>
    public class XmlPathClosed : IXmlPathClosed
    {
        private readonly XmlPathOpen parent;
        private readonly string elementName;
        private readonly bool declarative;
        private readonly string attributeName;
        private IList<string> elementNames;

        /// <summary>
        /// Gets the path of the parent element.
        /// </summary>
        protected XmlPathOpen Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Gets the name of the element at the current depth.
        /// </summary>
        protected string ElementName
        {
            get
            {
                return elementName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the element at the current depth
        /// is a declarative element in the xml fragment.
        /// </summary>
        protected bool Declarative
        {
            get
            {
                return declarative;
            }
        }

        /// <inheritdoc />
        internal string AttributeName
        {
            get
            {
                return attributeName;
            }
        }

        /// <inheritdoc />
        internal bool IsEmpty
        {
            get
            {
                return parent == null;
            }
        }

        /// <inheritdoc />
        internal IList<string> ElementNames
        {
            get
            {
                if (elementNames == null)
                {
                    XmlPathOpen current = parent;
                    var list = new List<string>();

                    if (elementName != null)
                        list.Add(elementName);

                    while (!current.IsEmpty)
                    {
                        list.Add(current.elementName);
                        current = current.parent;
                    }

                    list.Reverse();
                    elementNames = new ReadOnlyCollection<string>(list);
                }

                return elementNames;
            }
        }

        /// <summary>
        /// Constructs an empty path.
        /// </summary>
        protected XmlPathClosed()
        {
        }

        /// <summary>
        /// Constructs a path with the specified parameters.
        /// </summary>
        /// <param name="parent">The path of the extended parent element.</param>
        /// <param name="elementName">The name of the element at the current depth.</param>
        /// <param name="declarative">Indicates whether the element at the current depth is a declarative element.</param>
        /// <param name="attributeName">The name of the attribute at the current depth, or null.</param>
        protected internal XmlPathClosed(XmlPathOpen parent, string elementName, bool declarative, string attributeName)
        {
            this.parent = parent;
            this.elementName = elementName;
            this.declarative = declarative;
            this.attributeName = attributeName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var output = new StringBuilder();

            if (!IsEmpty)
            {
                if (parent != null && !parent.IsEmpty)
                    output.Append(parent.ToString());

                if (attributeName != null)
                {
                    output.AppendFormat("<{0}{1} {2}='...'{3}>", declarative ? "?" : String.Empty, elementName, attributeName, declarative ? " ?" : String.Empty);
                }
                else
                {
                    output.AppendFormat("<{0}{1}{0}>", declarative ? "?" : String.Empty, elementName);
                }
            }

            return output.ToString();
        }
    }

    /// <summary>
    /// Represents the immutable location of an element or an attribute in an XML fragment. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The path is open (i.e. it can extended to the location of a child element or an inner attribute.)
    /// </para>
    /// </remarks>
    public class XmlPathOpen : XmlPathClosed, IXmlPathOpen
    {
        /// <summary>
        /// Returns an empty open path you can extend to child elements.
        /// </summary>
        internal readonly static IXmlPathOpen Empty = new XmlPathOpen();

        private XmlPathOpen()
        {
        }

        private XmlPathOpen(XmlPathOpen parent, string elementName, bool declarative)
            : base(parent, elementName, declarative, null)
        {
        }

        /// <summary>
        /// Extends the path to the specified child element.
        /// </summary>
        /// <param name="elementName">The name of the child element.</param>
        /// <returns>A new open instance of the path.</returns>
        public IXmlPathOpen Element(string elementName)
        {
            return Element(elementName, false);
        }

        /// <summary>
        /// Extends the path to the specified child element.
        /// </summary>
        /// <param name="elementName">The name of the child element.</param>
        /// <param name="declarative">A value indicating whether the child element is declarative.</param>
        /// <returns>A new open instance of the path.</returns>
        public IXmlPathOpen Element(string elementName, bool declarative)
        {
            if (elementName == null)
                throw new ArgumentNullException("elementName");
            if (elementName.Length == 0)
                throw new ArgumentException("The name of an element cannot be an empty string.", "elementName");

            return new XmlPathOpen(this, elementName, declarative);
        }

        /// <summary>
        /// Extends the path to the specified attribute located in the element of the current depth.
        /// </summary>
        /// <param name="attributeName">The name of the inner attribute.</param>
        /// <returns>A new closed instance of the path.</returns>
        public IXmlPathClosed Attribute(string attributeName)
        {
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");
            if (attributeName.Length == 0)
                throw new ArgumentException("The name of an attribute cannot be an empty string.", "attributeName");

            return new XmlPathClosed(Parent, ElementName, Declarative, attributeName);
        }
    }

    /// <summary>
    /// An entry-point for defining XML pathes.
    /// </summary>
    public static class XmlPathRoot
    {
        /// <summary>
        /// Returns an empty open path you can extend to child elements.
        /// </summary>
        public readonly static IXmlPathOpen Empty = XmlPathOpen.Empty;

        /// <summary>
        /// Returns a new path initialized with the specified root element name.
        /// </summary>
        /// <param name="rootElementName">The name of the root element in the XML fragment.</param>
        /// <returns>The open path to the root element.</returns>
        public static IXmlPathOpen Element(string rootElementName)
        {
            return XmlPathOpen.Empty.Element(rootElementName, false);
        }
    }
}

