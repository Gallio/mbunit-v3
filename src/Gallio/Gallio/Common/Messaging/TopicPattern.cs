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
using System.Text;
using System.Text.RegularExpressions;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// A topic pattern expresses a simple matching rule for topics.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Topic pattern matching proceeds word-wise where words within the topic are
    /// separated by periods ('.').  There are two wildcards: '*' matches a single word and '#'
    /// matches zero or more consecutive words.  The topic must be matched in
    /// full by the topic pattern; partial matches are of no significance.
    /// </para>
    /// </remarks>
    [Serializable]
    public class TopicPattern
    {
        private static readonly Regex PatternRegex = new Regex(@"^(?:[0-9a-zA-Z]*|#|\*)?(?:\.(?:[0-9a-zA-Z]*|#|\*))*$", RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private readonly string pattern;

        [NonSerialized]
        private Regex matchRegex;

        /// <summary>
        /// Creates a topic pattern.
        /// </summary>
        /// <param name="pattern">The topic pattern string.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pattern"/> is null.</exception>
        public TopicPattern(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");
            if (!PatternRegex.IsMatch(pattern))
                throw new ArgumentException("A topic pattern must consist of dot-delimited alphanumeric words with the optional wildcards '*' amd '#'.", "pattern");

            this.pattern = pattern;
        }

        /// <summary>
        /// Gets the topic pattern string.
        /// </summary>
        public string Pattern
        {
            get { return pattern; }
        }

        /// <summary>
        /// Returns true if this pattern matches the specified topic.
        /// </summary>
        /// <param name="topic">The topic to consider.</param>
        /// <returns>True if this pattern matches the specified topic.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="topic"/> is null.</exception>
        public bool IsMatch(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException("topic");

            return GetMatchRegex().IsMatch(topic.Key);
        }

        /// <summary>
        /// Returns the topic pattern string.
        /// </summary>
        /// <returns>The topic pattern string.</returns>
        public override string ToString()
        {
            return pattern;
        }

        private Regex GetMatchRegex()
        {
            if (matchRegex == null)
            {
                matchRegex = new Regex(
                    "^" + Regex.Escape(pattern).Replace(@"\*", "[^.]+").Replace(@"\#", ".*") + "$",
                    RegexOptions.CultureInvariant | RegexOptions.Singleline);
            }

            return matchRegex;
        }
    }
}