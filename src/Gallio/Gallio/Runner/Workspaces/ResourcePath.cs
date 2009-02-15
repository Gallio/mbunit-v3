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
using Gallio.Collections;

namespace Gallio.Runner.Workspaces
{
    /// <summary>
    /// Specifies a resource path as a list of backslash-delimited relative path segments.
    /// </summary>
    [Serializable]
    public struct ResourcePath : IEquatable<ResourcePath>
    {
        private static readonly char[] SplitChars = new char[] { '\\' };

        private string path;

        [NonSerialized]
        private string[] segments;

        /// <summary>
        /// Gets an empty path.
        /// </summary>
        public static readonly ResourcePath Empty = new ResourcePath(string.Empty, EmptyArray<string>.Instance);

        /// <summary>
        /// Gets the root path.
        /// </summary>
        public static readonly ResourcePath Root = new ResourcePath(@"\", EmptyArray<string>.Instance);

        private ResourcePath(string path, string[] segments)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            this.path = path;
            this.segments = segments;
        }

        /// <summary>
        /// Creates a path.
        /// </summary>
        /// <param name="path">The path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public ResourcePath(string path)
            : this(path, null)
        {
        }

        /// <summary>
        /// Returns the path as a backslash-delimited string.
        /// </summary>
        public string Path
        {
            get
            {
                if (path == null)
                    path = String.Join("\\", segments);
                return path; 
            }
        }

        /// <summary>
        /// Gets the separate path segments.
        /// </summary>
        public string[] Segments
        {
            get
            {
                if (segments == null)
                    segments = path.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
                return segments;
            }
        }

        /// <summary>
        /// Returns true if the path is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return path.Length == 0; }
        }

        /// <summary>
        /// Returns true if the path is absolute (has a leading backslash).
        /// </summary>
        public bool IsAbsolute
        {
            get { return path.Length != 0 && path[0] == '\\'; }
        }

        /// <summary>
        /// Gets the containing path, or an empty path if the path is the root.
        /// </summary>
        public ResourcePath Container
        {
            get
            {
                int lastSplit = path.LastIndexOf('\\');
                if (lastSplit < 0)
                    return Empty;
                return new ResourcePath(path.Substring(0, lastSplit));
            }
        }

        /// <summary>
        /// Combines this path with another one and returns the combination.
        /// If the other path is absolute then returns the other path otherwise appends
        /// the path to this one.
        /// </summary>
        /// <param name="other">The other path</param>
        /// <returns>The combined path</returns>
        public ResourcePath CombinedWith(ResourcePath other)
        {
            if (IsEmpty || other.IsAbsolute)
                return other;
            if (other.IsEmpty)
                return this;
            return new ResourcePath(string.Concat(path, "\\", other.path));
        }

        /// <inheritdoc />
        public bool Equals(ResourcePath other)
        {
            return path == other.path;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ResourcePath && Equals((ResourcePath)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return path.GetHashCode();
        }

        /// <summary>
        /// Returns true if the paths are equal.
        /// </summary>
        /// <param name="a">The first path</param>
        /// <param name="b">The second path</param>
        /// <returns>True if the paths are equal</returns>
        public static bool operator ==(ResourcePath a, ResourcePath b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Returns true if the paths are unequal.
        /// </summary>
        /// <param name="a">The first path</param>
        /// <param name="b">The second path</param>
        /// <returns>True if the paths are unequal</returns>
        public static bool operator !=(ResourcePath a, ResourcePath b)
        {
            return ! (a == b);
        }

        /// <summary>
        /// Returns the path.
        /// </summary>
        /// <returns>The path</returns>
        public override string ToString()
        {
            return Path;
        }
    }
}
