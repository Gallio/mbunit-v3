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
    /// Generator of random <see cref="Decimal"/> values within a given range.
    /// </summary>
    public class RandomNumbersGenerator : RandomGenerator<decimal>
    {
        /// <summary>
        /// Constructs a generator of random <see cref="Decimal"/> numbers.
        /// </summary>
        public RandomNumbersGenerator()
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<decimal> GetSequence()
        {
            CheckProperty(Minimum.Value, "Minimum");
            CheckProperty(Maximum.Value, "Maximum");

            for (int i = 0; i < Count.Value; i++)
            {
                yield return GetNextRandomValue();
            }
        }

        private decimal GetNextRandomValue()
        {
            // TODO: find a better way to generate a random Decimal value with a range.
            return Minimum.Value + Convert.ToDecimal(InnerGenerator.NextDouble()) * (Maximum.Value - Minimum.Value);
        }
    }
}
