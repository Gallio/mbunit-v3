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

            FilterToken nextToken = LookAhead(lexer, 1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Or)
            {
                GetNextToken(lexer);
                filters.Add(MatchAndFilter(lexer));
                nextToken = LookAhead(lexer, 1);
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

            FilterToken nextToken = LookAhead(lexer, 1);
            while (nextToken != null && nextToken.Type == FilterTokenType.And)
            {
                GetNextToken(lexer);
                filters.Add(MatchNegationFilter(lexer));
                nextToken = LookAhead(lexer, 1);
            }

            if (filters.Count > 1)
                return new AndFilter<T>(filters.ToArray());
            else
                return firstFilter;
        }

        private Filter<T> MatchNegationFilter(FilterLexer lexer)
        {
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken != null && nextToken.Type == FilterTokenType.Not)
            {
                GetNextToken(lexer);
                return new NotFilter<T>(MatchParenthesizedFilter(lexer));
            }

            return MatchParenthesizedFilter(lexer);
        }

        private Filter<T> MatchParenthesizedFilter(FilterLexer lexer)
        {
            Filter<T> filter = null;
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken != null)
            {
                if (nextToken.Type == FilterTokenType.LeftBracket)
                {
                    GetNextToken(lexer);
                    filter = MatchOrFilter(lexer);
                    MatchRightBracket(lexer);
                }
                else if (nextToken.Type == FilterTokenType.Star)
                {
                    GetNextToken(lexer);
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
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                // Should never happen because we call this method when we know a word
                // token is next
                throw new FilterRecognitionException(Resources.FilterParser_StringLiteralExpected);
            }
            GetNextToken(lexer);

            return nextToken.Text;
        }

        private static void MatchColon(FilterLexer lexer)
        {
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Colon)
            {
                throw new FilterRecognitionException(Resources.FilterParser_ColonExpected);
            }
            GetNextToken(lexer);
        }

        private static Filter<string> MatchMatchSequence(FilterLexer lexer)
        {
            List<Filter<string>> values = new List<Filter<string>>();
            values.Add(MatchValue(lexer));

            FilterToken nextToken = LookAhead(lexer, 1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Comma)
            {
                MatchComma(lexer);
                values.Add(MatchValue(lexer));
                nextToken = LookAhead(lexer, 1);
            }

            if (values.Count == 1)
                return values[0];
            else
                return new OrFilter<string>(values.ToArray());
        }

        private static Filter<string> MatchValue(FilterLexer lexer)
        {
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken != null)
            {
                if (nextToken.Type == FilterTokenType.RegexWord)
                {
                    GetNextToken(lexer);
                    RegexOptions options = RegexOptions.Compiled;
                    FilterToken caseInsensitiveToken = LookAhead(lexer, 1);
                    if (caseInsensitiveToken != null && caseInsensitiveToken.Type == FilterTokenType.CaseInsensitiveModifier)
                    {
                        options |= RegexOptions.IgnoreCase;
                        GetNextToken(lexer);
                    }
                    return new RegexFilter(new Regex(nextToken.Text, options));
                }
                else if (IsWord(nextToken))
                {
                    GetNextToken(lexer);
                    return new EqualityFilter<string>(nextToken.Text);
                }
            }

            throw new FilterRecognitionException(Resources.FilterParser_ValueExpected);
        }

        private static void MatchRightBracket(FilterLexer lexer)
        {
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken == null || nextToken.Type != FilterTokenType.RightBracket)
            {
                throw new FilterRecognitionException(Resources.FilterParser_RightBracketExpected);
            }
            GetNextToken(lexer);
        }

        private static void MatchComma(FilterLexer lexer)
        {
            FilterToken nextToken = LookAhead(lexer, 1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Comma)
            {
                // Should never happen because we call this method when we know a comma
                // token is next
                throw new FilterRecognitionException(Resources.FilterParser_CommaExpected);
            }
            GetNextToken(lexer);
        }

        private static bool IsWord(FilterToken token)
        {
            return token.Type == FilterTokenType.Word || token.Type == FilterTokenType.RegexWord;
        }

        private static bool IsNotWord(FilterToken token)
        {
            return !IsWord(token);
        }

        private static void GetNextToken(FilterLexer lexer)
        {
            lexer.GetNextToken();
        }

        private static FilterToken LookAhead(FilterLexer lexer, int index)
        {
            FilterToken token = lexer.LookAhead(index);
            if (token != null && token.Type == FilterTokenType.Error)
            {
                throw new FilterRecognitionException(token.Text);
            }
            return token;
        }
    }

    internal class FilterRecognitionException : Exception
    {
        internal FilterRecognitionException(string message)
            : base(message)
        {
        }
    }
}
