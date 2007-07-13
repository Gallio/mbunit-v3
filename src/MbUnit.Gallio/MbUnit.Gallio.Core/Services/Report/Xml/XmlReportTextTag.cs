using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable block of preformatted text to include in a report.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReportTextTag : XmlReportTag
    {
        private string text;

        /// <summary>
        /// Gets or sets the text within the tag, not null.
        /// </summary>
        [XmlText]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="text">The text</param>
        public static XmlReportTextTag Create(string text)
        {
            XmlReportTextTag tag = new XmlReportTextTag();
            tag.text = text;
            return tag;
        }
    }
}
