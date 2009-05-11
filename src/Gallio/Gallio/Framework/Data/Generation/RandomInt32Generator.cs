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
    /// Generator of random <see cref="Int32"/> values within a given range.
    /// </summary>
    public class RandomInt32Generator : RandomGenerator<Int32>
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
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MaxValue"/></item>
        /// </list>
        /// </exception>
        public RandomInt32Generator(int minimum, int maximum, int count)
            :base(minimum, maximum, count)
        {
            CheckValidInt32(minimum, "minimum");
            CheckValidInt32(maximum, "maximum");
        }

        /// <inheritdoc/>
        protected override int GetNextRandomValue()
        {
            return InnerGenerator.Next(Minimum, Maximum);
        }
    }
}
