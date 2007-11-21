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
using Gallio.Collections;
using Gallio.Model.Filters;
using Gallio.Model;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// Provides functions for construncting test filters a string representation.
    /// </summary>
    public class FilterParser
    {
        /// <summary>
        /// Parses a description of a list of filters that must be jointly satisfied
        /// in the format "FilterKey1=Value1;FilterKey2=Value2a,Value2b;..."
        /// and constructs a <see cref="Filter{ITest}" /> from it.  The format allows for
        /// compact specification of alternative values delimited by commas.
        /// </summary>
        /// <typeparam name="T">The type to filter</typeparam>
        /// <param name="filterListDescription">The filter list description</param>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterListDescription"/> is null</exception>
        public static Filter<T> ParseFilterList<T>(string filterListDescription)
            where T : IModelComponent
        {
            FilterParser<T> parser = new FilterParser<T>(filterListDescription);
            return parser.ParsedFilter;
        }

        /// <summary>
        /// Parses a description of a filter in the format "FilterKey=Value1,Value2,Value3,..."
        /// and constructs a <see cref="Filter{ITest}" /> from it.  The format allows for
        /// compact specification of alternative values delimited by commas.
        /// </summary>
        /// <param name="filterDescription">The filter description</param>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterDescription"/> is null</exception>
        public static Filter<T> ParseFilter<T>(string filterDescription)
            where T : IModelComponent
        {
            if (filterDescription == null)
                throw new ArgumentNullException(@"filterDescription");

            int equalsPos = filterDescription.IndexOf('=');
            if (equalsPos <= 0)
                throw new ArgumentException("Missing '=' between filter key and values.", "filterDescription");

            string key = filterDescription.Substring(0, equalsPos);
            string values = equalsPos + 1 == filterDescription.Length ? "" : filterDescription.Substring(equalsPos + 1);
            string[] splitValues = values.Split(',');
            return BuildFilter<T>(key, splitValues);
        }

        /// <summary>
        /// Builds a filter given a filter key and a list of accepted values for that filter.
        /// </summary>
        /// <remarks>
        /// Recognizes the following filter keys:
        /// <list type="bullet">
        /// <item>Id: Filter by id</item>
        /// <item>Assembly: Filter by assembly name</item>
        /// <item>Namespace: Filter by namespace name</item>
        /// <item>Type: Filter by type name</item>
        /// <item>Member: Filter by member name</item>
        /// <item>*: All other names are assumed to correspond to metadata keys</item>
        /// </list>
        /// </remarks>
        /// <param name="filterKey">The filter key</param>
        /// <param name="filterValues">The accepted values</param>
        /// <typeparam name="T">The type to filter</typeparam>
        /// <returns>The constructed filter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterKey"/> or <paramref name="filterValues"/> is null</exception>
        public static Filter<T> BuildFilter<T>(string filterKey, string[] filterValues)
            where T : IModelComponent
        {
            if (filterKey == null)
                throw new ArgumentNullException(@"filterKey");
            if (filterValues == null)
                throw new ArgumentNullException(@"filterValues");

            Filter<T>[] filters = GenericUtils.ConvertAllToArray<string, Filter<T>>(filterValues, delegate(string filterValue)
            {
                return BuildFilter<T>(filterKey, filterValue);
            });

            return new OrFilter<T>(filters);
        }

        private static Filter<T> BuildFilter<T>(string filterKey, string filterValue)
            where T : IModelComponent
        {
            switch (filterKey)
            {
                case "Id":
                    return new IdFilter<T>(filterValue);
                case "Assembly":
                    return new AssemblyFilter<T>(filterValue);
                case "Namespace":
                    return new NamespaceFilter<T>(filterValue);
                case "Type":
                    return new TypeFilter<T>(filterValue, true);
                case "Member":
                    return new MemberFilter<T>(filterValue);
                default:
                    return new MetadataFilter<T>(filterKey, filterValue);
            }
        }
    }

    internal class FilterParser<T>
        where T : IModelComponent
    {
        private Filter<T> parsedFilter = null;
        private readonly FilterLexer lexer;

        internal FilterParser(string filterListDescription)
        {
            lexer = new FilterLexer(filterListDescription);
            Parse();
        }

        public Filter<T> ParsedFilter
        {
            get { return parsedFilter; }
        }

        private void Parse()
        {
            parsedFilter = MatchFilter();
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
                    List<Filter<T>> values = new List<Filter<T>>();
                    FilterToken key = MatchKey();
                    MatchColon();
                    List<FilterToken> matchSequence = MatchMatchSequence();
                    if (matchSequence.Count > 1)
                    {
                        foreach (FilterToken value in matchSequence)
                        {
                            values.Add(BuildFilter(key.Text, value.Text));
                        }
                        return new OrFilter<T>(values.ToArray());
                    }
                    else
                    {
                        return BuildFilter(key.Text, matchSequence[0].Text);
                    }
                }
            }

            return filter;
        }

        private FilterToken MatchKey()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                throw new Exception("String literal expected");
            }
            lexer.GetNextToken();

            return nextToken;
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

        private List<FilterToken> MatchMatchSequence()
        {
            List<FilterToken> values = new List<FilterToken>();
            values.Add(MatchValue());
            FilterToken nextToken = lexer.LookAhead(1);
            while (nextToken != null && nextToken.Type == FilterTokenType.Comma)
            {
                MatchComma();
                values.Add(MatchValue());
                nextToken = lexer.LookAhead(1);
            }

            return values;
        }

        private FilterToken MatchValue()
        {
            FilterToken nextToken = lexer.LookAhead(1);
            if (nextToken == null || IsNotWord(nextToken))
            {
                throw new Exception("Value expected");
            }
            lexer.GetNextToken();
            return nextToken;
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

        private static Filter<T> BuildFilter(string filterKey, string filterValue)
        {
            switch (filterKey)
            {
                case "Id":
                    return new IdFilter<T>(filterValue);
                case "Assembly":
                    return new AssemblyFilter<T>(filterValue);
                case "Namespace":
                    return new NamespaceFilter<T>(filterValue);
                case "Type":
                    return new TypeFilter<T>(filterValue, true);
                case "Member":
                    return new MemberFilter<T>(filterValue);
                default:
                    return new MetadataFilter<T>(filterKey, filterValue);
            }
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