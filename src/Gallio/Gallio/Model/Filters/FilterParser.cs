// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Model.Filters;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// <para>
    /// A filter parser constructs filters from its textual representation as a filter expression.
    /// </para>
    /// <para>
    /// The filter grammar is defined as follows:
    /// </para>
    /// <para>
    /// NOTE TO JULIAN:
    /// Please document the filter grammar here.
    /// </para>
    /// </summary>
    public class FilterParser<T>
    {
        private readonly IFilterFactory<T> factory;
        private FilterLexer lexer;

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
            try
            {
                // TODO: Refactor to just pass the lexer around as a parameter in each function.
                lexer = new FilterLexer(filterExpr);
                return MatchFilter();
            }
            finally
            {
                lexer = null;
            }
        }

        private Filter<T> MatchFilter()
        {
            return MatchOrFilter();
        }

        private Filter<T> MatchOrFilter()
        {
            List<Filter<T>> filters = new List<Filter<T>>();
            Filter<T> firstFilter = MatchAndFilter();
            filters.Add(firstFilter);
            
            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Or)
            {
                lexer.GetNextToken();
                filters.Add(MatchAndFilter());
                nextToken = lexer.LookAhead(1);
            }

            if (filters.Count > 1)
                return new OrFilter<T>(filters.ToArray());
            else
                return firstFilter;
        }

        private Filter<T> MatchAndFilter()
        {
            List<Filter<T>> filters = new List<Filter<T>>();
            Filter<T> firstFilter = MatchNegationFilter();
            filters.Add(firstFilter);

            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.And)
            {
                lexer.GetNextToken();
                filters.Add(MatchNegationFilter());
                nextToken = lexer.LookAhead(1);
            }

            if (filters.Count > 1)
                return new AndFilter<T>(filters.ToArray());
            else
                return firstFilter;
        }

        private Filter<T> MatchNegationFilter()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken != null && nextToken.Type == FilterTokenType.Not)
            {
                lexer.GetNextToken();
                return new NotFilter<T>(MatchParenthesizedFilter());
            }

            return MatchParenthesizedFilter();
        }

        private Filter<T> MatchParenthesizedFilter()
        {
            Filter<T> filter = null;
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken != null)
            {
                if (nextToken.Type == FilterTokenType.LeftBracket)
                {
                    lexer.GetNextToken();
                    filter = MatchOrFilter();
                    MatchRightBracket();
                }
                else
                {
                    string key = MatchKey();
                    MatchColon();

                    Filter<string> valueFilter = MatchMatchSequence();
                    return factory.CreateFilter(key, valueFilter);
                }
            }

            return filter;
        }

        private string MatchKey()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                throw new Exception("String literal expected");
            }
            lexer.GetNextToken();

            return nextToken.Text;
        }

        private void MatchColon()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Colon)
            {
                throw new Exception("Colon expected");
            }
            lexer.GetNextToken();
        }

        private Filter<string> MatchMatchSequence()
        {
            List<Filter<string>> values = new List<Filter<string>>();
            values.Add(MatchValue());
            
            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Comma)
            {
                MatchComma();
                values.Add(MatchValue());
                nextToken = lexer.LookAhead(1);
            }

            if (values.Count == 1)
                return values[0];
            else
                return new OrFilter<string>(values.ToArray());
        }

        private Filter<string> MatchValue()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                throw new Exception("Value expected");
            }
            lexer.GetNextToken();
            return new EqualityFilter<string>(nextToken.Text);
        }

        private void MatchRightBracket()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.RightBracket)
            {
                throw new Exception("Right bracket expected");
            }
            lexer.GetNextToken();
        }

        private void MatchComma()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || nextToken.Type != FilterTokenType.Comma)
            {
                throw new Exception("Comma expected");
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
        internal FilterRecognitionException(FilterTokenType expected, FilterToken[] found)
            : base("Expected character '" + expected + "', but '" + found + "' was found instead.")
        {
        }
    }
}