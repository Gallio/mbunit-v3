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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of random <see cref="string"/> values as a data source.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Initialize the random string generator by setting the 2 named parameters 
    /// <see cref="Pattern"/> and <see cref="Count"/>.
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
    /// </remarks>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class RandomStringsAttribute : GenerationDataAttribute
    {
        private int? count = null;

        /// <summary>
        /// Gets or sets a regular expression pattern to generate random string from.
        /// </summary>
        public string Pattern
        {
            get;
            set;
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
        /// Adds a column of random <see cref="string"/> values.
        /// </summary>
        public RandomStringsAttribute()
        {
        }

        /// <inheritdoc />
        protected override IGenerator GetGenerator()
        {
            try
            {
                return new RandomStringsGenerator
                {
                    RegularExpressionPattern = Pattern,
                    Count = count
                };
            }
            catch (GenerationException exception)
            {
                ThrowUsageErrorException("The random strings generator was incorrectly initialized.", exception);
                return null; // Make the compiler happy.
            }
        }
    }
}
