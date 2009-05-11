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
    /// Generator of random <see cref="Double"/> values within a given range.
    /// </summary>
    public class RandomDoubleGenerator : RandomGenerator<double>
    {
        /// <summary>
        /// Constructs a generator of random <see cref="Double"/> numbers.
        /// </summary>
        /// <param name="count">The length of the sequence of values that
        /// the generator must create.</param>
        /// <param name="minimum">The lower bound of the range.</param>
        /// <param name="maximum">The upper bound of the range.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="minimum"/> is greater than <paramref name="maximum"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="minimum"/> or <paramref name="maximum"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Double.NaN"/></item>
        /// <item><see cref="Double.PositiveInfinity"/></item>
        /// <item><see cref="Double.NegativeInfinity"/></item>
        /// <item><see cref="Double.MinValue"/></item>
        /// <item><see cref="Double.MaxValue"/></item>
        /// </list>
        /// </exception>
        public RandomDoubleGenerator(double minimum, double maximum, int count)
            :base(minimum, maximum, count)
        {
            CheckValidDouble(minimum, "minimum");
            CheckValidDouble(maximum, "maximum");
        }

        /// <inheritdoc/>
        protected override double GetNextRandomValue()
        {
            return Minimum + InnerGenerator.NextDouble() * (Maximum - Minimum);
        }
    }
}
