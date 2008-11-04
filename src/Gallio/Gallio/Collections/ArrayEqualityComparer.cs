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

using System.Collections.Generic;

namespace Gallio.Collections
{
    /// <summary>
    /// Compares arrays for equality by element.
    /// </summary>
    public class ArrayEqualityComparer<T> : IEqualityComparer<T[]>
    {
        private readonly IEqualityComparer<T> elementComparer;

        /// <summary>
        /// Gets a default instance of the array equality comparer.
        /// </summary>
        public static readonly ArrayEqualityComparer<T> Default = new ArrayEqualityComparer<T>();

        /// <summary>
        /// Creates a default array equality comparer.
        /// </summary>
        public ArrayEqualityComparer()
            : this(null)
        {
        }

        /// <summary>
        /// Creates an array equality comparer using the specified element comparer.
        /// </summary>
        /// <param name="elementComparer">The comparer to use to compare individual elements,
        /// or null to use <see cref="EqualityComparer{T}.Default" /></param>
        public ArrayEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            if (elementComparer == null)
                elementComparer = EqualityComparer<T>.Default;

            this.elementComparer = elementComparer;
        }

        /// <inheritdoc />
        public bool Equals(T[] x, T[] y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null)
                return false;

            int count = x.Length;
            if (y.Length != count)
                return false;

            for (int i = 0; i < count; i++)
            {
                if (! elementComparer.Equals(x[i], y[i]))
                    return false;
            }

            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(T[] obj)
        {
            int hashCode = 0;

            foreach (T value in obj)
            {
                hashCode *= 29;
                hashCode ^= elementComparer.GetHashCode(value);
            }

            return hashCode;
        }
    }
}
