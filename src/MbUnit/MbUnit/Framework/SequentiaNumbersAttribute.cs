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
using Gallio.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of sequential <see cref="Double"/> values as a data source.
    /// </para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod1([SequentialNumbers(Start = 1, Step = 1, Count = 4)] int value)
    ///     {
    ///         // This test will run 4 times with the values 1, 2, 3 and 4.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod2([SequentialNumbers(Start = 0, Stop = 10, Count = 5)] double value)
    ///     {
    ///         // This test will run 5 times with the values 0, 2.5, 5, 7.5, and 10.
    ///     }
    /// }]]></code>
    /// </example>
    /// </summary>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class SequentiaNumbersAttribute : GenerationDataAttribute
    {
        private double? start = null;
        private double? stop = null;
        private double? step = null;
        private int? count = null;

        /// <summary>
        /// Gets or sets the starting value of the sequence.
        /// </summary>
        public double Start
        {
            get
            {
                return start ?? 0;
            }

            set
            {
                start = value;
            }
        }

        /// <summary>
        /// Gets or sets the ending value of the sequence.
        /// </summary>
        public double End
        {
            get
            {
                return stop ?? 0;
            }

            set
            {
                stop = value;
            }
        }

        /// <summary>
        /// Gets or sets the increment between each value of the sequence.
        /// </summary>
        public double Step
        {
            get
            {
                return step ?? 0;
            }

            set
            {
                step = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of the sequence.
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
        /// Adds a column of sequential <see cref="Double"/> values.
        /// </summary>
        public SequentiaNumbersAttribute()
        {
        }

        /// <inheritdoc />
        protected override IGenerator GetGenerator()
        {
            try
            {
                return new SequentialNumbersGenerator
                {
                    Start = (decimal?)start,
                    Stop = (decimal?)stop,
                    Step = (decimal?)step,
                    Count = count
                };
            }
            catch (GenerationException exception)
            {
                ThrowUsageErrorException("The sequential numbers generator was incorrectly initialized.", exception);
                return null; // Make the compiler happy.
            }
        }
    }
}
