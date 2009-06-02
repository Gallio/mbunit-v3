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
    /// Provides a column of random <see cref="string"/> values as a data source.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Initialize the random string generator by setting the named parameter <see cref="Count"/>, which specifies
    /// how many random values to generate; and one of the two named parameters <see cref="Pattern"/> or <see cref="Stock"/>.
    /// </para>
    /// <para>
    /// The <see cref="Pattern"/> property accepts a simplified regular expression syntax. 
    /// The following syntactic features are supported:
    /// <list type="bullet">
    /// <item><term>Logical Grouping</term><description>Group a part of the expression (<code>(...)</code>).</description></item>
    /// <item><term>Explicit Set</term><description>Define a set of possible characters (<code>[...]</code>). 
    /// Ranges defined with a tiret are accepted.</description></item>
    /// <item><term>Explicit Quantifier</term><description>Specify the number of times the previous expression must be repeated. 
    /// 'Constant' (<code>{N}</code>) or 'Range' (<code>{N,M}</code>) syntax are both accepted.</description></item>
    /// <item><term>Zero Or One Quantifier Metacharacter</term><description>0 or 1 of the previous expression (<code>?</code>).
    /// Same effect as <code>{0,1}</code>.</description></item>
    /// <item><term>Escape Character</term><description>Makes the next character literal instead of a special character (<code>\</code>).</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <example>
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void MyTestMethod([RandomStrings(Count = 3, Pattern = @"[A-Z]{5,8}")] string text)
    ///     {
    ///         // This test will run 3 times. It generates at each iteration
    ///         // a random string containing 5 to 8 uppercase alphabetic characters.
    ///     }
    /// }]]></code>
    /// </example>
    /// </para>
    /// <para>
    /// The <see cref="Stock"/> property selects a stock of predefined strings from which to draw random values.
    /// </para>
    /// </remarks>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class RandomStringsAttribute : GenerationDataAttribute
    {
        private int? count = null;
        private RandomStringStock? stock = null;

        /// <summary>
        /// Gets or sets a regular expression pattern to generate random string from.
        /// </summary>
        public string Pattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stock of predefined strings from which to draw random values.
        /// </summary>
        public RandomStringStock Stock
        {
            get
            {
                return stock ?? RandomStringStock.EnUSMaleNames;
            }

            set
            {
                stock = value;
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
        /// The method must accepts one argument of the type <see cref="string"/>, and
        /// returns a <see cref="Boolean"/> value indicating whether the specified value
        /// must be accepted or rejected.
        /// </para>
        /// <para>
        /// <example>
        /// <code><![CDATA[
        /// [TestFixture]
        /// public class MyTestFixture
        /// {
        ///     [Test]
        ///     public void Generate_filtered_sequence([RandomStrings(Count = 3, Pattern = @"[A-Z]{5,8}", Filter = "MyFilter")]] string text)
        ///     {
        ///         // Code logic here...
        ///     }
        /// 
        ///     public static bool MyFilter(string text)
        ///     {
        ///         return text != "AAAAA";
        ///     }
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
        /// Adds a column of random <see cref="string"/> values.
        /// </summary>
        public RandomStringsAttribute()
        {
        }

        /// <inheritdoc />
        protected override IGenerator GetGenerator(IPatternScope scope)
        {
            var invoker = MakeFilterInvoker(scope);

            if (Pattern == null && stock == null)
                throw new PatternUsageErrorException("You must specify how to generate random strings by setting either 'Pattern' or 'Stock' appropriately.");

            if (!String.IsNullOrEmpty(Pattern) && stock.HasValue)
                throw new PatternUsageErrorException("You must specify how to generate random strings by setting either 'Pattern' or 'Stock' exclusively.");

            try
            {
                if (stock.HasValue)
                {
                    return new RandomStockStringsGenerator
                    {
                        Values = RandomStringStockInfo.FromStock(stock.Value).GetItems(),
                        Count = count,
                        Filter = invoker
                    };
                }
                else
                {
                    return new RandomRegexLiteStringsGenerator
                    {
                        RegularExpressionPattern = Pattern,
                        Count = count,
                        Filter = invoker
                    };
                }
            }
            catch (GenerationException exception)
            {
                throw new PatternUsageErrorException(String.Format(
                    "The random strings generator was incorrectly initialized ({0}).", exception.Message), exception);
            }
        }

        private Func<string, bool> MakeFilterInvoker(IPatternScope scope)
        {
            if (Filter == null)
                return null;

            var invoker = new FixtureMemberInvoker<bool>(null, scope, Filter);
            return t => invoker.Invoke(t);
        }
    }
}
