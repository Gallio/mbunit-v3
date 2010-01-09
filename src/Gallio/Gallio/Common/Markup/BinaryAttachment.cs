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
using Gallio.Common.Collections;
using Gallio.Common.Normalization;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Represents a binary-encoded attachment.
    /// </summary>
    [Serializable]
    public sealed class BinaryAttachment : Attachment, IEquatable<BinaryAttachment>
    {
        private readonly byte[] bytes;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The attachment name, not null.</param>
        /// <param name="contentType">The content type, not null.</param>
        /// <param name="bytes">The binary data, not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="bytes"/> is null.</exception>
        public BinaryAttachment(string name, string contentType, byte[] bytes)
            : base(name, contentType)
        {
            if (bytes == null)
                throw new ArgumentNullException("bytes");

            this.bytes = bytes;
        }

        /// <summary>
        /// Gets the binary content of the attachment, not null.
        /// </summary>
        public byte[] Bytes
        {
            get { return bytes; }
        }

        /// <inheritdoc />
        public override AttachmentData ToAttachmentData()
        {
            return new AttachmentData(Name, ContentType, AttachmentEncoding.Base64, null, bytes);
        }

        /// <inheritdoc />
        public override Attachment Normalize()
        {
            string normalizedName = MarkupNormalizationUtils.NormalizeAttachmentName(Name);
            string normalizedContentType = MarkupNormalizationUtils.NormalizeContentType(ContentType);

            if (ReferenceEquals(Name, normalizedName)
                && ReferenceEquals(ContentType, normalizedContentType))
                return this;

            return new BinaryAttachment(normalizedName, normalizedContentType, bytes);
        }

        /// <inheritdoc />
        public bool Equals(BinaryAttachment other)
        {
            return other != null
                && Name == other.Name
                && ContentType == other.ContentType
                && GenericCollectionUtils.ElementsEqual(bytes, other.bytes);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BinaryAttachment);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ContentType.GetHashCode() ^ -1;
        }
    }
}