// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
    /// Represents an element of a regular expression tree that serves as logical container child elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is usually the representation of a logical group, noted as "(...)" in a regular expression.
    /// </para>
    /// </remarks>
    internal class ElementGroup : Element
    {
        private readonly IElement[] children;

        /// <summary>
        /// Constructs a group element that contains child elements.
        /// </summary>
        /// <param name="quantifier">A quantifier specifying how many times the element is repeated.</param>
        /// <param name="children">The child elements.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantifier"/> or <paramref name="children"/> are null.</exception>
        public ElementGroup(Quantifier quantifier, IEnumerable<IElement> children)
            : base(quantifier)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            this.children = new List<IElement>(children).ToArray();
        }

        /// <inheritdoc />
        protected override string GetRandomStringImpl(Random random)
        {
            var output = new StringBuilder();

            for (int i = 0; i < children.Length; i++)
            {
                output.Append(children[i].GetRandomString(random));
            }

            return output.ToString();
        }
    }
}
