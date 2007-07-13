using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MbUnit.Core.Services.Report.Xml;

namespace MbUnit.Core.Services.Report.Attachments
{
    /// <summary>
    /// An attachment is an embedded object in a report.  An attachment must specify a
    /// content type (a MIME type), and some contents.
    /// </summary>
    [Serializable]
    public abstract class Attachment
    {
        private string name;
        private string contentType;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The name of attachment, or null to automatically assign one</param>
        /// <param name="contentType">The content type, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> is null</exception>
        protected Attachment(string name, string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException("contentType");

            this.name = name == null ? Guid.NewGuid().ToString() : name;
            this.contentType = contentType;
        }

        /// <summary>
        /// Gets the name of the attachment, not null.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the content type of the attachment specified as a MIME type, not null.
        /// <seealso cref="MimeTypes"/> for definitions of common MIME types used by MbUnit.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// Serializes the attachment to Xml.
        /// </summary>
        /// <returns>The Xml-serializable attachment</returns>
        public abstract XmlReportAttachment XmlSerialize();

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
