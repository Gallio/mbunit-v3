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
using System.Text.RegularExpressions;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A regex filter matches strings against a given <see cref="Regex" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Null string values are converted to empty strings for regex evaluation purposes.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class RegexFilter : Filter<string>
    {
        private readonly Regex regex;

        /// <summary>
        /// Creates an regex filter.
        /// </summary>
        /// <param name="regex">The regular expression for matching strings.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regex"/> is null.</exception>
        public RegexFilter(Regex regex)
        {
            if (regex == null)
                throw new ArgumentNullException("regex");

            this.regex = regex;
        }

        /// <summary>
        /// Gets the regular expression for matching strings.
        /// </summary>
        public Regex Regex
        {
            get { return regex; }
        }

        /// <inheritdoc />
        public override bool IsMatch(string value)
        {
            return regex.IsMatch(value ?? @"");
        }

        /// <inheritdoc />
        public override void Accept(IFilterVisitor visitor)
        {
            visitor.VisitRegexFilter(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Regex('" + regex + @"', " + regex.Options + @")";
        }
    }
}