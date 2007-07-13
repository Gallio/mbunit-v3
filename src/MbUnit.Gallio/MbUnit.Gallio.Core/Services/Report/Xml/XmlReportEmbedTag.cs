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
using System.Xml.Serialization;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Services.Reports.Xml
{
    /// <summary>
    /// An Xml-serializable tag for embedding an attachment within a report.
    /// </summary>
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    [Serializable]
    public sealed class XmlReportEmbedTag : XmlReportTag
    {
        private string attachmentName;

        /// <summary>
        /// Gets or sets the name of the referenced attachment to embed, not null.
        /// </summary>
        [XmlAttribute("attachmentName")]
        public string AttachmentName
        {
            get { return attachmentName; }
            set { attachmentName = value; }
        }

        /// <summary>
        /// Creates an empty but fully initialized instance.
        /// </summary>
        /// <param name="attachmentName">The name of the referenced attachment to embed</param>
        public static XmlReportEmbedTag Create(string attachmentName)
        {
            XmlReportEmbedTag tag = new XmlReportEmbedTag();
            tag.attachmentName = attachmentName;
            return tag;
        }
    }
}
