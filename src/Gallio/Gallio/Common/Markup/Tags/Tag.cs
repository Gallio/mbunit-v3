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
using Gallio.Common;

namespace Gallio.Common.Markup.Tags
{
    /// <summary>
    /// An tag is the fundamental unit of content within a markup stream.
    /// </summary>
    [Serializable]
    public abstract class Tag : ICloneable<Tag>, IEquatable<Tag>, IMarkupStreamWritable
    {
        /// <summary>
        /// Invokes the appropriate visitor method for this tag type.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is null.</exception>
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
        /// <returns>The formatted text.</returns>
        public override string ToString()
        {
            TagFormatter formatter = new TagFormatter();
            Accept(formatter);
            return formatter.ToString();
        }

        /// <summary>
        /// Writes the tag to a <see cref="MarkupStreamWriter" />.
        /// </summary>
        /// <param name="writer">The markup stream writer.</param>
        /// <exception cref="ArgumentNullException">Throw if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupStreamWriter writer)
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
        internal abstract void WriteToImpl(MarkupStreamWriter writer);
    }
}
