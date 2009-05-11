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
using System.Collections;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of values.
    /// </summary>
    /// <typeparam name="T">The type of the values returned by the generator.</typeparam>
    public abstract class Generator<T> : IGenerator
    {
        private readonly int count;

        /// <summary>
        /// Gets the length of the sequence of values
        /// created by the generator.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="count">The length of the sequence of values that
        /// the generator must create.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative.</exception>
        protected Generator(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "The length of the sequence cannot be negative.");

            this.count = count;
        }

        /// <inheritdoc/>
        public IEnumerable Run()
        {
            for (int i = 0; i < count; i++)
            {
                yield return GetValue(i);
            }
        }

        /// <summary>
        /// Returns a value of the sequence.
        /// </summary>
        /// <param name="index">The index in the sequence.</param>
        /// <returns>The generated value.</returns>
        protected abstract T GetValue(int index);

        /// <summary>
        /// Checks for the specified value to be not one of the following:
        /// <list type="bullet">
        /// <item><see cref="Double.NaN"/></item>
        /// <item><see cref="Double.PositiveInfinity"/></item>
        /// <item><see cref="Double.NegativeInfinity"/></item>
        /// <item><see cref="Double.MinValue"/></item>
        /// <item><see cref="Double.MaxValue"/></item>
        /// </list>
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value to be checked.</param>
        /// <param name="valueName">A friendly name for the value.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified value is invalid.</exception>
        protected static void CheckValidDouble(double value, string valueName)
        {
            if (Double.IsNaN(value) ||
                Double.IsInfinity(value) ||
                value == Double.MinValue ||
                value == Double.MaxValue)
                throw new ArgumentOutOfRangeException(String.Format("The {0} value cannot be one of the following: " +
                    "Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity, Double.MinValue, " +
                    "Double.MaxValue.", valueName), valueName);
        }

        /// <summary>
        /// Checks for the specified value to be not one of the following:
        /// <list type="bullet">
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MaxValue"/></item>
        /// </list>
        /// </summary>
        /// <param name="value">The <see cref="Double"/> value to be checked.</param>
        /// <param name="valueName">A friendly name for the value.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified value is invalid.</exception>
        protected static void CheckValidInt32(double value, string valueName)
        {
            if (value == Int32.MinValue || value == Int32.MaxValue)
                throw new ArgumentOutOfRangeException(String.Format("The {0} value cannot be one of the following: " +
                    "Int32.MinValue or Int32.MaxValue.", valueName), valueName);
        }
    }
}
