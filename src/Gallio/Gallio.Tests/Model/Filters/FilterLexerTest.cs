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

extern alias MbUnit2;
using System;
using Gallio.Model.Filters;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(FilterLexer))]
    [Author("Julian Hidalgo")]
    public class FilterLexerTest
    {
        [RowTest]
        [Row(null)]
        [Row("")]
        [Row("\t")]
        [Row("\n \n")]
        public void Empty(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.IsNotNull(lexer);
            Assert.AreEqual(0, lexer.Tokens.Count);
            Assert.IsNull(lexer.GetNextToken());
            Assert.IsNull(lexer.LookAhead(1));
            Assert.IsNull(lexer.GetNextToken());
        }

        [RowTest]
        // We can't use FilterTokenType directly because it's internal and this method is public
        [Row("(", "LeftBracket")]
        [Row(")", "RightBracket")]
        [Row(",", "Comma")]
        [Row("and", "And")]
        [Row("And", "And")]
        [Row("aNd", "And")]
        [Row("AND", "And")]        
        [Row("or", "Or")]
        [Row("OR", "Or")]        
        [Row("not", "Not")]
        [Row("NOT", "Not")]        
        [Row(":", "Colon")]
        [Row(",", "Comma")]
        [Row("*", "Star")]
        public void SingleElement(string filter, string type)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken filterToken = lexer.Tokens[0];
            Assert.AreEqual(filterToken.Type, ParseTokenType(type));
            Assert.AreEqual(filterToken.Position, 0);
            Assert.AreEqual(filterToken.Text, null);
        }
                
        //[RowTest]
        //[Row("#", 0)]
        //[Row("aaaaaaaa#bbbbbbb", 8)]
        //[Row("aaaaaaaaa#bbbbbb", 9)]
        //[Row("aaaaaaaaaa#bbbbb", 10)]
        //[Row("aaaaaaaaaaa#bbbb", 11)]
        //public void BadInput(string filter, int position)
        //{
        //    string exceptionMessage = null;
        //    try
        //    {
        //        new FilterLexer(filter);
        //    }
        //    catch (Exception e)
        //    {
        //        exceptionMessage = e.Message;
        //    }
        //    Assert.AreEqual(exceptionMessage, "Unexpected character '#' found at position " + position);
        //}

        [RowTest]
        [Row("anX")]
        [Row("oX")]
        [Row("noX")]
        [Row("an")]
        [Row("o")]
        [Row("no")]
        [Row("andOr")]
        [Row("orAnd")]
        [Row("&")]
        [Row("|")]
        [Row("!")]
        public void UnrecognizedElement(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken filterToken = lexer.Tokens[0];
            Assert.AreEqual(filterToken.Type, FilterTokenType.UnquotedWord);
            Assert.AreEqual(filterToken.Position, 0);
            Assert.AreEqual(filterToken.Text, filter);
        }

        [RowTest]
        [Row("()", "LeftBracket", "RightBracket")]
        [Row(")(", "RightBracket", "LeftBracket")]
        public void TwoElements(string filter, string type1, string type2)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 2);

            FilterToken filterToken = lexer.Tokens[0];
            Assert.AreEqual(filterToken.Type, Enum.Parse(typeof(FilterTokenType), type1));
            Assert.AreEqual(filterToken.Position, 0);
            Assert.AreEqual(filterToken.Text, null);

            filterToken = lexer.Tokens[1];
            Assert.AreEqual(filterToken.Type, Enum.Parse(typeof(FilterTokenType), type2));
        }

        [RowTest]
        [Row("Author", ":", "JulianHidalgo")]
        [Row("Andy", ":", "Ordy")]
        [Row("AndAnd", ":", "NotNot")]
        public void ThreeElements(string key, string colon, string value)
        {
            string filter = key + colon + value;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 3);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.UnquotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, key);

            FilterToken secondFilterToken = lexer.Tokens[1];
            Assert.AreEqual(secondFilterToken.Type, FilterTokenType.Colon);
            Assert.AreEqual(secondFilterToken.Position, key.Length);
            Assert.AreEqual(secondFilterToken.Text, null);

            FilterToken thirdFilterToken = lexer.Tokens[2];
            Assert.AreEqual(thirdFilterToken.Type, FilterTokenType.UnquotedWord);
            Assert.AreEqual(thirdFilterToken.Position, key.Length + 1);
            Assert.AreEqual(thirdFilterToken.Text, value);
        }

        [RowTest]
        [Row("\"abcdefghijklmnopqrstA)#)348038403[:LÑ\"", "QuotedWord")]
        [Row("'abcdefghijklmnopqrstA)#)348038403[:LÑ'", "QuotedWord")]
        [Row("/abcdefghijklmnopqrstA)#)348038403[:LÑ/", "RegexWord")]
        public void DelimiteddElement(string filter, string type)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, ParseTokenType(type));
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(filter));
        }

        [RowTest]
        [Row("\"abcdefghijklmnopqrst")]
        [Row("'abcdefghijklmnopqrst")]
        [Row("/abcdefghijklmnopqrst")]
        public void DelimitedElementWithMissingEndDelimiter(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 2);
            FilterToken token = lexer.Tokens[1];
            Assert.AreEqual(token.Type, FilterTokenType.Error);
            Assert.AreEqual(token.Position, 20);
            Assert.AreEqual(token.Text, null);
        }

        [RowTest]
        [Row("\"abcde\\\"fghijklmnopqrstA)#)348038403[:LÑ\"", "QuotedWord")]
        [Row(@"'abcdefg\'hijklmnopqrstA)#)348038403[:LÑ'", "QuotedWord")]
        [Row(@"/123 456 \/ 789/", "RegexWord")]
        public void DelimitedElementWithEscapedDelimiter(string filter, string tokenType)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, ParseTokenType(tokenType));
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(filter));
        }

        [RowTest]
        [Row("\"Author\"", "Author", "QuotedWord")]
        [Row("\"A\\\"uthor\"", "A\"uthor", "QuotedWord")]
        [Row("\"Author\\\"\"", "Author\"", "QuotedWord")]
        [Row("'Author'", "Author", "QuotedWord")]
        [Row("'A\\'uthor'", "A'uthor", "QuotedWord")]
        [Row("'Author\\''", "Author'", "QuotedWord")]
        [Row("'A\\\"uthor'", "A\\\"uthor", "QuotedWord")]
        [Row("\"Author\\'\"", "Author\\'", "QuotedWord")]
        [Row("/Author/", "Author", "RegexWord")]
        [Row(@"/\/Author\//", "/Author/", "RegexWord")]
        public void DelimitersAreUnescaped(string filter, string expected, string tokenType)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, ParseTokenType(tokenType));
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, expected);
        }

        [RowTest]
        [Row(@"/Regex/", "Regex")]
        [Row(@"//", "")]
        public void Regex(string key, string text)
        {
            string filter = key;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.RegexWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, text);
        }

        [RowTest]
        [Row(@"/Regex/i", "Regex")]
        [Row(@"//i", "")]
        public void CaseInsensitiveRegex(string key, string text)
        {
            string filter = key;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 2);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.RegexWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, text);

            FilterToken secondToken = lexer.Tokens[1];
            Assert.AreEqual(secondToken.Type, FilterTokenType.CaseInsensitiveModifier);
            Assert.AreEqual(secondToken.Position, key.Length - 1);
            Assert.AreEqual(secondToken.Text, null);
        }

        [RowTest]
        [Row(@"/Regex/ i", "Regex")]
        [Row(@"//@i", "")]
        [Row("//\ti", "")]
        [Row(@"//  i", "")]
        public void CaseSensitiveRegex(string key, string text)
        {
            string filter = key;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 2);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.RegexWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, text);

            FilterToken secondToken = lexer.Tokens[1];
            Assert.AreEqual(secondToken.Type, FilterTokenType.UnquotedWord);
        }

        [RowTest]
        [Row("\"Author\"", ":", "\"JulianHidalgo\"")]
        [Row("\"Author\"", ":", "\"Julian.Hidalgo\"")]
        [Row("\"Author\"", ":", "\"Julian.Hidalgo@MbUnit.com\"")]
        public void QuotedElements(string key, string colon, string value)
        {
            string filter = key + colon + value;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 3);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(key));

            FilterToken secondToken = lexer.Tokens[1];
            Assert.AreEqual(secondToken.Type, FilterTokenType.Colon);
            Assert.AreEqual(secondToken.Position, key.Length);
            Assert.AreEqual(secondToken.Text, null);

            FilterToken thirdToken = lexer.Tokens[2];
            Assert.AreEqual(thirdToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(thirdToken.Position, key.Length + 1);
            Assert.AreEqual(thirdToken.Text, GetUnquotedString(value));
        }

        [RowTest]
        [Row("\"Author\"", ":", "\"JulianHidalgo\"", "\"Jeff Brown\"")]
        public void QuotedElementsAndMultipleValues(string key, string colon, string value1, string value2)
        {
            string filter = key + colon + value1 + "," + value2;
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 5);

            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(key));

            FilterToken secondToken = lexer.Tokens[1];
            Assert.AreEqual(secondToken.Type, FilterTokenType.Colon);
            Assert.AreEqual(secondToken.Position, key.Length);
            Assert.AreEqual(secondToken.Text, null);

            FilterToken thirdToken = lexer.Tokens[2];
            Assert.AreEqual(thirdToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(thirdToken.Position, key.Length + 1);
            Assert.AreEqual(thirdToken.Text, GetUnquotedString(value1));

            FilterToken fourthToken = lexer.Tokens[3];
            Assert.AreEqual(fourthToken.Type, FilterTokenType.Comma);
            Assert.AreEqual(fourthToken.Position, key.Length + value1.Length + 1);
            Assert.AreEqual(fourthToken.Text, null);

            FilterToken fifthToken = lexer.Tokens[4];
            Assert.AreEqual(fifthToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(fifthToken.Position, key.Length + value1.Length + 2);
            Assert.AreEqual(fifthToken.Text, GetUnquotedString(value2));
        }

        private static string GetUnquotedString(string quotedString)
        {
            string unquotedString = quotedString.Substring(1, quotedString.Length - 2);
            if (quotedString[0] == '"')
            {
                unquotedString = unquotedString.Replace("\\\"", "\"");
            }
            else if (quotedString[0] == '\'')
            {
                unquotedString = unquotedString.Replace("\\'", "'");
            }
            else if (quotedString[0] == '/')
            {
                unquotedString = unquotedString.Replace(@"\/", "/");
            }
            return unquotedString;
        }

        private static object ParseTokenType(string type)
        {
            return Enum.Parse(typeof(FilterTokenType), type);
        }
    }
}
