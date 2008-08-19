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
using System.Xml.Serialization;
using Gallio.Model.Logging;
using Gallio.Utilities;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// A structured test log is an Xml-serializable in-memory representation of a test
    /// log written by a <see cref="TestLogWriter" />.
    /// </summary>
    /// <seealso cref="StructuredTestLogWriter"/>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlRoot("testLog", Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class StructuredTestLog
    {
        private readonly List<StructuredTestLogStream> streams;
        private readonly List<AttachmentData> attachments;

        /// <summary>
        /// Creates an empty log.
        /// </summary>
        public StructuredTestLog()
        {
            streams = new List<StructuredTestLogStream>();
            attachments = new List<AttachmentData>();
        }

        /// <summary>
        /// Gets the list of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("stream", typeof(StructuredTestLogStream), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<StructuredTestLogStream> Streams
        {
            get { return streams; }
        }

        /// <summary>
        /// Gets the list of attachments, not null.
        /// </summary>
        [XmlArray("attachments", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("attachment", typeof(AttachmentData), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<AttachmentData> Attachments
        {
            get { return attachments; }
        }

        /// <summary>
        /// Gets an attachment by name.
        /// </summary>
        /// <param name="name">The attachment name</param>
        /// <returns>The attachment or null if not found</returns>
        public AttachmentData GetAttachment(string name)
        {
            return attachments.Find(attachment => attachment.Name == name);
        }

        /// <summary>
        /// Gets a stream by name.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <returns>The stream or null if not found</returns>
        public StructuredTestLogStream GetStream(string name)
        {
            return streams.Find(stream => stream.Name == name);
        }
    }
}