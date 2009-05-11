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
using Gallio.Common.Reflection;
using System.Collections;
using Gallio.Framework.Data.Generation;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of sequential values as a data source.
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod1([SequenceData(1, 1, 4)] int value)
    ///     {
    ///         // This test will run 4 times with the values 1, 2, 3 and 4.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod2([SequenceData(0, 2.5, 5)] double value)
    ///     {
    ///         // This test will run 5 times with the values 0, 2.5, 5, 7.5, and 10.
    ///     }
    /// }]]></code>
    /// </example>
    /// </summary>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class SequenceDataAttribute : DataAttribute
    {
        private readonly IGenerator generator;

        /// <summary>
        /// <para>
        /// Adds a column of sequential <see cref="Int32"/> values.
        /// </para>
        /// </summary>
        /// <param name="from">The starting value of the sequence.</param>
        /// <param name="step">The increment between each value of the sequence.</param>
        /// <param name="count">The length of the sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="from"/> or <paramref name="step"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MaxValue"/></item>
        /// </list>
        /// </exception>
        [CLSCompliant(false)]
        public SequenceDataAttribute(int from, int step, int count)
        {
            this.generator = new SequenceInt32Generator(from, step, count);
        }

        /// <summary>
        /// <para>
        /// Adds a column of sequential <see cref="Double"/> values.
        /// </para>
        /// </summary>
        /// <param name="from">The starting value of the sequence.</param>
        /// <param name="step">The increment between each value of the sequence.</param>
        /// <param name="count">The length of the sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is negative, 
        /// or if <paramref name="from"/> or <paramref name="step"/> are one of the following:        
        /// <list type="bullet">
        /// <item><see cref="Double.NaN"/></item>
        /// <item><see cref="Double.PositiveInfinity"/></item>
        /// <item><see cref="Double.NegativeInfinity"/></item>
        /// <item><see cref="Double.MinValue"/></item>
        /// <item><see cref="Double.MaxValue"/></item>
        /// </list>
        /// </exception>
        [CLSCompliant(false)]
        public SequenceDataAttribute(double from, double step, int count)
        {
            this.generator = new SequenceDoubleGenerator(from, step, count);
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(generator.Run(), GetMetadata(), false));
        }
    }
}
