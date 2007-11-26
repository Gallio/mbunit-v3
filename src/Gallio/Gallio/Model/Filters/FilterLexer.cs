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
using System.IO;
using System.Text;

namespace Gallio.Model.Filters
{
    internal sealed class FilterLexer
    {
        private readonly List<FilterToken> tokens = new List<FilterToken>();
        private readonly StringReader input = null;
        private int inputPosition = -1;
        private int tokenStreamPosition = -1;

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
            return null;
        }

        internal FilterToken LookAhead(int index)
        {
            if (tokens.Count > tokenStreamPosition + index)
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
            Dictionary<char, FilterTokenType> singleCharacterTokens = GetSingleCharacterTokens();

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
                else if (IsQuotationMark(c))
                {
                    MatchQuotedString(c);
                }
                else if (IsString(c))
                {
                    StringBuilder chars = new StringBuilder();
                    int startPosition = inputPosition + 1;
                    chars.Append(ConsumeNextChar());
                    int nextChar = input.Peek();
                    while (nextChar != -1 && IsString((char)nextChar))
                    {
                        chars.Append(ConsumeNextChar());
                        nextChar = input.Peek();
                    }
                    string token = chars.ToString();
                    FilterTokenType filterTokenType = GetReservedWord(token);
                    if (filterTokenType != FilterTokenType.None)
                    {
                        tokens.Add(new FilterToken(filterTokenType, null, startPosition));
                    }
                    else
                    {
                        tokens.Add(new FilterToken(FilterTokenType.UnquotedWord, token, startPosition));
                    }
                }
                else
                {
                    inputPosition++;
                    input.Read();
                }
            }
        }

        private void MatchQuotedString(char quotationMark)
        {
            StringBuilder chars = new StringBuilder();
            int startPosition = inputPosition + 1;
            char nextChar = (char)0;
            char previousChar = (char)0;
            bool finalQuotationMarkFound = false;

            ConsumeNextChar();
            while (input.Peek() != -1)
            {
                nextChar = (char)input.Peek();
                if (nextChar == quotationMark)
                {
                    if (previousChar != '\\')
                    {
                        ConsumeNextChar();
                        finalQuotationMarkFound = true;
                        break;
                    }
                    else
                    {
                        // Add the quotation mark
                        chars.Append(ConsumeNextChar());
                    }
                }
                else 
                {
                    // previousChar was a \ but not followed by a quotation mark
                    if (previousChar == '\\')
                    {
                        chars.Append('\\');
                    }
                    // If current char is a \ then hold it
                    if (nextChar != '\\')
                    {
                        chars.Append(nextChar);
                    }
                    ConsumeNextChar();
                }
                previousChar = nextChar;
            }
            if (!finalQuotationMarkFound)
            {
                throw new RecognitionException(quotationMark, nextChar);
            }

            tokens.Add(new FilterToken(FilterTokenType.QuotedWord, chars.ToString(), startPosition));
        }

        private char ConsumeNextChar()
        {
            inputPosition++;
            return (char)input.Read();
        }

        private static FilterTokenType GetReservedWord(string token)
        {
            FilterTokenType reservedWord = FilterTokenType.None;
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

            return reservedWord;
        }

        private void CheckEndOfStream()
        {
            if (input.Peek() == -1) throw new UnexpectedEndOfFilterException("Unexpected end of filter");
        }

        private void Match(IEnumerable<char> token, FilterTokenType filterTokenType)
        {
            int startPosition = inputPosition + 1;
            foreach (char c in token)
            {
                CheckEndOfStream();
                char inputChar = (char)input.Peek();
                if (char.ToLower(inputChar) == c)
                {
                    ConsumeNextChar();
                }
                else
                {
                    throw new RecognitionException(c, inputChar);
                }
            }
            tokens.Add(new FilterToken(filterTokenType, null, startPosition));
        }

        private static Dictionary<char, FilterTokenType> GetSingleCharacterTokens()
        {
            Dictionary<char, FilterTokenType> singleCharacterTokens = new Dictionary<char, FilterTokenType>();
            singleCharacterTokens.Add(':', FilterTokenType.Colon);
            singleCharacterTokens.Add('&', FilterTokenType.And);
            singleCharacterTokens.Add('!', FilterTokenType.Not);
            singleCharacterTokens.Add('|', FilterTokenType.Or);
            singleCharacterTokens.Add('(', FilterTokenType.LeftBracket);
            singleCharacterTokens.Add(')', FilterTokenType.RightBracket);
            singleCharacterTokens.Add(',', FilterTokenType.Comma);
            singleCharacterTokens.Add('~', FilterTokenType.Tilde);

            return singleCharacterTokens;
        }

        private static bool IsQuotationMark(char c)
        {
            return c == '"' || c == '\'';
        }

        private static bool IsString(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '\\' || c == '-' || c == '+' || c == '.' || c == '*' || c == '@';
        }
    }

    internal class RecognitionException : Exception
    {
        internal RecognitionException(char expected, char found)
            : base("Expected character '" + expected + "', but '" + found + "' was found instead.")
        {
        }
    }

    internal class UnexpectedEndOfFilterException : Exception
    {
        internal UnexpectedEndOfFilterException(string message)
            : base(message)
        {
        }
    }
}