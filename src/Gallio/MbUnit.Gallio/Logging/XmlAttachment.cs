// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Logging
{
    /// <summary>
    /// Represents a XML-encoded attachment.
    /// </summary>
    [Serializable]
    public class XmlAttachment : Attachment
    {
        private string xmlString;
        private XmlElement xmlElement;

        /// <summary>
        /// Creates an attachment with Xml content from a string.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="xmlString">The well-formed Xml, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="xmlString"/> is null</exception>
        public XmlAttachment(string name, string contentType, string xmlString)
            : base(name, contentType)
        {
            if (xmlString == null)
                throw new ArgumentNullException("xmlString");

            this.xmlString = xmlString;
        }

        /// <summary>
        /// Creates an attachment with Xml content from an <see cref="XmlElement" />.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="xmlElement">The Xml element, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="xmlElement"/> is null</exception>
        public XmlAttachment(string name, string contentType, XmlElement xmlElement)
            : base(name, contentType)
        {
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            this.xmlElement = xmlElement;
        }

        /// <summary>
        /// Gets the Xml contents as a string, not null.
        /// </summary>
        public string XmlString
        {
            get
            {
                if (xmlString == null)
                {
                    xmlString = xmlElement.OuterXml;
                }

                return xmlString;
            }
        }

        /// <summary>
        /// Gets the Xml contents as an <see cref="XmlElement" />, not null.
        /// </summary>
        public XmlElement XmlElement
        {
            get
            {
                if (xmlElement == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlString);
                    xmlElement = doc.DocumentElement;
                }

                return xmlElement;
            }
        }

        /// <inheritdoc />
        public override void Accept(IAttachmentVisitor visitor)
        {
            visitor.VisitXmlAttachment(this);
        }
    }
}
