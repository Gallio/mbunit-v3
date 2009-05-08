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

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Describes the relative cost of performing a conversion so that different
    /// conversions can be ranked by cost.  A typical direct conversion should have
    /// a cost of one.  Lossy conversions should be more costly.
    /// </summary>
    public struct ConversionCost : IComparable<ConversionCost>
    {
        private const int MaximumValue = 1000000000;
        private const int InvalidValue = int.MaxValue;

        private readonly int value;

        /// <summary>
        /// The conversion has zero cost because no work is required.
        /// </summary>
        /// <value>The associated cost value is 0.</value>
        public static readonly ConversionCost Zero = new ConversionCost(0);

        /// <summary>
        /// The conversion is the best possible conversion available.
        /// Built-in conversions should not use this value.  It should be reserved for
        /// user-created conversions that are intended to override the built-in conversions.
        /// </summary>
        /// <value>The associated cost value is 1.</value>
        public static readonly ConversionCost Best = new ConversionCost(1);

        /// <summary>
        /// The conversion costs a typical amount of work to perform.
        /// It may yet be trumped by a conversion that costs less.
        /// </summary>
        /// <value>The associated cost value is 1000.</value>
        public static readonly ConversionCost Typical = new ConversionCost(1000);

        /// <summary>
        /// The conversion is a poor default choice.  It costs a sufficient number of units
        /// of work to perform to ensure that a non-default conversion will be chosen if possible.
        /// </summary>
        /// <value>The associated cost value is 1000000.</value>
        public static readonly ConversionCost Default = new ConversionCost(1000000);

        /// <summary>
        /// The conversion costs the maximum possible amount of work to perform.
        /// </summary>
        /// <value>The associated cost value is 1000000000.</value>
        public static readonly ConversionCost Maximum = new ConversionCost(MaximumValue);

        /// <summary>
        /// The conversion is not supported.
        /// </summary>
        /// <value>The associated cost value is <see cref="int.MaxValue" />.</value>
        public static readonly ConversionCost Invalid = new ConversionCost(InvalidValue);

        /// <summary>
        /// Creates a conversion cost.
        /// </summary>
        /// <param name="value">The number of units of work required by the conversion</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
        public ConversionCost(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value");
            this.value = value;
        }

        /// <summary>
        /// Gets the cost value.
        /// </summary>
        public int Value
        {
            get { return value; }
        }

        /// <summary>
        /// Returns true if the conversion is not supported.
        /// </summary>
        public bool IsInvalid
        {
            get { return value == InvalidValue; }
        }

        /// <inheritdoc />
        public int CompareTo(ConversionCost other)
        {
            return value.CompareTo(other.value);
        }

        /// <summary>
        /// Adds this conversion cost with the other and returns the sum.
        /// </summary>
        /// <param name="other">The other conversion cost</param>
        /// <returns>The summed conversion cost</returns>
        public ConversionCost Add(ConversionCost other)
        {
            return IsInvalid || other.IsInvalid ? Invalid : new ConversionCost(Math.Min(value + other.value, MaximumValue));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "Conversion Cost: " + value;
        }
    }
}
