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
using System.Xml;
using System.IO;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// A parsing service that extracts a composite tree from an XML fragment.
    /// </summary>
    public class Parser
    {
        private readonly string xml;

        /// <summary>
        /// Parses the specified XML fragment.
        /// </summary>
        /// <param name="xml">The XML fragment to parse.</param>
        /// <param name="options">Parsing options.</param>
        /// <returns>The resulting document representing the parsed XML fragment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null.</exception>
        public static NodeFragment Run(string xml, Options options)
        {
            var parser = new Parser(xml);
            return parser.RunImpl(options);
        }

        private Parser(string xml)
        {
            if (xml == null)
                throw new ArgumentNullException("xml");

            this.xml = xml;
        }

        private NodeFragment RunImpl(Options options)
        {
            INode root = null;
            var declarationAttributes = NodeAttributeCollection.Empty;
            var settings = new XmlReaderSettings()
            {
                IgnoreComments = ((options & Options.IgnoreComments) != 0),
                IgnoreWhitespace = true
            };

            using (var reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            root = ParseElement(0, reader);
                            break;

                        case XmlNodeType.XmlDeclaration:
                            declarationAttributes = new NodeAttributeCollection(GetAttributes(reader));
                            break;

                        default:
                            break;
                    }
                }
            }

            if (root == null)
                throw new ArgumentException("The specified XML fragment does not have any root element.", "xml");

            return new NodeFragment(new NodeDeclaration(declarationAttributes), root);
        }

        private static NodeElement ParseElement(int index, XmlReader reader)
        {
            string name = reader.Name;
            var children = new List<INode>();
            var attributes = new NodeAttributeCollection(GetAttributes(reader));

            if (!reader.IsEmptyElement)
            {
                int i = 0;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.EndElement:
                            return new NodeElement(index, name, attributes, children);

                        case XmlNodeType.Text:
                            children.Add(new NodeContent(i++, reader.Value));
                            break;

                        case XmlNodeType.Element:
                            children.Add(ParseElement(i++, reader));
                            break;

                        case XmlNodeType.Comment:
                            children.Add(new NodeComment(i++, reader.Value));
                            break;

                        default:
                            break;
                    }
                }
            }

            return new NodeElement(index, name, attributes, children);
        }

        private static IEnumerable<NodeAttribute> GetAttributes(XmlReader reader)
        {
            var tokens = new List<Pair<string, string>>();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    tokens.Add(new Pair<string, string>(reader.Name, reader.Value));

                } while (reader.MoveToNextAttribute());
            }

            reader.MoveToElement();

            for (int i = 0; i < tokens.Count; i++)
            {
                yield return new NodeAttribute(i, tokens[i].First, tokens[i].Second, tokens.Count);
            }
        }
    }
}
