using System;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// An tag represents a portion of the contents of a structured test log stream.
    /// Each one can be thought of as a command that will regenerate the structured
    /// test log stream when written back out.
    /// </summary>
    [Serializable]
    public abstract class Tag : ICloneable<Tag>, IEquatable<Tag>
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
        
        /// <inheritdoc />
        public bool Equals(Tag other)
        {
            return Equals((object) other);
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

        /// <inheritdoc />
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
}