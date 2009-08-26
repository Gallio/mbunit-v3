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
using System.Xml;
using System.IO;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// A parsing service that extracts a composite tree from an XML fragment.
    /// </summary>
    public class Parser
    {
        // TODO: Support "CDATA" and "Comment" nodes.
        // TODO: Possibly support "Entity" and "Notation" nodes.

        private readonly string xml;

        /// <summary>
        /// Constructs the parser.
        /// </summary>
        /// <param name="xml">The XML fragment to parse.</param>
        public Parser(string xml)
        {
            if (xml == null)
                throw new ArgumentNullException("xml");

            this.xml = xml;
        }

        /// <summary>
        /// Parses the XML fragment.
        /// </summary>
        /// <returns>The resulting document representing the parsed XML fragment.</returns>
        public Document Run()
        {
            INode root = Null.Instance;
            var declarationAttributes = AttributeCollection.Empty;

            using (var reader = XmlTextReader.Create(new StringReader(xml)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            root = ParseElement(reader);
                            break;

                        case XmlNodeType.XmlDeclaration:
                            declarationAttributes = GetAttributes(reader);
                            break;

                        default:
                            break;
                    }
                }
            }

            return new Document(new Declaration(declarationAttributes), root);
        }

        private Element ParseElement(XmlReader reader)
        {
            string name = reader.Name;
            string value = String.Empty;
            var children = new List<Element>();
            AttributeCollection attributes = GetAttributes(reader);

            if (!reader.IsEmptyElement)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            return new Element(ToXmlTree(children), name, value, attributes);

                        case XmlNodeType.Text:
                            value = reader.Value;
                            break;

                        case XmlNodeType.Element:
                            children.Add(ParseElement(reader));
                            break;

                        default:
                            break;
                    }
                }
            }

            return new Element(ToXmlTree(children), name, value, attributes);
        }

        private static AttributeCollection GetAttributes(XmlReader reader)
        {
            var attributes = new List<Attribute>();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    attributes.Add(new Attribute(reader.Name, reader.Value));

                } while (reader.MoveToNextAttribute());
            }

            reader.MoveToElement();
            return new AttributeCollection(attributes);
        }

        private static INode ToXmlTree(IList<Element> elements)
        {
            return elements.Count == 0
                ? Null.Instance
                : (INode)new ElementCollection(elements);
        }
    }
}
