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
using System.Text;
using System.Text.RegularExpressions;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Formats filters in such manner that they can be reparsed using the
    /// <see cref="FilterParser{T}" />.
    /// </summary>
    /// <seealso cref="FilterParser{T}"/>
    public class FilterFormatter : IFilterVisitor
    {
        private readonly StringBuilder result = new StringBuilder();

        /// <inheritdoc />
        public void VisitAnyFilter<T>(AnyFilter<T> filter)
        {
            result.Append('*');
        }

        /// <inheritdoc />
        public void VisitNoneFilter<T>(NoneFilter<T> filter)
        {
            result.Append(@"not *");
        }

        /// <inheritdoc />
        public void VisitAndFilter<T>(AndFilter<T> filter)
        {
            FormatConjuction(filter.Filters, @" and ");
        }

        /// <inheritdoc />
        public void VisitOrFilter<T>(OrFilter<T> filter)
        {
            FormatConjuction(filter.Filters, @" or ");
        }

        /// <inheritdoc />
        public void VisitNotFilter<T>(NotFilter<T> filter)
        {
            result.Append(@"not ");
            filter.Filter.Accept(this);
        }

        /// <inheritdoc />
        public void VisitPropertyFilter<T>(PropertyFilter<T> filter)
        {
            AppendPhrase(filter.Key);

            result.Append(@": ");

            filter.ValueFilter.Accept(new ValueFilterFormatter(this));
        }

        /// <inheritdoc />
        public void VisitEqualityFilter<T>(EqualityFilter<T> filter) where T : class, IEquatable<T>
        {
            ThrowNotSupportedException();
        }

        /// <inheritdoc />
        public void VisitRegexFilter(RegexFilter filter)
        {
            ThrowNotSupportedException();
        }

        private void FormatConjuction<T>(Filter<T>[] filters, string conjunction)
        {
            if (filters.Length == 0)
            {
                result.Append('*');
            }
            else if (filters.Length == 1)
            {
                filters[0].Accept(this);
            }
            else
            {
                result.Append('(');

                for (int i = 0; i < filters.Length; i++)
                {
                    if (i != 0)
                        result.Append(conjunction);
                    filters[i].Accept(this);
                }

                result.Append(')');
            }
        }

        /// <summary>
        /// Gets the formatted filter contents.
        /// </summary>
        /// <returns>The formatted filter</returns>
        public override string ToString()
        {
 	        return result.ToString();
        }

        private void AppendPhrase(string phrase)
        {
            if (phrase.Length == 0)
            {
                AppendQuotedString(phrase);
                return;
            }
            else
            {
                for (int i = 0; i < phrase.Length; i++)
                {
                    if (!FilterLexer.IsWordChar(phrase[i]))
                    {
                        AppendQuotedString(phrase);
                        return;
                    }
                }
            }
            
            result.Append(phrase);
        }

        private void AppendRegex(Regex regex)
        {
            AppendEscapedString(regex.ToString(), '/');

            if ((regex.Options & RegexOptions.IgnoreCase) != 0)
                result.Append('i');
        }

        private void AppendQuotedString(string str)
        {
            AppendEscapedString(str, '\'');
        }

        private void AppendEscapedString(string str, char delimiter)
        {
            result.Append(delimiter);

            foreach (char c in str)
            {
                if (c == '\\' || c == delimiter)
                    result.Append('\\');
                result.Append(c);
            }

            result.Append(delimiter);
        }

        private static void ThrowNotSupportedException()
        {
            throw new NotSupportedException("This filter cannot be formatted because its structure is not supported by the filter syntax.");
        }

        private sealed class ValueFilterFormatter : IFilterVisitor
        {
            private readonly FilterFormatter formatter;
            private bool needComma;

            public ValueFilterFormatter(FilterFormatter formatter)
            {
                this.formatter = formatter;
            }

            public void VisitAnyFilter<T>(AnyFilter<T> filter)
            {
                ThrowNotSupportedException();
            }

            public void VisitNoneFilter<T>(NoneFilter<T> filter)
            {
                ThrowNotSupportedException();
            }

            public void VisitAndFilter<T>(AndFilter<T> filter)
            {
                ThrowNotSupportedException();
            }

            public void VisitOrFilter<T>(OrFilter<T> filter)
            {
                Filter<T>[] filters = filter.Filters;
                if (filters.Length == 0)
                {
                    ThrowNotSupportedException();
                }
                else
                {
                    for (int i = 0; i < filters.Length; i++)
                    {
                        if (needComma)
                            formatter.result.Append(", ");
                        filters[i].Accept(this);
                    }
                }
            }

            public void VisitNotFilter<T>(NotFilter<T> filter)
            {
                ThrowNotSupportedException();
            }

            public void VisitPropertyFilter<T>(PropertyFilter<T> filter)
            {
                ThrowNotSupportedException();
            }

            public void VisitEqualityFilter<T>(EqualityFilter<T> filter) where T : class, IEquatable<T>
            {
                EqualityFilter<string> stringFilter = filter as EqualityFilter<string>;
                if (stringFilter == null)
                    ThrowNotSupportedException();

                formatter.AppendPhrase(stringFilter.Comparand);

                needComma = true;
            }

            public void VisitRegexFilter(RegexFilter filter)
            {
                formatter.AppendRegex(filter.Regex);

                needComma = true;
            }
        }
    }
}
