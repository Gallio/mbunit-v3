// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace Gallio.Model
{
    /// <summary>
    /// Abstract base class for read-only reflection model objects.
    /// </summary>
    public abstract class BaseInfo
    {
        private readonly object source;

        /// <summary>
        /// Copies the contents of a model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        internal BaseInfo(object source)
        {
            if (source == null)
                throw new ArgumentNullException(@"source");

            this.source = source;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as BaseInfo);
        }

        /// <summary>
        /// Compares this object's source for equality with the other's source.
        /// </summary>
        /// <param name="other">The other object</param>
        /// <returns>True if the objects are equal</returns>
        public bool Equals(BaseInfo other)
        {
            return ReferenceEquals(this, other) || ! ReferenceEquals(other, null) && source.Equals(other.source);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return source.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return source.ToString();
        }

        /// <summary>
        /// Compares two objects for equality.
        /// </summary>
        /// <returns>True if they are equal</returns>
        public static bool operator ==(BaseInfo a, BaseInfo b)
        {
            return ReferenceEquals(a, b) ||
                ! ReferenceEquals(a, null) && ! ReferenceEquals(b, null) && a.source.Equals(b.source);
        }

        /// <summary>
        /// Compares two objects for inequality.
        /// </summary>
        /// <returns>True if they are not equal</returns>
        public static bool operator !=(BaseInfo a, BaseInfo b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Gets the model object wrapped by this instance.
        /// </summary>
        internal object Source
        {
            get { return source; }
        }
    }
}
