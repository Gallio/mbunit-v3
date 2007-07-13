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
using System.Text;
using System.Xml;
using MbUnit.Framework.Services.Reports;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Utilities;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// Serializes an attachment to Xml as it is visited.
    /// </summary>
    public class XmlReportAttachmentSerializer : IAttachmentVisitor
    {
        private XmlReportAttachment xmlReportAttachment;

        /// <summary>
        /// Gets the serialized attachment.
        /// </summary>
        public XmlReportAttachment XmlReportAttachment
        {
            get { return xmlReportAttachment; }
        }

        /// <inheritdoc />
        public void VisitTextAttachment(TextAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Text,
                attachment.Text, 
                EmptyArray<XmlElement>.Instance);
        }

        /// <inheritdoc />
        public void VisitXmlAttachment(XmlAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Xml,
                "",
                new XmlElement[] { attachment.XmlElement });
        }

        /// <inheritdoc />
        public void VisitBinaryAttachment(BinaryAttachment attachment)
        {
            xmlReportAttachment = XmlReportAttachment.Create(attachment.Name, attachment.ContentType,
                XmlContentEncoding.Base64,
                Convert.ToBase64String(attachment.Data, Base64FormattingOptions.None),
                EmptyArray<XmlElement>.Instance);
        }
    }
}
