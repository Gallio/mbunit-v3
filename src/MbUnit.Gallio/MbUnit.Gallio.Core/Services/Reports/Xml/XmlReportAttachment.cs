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
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// <para>
    /// An Xml-serializable attachment.
    /// </para>
    /// <para>
    /// The contents of the attachment are encoded and embedded in the report according to
    /// their encoding.  Text and Xml markup are directly embedded into the Xml report whereas
    /// binary attachments are base 64 encoded.
    /// </para>
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class XmlReportAttachment
    {
        private string name;
        private string contentType;
        private XmlContentEncoding contentEncoding;
        private string innerText;
        private XmlElement[] innerXml;

        /// <summary>
        /// Gets or sets the name of the attachment, not null.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the content type of the attachment as a MIME type, not null.
        /// </summary>
        [XmlAttribute("contentType")]
        public string ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }

        /// <summary>
        /// Gets or sets the content encoding of the attachment.
        /// This value specifies how the attachment is embedded in Xml.
        /// </summary>
        [XmlAttribute("contentEncoding")]
        public XmlContentEncoding ContentEncoding
        {
            get { return contentEncoding; }
            set { contentEncoding = value; }
        }

        /// <summary>
        /// Gets or sets the attachment content serialized as text (including Base64 attachments), possibly null if none.
        /// </summary>
        [XmlText]
        public string InnerText
        {
            get { return innerText; }
            set { innerText = value; }
        }

        /// <summary>
        /// Gets or sets the attachment content serialized as Xml, possibly null if none.
        /// </summary>
        [XmlAnyElement]
        public XmlElement[] InnerXml
        {
            get { return innerXml; }
            set { innerXml = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <param name="contentType">The content type</param>
        /// <param name="contentEncoding">The content encoding</param>
        /// <param name="innerText">The inner text or "" if none</param>
        /// <param name="innerXml">The inner Xml or an empty array if none</param>
        public static XmlReportAttachment Create(string attachmentName, string contentType,
            XmlContentEncoding contentEncoding, string innerText, XmlElement[] innerXml)
        {
            XmlReportAttachment tag = new XmlReportAttachment();
            tag.name = attachmentName;
            tag.contentType = contentType;
            tag.contentEncoding = contentEncoding;
            tag.innerText = innerText;
            tag.innerXml = innerXml;
            return tag;
        }

        /// <summary>
        /// Serializes the attachment to Xml.
        /// </summary>
        /// <returns>The Xml-serializable attachment</returns>
        public static XmlReportAttachment XmlSerialize(Attachment attachment)
        {
            XmlReportAttachmentSerializer serializer = new XmlReportAttachmentSerializer();
            attachment.Accept(serializer);
            return serializer.XmlReportAttachment;
        }

        /// <summary>
        /// Deserializes the attachment from Xml.
        /// </summary>
        /// <param name="xmlAttachment">The Xml report attachment</param>
        /// <returns>The deserialized form</returns>
        public static Attachment XmlDeserialize(XmlReportAttachment xmlAttachment)
        {
            if (xmlAttachment.Name == null)
                throw new XmlException("The attachment is missing its name attribute.");
            if (xmlAttachment.ContentType == null)
                throw new XmlException("The attachment is missing its contentType attribute.");

            switch (xmlAttachment.ContentEncoding)
            {
                case XmlContentEncoding.Xml:
                    {
                        if (xmlAttachment.InnerXml == null || xmlAttachment.InnerXml.Length == 0)
                            throw new XmlException("The xml attachment is missing its inner Xml content.");

                        return new XmlAttachment(xmlAttachment.Name, xmlAttachment.ContentType, xmlAttachment.InnerXml[0]);
                    }

                case XmlContentEncoding.Text:
                    {
                        if (xmlAttachment.InnerText == null)
                            throw new XmlException("The text attachment is missing its inner text content.");

                        return new TextAttachment(xmlAttachment.Name, xmlAttachment.ContentType, xmlAttachment.InnerText);
                    }

                case XmlContentEncoding.Base64:
                    {
                        if (xmlAttachment.InnerText == null)
                            throw new XmlException("The base64 attachment is missing its inner text content.");

                        return new BinaryAttachment(xmlAttachment.Name, xmlAttachment.ContentType, Convert.FromBase64String(xmlAttachment.InnerText));
                    }
            }

            throw new XmlException(String.Format(CultureInfo.CurrentCulture,
                "Unrecognized Xml content encoding '{0}'.", xmlAttachment.ContentEncoding));
        }
    }
}
