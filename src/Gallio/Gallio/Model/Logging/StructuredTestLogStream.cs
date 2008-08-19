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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Gallio.Utilities;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A structured log stream object represents a recursively structured stream of rich text that
    /// supports embedded attachments, nested sections and marked regions.  Each part of the
    /// text is captured by a tag, some of which are composable and may therefore contain other tags.
    /// </para>
    /// <para>
    /// It is effectively an xml-serializable representation of a test log stream written by
    /// a <see cref="TestLogStreamWriter"/>.
    /// </para>
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class StructuredTestLogStream
    {
        private string name;
        private BodyTag body;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private StructuredTestLogStream()
        {
        }

        /// <summary>
        /// Creates an initialized stream.
        /// </summary>
        /// <param name="name">The stream name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public StructuredTestLogStream(string name)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.name = name;
        }

        /// <summary>
        /// Gets or sets the name of the log stream, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                name = value;
            }
        }

        /// <summary>
        /// Gets or sets the body of the log stream, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("body", IsNullable = false)]
        public BodyTag Body
        {
            get
            {
                if (body == null)
                    body = new BodyTag();
                return body;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                body = value;
            }
        }

        /// <summary>
        /// Writes the structured text to a test log stream writer.
        /// </summary>
        /// <param name="writer">The writer</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null</exception>
        public void WriteTo(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (body != null)
                body.WriteTo(writer);
        }

        /// <summary>
        /// Formats the stream using a <see cref="TagFormatter" />.
        /// </summary>
        /// <returns>The formatted text</returns>
        public override string ToString()
        {
            return body != null ? body.ToString() : string.Empty;
        }

        /// <summary>
        /// An tag represents a portion of the contents of a structured test log stream.
        /// Each one can be thought of as a command that will regenerate the structured
        /// test log stream when written back out.
        /// </summary>
        [Serializable]
        public abstract class Tag : ICloneable
        {
            /// <summary>
            /// Invokes the appropriate visitor method for this tag type.
            /// </summary>
            /// <param name="visitor">The visitor</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is null</exception>
            public void Accept(ITagVisitor visitor)
            {
                if (visitor == null)
                    throw new ArgumentNullException("visitor");
                AcceptImpl(visitor);
            }

            /// <summary>
            /// Formats the tag using a <see cref="TagFormatter" />.
            /// </summary>
            /// <returns>The formatted text</returns>
            public override string ToString()
            {
                TagFormatter formatter = new TagFormatter();
                Accept(formatter);
                return formatter.ToString();
            }

            /// <summary>
            /// Writes the structured text tag to a <see cref="TestLogStreamWriter" />.
            /// </summary>
            /// <param name="writer">The structured text writer</param>
            /// <exception cref="ArgumentNullException">Throw if <paramref name="writer"/> is null</exception>
            public void WriteTo(TestLogStreamWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");
                WriteToImpl(writer);
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            public Tag Clone()
            {
                return CloneImpl();
            }

            object ICloneable.Clone()
            {
                return Clone();
            }

            internal abstract Tag CloneImpl();
            internal abstract void AcceptImpl(ITagVisitor visitor);
            internal abstract void WriteToImpl(TestLogStreamWriter writer);
        }

        /// <summary>
        /// An abstract container tag for representing tags that can contain other tags.
        /// </summary>
        [Serializable]
        public abstract class ContainerTag : Tag
        {
            private readonly List<Tag> contents;

            /// <summary>
            /// Creates an empty container tag.
            /// </summary>
            protected ContainerTag()
            {
                contents = new List<Tag>();
            }

            /// <summary>
            /// Gets the list of nested contents of this tag.
            /// </summary>
            [XmlArray("contents", IsNullable=false)]
            [XmlArrayItem("section", typeof(SectionTag), IsNullable = false)]
            [XmlArrayItem("marker", typeof(MarkerTag), IsNullable = false)]
            [XmlArrayItem("text", typeof(TextTag), IsNullable = false)]
            [XmlArrayItem("embed", typeof(EmbedTag), IsNullable = false)]
            public List<Tag> Contents
            {
                get { return contents; }
            }

            /// <summary>
            /// Invokes the appropriate visitor method each element contained within this tag.
            /// </summary>
            /// <param name="visitor">The visitor</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is null</exception>
            public void AcceptContents(ITagVisitor visitor)
            {
                if (visitor == null)
                    throw new ArgumentNullException("visitor");

                foreach (Tag content in contents)
                    content.AcceptImpl(visitor);
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            new public ContainerTag Clone()
            {
                return (ContainerTag)CloneImpl();
            }

            internal void CopyTo(ContainerTag copy)
            {
                foreach (Tag content in contents)
                    copy.contents.Add((Tag)content.Clone());
            }

            internal override void WriteToImpl(TestLogStreamWriter writer)
            {
                foreach (Tag content in contents)
                    content.WriteToImpl(writer);
            }
        }

        /// <summary>
        /// The top-level container tag of structured text.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class BodyTag : ContainerTag
        {
            internal override void AcceptImpl(ITagVisitor visitor)
            {
                visitor.VisitBodyTag(this);
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            new public BodyTag Clone()
            {
                BodyTag copy = new BodyTag();
                CopyTo(copy);
                return copy;
            }

            internal override Tag CloneImpl()
            {
                return Clone();
            }
        }

        /// <summary>
        /// A section tag.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class SectionTag : ContainerTag
        {
            private string name;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private SectionTag()
            {
            }

            /// <summary>
            /// Creates an initialized tag.
            /// </summary>
            /// <param name="name">The section name</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
            public SectionTag(string name)
            {
                if (name == null)
                    throw new ArgumentNullException(@"name");
                this.name = name;
            }

            /// <summary>
            /// Gets or sets the section name, not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(@"value");
                    name = value;
                }
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            new public SectionTag Clone()
            {
                SectionTag copy = new SectionTag(name);
                CopyTo(copy);
                return copy;
            }

            internal override Tag CloneImpl()
            {
                return Clone();
            }

            internal override void AcceptImpl(ITagVisitor visitor)
            {
                visitor.VisitSectionTag(this);
            }

            internal override void WriteToImpl(TestLogStreamWriter writer)
            {
                using (writer.BeginSection(name))
                    base.WriteToImpl(writer);
            }
        }

        /// <summary>
        /// A marker tag.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class MarkerTag : ContainerTag
        {
            private string @class;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private MarkerTag()
            {
            }

            /// <summary>
            /// Creates an initialized tag.
            /// </summary>
            /// <param name="marker">The marker</param>
            public MarkerTag(Marker marker)
            {
                @class = marker.Class;
            }

            /// <summary>
            /// Gets or sets the marker class, not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            /// <exception cref="ArgumentException">Thrown if the <paramref name="value"/> is not a valid identifier.
            /// <seealso cref="Gallio.Model.Logging.Marker.ValidateClass"/></exception>
            [XmlAttribute("class")]
            public string Class
            {
                get { return @class; }
                set
                {
                    Marker.ValidateClass(@class);
                    @class = value;
                }
            }

            /// <summary>
            /// Gets the marker.
            /// </summary>
            [XmlIgnore]
            public Marker Marker
            {
                get { return new Marker(@class); }
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            new public MarkerTag Clone()
            {
                MarkerTag copy = new MarkerTag();
                copy.@class = @class;
                CopyTo(copy);
                return copy;
            }

            internal override Tag CloneImpl()
            {
                return Clone();
            }

            internal override void AcceptImpl(ITagVisitor visitor)
            {
                visitor.VisitMarkerTag(this);
            }

            internal override void WriteToImpl(TestLogStreamWriter writer)
            {
                using (writer.BeginMarker(Marker))
                    base.WriteToImpl(writer);
            }
        }

        /// <summary>
        /// An embedded attachment tag.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class EmbedTag : Tag
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
            /// <param name="attachmentName">The name of the attachment to embed</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null</exception>
            public EmbedTag(string attachmentName)
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

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
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

            internal override void WriteToImpl(TestLogStreamWriter writer)
            {
                writer.EmbedExisting(attachmentName);
            }
        }

        /// <summary>
        /// A text tag, containing text.
        /// </summary>
        [Serializable]
        public sealed class TextTag : Tag, IXmlSerializable
        {
            private string text;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private TextTag()
            {
            }

            /// <summary>
            /// Creates an initialized text tag.
            /// </summary>
            /// <param name="text">The text within the tag</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null</exception>
            public TextTag(string text)
            {
                if (text == null)
                    throw new ArgumentNullException(@"text");
                this.text = text;
            }

            /// <summary>
            /// Gets or sets the text within the tag, not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            public string Text
            {
                get { return text; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(@"value");
                    text = value;
                }
            }

            /// <summary>
            /// Clones a tag.
            /// </summary>
            /// <returns>The cloned tag</returns>
            new public TextTag Clone()
            {
                TextTag copy = new TextTag();
                copy.text = text;
                return copy;
            }

            internal override Tag CloneImpl()
            {
                return Clone();
            }

            internal override void AcceptImpl(ITagVisitor visitor)
            {
                visitor.VisitTextTag(this);
            }

            internal override void WriteToImpl(TestLogStreamWriter writer)
            {
                writer.Write(text);
            }

            #region Xml Serialization
            XmlSchema IXmlSerializable.GetSchema()
            {
                return null;
            }

            void IXmlSerializable.ReadXml(XmlReader reader)
            {
                bool isEmptyMetadata = reader.IsEmptyElement;
                reader.ReadStartElement(@"text");

                if (isEmptyMetadata)
                    return;

                text = reader.Value;

                reader.ReadEndElement();
            }

            void IXmlSerializable.WriteXml(XmlWriter writer)
            {
                // encode text with linefeeds so that they do not get lost during Xml whitespace stripping
                if (text.Contains("\n"))
                    writer.WriteCData(text);
                else
                    writer.WriteValue(text);
            }
            #endregion
        }

        /// <summary>
        /// Visits a <see cref="Tag" />.
        /// </summary>
        public interface ITagVisitor
        {
            /// <summary>
            /// Visits a body tag.
            /// </summary>
            /// <param name="tag">The tag to visit</param>
            void VisitBodyTag(BodyTag tag);

            /// <summary>
            /// Visits a section tag.
            /// </summary>
            /// <param name="tag">The tag to visit</param>
            void VisitSectionTag(SectionTag tag);

            /// <summary>
            /// Visits a marker tag.
            /// </summary>
            /// <param name="tag">The tag to visit</param>
            void VisitMarkerTag(MarkerTag tag);

            /// <summary>
            /// Visits an embedded attachment tag.
            /// </summary>
            /// <param name="tag">The tag to visit</param>
            void VisitEmbedTag(EmbedTag tag);

            /// <summary>
            /// Visits a text tag.
            /// </summary>
            /// <param name="tag">The tag to visit</param>
            void VisitTextTag(TextTag tag);
        }

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
            /// <param name="spacing">The minimum number of line breaks to insert</param>
            protected void RequestMinimumSpacing(int spacing)
            {
                pendingSpacing = Math.Max(pendingSpacing, spacing);
            }

            /// <summary>
            /// Appends text to the buffer including any requested spacing.
            /// </summary>
            /// <param name="text">The text to append</param>
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
}