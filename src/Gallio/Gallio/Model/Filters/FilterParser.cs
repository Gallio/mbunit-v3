// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text.RegularExpressions;
using Gallio.Model.Filters;
using Gallio.Properties;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter parser constructs filters from its textual representation as a filter
    /// expression. 
    /// </summary>
    /// <remarks>
    /// <include file='../../../../Gallio/docs/FilterSyntax.xml' path='doc/remarks/*' />
    /// </remarks>
    /// <example>
    /// <include file='../../../../Gallio/docs/FilterSyntax.xml' path='doc/example/*' />
    /// </example>
    /// <seealso cref="FilterFormatter"/>
    public class FilterParser<T>
    {
        private readonly IFilterFactory<T> factory;

        /// <summary>
        /// Creates a filter parser.
        /// </summary>
        /// <param name="factory">The factory to use for constructing
        /// filters based on filter keys and a filter rule for matching string values</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory"/> is null</exception>
        public FilterParser(IFilterFactory<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            this.factory = factory;
        }

        /// <summary>
        /// Creates a filter from its textual representation as a filter expression.
        /// </summary>
        /// <param name="filterExpr">The filter expression</param>
        /// <returns>The parsed filter</returns>
        public Filter<T> Parse(string filterExpr)
        {
            return MatchFilter(new FilterLexer(filterExpr));
        }

        private Filter<T> MatchFilter(FilterLexer lexer)
        {
            if (lexer.Tokens.Count == 0)
                throw new FilterRecognitionException(Resources.FilterParser_EmptyFilterError);
            
            return MatchOrFilter(lexer);
        }

        private Filter<T> MatchOrFilter(FilterLexer lexer)
        {
            List<Filter<T>> filters = new List<Filter<T>>();
            Filter<T> firstFilter = MatchAndFilter(lexer);
            filters.Add(firstFilter);

            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Or)
            {
                lexer.GetNextToken();
                filters.Add(MatchAndFilter(lexer));
                nextToken = lexer.LookAhead(1);
            }

            if (filters.Count > 1)
                return new OrFilter<T>(filters.ToArray());
            else
                return firstFilter;
        }

        private Filter<T> MatchAndFilter(FilterLexer lexer)
        {
            List<Filter<T>> filters = new List<Filter<T>>();
            Filter<T> firstFilter = MatchNegationFilter(lexer);
            filters.Add(firstFilter);

            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.And)
            {
                lexer.GetNextToken();
                filters.Add(MatchNegationFilter(lexer));
                nextToken = lexer.LookAhead(1);
            }

            if (filters.Count > 1)
                return new AndFilter<T>(filters.ToArray());
            else
                return firstFilter;
        }

        private Filter<T> MatchNegationFilter(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken != null && nextToken.Type == FilterTokenType.Not)
            {
                lexer.GetNextToken();
                return new NotFilter<T>(MatchParenthesizedFilter(lexer));
            }

            return MatchParenthesizedFilter(lexer);
        }

        private Filter<T> MatchParenthesizedFilter(FilterLexer lexer)
        {
            Filter<T> filter = null;
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken != null)
            {
                if (nextToken.Type == FilterTokenType.LeftBracket)
                {
                    lexer.GetNextToken();
                    filter = MatchOrFilter(lexer);
                    MatchRightBracket(lexer);
                }
                else if (nextToken.Type == FilterTokenType.Star)
                {
                    lexer.GetNextToken();
                    return new AnyFilter<T>();
                }
                else if (IsWord(nextToken))
                {
                    string key = MatchKey(lexer);
                    MatchColon(lexer);

                    Filter<string> valueFilter = MatchMatchSequence(lexer);
                    return factory.CreateFilter(key, valueFilter);
                }                
                else
                {
                    filter = MatchOrFilter(lexer);
                }
            }

            return filter;
        }

        private static string MatchKey(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                throw new FilterRecognitionException(Resources.FilterParser_StringLiteralExpected);
            }
            lexer.GetNextToken();

            return nextToken.Text;
        }

        private static void MatchColon(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Colon)
            {
                throw new FilterRecognitionException(Resources.FilterParser_ColonExpected);
            }
            lexer.GetNextToken();
        }

        private static Filter<string> MatchMatchSequence(FilterLexer lexer)
        {
            List<Filter<string>> values = new List<Filter<string>>();
            values.Add(MatchValue(lexer));

            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Comma)
            {
                MatchComma(lexer);
                values.Add(MatchValue(lexer));
                nextToken = lexer.LookAhead(1);
            }

            if (values.Count == 1)
                return values[0];
            else
                return new OrFilter<string>(values.ToArray());
        }

        private static Filter<string> MatchValue(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken != null)
            {
                if (nextToken.Type == FilterTokenType.RegexWord)
                {
                    lexer.GetNextToken();
                    RegexOptions options = RegexOptions.Compiled;
                    FilterToken caseInsensitiveToken = lexer.LookAhead(1);
                    if (caseInsensitiveToken != null && caseInsensitiveToken.Type == FilterTokenType.CaseInsensitiveModifier)
                    {
                        options |= RegexOptions.IgnoreCase;
                        lexer.GetNextToken();
                    }
                    return new RegexFilter(new Regex(nextToken.Text, options));
                }
                else if (IsWord(nextToken))
                {
                    lexer.GetNextToken();
                    return new EqualityFilter<string>(nextToken.Text);
                }
            }

            throw new FilterRecognitionException(Resources.FilterParser_ValueExpected);
        }

        private static void MatchRightBracket(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.RightBracket)
            {
                throw new FilterRecognitionException(Resources.FilterParser_RightBracketExpected);
            }
            lexer.GetNextToken();
        }

        private static void MatchComma(FilterLexer lexer)
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Comma)
            {
                throw new FilterRecognitionException(Resources.FilterParser_CommaExpected);
            }
            lexer.GetNextToken();
        }

        private static bool IsWord(FilterToken token)
        {
            return token.Type == FilterTokenType.QuotedWord || token.Type == FilterTokenType.UnquotedWord;
        }

        private static bool IsNotWord(FilterToken token)
        {
            return !IsWord(token);
        }
    }

    internal class FilterRecognitionException : Exception
    {
        internal FilterRecognitionException(string message)
            : base(message)
        {
        }

        internal FilterRecognitionException(FilterTokenType expected, FilterToken[] found)
            : this("Expected character '" + expected + "', but '" + found + "' was found instead.")
        {
        }
    }
}
