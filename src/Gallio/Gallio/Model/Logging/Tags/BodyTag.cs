using System;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// The top-level container tag of structured text.
    /// </summary>
    [Serializable]
    [XmlRoot("body", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class BodyTag : ContainerTag, ICloneable<BodyTag>, IEquatable<BodyTag>
    {
        /// <inheritdoc />
        new public BodyTag Clone()
        {
            BodyTag copy = new BodyTag();
            CopyTo(copy);
            return copy;
        }

        /// <inheritdoc />
        public bool Equals(BodyTag other)
        {
            return other != null
                && GenericUtils.ElementsEqual(Contents, other.Contents);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BodyTag);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 1 ^ Contents.Count;
        }

        internal override Tag CloneImpl()
        {
            return Clone();
        }

        internal override void AcceptImpl(ITagVisitor visitor)
        {
            visitor.VisitBodyTag(this);
        }
    }
}