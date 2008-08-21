using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// An abstract container tag for representing tags that can contain other tags.
    /// </summary>
    [Serializable]
    public abstract class ContainerTag : Tag, ICloneable<ContainerTag>, IEquatable<ContainerTag>
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

        /// <inheritdoc />
        public bool Equals(ContainerTag other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
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
}