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
    ///     public void MyTestMethod([RandomData(0, 10, 3)] int value)
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
        private readonly IGenerator generator;

        /// <summary>
        /// <para>
        /// Adds a column of random <see cref="Int32"/> values.
        /// </para>
        /// </summary>
        /// <param name="minimum">The lower bound of the range.</param>
        /// <param name="maximum">TThe lower bound of the range.</param>
        /// <param name="count">The number of random values to generate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="minimum"/> is greater than the <paramref name="maximum"/>, 
        /// or if <paramref name="minimum"/> or <paramref name="maximum"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MaxValue"/></item>
        /// </list>
        /// </exception>
        [CLSCompliant(false)]
        public RandomDataAttribute(int minimum, int maximum, int count)
        {
            this.generator = new RandomInt32Generator(minimum, maximum, count);
        }

        /// <summary>
        /// <para>
        /// Adds a column of random <see cref="Double"/> values.
        /// </para>
        /// </summary>
        /// <param name="minimum">The lower bound of the range.</param>
        /// <param name="maximum">TThe lower bound of the range.</param>
        /// <param name="count">The number of random values to generate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="minimum"/> is greater than the <paramref name="maximum"/>, 
        /// or if <paramref name="minimum"/> or <paramref name="maximum"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Double.NaN"/></item>
        /// <item><see cref="Double.PositiveInfinity"/></item>
        /// <item><see cref="Double.NegativeInfinity"/></item>
        /// <item><see cref="Double.MinValue"/></item>
        /// <item><see cref="Double.MaxValue"/></item>
        /// </list>
        /// </exception>
        [CLSCompliant(false)]
        public RandomDataAttribute(double minimum, double maximum, int count)
        {
            this.generator = new RandomDoubleGenerator(minimum, maximum, count);
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(), GetMetadata(), false));
        }

        private IEnumerable GetSequence()
        {
            return generator.Run();
        }
    }
}
