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
        public void NullFilter(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.IsNotNull(lexer);
            Assert.AreEqual(0, lexer.Tokens.Count);
            Assert.IsNull(lexer.GetNextToken());
            Assert.IsNull(lexer.LookAhead(1));
        }

        [RowTest]
        [Row(" ")]
        [Row("\t")]
        [Row("\n \n")]
        public void WhiteSpaceOnly(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 0);
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
        [Row("&", "And")]
        [Row("or", "Or")]
        [Row("OR", "Or")]
        [Row("|", "Or")]
        [Row("not", "Not")]
        [Row("NOT", "Not")]
        [Row("!", "Not")]
        [Row(":", "Colon")]
        [Row(",", "Comma")]
        public void SingleElement(string filter, string type)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken filterToken = lexer.Tokens[0];
            Assert.AreEqual(filterToken.Type, Enum.Parse(typeof(FilterTokenType), type));
            Assert.AreEqual(filterToken.Position, 0);
            Assert.AreEqual(filterToken.Text, null);
        }

        [RowTest]
        // We can't use FilterTokenType directly because it's internal and this method is public
        [Row("anX")]
        [Row("oX")]
        [Row("noX")]
        [Row("an")]
        [Row("o")]
        [Row("no")]
        [Row("andOr")]
        [Row("orAnd")]
        public void MalformedSingleElement(string filter)
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
        [Row("&!", "And", "Not")]
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
        [Row("\"abcdefghijklmnopqrstA)#)348038403[:LÑ\"")]
        [Row("'abcdefghijklmnopqrstA)#)348038403[:LÑ'")]
        public void QuotedElement(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(filter));
        }

        [RowTest]
        [Row("\"abcdefghijklmnopqrst", ExpectedException = typeof(RecognitionException))]
        [Row("'abcdefghijklmnopqrst", ExpectedException = typeof(RecognitionException))]
        public void QuotedElementWithMissingEndQuotationMark(string filter)
        {
            new FilterLexer(filter);
        }

        [RowTest]
        [Row("\"abcde\\\"fghijklmnopqrstA)#)348038403[:LÑ\"")]
        [Row(@"'abcdefg\'hijklmnopqrstA)#)348038403[:LÑ'")]
        public void QuotedElementWithEscapedQuotationMarks(string filter)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, GetUnquotedString(filter));
        }

        [RowTest]
        [Row("\"Author\"", "Author")]
        [Row("\"A\\\"uthor\"", "A\"uthor")]
        [Row("\"Author\\\"\"", "Author\"")]
        [Row("'Author'", "Author")]
        [Row("'A\\'uthor'", "A'uthor")]
        [Row("'Author\\''", "Author'")]
        [Row("'A\\\"uthor'", "A\\\"uthor")]
        [Row("\"Author\\'\"", "Author\\'")]
        public void QuotationMarksAreUnescaped(string filter, string expected)
        {
            FilterLexer lexer = new FilterLexer(filter);
            Assert.AreEqual(lexer.Tokens.Count, 1);
            FilterToken firstFilterToken = lexer.Tokens[0];
            Assert.AreEqual(firstFilterToken.Type, FilterTokenType.QuotedWord);
            Assert.AreEqual(firstFilterToken.Position, 0);
            Assert.AreEqual(firstFilterToken.Text, expected);
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
            return unquotedString;
        }
    }
}
