// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Text
{
    /// <summary>
    /// A range denotes a segment of a string or other indexed data structure.
    /// </summary>
    [Serializable]
    public struct Range : IEquatable<Range>
    {
        private readonly int startIndex;
        private readonly int length;

        /// <summary>
        /// Creates a range of indices.
        /// </summary>
        /// <param name="startIndex">The starting index of the range.</param>
        /// <param name="length">The number of characters within the range, may be 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="startIndex"/>
        /// or <paramref name="length"/> is negative.</exception>
        public Range(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", startIndex, "Index must be non-negative.");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", startIndex, "Length must be non-negative.");

            this.startIndex = startIndex;
            this.length = length;
        }

        /// <summary>
        /// Creates a new range between the start index (inclusively) and end index (exclusively).
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        /// <returns>The new range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="startIndex"/>
        /// is negative or greater than <paramref name="endIndex"/>.</exception>
        public static Range Between(int startIndex, int endIndex)
        {
            return new Range(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Gets the starting index of the range.
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }

        /// <summary>
        /// Gets the ending index of the range, which is the start index plus the length.
        /// The ending index is one past the last character within the range.
        /// </summary>
        public int EndIndex
        {
            get { return startIndex + length; }
        }

        /// <summary>
        /// Gets the length of the range.
        /// </summary>
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Gets a substring of the specified text using this range.
        /// </summary>
        /// <param name="text">The source text.</param>
        /// <returns>The substring of the source text that represents this range.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public string SubstringOf(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return text.Substring(startIndex, length);
        }

        /// <summary>
        /// Extends a range into an adjacent range and returns the combined range.
        /// </summary>
        /// <param name="range">The adjacent range.</param>
        /// <returns>The extended range.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="range"/> is not an adjacent range.</exception>
        public Range ExtendWith(Range range)
        {
            if (EndIndex == range.startIndex)
                return new Range(startIndex, length + range.length);
            if (startIndex == range.EndIndex)
                return new Range(range.startIndex, length + range.length);

            throw new ArgumentException("The ranges must be adjacent.", "range");
        }

        /// <inheritdoc />
        public bool Equals(Range other)
        {
            return startIndex == other.startIndex
                && length == other.length;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Range && Equals((Range)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (startIndex << 16) ^ length;
        }

        /// <summary>
        /// Returns true if two ranges are equal.
        /// </summary>
        /// <param name="a">The first range to compare.</param>
        /// <param name="b">The second range to compare.</param>
        /// <returns>True if the ranges are equal.</returns>
        public static bool operator ==(Range a, Range b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if two ranges are not equal.
        /// </summary>
        /// <param name="a">The first range to compare.</param>
        /// <param name="b">The second range to compare.</param>
        /// <returns>True if the ranges are not equal.</returns>
        public static bool operator !=(Range a, Range b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[{0} .. {1})", startIndex, EndIndex);
        }
    }
}