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
using Gallio.Framework.Data.Generation;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using System.Collections;
using System.Text;
using Gallio.Framework;
using Gallio.Common;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of random <see cref="Decimal"/> values as a data source.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Initialize the random number generator by setting the 3 named parameters <see cref="Minimum"/>, 
    /// <see cref="Maximum"/>, and <see cref="Count"/> according to the desired number of values, and to
    /// the expected range of numbers.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod([RandomNumbers(Minimum = 0, Maximum = 10, Count = 3)] decimal value)
    ///     {
    ///         // This test will run 3 times. It generates at each iteration
    ///         // a decimal number between 0 and 10.
    ///     }
    /// }]]></code>
    /// </example>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class RandomNumbersAttribute : GenerationDataAttribute
    {
        private double? minimum = null;
        private double? maximum = null;
        private int? count = null;

        /// <summary>
        /// Gets or sets the lower bound of the numeric range where random values are going to be generated.
        /// </summary>
        public double Minimum
        {
            get
            {
                return minimum ?? 0;
            }

            set
            {
                minimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the upper bound of the numeric range where random values are going to be generated.
        /// </summary>
        public double Maximum
        {
            get
            {
                return maximum ?? 0;
            }

            set
            {
                maximum = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of random values that are going to be generated.
        /// </summary>
        public int Count
        {
            get
            {
                return count ?? 0;
            }

            set
            {
                count = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of a method present in the test fixture
        /// whose purpose is to prevent some specific values to be generated.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The method must accepts one argument of a type that represents a number, such as <see cref="Decimal"/>, 
        /// <see cref="Double"/>, or <see cref="Int32"/>, and returns a <see cref="Boolean"/> value indicating 
        /// whether the specified value must be accepted or rejected.
        /// </para>
        /// <para>
        /// <example>
        /// <code><![CDATA[
        /// [TestFixture]
        /// public class MyTestFixture
        /// {
        ///     [Test]
        ///     public void Generate_filtered_sequence([RandomNumbers(Minimum = 1, Maximum = 100, Count = 50, Filter = "MyFilter")] int value)
        ///     {
        ///         // Code logic here...
        ///     }
        /// 
        ///     public static bool MyFilter(int number)
        ///     {
        ///         return (n % 3 == 0) || (n % 10 == 0);
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// </para>
        /// </remarks>
        public string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a column of random <see cref="Decimal"/> values.
        /// </summary>
        public RandomNumbersAttribute()
        {
        }

        /// <inheritdoc />
        protected override IGenerator GetGenerator(IPatternScope scope)
        {
            var invoker = MakeFilterInvoker(scope);
            
            try
            {
                return new RandomNumbersGenerator
                {
                    Minimum = (decimal?)minimum,
                    Maximum = (decimal?)maximum, 
                    Count = count,
                    Filter = invoker
                };
            }
            catch (GenerationException exception)
            {
                throw new PatternUsageErrorException(String.Format(
                    "The random numbers generator was incorrectly initialized ({0}).", exception.Message), exception);
            }
        }

        private Func<decimal, bool> MakeFilterInvoker(IPatternScope scope)
        {
            if (Filter == null)
                return null;

            var invoker = new FixtureMemberInvoker<bool>(null, scope, Filter);
            return d => invoker.Invoke(d);
        }
    }
}
