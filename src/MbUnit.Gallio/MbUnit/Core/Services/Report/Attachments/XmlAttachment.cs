using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MbUnit.Core.Services.Report.Xml;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Services.Report.Attachments
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
        public override XmlReportAttachment XmlSerialize()
        {
            return XmlReportAttachment.Create(Name, ContentType, XmlContentEncoding.Base64,
                "", new XmlElement[] { XmlElement });
        }
    }
}
