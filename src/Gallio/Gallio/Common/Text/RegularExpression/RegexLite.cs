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

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// A lightweight simplified regular expression pattern.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following syntactic features are supported:
    /// <list type="bullet">
    /// <item>
    /// <strong>Logical Grouping</strong> : Group a part of the expression (<c>(...)</c>).
    /// </item>
    /// <item>
    /// <strong>Explicit Set</strong> : Define a set of possible characters (<c>[...]</c>). 
    /// Ranges defined with a tiret are accepted.
    /// </item>
    /// <item>
    /// <strong>Explicit Quantifier</strong> : Specify the number of times the previous expression must be repeated. 
    /// 'Constant' (<c>{N}</c>) or 'Range' (<c>{N,M}</c>) syntax are both accepted.
    /// </item>
    /// <item>
    /// <strong>Zero Or One Quantifier Metacharacter</strong> : 0 or 1 of the previous expression (<c>?</c>).
    /// Same effect as <c>{0,1}</c>.
    /// </item>
    /// <item>
    /// <strong>Escape Character</strong> : Makes the next character literal instead of a special character (<c>\</c>).
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    public sealed class RegexLite
    {
        private readonly IElement root;

        /// <summary>
        /// Gets the root element (for testing purpose.)
        /// </summary>
        internal IElement Root
        {
            get
            {
                return root;
            }
        }

        /// <summary>
        /// Constructs a simplified regular expression pattern.
        /// </summary>
        /// <param name="pattern">The input pattern.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pattern"/> is null.</exception>
        /// <exception cref="RegexLiteException">Thrown if <paramref name="pattern"/> is invalid.</exception>
        public RegexLite(string pattern)
        {
            if (pattern == null)
                throw new ArgumentNullException("pattern");

            this.root = Parser.Run(pattern);
        }

        /// <summary>
        /// Returns the random string that matches the regular expression pattern.
        /// </summary>
        /// <returns>A random string that matches the regular expression pattern.</returns>
        public string GetRandomString()
        {
            return root.GetRandomString();
        }
    }
}
