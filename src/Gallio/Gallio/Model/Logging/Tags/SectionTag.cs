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
using Gallio.Common.Collections;
using Gallio.Common.Xml;

namespace Gallio.Model.Logging.Tags
{
    /// <summary>
    /// A section tag.
    /// </summary>
    [Serializable]
    [XmlRoot("section", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class SectionTag : ContainerTag, ICloneable<SectionTag>, IEquatable<SectionTag>
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

        /// <inheritdoc />
        new public SectionTag Clone()
        {
            SectionTag copy = new SectionTag(name);
            CopyTo(copy);
            return copy;
        }

        /// <inheritdoc />
        public bool Equals(SectionTag other)
        {
            return other != null
                && name == other.name
                && GenericCollectionUtils.ElementsEqual(Contents, other.Contents);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as SectionTag);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 3 ^ Contents.Count;
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
}
