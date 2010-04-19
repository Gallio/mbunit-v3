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
            int i = 0;

            while (i < Count.Value)
            {
                var value = GetNextRandomValue();

                if (DoFilter(value))
                {
                    yield return value;
                    i++;
                }
            }
        }

        private decimal GetNextRandomValue()
        {
            return Minimum.Value + RandomDecimal() * (Maximum.Value - Minimum.Value);
        }

        // Provides a pseudo-random decimal number uniformely spread 
        // between 0 (included) and 1 (excluded).
        //
        // 0x204fce5dffffffffffffffff is the highest 96-bit integer number with the first 32-bits 
        // set to 1, and less than 10E28. Thus, the highest decimal value which can be obtained
        // with is method is 0x204fce5dffffffffffffffff / 10E28 = 0.9999999995522011979606654975.
        private static decimal RandomDecimal()
        {
            var data = new byte[8];
            InnerGenerator.NextBytes(data);
            int lo = ToInt32(data, 0);
            int mid = ToInt32(data, 4);
            int hi = InnerGenerator.Next(0x204fce5d);
            return new Decimal(lo, mid, hi, false, 28);
        }

        private static int ToInt32(byte[] bytes, int offset)
        {
            return (int)bytes[offset] 
                | ((int)bytes[offset + 1] << 8) 
                | ((int)bytes[offset + 2] << 16) 
                | ((int)bytes[offset + 3] << 24);
        }
    }
}
