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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of random <see cref="Double"/> values as a data source.
    /// </para>
    /// <para>
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod([RandomNumbers(Minimum = 0, Maximum = 10, Count = 3)] int value)
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
    public class RandomNumbersAttribute : GenerationDataAttribute
    {
        /// <summary>
        /// Gets or sets the lower bound of the numeric range where random values are going to be generated.
        /// The default value is 0.
        /// </summary>
        public double Minimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the upper bound of the numeric range where random values are going to be generated.
        /// The default value is 1.
        /// </summary>
        public double Maximum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of random values that are going to be generated.
        /// The default value is 3.
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a column of random <see cref="Double"/> values.
        /// </summary>
        [CLSCompliant(false)]
        public RandomNumbersAttribute()
        {
            Minimum = 0d;
            Maximum = 1d;
            Count = 3;
        }

        /// <summary>
        /// Returns a generator of random values.
        /// </summary>
        /// <returns>A generator.</returns>
        protected override IGenerator GetGenerator()
        {
            try
            {
                return new RandomNumbersGenerator(Minimum, Maximum, Count);
            }
            catch(ArgumentException exception)
            {
                ThrowUsageErrorException("The MbUnit random 'Double' data generator was incorrectly initialized.", exception);
                return null; // Make the compiler happy.
            }
        }
    }
}
