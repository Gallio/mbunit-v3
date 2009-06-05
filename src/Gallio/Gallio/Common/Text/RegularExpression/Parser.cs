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
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Lightweight parser for simplified regular expressions.
    /// </summary>
    internal sealed class Parser
    {
        private readonly static Regex ConstantQuantifier = new Regex(@"^\{(?<cst>\d+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly static Regex RangedQuantifier = new Regex(@"^\{(?<min>\d+),(?<max>\d+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private readonly string input;
        private readonly ParsingInfo parsingInfo;
        private readonly Token token = new Token();
        private readonly Quantifier rootQuantifier;
        private const char EscapeCharacter = '\\';
        private const char OptionalMetaCharacter = '?';
        private int initiatorCount;
        private int index;
        private List<IElement> children;

        /// <summary>
        /// RegexLite text parser.
        /// </summary>
        /// <param name="input">The text to be parsed.</param>
        /// <returns>A root element containing the resulting regular expression tree.</returns>
        public static IElement Run(string input)
        {
            var parser = new Parser(input, Quantifier.One);
            return parser.Run();
        }

        internal Parser(string input, Quantifier quantifier)
            : this(input, quantifier, ParsingInfo.Root)
        {
        }

        internal Parser(string input, Quantifier quantifier, ParsingInfo parsingInfo)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            
            if (quantifier == null)
                throw new ArgumentNullException("quantifier");

            this.input = input;
            this.rootQuantifier = quantifier;
            this.parsingInfo = parsingInfo;
        }

        internal IElement Run()
        {
            Initialize();
            bool escaping = false;

            while (index < input.Length)
            {
                if (!escaping && input[index] == EscapeCharacter)
                {
                    escaping = true;
                }
                else if (!escaping && parsingInfo.IsInitiator(input[index]))
                {
                    initiatorCount++;
                }
                else if (!escaping && parsingInfo.IsTerminator(input[index]))
                {
                    if (initiatorCount == 0)
                    {
                        int length;
                        var quantifier = GetQuantifier(index + 1, out length);
                        index += length;
                        return parsingInfo.ParseToken(token.Close(true), quantifier ?? Quantifier.One);
                    }
                    else
                    {
                        initiatorCount--;
                    }
                }
                else if (!escaping && parsingInfo.IsRoot && !ParsingInfo.FromInitiator(input[index]).IsRoot)
                {
                    FinalizeElement(Quantifier.One);
                    var innerParser = new Parser(input.Substring(index + 1), Quantifier.One, ParsingInfo.FromInitiator(input[index]));
                    children.Add(innerParser.Run());
                    this.index += innerParser.index + 1;
                }
                else
                {
                    int length;
                    var quantifier = GetQuantifier(index + 1, out length);

                    if (parsingInfo.AcceptMetacharacters && quantifier != null)
                    {
                        FinalizeElement(Quantifier.One);
                        token.Open();
                        token.Add(input[index], escaping);
                        FinalizeElement(quantifier);
                        index += length;
                    }
                    else
                    {
                        token.Open();
                        token.Add(input[index], escaping);
                    }

                    escaping = false;
                }

                index++;
            }

            if (!parsingInfo.IsRoot)
            {
                throw new RegexLiteException(String.Format("The specified regular expression cannot be parsed. " +
                    "Expected to find the closing character '{0}', but none was found.", parsingInfo.Terminator));
            }

            return GetResult();
        }

        private void Initialize()
        {
            children = new List<IElement>();
            index = 0;
            initiatorCount = 0;
        }

        private void FinalizeElement(Quantifier quantifier)
        {
            if (token.IsOpen)
            {
                string result = token.Close(false);

                if (result.Length > 0)
                {
                    children.Add(new ElementLiteral(quantifier, result));
                }
            }
        }

        private Quantifier GetQuantifier(int i, out int length)
        {
            if (i < input.Length)
            {
                Match match;

                if (input[i] == OptionalMetaCharacter)
                {
                    length = 1;
                    return Quantifier.ZeroOrOne;
                }
                else if ((match = ConstantQuantifier.Match(input.Substring(i))).Success)
                {
                    length = match.Length;
                    return new Quantifier(Int32.Parse(match.Groups[1].Value));
                }
                else if ((match = RangedQuantifier.Match(input.Substring(i))).Success)
                {
                    length = match.Length;
                    return new Quantifier(Int32.Parse(match.Groups[1].Value), Int32.Parse(match.Groups[2].Value));
                }
            }

            length = 0;
            return null;
        }

        private IElement GetResult()
        {
            if (children.Count == 0 && token.IsOpen && !token.IsEmpty)
            {
                FinalizeElement(rootQuantifier);
            }
            else
            {
                FinalizeElement(Quantifier.One);
            }

            switch (children.Count)
            {
                case 0:
                    return new ElementEmpty();

                case 1:
                    return children[0];

                default:
                    return new ElementGroup(rootQuantifier, children);
            }
        }

        private class Token
        {
            private readonly List<int> escapes = new List<int>();
            private StringBuilder builder;

            public bool IsOpen
            {
                get
                {
                    return builder != null;
                }
            }

            public bool IsEmpty
            {
                get
                {
                    return builder != null && builder.Length == 0;
                }
            }

            public void Open()
            {
                if (builder == null)
                {
                    builder = new StringBuilder();
                    escapes.Clear();
                }
            }

            public string Close(bool keepEscapeCharacters)
            {
                if (builder != null)
                {
                    if (keepEscapeCharacters)
                    {
                        AddEscapeCharacters();
                    }

                    var result = builder.ToString();
                    builder = null;
                    return result;
                }

                return String.Empty;
            }

            private void AddEscapeCharacters()
            {
                for (int i = escapes.Count - 1; i >= 0; i--)
                {
                    builder.Insert(escapes[i], Parser.EscapeCharacter);
                }
            }

            public void Add(char character, bool escape)
            {
                Debug.Assert(builder != null);
                
                if (escape)
                {
                    escapes.Add(builder.Length);
                }

                builder.Append(character);
            }
        }
    }
}
