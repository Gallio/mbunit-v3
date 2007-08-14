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
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Services.ExecutionLogs;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// <para>
    /// An Xml-serializable attachment.
    /// </para>
    /// <para>
    /// The contents of the attachment are embedded in the execution log according to
    /// their encoding.  Text and Xml markup are directly embedded into the Xml whereas
    /// binary attachments are base 64 encoded.
    /// </para>
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class ExecutionLogAttachment
    {
        private string name;
        private string contentType;
        private ExecutionLogAttachmentEncoding encoding;
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
        /// Gets or sets the encoding of the attachment.
        /// This value specifies how the attachment is embedded in Xml.
        /// </summary>
        [XmlAttribute("encoding")]
        public ExecutionLogAttachmentEncoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
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
        /// <param name="encoding">The content encoding</param>
        /// <param name="innerText">The inner text or "" if none</param>
        /// <param name="innerXml">The inner Xml or an empty array if none</param>
        public static ExecutionLogAttachment Create(string attachmentName, string contentType,
            ExecutionLogAttachmentEncoding encoding, string innerText, XmlElement[] innerXml)
        {
            ExecutionLogAttachment tag = new ExecutionLogAttachment();
            tag.name = attachmentName;
            tag.contentType = contentType;
            tag.encoding = encoding;
            tag.innerText = innerText;
            tag.innerXml = innerXml;
            return tag;
        }

        /// <summary>
        /// Serializes the attachment to Xml.
        /// </summary>
        /// <returns>The Xml-serializable attachment</returns>
        public static ExecutionLogAttachment XmlSerialize(Attachment attachment)
        {
            ExecutionLogAttachmentSerializer serializer = new ExecutionLogAttachmentSerializer();
            attachment.Accept(serializer);
            return serializer.SerializedAttachment;
        }

        /// <summary>
        /// Deserializes the attachment from Xml.
        /// </summary>
        /// <param name="attachment">The Xml execution log attachment</param>
        /// <returns>The deserialized form</returns>
        public static Attachment XmlDeserialize(ExecutionLogAttachment attachment)
        {
            if (attachment.Name == null)
                throw new XmlException("The attachment is missing its name attribute.");
            if (attachment.ContentType == null)
                throw new XmlException("The attachment is missing its contentType attribute.");

            switch (attachment.Encoding)
            {
                case ExecutionLogAttachmentEncoding.Xml:
                    {
                        if (attachment.InnerXml == null || attachment.InnerXml.Length == 0)
                            throw new XmlException("The xml attachment is missing its inner Xml content.");

                        return new XmlAttachment(attachment.Name, attachment.ContentType, attachment.InnerXml[0]);
                    }

                case ExecutionLogAttachmentEncoding.Text:
                    {
                        if (attachment.InnerText == null)
                            throw new XmlException("The text attachment is missing its inner text content.");

                        return new TextAttachment(attachment.Name, attachment.ContentType, attachment.InnerText);
                    }

                case ExecutionLogAttachmentEncoding.Base64:
                    {
                        if (attachment.InnerText == null)
                            throw new XmlException("The base64 attachment is missing its inner text content.");

                        return new BinaryAttachment(attachment.Name, attachment.ContentType, Convert.FromBase64String(attachment.InnerText));
                    }
            }

            throw new XmlException(String.Format(CultureInfo.CurrentCulture,
                "Unrecognized Xml content encoding '{0}'.", attachment.Encoding));
        }
    }
}
