using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Services.Report.Attachments;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// An xml-serializable report stram.
    /// </summary>
    [XmlType]
    [Serializable]
    public sealed class XmlReportStream : TextWriter, IReportStream
    {
        [NonSerialized]
        private readonly object syncRoot = new object();

        private IReport report;
        private string name;

        private Stack<XmlReportContainerTag> containerStack;
        private XmlReportBodyTag body;
        private StringBuilder textBuffer;

        /// <summary>
        /// Creates an empty report stream.
        /// </summary>
        public XmlReportStream()
        {
            containerStack = new Stack<XmlReportContainerTag>();
            body = new XmlReportBodyTag();
            textBuffer = new StringBuilder();
            containerStack.Push(body);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        /// <summary>
        /// Gets or sets the report to which the stream belongs.
        /// The property must be set after creation or Xml deserialization.
        /// </summary>
        [XmlIgnore]
        public IReport Report
        {
            get { return report; }
            set { report = value; }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                name = value;
            }
        }

        [XmlIgnore]
        public TextWriter TextWriter
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or sets the body of the report stream, not null.
        /// </summary>
        [XmlElement]
        public XmlReportBodyTag Body
        {
            get { return body; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                body = value;
            }
        }

        /// <summary>
        /// Flushes the report stream.
        /// This method should be called prior to serializing the report to Xml.
        /// </summary>
        public override void Flush()
        {
            lock (syncRoot)
            {
                if (textBuffer.Length != 0)
                {
                    XmlReportTextTag textTag = new XmlReportTextTag();
                    textTag.Text = textBuffer.ToString();
                    textBuffer.Length = 0;

                    TopContainer.Append(textTag);
                }
            }
        }

        public void BeginSection(string sectionHeading)
        {
            lock (syncRoot)
            {
                Flush();

                XmlReportSectionTag sectionTag = new XmlReportSectionTag();
                sectionTag.Heading = sectionHeading;

                TopContainer.Append(sectionTag);
                containerStack.Push(sectionTag);
            }
        }

        public void EndSection()
        {
            lock (syncRoot)
            {
                if (containerStack.Count == 1)
                    throw new InvalidOperationException("EndSection was called more times than BeginSection for this report.");

                Flush();

                containerStack.Pop();
            }
        }

        public override void Write(char value)
        {
            lock (syncRoot)
            {
                textBuffer.Append(value);
            }
        }

        public override void Write(string value)
        {
            lock (syncRoot)
            {
                textBuffer.Append(value);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            lock (syncRoot)
            {
                textBuffer.Append(buffer, index, count);
            }
        }

        public Attachment Embed(Attachment attachment)
        {
            lock (syncRoot)
            {
                report.Attach(attachment);

                Flush();

                XmlReportEmbedTag embedTag = new XmlReportEmbedTag();
                embedTag.AttachmentName = attachment.Name;

                TopContainer.Append(embedTag);
                return attachment;
            }
        }

        public TextAttachment EmbedPlainText(string text)
        {
            return (TextAttachment) Embed(AttachmentUtils.CreatePlainTextAttachment(null, text));
        }

        public TextAttachment EmbedHtml(string html)
        {
            return (TextAttachment) Embed(AttachmentUtils.CreateHtmlAttachment(null, html));
        }

        public XmlAttachment EmbedXHtml(string xhtml)
        {
            return (XmlAttachment) Embed(AttachmentUtils.CreateXHtmlAttachment(null, xhtml));
        }

        public XmlAttachment EmbedXml(string xml)
        {
            return (XmlAttachment) Embed(AttachmentUtils.CreateXmlAttachment(null, xml));
        }

        public BinaryAttachment EmbedImage(Image image)
        {
            return (BinaryAttachment) Embed(AttachmentUtils.CreateImageAttachment(null, image));
        }

        public XmlAttachment EmbedObjectAsXml(object obj, XmlSerializer xmlSerializer)
        {
            return (XmlAttachment) Embed(AttachmentUtils.CreateObjectAsXmlAttachment(null, obj, xmlSerializer));
        }

        /// <summary>
        /// Gets the top container from the container stack.
        /// </summary>
        private XmlReportContainerTag TopContainer
        {
            get { return containerStack.Peek(); }
        }
    }
}
