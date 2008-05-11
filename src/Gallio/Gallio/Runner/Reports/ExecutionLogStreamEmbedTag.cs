// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// An Xml-serializable tag for embedding an attachment within an execution log.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class ExecutionLogStreamEmbedTag : ExecutionLogStreamTag
    {
        private string attachmentName;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private ExecutionLogStreamEmbedTag()
        {
        }

        /// <summary>
        /// Creates an initialized tag.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to embed</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
        public ExecutionLogStreamEmbedTag(string attachmentName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");

            this.attachmentName = attachmentName;
        }

        /// <summary>
        /// Gets or sets the name of the referenced attachment to embed, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("attachmentName")]
        public string AttachmentName
        {
            get { return attachmentName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                attachmentName = value;
            }
        }

        /// <inheritdoc />
        public override void Accept(IExecutionLogStreamTagVisitor visitor)
        {
            visitor.VisitEmbedTag(this);
        }
    }
}
