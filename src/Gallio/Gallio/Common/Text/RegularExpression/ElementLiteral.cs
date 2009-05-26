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
using Gallio.Common.Collections;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Represents an invariant literal text in a regular expression tree.
    /// </summary>
    internal class ElementLiteral : Element
    {
        private readonly string literal;

        /// <summary>
        /// Constructs an invariant literal text.
        /// </summary>
        /// <param name="quantifier">A quantifier specifying how many times the element is repeated.</param>
        /// <param name="literal">The invariant literal text.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantifier"/> or <paramref name="literal"/> are null.</exception>
        public ElementLiteral(Quantifier quantifier, string literal)
            : base(quantifier)
        {
            if (literal == null)
                throw new ArgumentNullException("literal");

            this.literal = literal;
        }

        /// <inheritdoc />
        protected override string GetRandomStringImpl()
        {
            return literal;
        }
    }
}
