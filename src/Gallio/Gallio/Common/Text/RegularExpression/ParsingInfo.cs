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
using Gallio.Common;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Provides information describing how to parse the current expression.
    /// </summary>
    internal class ParsingInfo
    {
        private readonly char initiator;
        private readonly char terminator;
        private readonly Options options;
        private readonly Func<string, Quantifier, IElement> parseToken;
        
        /// <summary>
        /// Primary mode for parsing root expressions.
        /// </summary>
        public readonly static ParsingInfo Root = new ParsingInfo(
            Options.IsRoot | Options.AcceptMetacharacters, 
            Char.MinValue, Char.MinValue,
            (token, quantifier) => new Parser(token, quantifier).Run());

        /// <summary>
        /// Special mode for parsing logical groups "(...)".
        /// </summary>
        public readonly static ParsingInfo Group = new ParsingInfo(
            Options.None, 
            '(', ')',
            (token, quantifier) => new Parser(token, quantifier).Run());

        /// <summary>
        /// Special mode for parsing explicit sets "[...]".
        /// </summary>
        public readonly static ParsingInfo Set = new ParsingInfo(
            Options.None, 
            '[', ']',
            (token, quantifier) => new ElementSet(quantifier, token));

        /// <summary>
        /// Gets the initiator character.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The initiator character is the character that starts of an inner expression.
        /// </para>
        /// </remarks>
        public char Initiator
        {
            get
            {
                return initiator;
            }
        }

        /// <summary>
        /// Gets the terminator character.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The initiator character is the character that ends an inner expression.
        /// </para>
        /// </remarks>
        public char Terminator
        {
            get
            {
                return terminator;
            }
        }

        private ParsingInfo(Options options, char initiator, char terminator, Func<string, Quantifier, IElement> parseToken)
        {
            this.options = options;
            this.initiator = initiator;
            this.terminator = terminator;
            this.parseToken = parseToken;
        }

        /// <summary>
        /// Gets a value indicating if the actual parsing mode is the primary 'Root' mode.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return (options & Options.IsRoot) != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating if the actual parsing mode should process metacharacters, 
        /// or just to ignore them for a later processing.
        /// </summary>
        public bool AcceptMetacharacters
        {
            get
            {
                return (options & Options.AcceptMetacharacters) != 0;
            }
        }

        /// <summary>
        /// Determines whether the specified character is 
        /// the initiator character for the current parsing mode.
        /// </summary>
        /// <param name="character">The character to evaluate</param>
        /// <returns>True if <paramref name="character"/> is an initiator; false otherwise.</returns>
        public bool IsInitiator(char character)
        {
            return !IsRoot && (character == initiator);
        }

        /// <summary>
        /// Determines whether the specified character is 
        /// the terminator character for the current parsing mode.
        /// </summary>
        /// <param name="character">The character to evaluate</param>
        /// <returns>True if <paramref name="character"/> is a terminator; false otherwise.</returns>
        public bool IsTerminator(char character)
        {
            return !IsRoot && (character == terminator);
        }

        /// <summary>
        /// Parses the specified expression in the context of the actual parsing mode.
        /// </summary>
        /// <param name="token">The text to parse.</param>
        /// <param name="quantifier">A quantifier to attach to the resulting element.</param>
        /// <returns>A resulting element representing the expression.</returns>
        public IElement ParseToken(string token, Quantifier quantifier)
        {
            return parseToken(token, quantifier);
        }

        /// <summary>
        /// Returns the information container corresponding to the specified initiator character, 
        /// or the 'Root' parsing mode, if not found.
        /// </summary>
        /// <param name="initiator">The character to evaluate.</param>
        /// <returns>The resulting information container</returns>
        public static ParsingInfo FromInitiator(char initiator)
        {
            if (ParsingInfo.Group.IsInitiator(initiator))
                return ParsingInfo.Group;

            if (ParsingInfo.Set.IsInitiator(initiator))
                return ParsingInfo.Set;

            return ParsingInfo.Root;
        }

        [Flags]
        private enum Options
        {
            None = 0,
            IsRoot = 1,
            AcceptMetacharacters = 2,
        }
    }
}
