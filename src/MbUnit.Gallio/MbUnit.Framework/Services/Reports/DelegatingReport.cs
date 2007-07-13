using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// An implementation of <see cref="IReport" /> that uses a <see cref="IReportWriter" />
    /// to write the report.
    /// </summary>
    public class DelegatingReport : IReport
    {
        private IReportWriter writer;
        private DelegatingReportStreamCollection streams;

        /// <summary>
        /// Creates a delegating report that forwards report writing operations
        /// to the specified report writer.
        /// </summary>
        /// <param name="writer">The report writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public DelegatingReport(IReportWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
            streams = new DelegatingReportStreamCollection(this);
        }

        /// <summary>
        /// Gets the report writer.
        /// </summary>
        public IReportWriter Writer
        {
            get { return writer; }
        }

        /// <inheritdoc />
        public IReportStreamCollection Streams
        {
            get { return streams; }
        }

        /// <inheritdoc />
        public Attachment Attach(Attachment attachment)
        {
            writer.WriteAttachment(null, attachment);
            return attachment;
        }

        /// <inheritdoc />
        public TextAttachment AttachPlainText(string text)
        {
            return (TextAttachment)Attach(AttachmentUtils.CreatePlainTextAttachment(null, text));
        }

        /// <inheritdoc />
        public TextAttachment AttachHtml(string html)
        {
            return (TextAttachment)Attach(AttachmentUtils.CreateHtmlAttachment(null, html));
        }

        /// <inheritdoc />
        public XmlAttachment AttachXHtml(string xhtml)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
        }

        /// <inheritdoc />
        public XmlAttachment AttachXml(string xml)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateXmlAttachment(null, xml));
        }

        /// <inheritdoc />
        public BinaryAttachment AttachImage(Image image)
        {
            return (BinaryAttachment)Attach(AttachmentUtils.CreateImageAttachment(null, image));
        }

        /// <inheritdoc />
        public XmlAttachment AttachObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return (XmlAttachment)Attach(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
        }

        private class DelegatingReportStreamCollection : IReportStreamCollection
        {
            private DelegatingReport report;

            public DelegatingReportStreamCollection(DelegatingReport report)
            {
                this.report = report;
            }

            public IReportStream this[string streamName]
            {
                get { return new DelegatingReportStream(report, streamName); }
            }
        }

        private class DelegatingReportStream : TextWriter, IReportStream
        {
            private DelegatingReport report;
            private string streamName;

            public DelegatingReportStream(DelegatingReport report, string streamName)
            {
                this.report = report;
                this.streamName = streamName;
            }

            public IReport Report
            {
                get { return report; }
            }

            public string Name
            {
                get { return streamName; }
            }

            public TextWriter TextWriter
            {
                get { return this; }
            }

            public override Encoding Encoding
            {
                get { return Encoding.Unicode; }
            }

            public void BeginSection(string sectionName)
            {
                report.writer.BeginSection(streamName, sectionName);
            }

            public void EndSection()
            {
                report.writer.EndSection(streamName);
            }

            public override void Write(string value)
            {
                report.writer.WriteText(streamName, value);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                Write(new String(buffer, index, count));
            }

            public Attachment Embed(Attachment attachment)
            {
                report.writer.WriteAttachment(streamName, attachment);
                return attachment;
            }

            public TextAttachment EmbedPlainText(string text)
            {
                return (TextAttachment)Embed(AttachmentUtils.CreatePlainTextAttachment(null, text));
            }

            public TextAttachment EmbedHtml(string html)
            {
                return (TextAttachment)Embed(AttachmentUtils.CreateHtmlAttachment(null, html));
            }

            public XmlAttachment EmbedXHtml(string xhtml)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
            }

            public XmlAttachment EmbedXml(string xml)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateXmlAttachment(null, xml));
            }

            public BinaryAttachment EmbedImage(Image image)
            {
                return (BinaryAttachment)Embed(AttachmentUtils.CreateImageAttachment(null, image));
            }

            public XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer)
            {
                return (XmlAttachment)Embed(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
            }
        }
    }
}
