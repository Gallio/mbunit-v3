using System;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// A marker tag.
    /// </summary>
    [Serializable]
    [XmlRoot("marker", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class MarkerTag : ContainerTag, ICloneable<MarkerTag>, IEquatable<MarkerTag>
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
                Marker.ValidateClass(value);
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

        /// <inheritdoc />
        new public MarkerTag Clone()
        {
            MarkerTag copy = new MarkerTag();
            copy.@class = @class;
            CopyTo(copy);
            return copy;
        }

        /// <inheritdoc />
        public bool Equals(MarkerTag other)
        {
            return other != null
                && Marker == other.Marker
                && GenericUtils.ElementsEqual(Contents, other.Contents);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as MarkerTag);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 4 ^ Contents.Count;
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
}