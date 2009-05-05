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
    ///     public void MyTestMethod([SequenceData(From = 1, To = 4)] int value)
    ///     {
    ///         // This test will run 4 times with the values 1, 2, 3 and 4.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod([SequenceData(From = 0, To = 10, Step = 2)] int value)
    ///     {
    ///         // This test will run 6 times with the values 0, 2, 4, 6, 8, and 10.
    ///     }
    /// }]]></code>
    /// </example>
    /// </summary>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class RandomNumberDataAttribute : DataAttribute
    {
        private static readonly Random generator = new Random();
        private readonly int repeat;
        private double minimum;
        private double maximum;

        /// <summary>
        /// 
        /// </summary>
        public double Minimum
        {
            get
            {
                return minimum;
            }

            set
            {
                if (Double.IsNaN(value) ||
                    Double.IsInfinity(value) ||
                    value == Double.MaxValue)
                    throw new ArgumentException("The minimum value cannot be equal to Double.NaN, Double.MaxValue, " +
                        "Double.NegativeInfinity, or Double.PositiveInfinity.", "value");

                minimum = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Maximum
        {
            get
            {
                return maximum;
            }

            set
            {
                if (Double.IsNaN(value) ||
                    Double.IsInfinity(value) ||
                    value == Double.MinValue)
                    throw new ArgumentException("The maximum value cannot be equal to Double.NaN, Double.MinValue, " +
                        "Double.NegativeInfinity, or Double.PositiveInfinity.", "value");

                maximum = value;
            }
        }

        /// <summary>
        /// 
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
        /// <exception cref="ArgumentOutOfRangeException">The argument must be greater than or equal to one.</exception>
        [CLSCompliant(false)]
        public RandomNumberDataAttribute(int repeat)
        {
            if (repeat <= 0)
                throw new ArgumentOutOfRangeException("repeat", "The specified value must be greater than or equal to one.");

            this.repeat = repeat;
            this.minimum = Double.MinValue;
            this.maximum = Double.MaxValue;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {

            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(), GetMetadata(), false));
        }

        private IEnumerable GetSequence()
        {
            var generator = new Random();

            for (int i = 0; i < repeat; i++ )
            {
                yield return GetRandomValue();
            }
        }

        private double GetRandomValue()
        {
            if ((minimum == Double.MinValue) ||
                (minimum == Double.MaxValue))
            {
                return GetUnboundRandomValue();
            }
            else
            {
                return GetBoundRandomValue();
            }
        }

        private double GetUnboundRandomValue()
        {
        }

        private double GetBoundRandomValue()
        {
            return minimum + (maximum - minimum) * generator.NextDouble();
        }
    }
}
