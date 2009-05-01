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
    /// Provides a column of sequential values as a data source.
    /// </para>
    /// <para>
    /// The range of values is determined by the <see cref="From"/> and <see cref="To"/> named parameters.
    /// The increment between each value is optionnally set by the <see cref="Step"/> named parameter.
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
    public class SequenceDataAttribute : DataAttribute
    {
        private double from;
        private bool fromSpecified;
        private double to;
        private bool toSpecified;

        /// <summary>
        /// Gets or sets the starting value of the sequence.
        /// </summary>
        public double From
        {
            get
            {
                return from;
            }
            set
            {
                fromSpecified = true;
                from = value;
            }
        }

        /// <summary>
        /// Gets or sets the ending value of the sequence.
        /// </summary>
        public double To
        {
            get
            {
                return to;
            }
            set
            {
                toSpecified = true;
                to = value;
            }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the increment between each value of the sequence.
        /// </para>
        /// <remark>
        /// This parameter is optional. The default value is 1.
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
        /// <para>
        /// Use the <see cref="From"/> and <see cref="To"/> named parameters to specify the
        /// range of the sequence. Optionnally, use the <see cref="Step"/> named parameter
        /// to specify a increment.
        /// </para>
        /// </summary>
        [CLSCompliant(false)]
        public SequenceDataAttribute()
        {
            Step = 1;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            if (!fromSpecified)
                ThrowUsageErrorException("The 'SequenceData' attribute needs a valid 'From' value.");

            if (!toSpecified)
                ThrowUsageErrorException("The 'SequenceData' attribute needs a valid 'To' value.");

            if (to == from)
                ThrowUsageErrorException("The 'SequenceData' attribute needs distinct 'From' and 'To' values.");

            if (Step == 0)
                ThrowUsageErrorException("Expected the 'SequenceData' step to be different from zero.");

            if (Math.Sign(to - from) != Math.Sign(Step))
                ThrowUsageErrorException("Expected the sign of the 'SequenceData' step to be consistent " +
                    "with the 'From' and 'To' values.");

            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(), GetMetadata(), false));
        }

        private IEnumerable GetSequence()
        {
            for (var n = from; n <= to; n += Step)
            {
                yield return n;
            }
        }
    }
}
