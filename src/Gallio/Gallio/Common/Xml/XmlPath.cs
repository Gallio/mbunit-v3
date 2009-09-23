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
    public class XmlPathClosed : IXmlPathClosed
    {
        private readonly XmlPathOpen parent;
        private readonly string elementName;
        private readonly bool declarative;
        private readonly string attributeName;
        private IList<string> elementNames;

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

        private XmlPathClosed(XmlPathOpen parent, string elementName, bool declarative, string attributeName)
        {
            this.parent = parent;
            this.elementName = elementName;
            this.declarative = declarative;
            this.attributeName = attributeName;
        }

        /// <summary>
        /// Returns an empty path.
        /// </summary>
        public readonly static IXmlPathOpen Empty = new XmlPathOpen();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public static IXmlPathOpen Element(string elementName)
        {
            return Empty.Element(elementName, false);
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

        private class XmlPathOpen : XmlPathClosed, IXmlPathOpen
        {
            public XmlPathOpen()
            {
            }

            private XmlPathOpen(XmlPathOpen parent, string elementName, bool declarative)
                : base(parent, elementName, declarative, null)
            {
            }

            public new IXmlPathOpen Element(string elementName)
            {
                return Element(elementName, false);
            }

            public IXmlPathOpen Element(string elementName, bool declarative)
            {
                if (elementName == null)
                    throw new ArgumentNullException("elementName");
                if (elementName.Length == 0)
                    throw new ArgumentException("The name of an element cannot be an empty string.", "elementName");

                return new XmlPathOpen(this, elementName, declarative);
            }

            public IXmlPathClosed Attribute(string attributeName)
            {
                if (attributeName == null)
                    throw new ArgumentNullException("attributeName");
                if (attributeName.Length == 0)
                    throw new ArgumentException("The name of an attribute cannot be an empty string.", "attributeName");

                return new XmlPathClosed(parent, elementName, declarative, attributeName);
            }
        }
    }
}

