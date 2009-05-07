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
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using System.Collections;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of random values as a data source.
    /// </para>
    /// <para>
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod([RandomData(3, 0, 10)] int value)
    ///     {
    ///         // This test will run 3 times. It generates at each iteration
    ///         // a integer between 0 and 10.
    ///     }
    /// }]]></code>
    /// </example>
    /// </summary>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class RandomDataAttribute : DataAttribute
    {
        private static readonly Random generator = new Random();
        private readonly int repeat;
        private readonly double minimum;
        private readonly double maximum;

        /// <summary>
        /// Gets the lower bound of the range of possible values.
        /// </summary>
        public double Minimum
        {
            get
            {
                return minimum;
            }
        }

        /// <summary>
        /// Gets the upper bound of the range of possible values.
        /// </summary>
        public double Maximum
        {
            get
            {
                return maximum;
            }
        }

        /// <summary>
        /// Gets the number of times the generator will run.
        /// </summary>
        public double Repeat
        {
            get
            {
                return repeat;
            }
        }

        /// <summary>
        /// <para>
        /// Adds a column of random values.
        /// </para>
        /// </summary>
        /// <param name="repeat">The number of random values to generate.</param>
        /// <param name="minimum">The lower bound of the range.</param>
        /// <param name="maximum">TThe lower bound of the range.</param>
        /// <exception cref="ArgumentOutOfRangeException">The repeat argument is negative or zero, or the 
        /// minimum or maximum values are <see cref="Double.NaN"/>, <see cref="Double.MinValue"/>, 
        /// <see cref="Double.MaxValue"/>, <see cref="Double.PositiveInfinity"/>, 
        /// or <see cref="Double.NegativeInfinity"/>.</exception>
        /// <exception cref="ArgumentException">The maximum value is less than the minimum value.</exception>
        [CLSCompliant(false)]
        public RandomDataAttribute(int repeat, double minimum, double maximum)
        {
            if (repeat <= 0)
                throw new ArgumentOutOfRangeException("repeat", "The specified value must be greater than or equal to one.");

            if (Double.IsNaN(minimum) ||
                Double.IsInfinity(minimum) ||
                minimum == Double.MinValue ||
                minimum == Double.MaxValue)
                throw new ArgumentOutOfRangeException("The minimum value cannot be Double.NaN, Double.MinValue, " +
                    "Double.MaxValue, Double.NegativeInfinity, or Double.PositiveInfinity.", "minimum");

            if (Double.IsNaN(maximum) ||
                Double.IsInfinity(maximum) ||
                maximum == Double.MinValue ||
                maximum == Double.MaxValue)
                throw new ArgumentOutOfRangeException("The maximum value cannot be Double.NaN, Double.MinValue, " +
                    "Double.MaxValue, Double.NegativeInfinity, or Double.PositiveInfinity.", "maximum");

            if (minimum > maximum)
                throw new ArgumentException("The maximum value must be greater than the minimum value.", "maximum");

            this.repeat = repeat;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(), GetMetadata(), false));
        }

        private IEnumerable GetSequence()
        {
            double range = maximum - minimum;

            for (int i = 0; i < repeat; i++)
            {
                yield return minimum + generator.NextDouble() * range;
            }
        }
    }
}
