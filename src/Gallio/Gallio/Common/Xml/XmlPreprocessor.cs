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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Gallio.Common.Collections;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Preprocesses XML based on the presence of processing instructions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Recognizes the following directives:
    /// </para>
    /// <list type="bullet">
    /// <item><code>&lt;?ifdef CONSTANT?&gt;</code>: Begins conditional block whose contents are output only if CONSTANT is defined.</item>
    /// <item><code>&lt;?ifndef CONSTANT?&gt;</code>: Begins conditional block whose contents are output only if CONSTANT is not defined.</item>
    /// <item><code>&lt;?else?&gt;</code>: Begins alternative conditional block whose contents are output only if the previous ifdef/ifndef condition was not met.</item>
    /// <item><code>&lt;?endif?&gt;</code>: Ends conditional block.</item>
    /// <item><code>&lt;?define CONSTANT?&gt;</code>: Defines a new constant.</item>
    /// </list>
    /// </remarks>
    public class XmlPreprocessor
    {
        private readonly HashSet<string> constants;

        /// <summary>
        /// Creates an Xml preprocessor.
        /// </summary>
        public XmlPreprocessor()
        {
            constants = new HashSet<string>();
        }

        /// <summary>
        /// Defines a preprocessor constant.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constant"/>
        /// is null.</exception>
        public void Define(string constant)
        {
            if (constant == null)
                throw new ArgumentNullException("constant");

            constants.Add(constant); 
        }

        /// <summary>
        /// Returns true if the specified preprocessor constant is defined.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="constant"/>
        /// is null.</exception>
        /// <returns>True if the constant is defined.</returns>
        public bool IsDefined(string constant)
        {
            if (constant == null)
                throw new ArgumentNullException("constant");

            return constants.Contains(constant);
        }

        /// <summary>
        /// Preprocesses and copies an Xml document from a reader into a writer.
        /// </summary>
        /// <param name="xmlReader">The Xml reader.</param>
        /// <param name="xmlWriter">The Xml writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xmlReader"/>
        /// or <paramref name="xmlWriter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the input Xml is malformed
        /// such as if it contains unbalanced ifdef/endif pairs.</exception>
        public void Preprocess(XmlReader xmlReader, XmlWriter xmlWriter)
        {
            if (xmlReader == null)
                throw new ArgumentNullException("xmlReader");
            if (xmlWriter == null)
                throw new ArgumentNullException("xmlWriter");

            // Tracks whether a given block has been included or excluded.
            Stack<bool> blockStack = new Stack<bool>();
            blockStack.Push(true);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.ProcessingInstruction)
                {
                    switch (xmlReader.Name)
                    {
                        case "define":
                            if (blockStack.Peek())
                                Define(xmlReader.Value.Trim());
                            continue;

                        case "ifdef":
                            blockStack.Push(blockStack.Peek() && IsDefined(xmlReader.Value.Trim()));
                            continue;

                        case "ifndef":
                            blockStack.Push(blockStack.Peek() && !IsDefined(xmlReader.Value.Trim()));
                            continue;

                        case "else":
                            if (blockStack.Count == 1)
                                throw new InvalidOperationException(
                                    "Found <?else?> instruction without enclosing <?ifdef?> or <?ifndef?> block.");
                            blockStack.Push(!blockStack.Pop() && blockStack.Peek()); // order matters
                            continue;

                        case "endif":
                            if (blockStack.Count == 1)
                                throw new InvalidOperationException(
                                    "Found <?endif?> instruction without matching <?ifdef?> or <?ifndef?>.");
                            blockStack.Pop();
                            continue;
                    }
                }

                if (!blockStack.Peek())
                    continue;

                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        xmlWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
                        xmlWriter.WriteAttributes(xmlReader, true);
                        if (xmlReader.IsEmptyElement)
                            xmlWriter.WriteEndElement();
                        break;

                    case XmlNodeType.Text:
                        xmlWriter.WriteValue(xmlReader.Value);
                        break;

                    case XmlNodeType.CDATA:
                        xmlWriter.WriteCData(xmlReader.Value);
                        break;

                    case XmlNodeType.EntityReference:
                        xmlWriter.WriteEntityRef(xmlReader.Name);
                        break;

                    case XmlNodeType.Comment:
                        xmlWriter.WriteComment(xmlReader.Value);
                        break;

                    case XmlNodeType.DocumentType:
                        xmlWriter.WriteDocType(xmlReader.Name, xmlReader.GetAttribute("PUBLIC"),
                            xmlReader.GetAttribute("SYSTEM"), xmlReader.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        xmlWriter.WriteWhitespace(xmlReader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        xmlWriter.WriteFullEndElement();
                        break;

                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.ProcessingInstruction:
                        xmlWriter.WriteProcessingInstruction(xmlReader.Name, xmlReader.Value);
                        break;
                }
            }

            if (blockStack.Count != 1)
                throw new InvalidOperationException("Missing <?endif?> instruction.");

            xmlWriter.Flush();
        }
    }
}