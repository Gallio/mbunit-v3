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
using Gallio.Common.Markup;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Represents a text-encoded attachment.
    /// </summary>
    [Serializable]
    public sealed class TextAttachment : Attachment, IEquatable<TextAttachment>
    {
        private readonly string text;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="contentType">The content type, not null.</param>
        /// <param name="text">The text string, not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="text"/> is null.</exception>
        public TextAttachment(string name, string contentType, string text)
            : base(name, contentType)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            this.text = text;
        }

        /// <summary>
        /// Gets the text of the attachment, not null.
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <inheritdoc />
        public override AttachmentData ToAttachmentData()
        {
            return new AttachmentData(Name, ContentType, AttachmentEncoding.Text, text, null);
        }

        /// <inheritdoc />
        public bool Equals(TextAttachment other)
        {
            return other != null
                && Name == other.Name
                && ContentType == other.ContentType
                && text == other.text;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as TextAttachment);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ContentType.GetHashCode() ^ text.GetHashCode();
        }

        /// <inheritdoc />
        public override Attachment Normalize()
        {
            string normalizedName = MarkupNormalizationUtils.NormalizeAttachmentName(Name);
            string normalizedContentType = MarkupNormalizationUtils.NormalizeContentType(ContentType);
            string normalizedText = MarkupNormalizationUtils.NormalizeText(text);

            if (ReferenceEquals(Name, normalizedName)
                && ReferenceEquals(ContentType, normalizedContentType)
                && ReferenceEquals(text, normalizedText))
                return this;

            return new TextAttachment(normalizedName, normalizedContentType, text);
        }
    }
}