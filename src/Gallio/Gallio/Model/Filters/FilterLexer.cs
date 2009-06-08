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
using System.IO;
using System.Text;
using Gallio.Properties;

namespace Gallio.Model.Filters
{
    internal sealed class FilterLexer
    {
        private readonly static Dictionary<char, FilterTokenType> singleCharacterTokens = new Dictionary<char, FilterTokenType>();
        private readonly static List<char> escapableCharacters = new List<char>();
        private readonly static List<char> wordDelimiters = new List<char>();
        private readonly List<FilterToken> tokens = new List<FilterToken>();
        private readonly StringReader input = null;
        private readonly char escapeCharacter = '\\';
        private int inputPosition = -1;
        private int tokenStreamPosition = -1;

        static FilterLexer()
        {
            AddSingleCharacterTokens();
            AddEscapableCharacters();
            AddWordDelimiters();
        }

        private static void AddSingleCharacterTokens()
        {
            singleCharacterTokens.Add(':', FilterTokenType.Colon);
            singleCharacterTokens.Add('(', FilterTokenType.LeftBracket);
            singleCharacterTokens.Add(')', FilterTokenType.RightBracket);
            singleCharacterTokens.Add(',', FilterTokenType.Comma);
            singleCharacterTokens.Add('*', FilterTokenType.Star);
        }

        private static void AddEscapableCharacters()
        {
            escapableCharacters.Add('"');
            escapableCharacters.Add('\'');
            escapableCharacters.Add('/');
            escapableCharacters.Add(',');
            escapableCharacters.Add('\\');
        }

        private static void AddWordDelimiters()
        {
            wordDelimiters.Add('"');
            wordDelimiters.Add('\'');
            wordDelimiters.Add('/');
        }

        internal FilterLexer(string filter)
        {
            if (filter == null)
            {
                filter = String.Empty;
            }
            input = new StringReader(filter);
            Scan();
        }

        internal List<FilterToken> Tokens
        {
            get { return tokens; }
        }

        internal FilterToken GetNextToken()
        {
            if (tokens.Count > tokenStreamPosition + 1)
            {
                return tokens[++tokenStreamPosition];
            }
            ++tokenStreamPosition;
            return null;
        }

        internal FilterToken LookAhead(int index)
        {
            int position = tokenStreamPosition + index;
            if (tokens.Count > position && position >= 0)
            {
                return tokens[tokenStreamPosition + index];
            }
            else
            {
                return null;
            }
        }

        private void Scan()
        {
            while (input.Peek() != -1)
            {
                char c = (char)input.Peek();
                if (char.IsWhiteSpace(c))
                {
                    input.Read();
                }
                else if (singleCharacterTokens.ContainsKey(c))
                {
                    Match(c.ToString(), singleCharacterTokens[c]);
                }
                else if (IsWordDelimiter(c))
                {
                    MatchDelimitedWord(c);
                    if (c == '/')
                    {
                        MatchCaseInsensitiveModifier();
                    }
                }
                else if (IsWordChar(c))
                {
                    MatchUndelimitedWord();
                }
                else
                {
                    ConsumeNextChar();
                    tokens.Add(new FilterToken(FilterTokenType.Error,
                        String.Format(Resources.FilterParser_UnexpectedCharacterFound, GetCharacterMessage(c)),
                        inputPosition));
                }
            }
        }

        private void MatchUndelimitedWord()
        {
            var chars = new StringBuilder();
            bool errorFound = false;
            int startPosition = inputPosition + 1;
            char previousChar = ConsumeNextChar();
            if (previousChar != escapeCharacter)
            {
                chars.Append(previousChar);
            }
            int nextCharCode = input.Peek();
            while (nextCharCode != -1)
            {
                char nextChar = (char)nextCharCode;
                if (previousChar == escapeCharacter)
                {
                    if (escapableCharacters.Contains(nextChar))
                    {
                        chars.Append(ConsumeNextChar());
                        // Avoid the case when the last slash in an expression like //'
                        // makes the following character to be escaped
                        previousChar = (char)0;
                    }
                    else
                    {
                        tokens.Add(new FilterToken(FilterTokenType.Error,
                            String.Format(Resources.FilterParser_CannotEscapeCharacter, GetCharacterMessage(nextChar)),
                            inputPosition));
                        ConsumeNextChar();
                        errorFound = true;
                        break;
                    }
                }
                else if (IsWordChar(nextChar))
                {
                    previousChar = ConsumeNextChar();
                    chars.Append(previousChar);
                }
                else
                {
                    break;
                }
                nextCharCode = input.Peek();
            }
            if (!errorFound)
            {
                if (previousChar == escapeCharacter)
                {
                    // The escape character was not followed by another character
                    tokens.Add(new FilterToken(FilterTokenType.Error, Resources.FilterParser_MissingEscapedCharacter, inputPosition));
                }
                else
                {
                    string tokenText = chars.ToString();
                    FilterTokenType filterTokenType = GetReservedWord(tokenText);
                    if (filterTokenType != FilterTokenType.None)
                    {
                        tokens.Add(new FilterToken(filterTokenType, null, startPosition));
                    }
                    else
                    {
                        tokens.Add(new FilterToken(FilterTokenType.Word, tokenText, startPosition));
                    }
                }
            }
        }

