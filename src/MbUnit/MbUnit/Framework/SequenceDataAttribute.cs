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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of sequential values as a data source.
    /// </para>
    /// <para>
    /// The range of values is determined by the 'from' and 'to' arguments specified in the constructor.
    /// The increment between each value of the sequence is optionnally set 
    /// by the <see cref="Step"/> named parameter. The default step is 1 or -1, depending on the
    /// direction of the sequence.
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod1([SequenceData(1, 4)] int value)
    ///     {
    ///         // This test will run 4 times with the values 1, 2, 3 and 4.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod2([SequenceData(0, 9, Step = 2.5)] double value)
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
        private readonly double from;
        private readonly double to;

        /// <summary>
        /// <para>
        /// Gets or sets the increment between each value of the sequence.
        /// </para>
        /// <remark>
        /// This parameter is optional. If the the 'from' and 'to' values specified
        /// in the <see cref="SequenceDataAttribute(double, double)"/> constructor define
        /// an ascending sequence, the default value is 1; otherwise it is -1.
        /// </remark>
        /// </summary>
        public double Step
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Adds a column of sequential values.
        /// </para>
        /// </summary>
        /// <param name="from">The starting value of the sequence.</param>
        /// <param name="to">The ending value of the sequence.</param>
        /// <exception cref="ArgumentException">One of the specified arguments is NaN of Infinity.</exception>
        [CLSCompliant(false)]
        public SequenceDataAttribute(double from, double to)
        {
            if (Double.IsNaN(from) || Double.IsInfinity(from))
                throw new ArgumentException("Cannot be NaN of Infinity.", "from");

            if (Double.IsNaN(to) || Double.IsInfinity(to))
                throw new ArgumentException("Cannot be NaN of Infinity.", "to");

            this.from = from;
            this.to = to;
            Step = (Math.Sign(to - from) >= 0 ? 1 : -1);
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            if (Step == 0)
                ThrowUsageErrorException("Expected the 'SequenceData' step to be different from zero.");

            if ((from != to) && (Math.Sign(to - from) != Math.Sign(Step)))
                ThrowUsageErrorException("Expected the sign of the 'SequenceData' step to be consistent " +
                    "with the 'From' and 'To' values.");

            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(), GetMetadata(), false));
        }

        private IEnumerable GetSequence()
        {
            for (double n = from; n <= to; n += Step)
            {
                yield return n;
            }
        }
    }
}
