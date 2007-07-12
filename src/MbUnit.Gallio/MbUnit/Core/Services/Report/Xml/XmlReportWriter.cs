using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MbUnit.Core.Services.Report.Attachments;

namespace MbUnit.Core.Services.Report.Xml
{
    /// <summary>
    /// Writes reports to an Xml-serializable format.
    /// </summary>
    public class XmlReportWriter : IReportWriter
    {
        private XmlReport xmlReport;
        private Dictionary<string, ReportStreamData> streamDataMap;
        private Dictionary<string, Attachment> attachmentMap;

        /// <summary>
        /// Creates an Xml report writer with a new Xml report.
        /// </summary>
        public XmlReportWriter()
        {
            xmlReport = new XmlReport();

            streamDataMap = new Dictionary<string, ReportStreamData>();
            attachmentMap = new Dictionary<string, Attachment>();
        }

        /// <summary>
        /// Gets the Xml Report under construction.
        /// </summary>
        public XmlReport XmlReport
        {
            get { return xmlReport; }
        }

        /// <inheritdoc />
        public void WriteText(string streamName, string text)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (text == null)
                throw new ArgumentNullException("text");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).WriteText(text);
            }
        }

        /// <inheritdoc />
        public void WriteAttachment(string streamName, Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            lock (this)
            {
                ThrowIfClosed();

                Attachment existingAttachment;
                if (attachmentMap.TryGetValue(attachment.Name, out existingAttachment))
                {
                    if (attachment != existingAttachment)
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                            "Attempted to add a different attachment instance with the name '{0}'.", attachment.Name));
                }
                else
                {
                    attachmentMap.Add(attachment.Name, attachment);
                    xmlReport.AddAttachment(attachment.XmlSerialize());
                }

                if (streamName != null)
                {
                    GetStreamData(streamName).WriteAttachment(attachment.Name);
                }
            }
        }

        /// <inheritdoc />
        public void BeginSection(string streamName, string sectionName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).BeginSection(sectionName);
            }
        }

        /// <inheritdoc />
        public void EndSection(string streamName)
        {
            if (streamName == null)
                throw new ArgumentNullException("streamName");

            lock (this)
            {
                ThrowIfClosed();

                GetStreamData(streamName).EndSection();
            }
        }

        /// <inheritdoc />
        public void Close()
        {
            lock (this)
            {
                if (streamDataMap == null)
                    return; // already closed

                foreach (ReportStreamData entry in streamDataMap.Values)
                    entry.Flush();

                streamDataMap = null;
                attachmentMap = null;
            }
        }

        private ReportStreamData GetStreamData(string streamName)
        {
            ReportStreamData streamData;
            if (!streamDataMap.TryGetValue(streamName, out streamData))
            {
                streamData = new ReportStreamData(streamName);
                streamDataMap.Add(streamName, streamData);

                xmlReport.AddStream(streamData.XmlReport);
            }

            return streamData;
        }

        private void ThrowIfClosed()
        {
            if (streamDataMap == null)
                throw new InvalidOperationException("Cannot perform this operation because the report writer has been closed.");
        }

        private class ReportStreamData
        {
            private XmlReportStream xmlStream;
            private Stack<XmlReportContainerTag> containerStack;
            private StringBuilder textBuilder;

            public ReportStreamData(string streamName)
            {
                xmlStream = XmlReportStream.Create(streamName);
                containerStack = new Stack<XmlReportContainerTag>();
                textBuilder = new StringBuilder();

                containerStack.Push(xmlStream.Body);
            }

            public XmlReportStream XmlReport
            {
                get { return xmlStream; }
            }

            public void Flush()
            {
                if (textBuilder.Length == 0)
                {
                    containerStack.Peek().AddContent(XmlReportTextTag.Create(textBuilder.ToString()));
                    textBuilder.Length = 0;
                }
            }

            public void WriteText(string text)
            {
                textBuilder.Append(text);
            }

            public void WriteAttachment(string attachmentName)
            {
                Flush();

                containerStack.Peek().AddContent(XmlReportEmbedTag.Create(attachmentName));
            }

            public void BeginSection(string sectionName)
            {
                Flush();

                XmlReportSectionTag tag = XmlReportSectionTag.Create(sectionName);
                containerStack.Peek().AddContent(tag);
                containerStack.Push(tag);
            }

            public void EndSection()
            {
                if (containerStack.Count == 1)
                    throw new InvalidOperationException("There is no current section to be ended.");

                Flush();
                containerStack.Pop();
            }
        }
    }
}
