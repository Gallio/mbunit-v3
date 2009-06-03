// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Xml;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// A structured document is an Xml-serializable in-memory representation of a markup
    /// document written by a <see cref="MarkupDocumentWriter" />.
    /// </summary>
    /// <seealso cref="StructuredDocumentWriter"/>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlRoot("testLog", Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class StructuredDocument
    {
        private readonly List<StructuredStream> streams;
        private readonly List<AttachmentData> attachments;

        /// <summary>
        /// Creates an empty document.
        /// </summary>
        public StructuredDocument()
        {
            streams = new List<StructuredStream>();
            attachments = new List<AttachmentData>();
        }

        /// <summary>
        /// Gets the list of streams, not null.
        /// Used for Xml-serialization.
        /// </summary>
        [XmlArray("streams", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("stream", typeof(StructuredStream), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<StructuredStream> Streams
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
        /// <param name="name">The attachment name.</param>
        /// <returns>The attachment or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public AttachmentData GetAttachment(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return attachments.Find(attachment => attachment.Name == name);
        }

        /// <summary>
        /// Gets a stream by name.
        /// </summary>
        /// <param name="name">The stream name.</param>
        /// <returns>The stream or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public StructuredStream GetStream(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return streams.Find(stream => stream.Name == name);
        }

        /// <summary>
        /// Writes the contents of the document to a markup document writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupDocumentWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (AttachmentData attachment in attachments)
                writer.Attach(Attachment.FromAttachmentData(attachment));

            foreach (StructuredStream stream in streams)
                stream.WriteTo(writer[stream.Name]);
        }

        /// <summary>
        /// Formats the document to a string by concatenating all formatted streams and
        /// displaying a "*** Stream Name ***" header for each stream name.
        /// </summary>
        /// <returns>The formatted text.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < streams.Count; i++)
            {
                if (i != 0)
                    builder.Append('\n');

                StructuredStream stream = streams[i];
                builder.Append("*** ").Append(stream.Name).Append(" ***").Append("\n\n");

                string contents = stream.ToString();
                builder.Append(contents);
                if (!contents.EndsWith("\n"))
                    builder.Append('\n');
            }

            return builder.ToString();
        }
    }
}