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
using System.Text;

namespace Gallio.Common.Markup.Tags
{
    /// <summary>
    /// <para>
    /// Formats <see cref="Tag" /> instances to plain text.
    /// </para>
    /// <para>
    /// Text tags are written as-is.  Sections introduce paragraph breaks with
    /// the header written out as the first line.  Embedded attachments are
    /// described by name.
    /// </para>
    /// </summary>
    public class TagFormatter : ITagVisitor
    {
        private readonly StringBuilder textBuilder = new StringBuilder();
        private int pendingSpacing;
        private int actualSpacing;

        /// <summary>
        /// Gets the formatted text that has been built.
        /// </summary>
        public override string ToString()
        {
            return textBuilder.ToString();
        }

        /// <inheritdoc />
        public virtual void VisitBodyTag(BodyTag tag)
        {
            RequestMinimumSpacing(2);
            tag.AcceptContents(this);
            RequestMinimumSpacing(2);
        }

        /// <inheritdoc />
        public virtual void VisitSectionTag(SectionTag tag)
        {
            RequestMinimumSpacing(2);
            Append(tag.Name);
            RequestMinimumSpacing(1);
            tag.AcceptContents(this);
            RequestMinimumSpacing(2);
        }

        /// <inheritdoc />
        public virtual void VisitMarkerTag(MarkerTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitEmbedTag(EmbedTag tag)
        {
            RequestMinimumSpacing(1);
            Append(String.Format("[Attachment: {0}]", tag.AttachmentName));
            RequestMinimumSpacing(1);
        }

        /// <inheritdoc />
        public virtual void VisitTextTag(TextTag tag)
        {
            Append(tag.Text);
        }

        /// <summary>
        /// Ensures that the next chunk of text appended is separated by at least the specified
        /// number of line breaks.
        /// </summary>
        /// <param name="spacing">The minimum number of line breaks to insert.</param>
        protected void RequestMinimumSpacing(int spacing)
        {
            pendingSpacing = Math.Max(pendingSpacing, spacing);
        }

        /// <summary>
        /// Appends text to the buffer including any requested spacing.
        /// </summary>
        /// <param name="text">The text to append.</param>
        protected void Append(string text)
        {
            int length = text.Length;
            if (length == 0)
                return;

            if (pendingSpacing != 0)
            {
                if (textBuilder.Length != 0 && pendingSpacing > actualSpacing)
                    textBuilder.Append('\n', pendingSpacing - actualSpacing);

                pendingSpacing = 0;
            }

            textBuilder.EnsureCapacity(textBuilder.Length + length);

            for (int i = 0; i < length; i++)
            {
                char c = text[i];

                if (c == '\r')
                    continue;

                if (c == '\n')
                    actualSpacing += 1;
                else
                    actualSpacing = 0;

                textBuilder.Append(c);
            }
        }
    }
}
