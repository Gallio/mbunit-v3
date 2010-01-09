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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// Specifies a topic key routing published messages to their subscribers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A topic consists of a sequence of dot ('.') delimited case-sensitive words.  Each word
    /// may contain the characters '0'-'9', 'a'-'z' and 'A'-'Z'.  eg. "testrun.1234.progress".
    /// Matching is performed using a <see cref="TopicPattern"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Topic
    {
        private static readonly Regex KeyRegex = new Regex("^[0-9a-zA-Z.]*$", RegexOptions.CultureInvariant | RegexOptions.Singleline);

        private readonly string key;

        /// <summary>
        /// Creates a topic with a given key.
        /// </summary>
        /// <param name="key">The topic key.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is not a valid topic.</exception>
        public Topic(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!KeyRegex.IsMatch(key))
                throw new ArgumentException("A topic key must consist of dot-delimited alphanumeric words.", "key");

            this.key = key;
        }

        /// <summary>
        /// Gets the complete topic key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Returns the topic key.
        /// </summary>
        /// <returns>The topic key.</returns>
        public override string ToString()
        {
            return key;
        }
    }
}