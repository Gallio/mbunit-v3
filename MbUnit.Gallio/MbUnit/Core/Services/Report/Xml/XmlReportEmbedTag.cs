using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An Xml-serializable tag for embedding an attachment within a report.
    /// </summary>
    [XmlType]
    [Serializable]
    public sealed class XmlReportEmbedTag : XmlReportTag
    {
        private string attachmentName;

        /// <summary>
        /// Gets or sets the name of the referenced attachment to embed, not null.
        /// </summary>
        [XmlAttribute("attributeName")]
        public string AttachmentName
        {
            get { return attachmentName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                attachmentName = value;
            }
        }
    }
}
