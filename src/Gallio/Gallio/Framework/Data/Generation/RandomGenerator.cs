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

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of random values within a given range.
    /// </summary>
    /// <typeparam name="T">The type of the value to generate.</typeparam>
    public abstract class RandomGenerator<T> : Generator<T>
        where T : IComparable<T>
    {
        private readonly T minimum;
        private readonly T maximum;

        /// <summary>
        /// A pseudo-random number generator.
        /// </summary>
        protected readonly static Random InnerGenerator = new Random();

        /// <summary>
        /// Gets the lower bound of the range.
        /// </summary>
        public T Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the upper bound of the range.
        /// </summary>
        public T Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="count">The length of the sequence of values that
        /// the generator must create.</param>
        /// <param name="minimum">The lower bound of the range.</param>
        /// <param name="maximum">The upper bound of the range.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="minimum"/> is greater than <paramref name="maximum"/>.</exception>
        protected RandomGenerator(T minimum, T maximum, int count)
            : base(count)
        {
            if (minimum.CompareTo(maximum) > 0)
                throw new ArgumentException("The minimum value must be less than or equal to the maximum value.", "minimum");

            this.minimum = minimum;
            this.maximum = maximum;
        }

        /// <inheritdoc/>
        protected sealed override T GetValue(int index)
        {
            return GetNextRandomValue();
        }

        /// <summary>
        /// Returns the next random value.
        /// </summary>
        /// <returns>The next random value.</returns>
        protected abstract T GetNextRandomValue();
    }
}