        private void MatchDelimitedWord(char delimiter)
        {
            var chars = new StringBuilder();
            bool errorFound = false;
            int startPosition = inputPosition + 1;
            char previousChar = (char)0;
            bool finalDelimiterFound = false;

            ConsumeNextChar();
            while (input.Peek() != -1)
            {
                char nextChar = (char)input.Peek();
                if (previousChar == escapeCharacter)
                {
                    if (escapableCharacters.Contains(nextChar))
                    {
                        chars.Append(nextChar);
                        // Avoid the case when the last slash in an expression like //'
                        // makes the following character to be escaped
                        nextChar = (char)0;
                    }
                    else
                    {
                        tokens.Add(new FilterToken(FilterTokenType.Error,
                            String.Format(Resources.FilterParser_CannotEscapeCharacter, GetCharacterMessage(nextChar)),
                            inputPosition));
                        errorFound = true;
                        ConsumeNextChar();
                        break;
                    }
                }
                else if (nextChar == delimiter)
                {
                    ConsumeNextChar();
                    finalDelimiterFound = true;
                    break;
                }
                // If current char is the escape character then hold it
                else if (nextChar != escapeCharacter)
                {
                    chars.Append(nextChar);
                }
                ConsumeNextChar();
                previousChar = nextChar;
            }
            if (!errorFound)
            {
                if (previousChar == escapeCharacter)
                {
                    // The escape character was not followed by another character
                    tokens.Add(new FilterToken(FilterTokenType.Error, Resources.FilterParser_MissingEscapedCharacter, inputPosition));
                }
                else if (!finalDelimiterFound)
                {
                    tokens.Add(new FilterToken(FilterTokenType.Error,
                        String.Format(Resources.FilterParser_MissingEndDelimiter, delimiter),
                        inputPosition));
                }
                else
                {
                    tokens.Add(new FilterToken(GetTokenTypeForDelimiter(delimiter), chars.ToString(), startPosition));
                }
            }
        }

        private void MatchCaseInsensitiveModifier()
        {
            if (input.Peek() != -1)
            {
                char nextChar = (char)input.Peek();
                if (nextChar == 'i')
                {
                    ConsumeNextChar();
                    tokens.Add(new FilterToken(FilterTokenType.CaseInsensitiveModifier, null, inputPosition));
                }
            }
        }

        private static FilterTokenType GetTokenTypeForDelimiter(char c)
        {
            if (c == '/')
                return FilterTokenType.RegexWord;
            return FilterTokenType.Word;
        }

        private char ConsumeNextChar()
        {
            inputPosition++;
            return (char)input.Read();
        }

        private static FilterTokenType GetReservedWord(string token)
        {
            var reservedWord = FilterTokenType.None;
            string loweredToken = token.ToLower();

            if (loweredToken.CompareTo("and") == 0)
            {
                reservedWord = FilterTokenType.And;
            }
            else if (loweredToken.CompareTo("or") == 0)
            {
                reservedWord = FilterTokenType.Or;
            }
            else if (loweredToken.CompareTo("not") == 0)
            {
                reservedWord = FilterTokenType.Not;
            }
            else if (loweredToken.CompareTo("include") == 0)
            {
                reservedWord = FilterTokenType.Include;
            }
            else if (loweredToken.CompareTo("exclude") == 0)
            {
                reservedWord = FilterTokenType.Exclude;
            }

            return reservedWord;
        }

        private bool IsEndOfStream()
        {
            return (input.Peek() == -1);
        }

        private void Match(IEnumerable<char> token, FilterTokenType filterTokenType)
        {
            int startPosition = inputPosition + 1;
            foreach (char c in token)
            {
                if (IsEndOfStream())
                {
                    tokens.Add(new FilterToken(FilterTokenType.Error, Resources.FilterParser_UnexpectedEndOfInput, inputPosition));
                    break;
                }
                char inputChar = (char)input.Peek();
                if (char.ToLower(inputChar) == c)
                {
                    ConsumeNextChar();
                }
                else
                {
                    tokens.Add(new FilterToken(FilterTokenType.Error, GetCharacterMessage(inputChar), inputPosition));
                }
            }
            tokens.Add(new FilterToken(filterTokenType, null, startPosition));
        }

        private static bool IsWordDelimiter(char c)
        {
            return wordDelimiters.Contains(c);
        }

        internal static bool IsWordChar(char c)
        {
            return (!singleCharacterTokens.ContainsKey(c))
                && (!char.IsWhiteSpace(c));
        }

        private static string GetCharacterMessage(char c)
        {
            string message = String.Empty;
            if (!char.IsControl(c))
            {
                message += c + " ";
            }
            message += String.Format(Resources.FilterParser_CharCode, (int)c);
            return message;
        }
    }
}