using System;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An xml-serializable report stram.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReportStream
    {
        private string name;
        private XmlReportBodyTag body;

        /// <summary>
        /// Gets or sets the name of the report stream, not null.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the body of the report stream, not null.
        /// </summary>
        [XmlElement("body", IsNullable=false)]
        public XmlReportBodyTag Body
        {
            get { return body; }
            set { body = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        public static XmlReportStream Create(string streamName)
        {
            XmlReportStream stream = new XmlReportStream();
            stream.name = streamName;
            stream.body = XmlReportBodyTag.Create();
            return stream;
        }
    }
}
