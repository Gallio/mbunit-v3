using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using MbUnit.Core.Collections;
using MbUnit.Core.Services.Report.Attachments;
using MbUnit.Core.Utilities;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An Xml-serializable implementation of a test report.
    /// </summary>
    [XmlType]
    [XmlRoot("report")]
    [Serializable]
    public sealed class XmlReport : IReport
    {
        private XmlReportStreamCollection streams;
        private AttachmentCollection attachments;

        /// <summary>
        /// Creates an empty report.
        /// </summary>
        public XmlReport()
        {
            streams = new XmlReportStreamCollection(this);
            attachments = new AttachmentCollection();
        }

        [XmlIgnore]
        public IReportStreamCollection Streams
        {
            get { return streams; }
        }

        [XmlIgnore]
        public IAttachmentCollection Attachments
        {
            get { return attachments; }
        }

        /// <summary>
        /// Flushes the report and all of its streams.
        /// This method should be called prior to serializing the report to Xml.
        /// </summary>
        public void Flush()
        {
            foreach (XmlReportStream stream in streams)
                stream.Flush();
        }

        /// <summary>
        /// Gets or sets the array of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable=false)]
        [XmlArrayItem("stream", IsNullable=false)]
        public XmlReportStream[] XmlReportStreams
        {
            get
            {
                return ListUtils.ConvertAllToArray<IReportStream, XmlReportStream>(streams.ToArray(), delegate(IReportStream stream)
                {
                    return (XmlReportStream) stream;
                });
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                streams.Clear();
                streams.AddAll(value);
            }
        }

        /// <summary>
        /// Gets or sets the array of report attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable=false)]
        [XmlArrayItem("attachment", IsNullable=false)]
        public XmlReportAttachment[] XmlReportAttachments
        {
            get
            {
                return ListUtils.ConvertAllToArray<Attachment, XmlReportAttachment>(attachments.ToArray(), delegate(Attachment attachment)
                {
                    XmlReportAttachment xmlReportAttachment = new XmlReportAttachment();
                    xmlReportAttachment.Attachment = attachment;
                    return xmlReportAttachment;
                });
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                lock (attachments.SyncRoot)
                {
                    attachments.Clear();
                    foreach (XmlReportAttachment xmlReportAttachment in value)
                        attachments.Add(xmlReportAttachment.Attachment);
                }
            }
        }

        public Attachment Attach(Attachment attachment)
        {
            lock (attachments.SyncRoot)
            {
                Attachment existingAttachment;
                if (attachments.TryGetValue(attachment.Name, out existingAttachment))
                {
                    if (existingAttachment != attachment)
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                            "The report already contains an attachment with name '{0}'.", attachment.Name));
                }
                else
                {
                    attachments.Add(attachment);
                }

                return attachment;
            }
        }

        public TextAttachment AttachPlainText(string text)
        {
            return (TextAttachment) Attach(AttachmentUtils.CreatePlainTextAttachment(null, text));
        }

        public TextAttachment AttachHtml(string html)
        {
            return (TextAttachment) Attach(AttachmentUtils.CreateHtmlAttachment(null, html));
        }

        public XmlAttachment AttachXHtml(string xhtml)
        {
            return (XmlAttachment) Attach(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
        }

        public XmlAttachment AttachXml(string xml)
        {
            return (XmlAttachment) Attach(AttachmentUtils.CreateXmlAttachment(null, xml));
        }

        public BinaryAttachment AttachImage(Image image)
        {
            return (BinaryAttachment) Attach(AttachmentUtils.CreateImageAttachment(null, image));
        }

        public XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return (XmlAttachment) Attach(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
        }
    }
}