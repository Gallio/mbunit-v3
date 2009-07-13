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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Xml;

namespace Gallio.Common.Markup.Tags
{
    /// <summary>
    /// An embedded attachment tag.
    /// </summary>
    [Serializable]
    [XmlRoot("embed", Namespace = SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class EmbedTag : Tag, ICloneable<EmbedTag>, IEquatable<EmbedTag>
    {
        private string attachmentName;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private EmbedTag()
        {
        }

        /// <summary>
        /// Creates an initialized tag.
        /// </summary>
        /// <param name="attachmentName">The name of the attachment to embed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null.</exception>
        public EmbedTag(string attachmentName)
        {
            if (attachmentName == null)
                throw new ArgumentNullException(@"attachmentName");

            this.attachmentName = attachmentName;
        }

        /// <summary>
        /// Gets or sets the name of the referenced attachment to embed, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
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
        public bool Equals(EmbedTag other)
        {
            return other != null
                && attachmentName == other.attachmentName;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as EmbedTag);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 6 ^ attachmentName.GetHashCode();
        }

        /// <inheritdoc />
        new public EmbedTag Clone()
        {
            EmbedTag copy = new EmbedTag();
            copy.attachmentName = attachmentName;
            return copy;
        }

        internal override Tag CloneImpl()
        {
            return Clone();
        }

        internal override void AcceptImpl(ITagVisitor visitor)
        {
            visitor.VisitEmbedTag(this);
        }

        internal override void WriteToImpl(MarkupStreamWriter writer)
        {
            writer.EmbedExisting(attachmentName);
        }
    }
}
