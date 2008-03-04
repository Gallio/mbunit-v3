// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.Xml.Serialization;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Specifies how attachments are stored in Xml.
    /// </summary>
    public enum ExecutionLogAttachmentContentDisposition
    {
        /// <summary>
        /// The attachment content is not present.
        /// </summary>
        [XmlEnum("absent")]
        Absent = 0,

        /// <summary>
        /// The attachment content is saved to a linked file indicated by <see cref="ExecutionLogAttachment.ContentPath" />.
        /// </summary>
        [XmlEnum("link")]
        Link,

        /// <summary>
        /// The attachment content is included inline as <see cref="ExecutionLogAttachment.InnerText" />
        /// with a given <see cref="ExecutionLogAttachment.Encoding" />.
        /// </summary>
        [XmlEnum("inline")]
        Inline
    }
}
