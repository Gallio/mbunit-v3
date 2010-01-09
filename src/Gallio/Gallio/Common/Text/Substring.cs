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
    /// A substring provides a convenient representation for a range within a larger string.
    /// </summary>
    public struct Substring : IEquatable<Substring>
    {
        private readonly string content;
        private readonly Range range;

        /// <summary>
        /// Creates a substring object defined over the entire range of a string.
        /// </summary>
        /// <param name="content">The string.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
        public Substring(string content)
        {
            if (content == null)
                throw new ArgumentNullException("content");

            this.content = content;
            range = new Range(0, content.Length);
        }

        /// <summary>
        /// Creates a substring object.
        /// </summary>
        /// <param name="content">The string.</param>
        /// <param name="range">The range within the string.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="range"/> is outside the bounds of the <paramref name="content"/> string.</exception>
        public Substring(string content, Range range)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            if (range.EndIndex > content.Length)
                throw new ArgumentException("The provided range is outside the bounds of the string.", "range");

            this.content = content;
            this.range = range;
        }

        /// <summary>
        /// Returns the whole string.
        /// </summary>
        public string Content
        {
            get { return content; }
        }

        /// <summary>
        /// Returns the range of the underlying string represented by the substring.
        /// </summary>
        public Range Range
        {
            get { return range; }
        }

        /// <summary>
        /// Gets the length of the substring.
        /// </summary>
        public int Length
        {
            get { return range.Length; }
        }

        /// <summary>
        /// Gets the character with a particular index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The character.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("The index is out of range of the substring.");
                return content[index + range.StartIndex];
            }
        }

        /// <summary>
        /// Returns a substring of the substring.
        /// </summary>
        /// <param name="index">The index at which to start.</param>
        /// <returns>The substring.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is negative or
        /// if is greater than <see cref="Length"/>.</exception>
        public Substring Extract(int index)
        {
            if (index < 0 || index > Length)
                throw new ArgumentOutOfRangeException("index", "The index is out of range.");

            return new Substring(content, new Range(range.StartIndex + index, range.Length - index));
        }

        /// <summary>
        /// Returns a substring of the substring.
        /// </summary>
        /// <param name="index">The index at which to start.</param>
        /// <param name="length">The length of the substring to produce.</param>
        /// <returns>The substring.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> or
        /// <paramref name="length"/> are negative or if their sum is greater than <see cref="Length"/>.</exception>
        public Substring Extract(int index, int length)
        {
            if (index < 0 || length < 0 || index + length > Length)
                throw new ArgumentOutOfRangeException("index", "The index or length is out of range.");

            return new Substring(content, new Range(range.StartIndex + index, length));
        }

        /// <summary>
        /// Finds the length of the common prefix of this substring and another one.
        /// </summary>
        /// <param name="other">The other substring.</param>
        /// <returns>The common prefix length, or 0 if the substrings have no prefix in common.</returns>
        public int FindCommonPrefixLength(Substring other)
        {
            int i = range.StartIndex;
            int j = other.range.StartIndex;
            int end = i + Math.Min(Length, other.Length);

            for (; i != end; i++, j++)
                if (content[i] != other.content[j])
                    break;

            return i - range.StartIndex;
        }

        /// <summary>
        /// Finds the length of the common suffix of this substring and another one.
        /// </summary>
        /// <param name="other">The other substring.</param>
        /// <returns>The common suffix length, or 0 if the substrings have no suffix in common.</returns>
        public int FindCommonSuffixLength(Substring other)
        {
            int start = range.EndIndex - 1;
            int i = start;
            int j = other.range.EndIndex - 1;
            int end = start - Math.Min(Length, other.Length);

            for (; i != end; i--, j--)
                if (content[i] != other.content[j])
                    break;

            return start - i;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return range.SubstringOf(content);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Substring && Equals((Substring)obj);
        }

        /// <inheritdoc />
        public bool Equals(Substring other)
        {
            if (Length != other.Length)
                return false;

            int end = range.EndIndex;
            for (int i = range.StartIndex, j = other.range.StartIndex; i != end; i++, j++)
                if (content[i] != other.content[j])
                    return false;

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // FIXME: Optimize this.
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns true if two substrings are equal.
        /// </summary>
        /// <param name="a">The first substring to compare.</param>
        /// <param name="b">The second substring to compare.</param>
        /// <returns>True if the substrings are equal.</returns>
        public static bool operator ==(Substring a, Substring b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if two substrings are not equal.
        /// </summary>
        /// <param name="a">The first substring to compare.</param>
        /// <param name="b">The second substring to compare.</param>
        /// <returns>True if the substrings are not equal.</returns>
        public static bool operator !=(Substring a, Substring b)
        {
            return !(a == b);
        }
    }
}