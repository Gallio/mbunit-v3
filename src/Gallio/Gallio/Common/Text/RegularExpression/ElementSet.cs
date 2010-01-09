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
using Gallio.Common.Collections;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Represents a arbitrary character among a set of possible values, in a regular expression tree.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is usually the representation of an explicit set, noted as "[...]" in a regular expression.
    /// </para>
    /// </remarks>
    internal class ElementSet : Element
    {
        private const char RangeMetacharacter = '-';
        private readonly char[] characters;

        /// <summary>
        /// Constructs an element representing a set of possible characters.
        /// </summary>
        /// <param name="quantifier">A quantifier specifying how many times the element is repeated.</param>
        /// <param name="raw">A raw string containing the characters to include into the set, 
        /// expressed in the usual regular expression syntax (example: "ABCX-Z0-9").</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantifier"/> or <paramref name="raw"/> are null.</exception>
        public ElementSet(Quantifier quantifier, string raw)
            : base(quantifier)
        {
            if (raw == null)
                throw new ArgumentNullException("raw");

            this.characters = Parse(raw);
        }

        /// <summary>
        /// Constructs an element representing a set of possible characters.
        /// </summary>
        /// <param name="quantifier">A quantifier specifying how many times the element is repeated.</param>
        /// <param name="characters">An array of characters to include into the set.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantifier"/> or <paramref name="characters"/> are null.</exception>
        public ElementSet(Quantifier quantifier, char[] characters)
            : base(quantifier)
        {
            if (characters == null)
                throw new ArgumentNullException("characters");

            this.characters = characters;
        }

        private static char[] Parse(string raw)
        {
            var list = new List<char>();

            for (int i = 0; i < raw.Length; i++)
            {
                if (i <= raw.Length - 3 && raw[i + 1] == RangeMetacharacter)
                {
                    char first = raw[i];
                    char last = raw[i + 2];
                    i += 2;

                    if (first > last)
                        throw new RegexLiteException(String.Format("Invalid regular expression range set. " +
                            "Expected the first character '{0}' to be less than the last character '{1}'.", first, last));

                    for (char c = first; c <= last; c++)
                    {
                        list.Add(c);
                    }
                }
                else
                {
                    list.Add(raw[i]);
                }
            }

            return list.ToArray();
        }

        /// <inheritdoc />
        protected override string GetRandomStringImpl()
        {
            int index = Generator.Next(0, characters.Length);
            return characters[index].ToString();
        }
    }
}
