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
using System.Collections.Generic;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Xml;

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
        private readonly List<Attribute> attributes;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private MarkerTag()
        {
            attributes = new List<Attribute>();
        }

        /// <summary>
        /// Creates an initialized tag.
        /// </summary>
        /// <param name="marker">The marker</param>
        public MarkerTag(Marker marker)
            : this()
        {
            @class = marker.Class;

            foreach (KeyValuePair<string, string> pair in marker.Attributes)
                attributes.Add(new Attribute(pair.Key, pair.Value));
        }

        /// <summary>
        /// Gets or sets the marker class, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="value"/> is not a valid identifier.
        /// <seealso cref="Logging.Marker.ValidateIdentifier"/></exception>
        [XmlAttribute("class")]
        public string Class
        {
            get { return @class; }
            set
            {
                Marker.ValidateIdentifier(value);
                @class = value;
            }
        }

        /// <summary>
        /// Gets the list of marker attributes.
        /// </summary>
        [XmlArray("attributes", IsNullable = false)]
        [XmlArrayItem("attribute", typeof(Attribute), IsNullable = false)]
        public List<Attribute> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Gets the marker.
        /// </summary>
        [XmlIgnore]
        public Marker Marker
        {
            get
            {
                var attributes = new Dictionary<string, string>();
                foreach (Attribute attribute in this.attributes)
                    attributes.Add(attribute.Name, attribute.Value);

                return new Marker(@class, attributes);
            }
        }

        /// <inheritdoc />
        new public MarkerTag Clone()
        {
            MarkerTag copy = new MarkerTag();
            copy.@class = @class;
            foreach (Attribute attribute in attributes)
                copy.attributes.Add(new Attribute(attribute.Name, attribute.Value));
            CopyTo(copy);
            return copy;
        }

        /// <inheritdoc />
        public bool Equals(MarkerTag other)
        {
            return other != null
                && Marker == other.Marker
                && GenericCollectionUtils.ElementsEqual(Contents, other.Contents);
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

        /// <summary>
        /// Represents a marker attribute.
        /// </summary>
        [Serializable]
        [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
        public sealed class Attribute
        {
            private string name;
            private string value;

            /// <summary>
            /// Creates an uninitialized instance for Xml deserialization.
            /// </summary>
            private Attribute()
            {
            }

            /// <summary>
            /// Creates an initialized attribute.
            /// </summary>
            /// <param name="name">The attribute name</param>
            /// <param name="value">The attribute value</param>
            /// <exception cref="ArgumentException">Thrown if <paramref name="name"/>
            /// is invalid</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
            /// or <paramref name="value"/> is null</exception>
            public Attribute(string name, string value)
            {
                Marker.ValidateAttribute(name, value);

                this.name = name;
                this.value = value;
            }

            /// <summary>
            /// Gets or sets the attribute name, not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            /// <exception cref="ArgumentException">Thrown if the <paramref name="value"/> is not a valid identifier.
            /// <seealso cref="Logging.Marker.ValidateIdentifier"/></exception>
            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set
                {
                    Marker.ValidateIdentifier(value);
                    name = value;
                }
            }

            /// <summary>
            /// Gets or sets the attribute value, not null.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
            [XmlAttribute("value")]
            public string Value
            {
                get { return this.value; }
                set
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    this.value = value;
                }
            }
        }
    }
}
