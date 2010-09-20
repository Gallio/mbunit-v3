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

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Quantifier for a regular expression element, 
    /// which specifies the number of times the element must be repeated.
    /// </summary>
    internal sealed class Quantifier
    {
        private readonly int minimum;
        private readonly int maximum;

        /// <summary>
        /// Gets a special quantifier specifying a single occurence of an element (1 time).
        /// </summary>
        public readonly static Quantifier One = new Quantifier(1);

        /// <summary>
        /// Gets a special quantifier specifying the optional occurence of an element (0 or 1 time).
        /// </summary>
        public readonly static Quantifier ZeroOrOne = new Quantifier(0, 1);

        /// <summary>
        /// Gets the minimum number of times.
        /// </summary>
        public int Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the maximum number of times.
        /// </summary>
        public int Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Constructs a quantifier with a constant value.
        /// </summary>
        /// <param name="repeat">The number of repetitions.</param>
        public Quantifier(int repeat)
        {
            if (repeat < 0)
                throw new ArgumentOutOfRangeException("repeat", "The specified number cannot be negative.");

            minimum = repeat;
            maximum = repeat;
        }

        /// <summary>
        /// Constructs a quantifier specifying a variable number of times.
        /// </summary>
        /// <param name="minimum">The minimum value of the range.</param>
        /// <param name="maximum">The maximum value of the range.</param>
        public Quantifier(int minimum, int maximum)
        {
            if (minimum < 0)
                throw new ArgumentOutOfRangeException("minimum", "The specified number cannot be negative.");
            if (maximum < 0)
                throw new ArgumentOutOfRangeException("maximum", "The specified number cannot be negative.");
            if (minimum > maximum)
                throw new ArgumentException("minimum", "The specified minimum value must be greater than or equal to the maximum value");

            this.minimum = minimum;
            this.maximum = maximum;
        }

        /// <summary>
        /// Returns a random number within the range of the quantifier.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <returns>A random number.</returns>
        public int GetRandomRepeat(Random random)
        {
            return random.Next(minimum, maximum + 1);
        }
    }
}
