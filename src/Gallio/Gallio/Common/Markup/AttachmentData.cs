// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Common.Xml;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// <para>
    /// An Xml-serializable structure that describes the contents, encoding and disposition of an attachment.
    /// </para>
    /// <para>
    /// The contents of the attachment are embedded in the execution log according to
    /// their encoding.  Text is directly embedded into the Xml whereas
    /// binary attachments are base 64 encoded.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class AttachmentData
    {
        private string name;
        private string contentType;
        private string serializedContents;
        private byte[] bytes;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private AttachmentData()
        {
            ContentDisposition = AttachmentContentDisposition.Inline;
        }

        internal AttachmentData(string name, string contentType, AttachmentType type, string serializedContents, byte[] bytes)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (contentType == null)
                throw new ArgumentNullException("contentType");

            ContentDisposition = AttachmentContentDisposition.Inline;
            this.name = name;
            this.contentType = contentType;
            this.Type = type;
            this.serializedContents = serializedContents;
            this.bytes = bytes;
        }

        /// <summary>
        /// Gets or sets the name of the attachment, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type of the attachment as a MIME type, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("contentType")]
        public string ContentType
        {
            get
            {
                return contentType;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                contentType = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the attachment.
        /// This value specifies how the attachment is represented in Xml.
        /// </summary>
        [XmlAttribute("type")]
        public AttachmentType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of the attachment contents relative to
        /// the directory that contains the Xml serialized report,
        /// or null if the content is embedded.
        /// </summary>
        [XmlAttribute("contentPath")]
        public string ContentPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content disposition of the attachment which
        /// indicates how the attachment has been stored.
        /// </summary>
        /// <value>The content disposition, initially <see cref="AttachmentContentDisposition.Inline" /></value>
        [XmlAttribute("contentDisposition")]
        public AttachmentContentDisposition ContentDisposition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the attachment content serialized as text (including Base64 attachments), possibly null if none.
        /// </summary>
        [XmlText]
        public string SerializedContents
        {
            get
            {
                if (serializedContents == null && bytes != null)
                {
                    serializedContents = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
                }

                return serializedContents;
            }

            set
            {
                serializedContents = value;
                bytes = null;
            }
        }

        /// <summary>
        /// Returns true if the attachment is textual, false if it is binary.
        /// </summary>
        [XmlIgnore]
        public bool IsText
        {
            get
            {
                return Type == AttachmentType.Text;
            }
        }

        /// <summary>
        /// Gets the text contents of a text attachment.
        /// </summary>
        /// <returns>The text, or null if not available.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the attachment is not textual.</exception>
        public string GetText()
        {
            if (!IsText)
                throw new InvalidOperationException("The attachment is not text.");

            return serializedContents;
        }

        /// <summary>
        /// Gets the binary contents of a binary attachment.
        /// </summary>
        /// <returns>The bytes, or null if not available.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the attachment is not binary.</exception>
        public byte[] GetBytes()
        {
            if (IsText)
              throw new InvalidOperationException("The attachment is not binary.");

            if (bytes == null && serializedContents != null)
            {
                bytes = Convert.FromBase64String(serializedContents);
            }

            return bytes;
        }

        /// <summary>
        /// Loads the attachment contents from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        /// <exception cref="IOException">If the attachment could not be loaded.</exception>
        public void LoadContents(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (IsText)
            {
                serializedContents = new StreamReader(stream).ReadToEnd();
                bytes = null;
            }
            else
            {
                bytes = new byte[stream.Length];

                if (stream.Read(bytes, 0, (int)stream.Length) != stream.Length)
                    throw new IOException("Did not read entire stream.");

                serializedContents = null;
            }
        }

        /// <summary>
        /// Saves the attachment contents to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The preferred encoding to use if writing text, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        /// <exception cref="IOException">If the attachment could not be saved.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the attachment contents are not available.</exception>
        public void SaveContents(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (serializedContents == null && bytes == null)
                throw new InvalidOperationException("The attachment contents cannot be saved because they are not available.");

            if (IsText)
            {
                using (var writer = new StreamWriter(stream, encoding ?? new UTF8Encoding(false)))
                {
                    writer.Write(serializedContents);
                }
            }
            else
            {
                GetBytes();
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}