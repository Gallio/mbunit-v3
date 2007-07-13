using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MbUnit.Framework.Services.Reports;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Utilities;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// Serializes an attachment to Xml as it is visited.
    /// </summary>
    public class XmlReportAttachmentSerializer : IAttachmentVisitor
    {
        private XmlReportAttachment xmlReportAttachment;

        /// <summary>
        /// Gets the serialized attachment.
        /// </summary>
        public XmlReportAttachment XmlReportAttachment
        {
            get { return xmlReportAttachment; }
        }

        /// <inheritdoc />
        public void VisitTextAttachment(TextAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Text,
                attachment.Text, 
                EmptyArray<XmlElement>.Instance);
        }

        /// <inheritdoc />
        public void VisitXmlAttachment(XmlAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Xml,
                "",
                new XmlElement[] { attachment.XmlElement });
        }

        /// <inheritdoc />
        public void VisitBinaryAttachment(BinaryAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Base64,
                Convert.ToBase64String(attachment.Data, Base64FormattingOptions.None),
                EmptyArray<XmlElement>.Instance);
        }
    }
}
