using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable tag for embedding an attachment within a report.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReportEmbedTag : XmlReportTag
    {
        private string attachmentName;

        /// <summary>
        /// Gets or sets the name of the referenced attachment to embed, not null.
        /// </summary>
        [XmlAttribute("attachmentName")]
        public string AttachmentName
        {
            get { return attachmentName; }
            set { attachmentName = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="attachmentName">The name of the referenced attachment to embed</param>
        public static XmlReportEmbedTag Create(string attachmentName)
        {
            XmlReportEmbedTag tag = new XmlReportEmbedTag();
            tag.attachmentName = attachmentName;
            return tag;
        }
    }
}
