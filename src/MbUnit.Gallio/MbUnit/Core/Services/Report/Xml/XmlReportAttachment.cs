using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Report.Xml
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
    }
}
