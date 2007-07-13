// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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