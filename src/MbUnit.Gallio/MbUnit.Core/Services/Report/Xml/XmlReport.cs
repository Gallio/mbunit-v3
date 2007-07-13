using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable report.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [XmlRoot("report", Namespace=SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReport
    {
        private List<XmlReportStream> streams;
        private List<XmlReportAttachment> attachments;

        /// <summary>
        /// Gets or sets the array of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable=false)]
        [XmlArrayItem("stream", IsNullable=false)]
        public XmlReportStream[] Streams
        {
            get { return streams.ToArray(); }
            set { streams = new List<XmlReportStream>(value); }
        }

        /// <summary>
        /// Gets or sets the array of report attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable=false)]
        [XmlArrayItem("attachment", IsNullable=false)]
        public XmlReportAttachment[] XmlReportAttachments
        {
            get { return attachments.ToArray(); }
            set { attachments = new List<XmlReportAttachment>(value); }
        }

        /// <summary>
        /// Adds a report stream to the report.
        /// </summary>
        /// <param name="stream">The report stream to add</param>
        public void AddStream(XmlReportStream stream)
        {
            streams.Add(stream);
        }

        /// <summary>
        /// Adds an attachment to the report.
        /// </summary>
        /// <param name="attachment">The attachment to add</param>
        public void AddAttachment(XmlReportAttachment attachment)
        {
            attachments.Add(attachment);
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        public static XmlReport Create()
        {
            XmlReport report = new XmlReport();
            report.streams = new List<XmlReportStream>();
            report.attachments = new List<XmlReportAttachment>();
            return report;
        }
    }
}