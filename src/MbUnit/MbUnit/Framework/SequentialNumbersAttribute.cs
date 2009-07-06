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
using Gallio.Common;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides a column of sequential <see cref="Decimal"/> values as a data source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The sequence is initialized by setting 3 of the 4 available named properties. 
    /// The following combinations are possible:
    /// <list type="bullet">
    /// <item><see cref="Start"/>, <see cref="End"/>, and <see cref="Count"/></item>
    /// <item><see cref="Start"/>, <see cref="Step"/>, and <see cref="Count"/></item>
    /// <item><see cref="Start"/>, <see cref="End"/>, and <see cref="Step"/></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod2([SequentialNumbers(Start = 0, End = 10, Count = 5)] decimal value)
    ///     {
    ///         // This test will run 5 times with the values 0, 2.5, 5, 7.5, and 10.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod1([SequentialNumbers(Start = 1, Step = 1, Count = 4)] decimal value)
    ///     {
    ///         // This test will run 4 times with the values 1, 2, 3 and 4.
    ///     }
    ///     
    ///     [Test]
    ///     public void MyTestMethod3([SequentialNumbers(Start = 0, End = 15, Step = 3)] decimal value)
    ///     {
    ///         // This test will run 6 times with the values 0, 3, 6, 9, 12, 15.
    ///     }
    /// }]]></code>
    /// </example>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class SequentialNumbersAttribute : GenerationDataAttribute
    {
        private double? start = null;
        private double? end = null;
        private double? step = null;
        private int? count = null;

        /// <summary>
        /// Gets or sets the starting value of the sequence.
        /// </summary>
        /// <remarks>
        /// <para>
        /// That property is used to define the sequence, when used with 2 other properties:
        /// <list type="bullet">
        /// <item><see cref="End"/> and <see cref="Count"/></item>
        /// <item><see cref="Step"/> and <see cref="Count"/></item>
        /// <item><see cref="End"/> and <see cref="Step"/></item>
        /// </list>
        /// </para>
        /// </remarks>
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
        /// <remarks>
        /// <para>
        /// That property is used to define the sequence, when used with 2 other properties:
        /// <list type="bullet">
        /// <item><see cref="Start"/> and <see cref="Count"/></item>
        /// <item><see cref="Start"/> and <see cref="Step"/></item>
        /// </list>
        /// </para>
        /// </remarks>
        public double End
        {
            get
            {
                return end ?? 0;
            }

            set
            {
                end = value;
            }
        }

        /// <summary>
        /// Gets or sets the increment between each value of the sequence.
        /// </summary>
        /// <remarks>
        /// <para>
        /// That property is used to define the sequence, when used with 2 other properties:
        /// <list type="bullet">
        /// <item><see cref="Start"/> and <see cref="Count"/></item>
        /// <item><see cref="Start"/> and <see cref="End"/></item>
        /// </list>
        /// </para>
        /// </remarks>
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
        /// <remarks>
        /// <para>
        /// That property is used to define the sequence, when used with 2 other properties:
        /// <list type="bullet">
        /// <item><see cref="Start"/> and <see cref="End"/></item>
        /// <item><see cref="Start"/> and <see cref="Step"/></item>
        /// </list>
        /// </para>
        /// </remarks>
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
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [TestFixture]
        /// public class MyTestFixture
        /// {
        ///     [Test]
        ///     public void Generate_filtered_sequence([SequentialNumbers(Start = 1, End = 100, Step = 1, Filter = "MyFilter")] int value)
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
        public string Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a column of sequential <see cref="Decimal"/> values.
        /// </summary>
        public SequentialNumbersAttribute()
        {
        }

        /// <inheritdoc />
        protected override IGenerator GetGenerator(IPatternScope scope)
        {
            var invoker = MakeFilterInvoker(scope);

            try
            {
                return new SequentialNumbersGenerator
                {
                    Start = (decimal?)start,
                    End = (decimal?)end,
                    Step = (decimal?)step,
                    Count = count,
                    Filter = invoker
                };
            }
            catch (GenerationException exception)
            {
                ThrowUsageErrorException("The sequential numbers generator was incorrectly initialized.", exception);
                return null; // Make the compiler happy.
            }
        }

        private Predicate<decimal> MakeFilterInvoker(IPatternScope scope)
        {
            if (Filter == null)
                return null;

            var invoker = new FixtureMemberInvoker<bool>(null, scope, Filter);
            return d => invoker.Invoke(d);
        }
    }
}
