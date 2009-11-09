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
    /// Provides a column of enumeration values as a data source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each value from the specified enumeration type is used as input for the
    /// data-driven test method.
    /// </para>
    /// <para>
    /// It is possible to exclude some specific values of the enumeration from the column.
    /// Use <see cref="EnumDataAttribute.Exclude"/> or <see cref="EnumDataAttribute.ExcludeArray"/> 
    /// for that purpose.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public enum Planet
    /// {
    ///     Mercury,
    ///     Venus,
    ///     Earth,
    ///     Mars,
    ///     Jupiter,
    ///     Saturn,
    ///     Uranus,
    ///     Neptune
    /// }
    /// 
    /// [TestFixture]
    /// public class MyTestFixture
    /// {
    ///     [Test]
    ///     public void Test([EnumData(typeof(Planet))] Planet planet)
    ///     {
    ///         // This test will run 8 times with all the possible values of
    ///         // the specified enumeration type.
    ///     }
    /// 
    ///     [Test]
    ///     public void TestWithRestrictions([EnumData(typeof(Planet), Exclude = Planet.Earth)] Planet planet)
    ///     {
    ///         // This test will run only 7 times.
    ///     }
    /// }]]></code>
    /// </example>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class EnumDataAttribute : DataAttribute
    {
        private readonly Type enumerationType;

        /// <summary>
        /// Gets the type of the enumeration.
        /// </summary>
        public Type EnumerationType
        {
            get
            {
                return enumerationType;
            }
        }

        /// <summary>
        /// Sets or gets the single enumeration value that must be excluded from the column.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you want to exclude several values at once, use <see cref="EnumDataAttribute.ExcludeArray"/> instead.
        /// </para>
        /// </remarks>
        /// <seealso cref="EnumDataAttribute.ExcludeArray"/>
        public object Exclude
        {
            get;
            set;
        }

        /// <summary>
        /// Sets or gets an array of values that must be excluded from the column.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you want to exclude one value only, use <see cref="EnumDataAttribute.Exclude"/> instead.
        /// </para>
        /// </remarks>
        /// <seealso cref="EnumDataAttribute.Exclude"/>
        public object[] ExcludeArray
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a column of enumeration values.
        /// </summary>
        /// <param name="enumerationType">The type of the enumeration.</param>
        /// <exception cref="ArgumentNullException">The specified type is null.</exception>
        /// <exception cref="ArgumentException">The specified type is not an enumeration type.</exception>
        [CLSCompliant(false)]
        public EnumDataAttribute(Type enumerationType)
        {
            if (enumerationType == null)
                throw new ArgumentNullException("enumerationType");

            if (!enumerationType.IsEnum)
                throw new ArgumentException(String.Format(
                    "Expected '{0}' to be an enumeration type.", enumerationType), "enumerationType");

            this.enumerationType = enumerationType;
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(GetValues(), GetMetadata(), false));
        }

        private ArrayList GetAllExcluded()
        {
            var allExcluded = new ArrayList();

            if (ExcludeArray != null)
                allExcluded.AddRange(ExcludeArray);

            if (Exclude != null)
                allExcluded.Add(Exclude);

            foreach (object value in allExcluded)
            {
                if (!Enum.IsDefined(enumerationType, value))
                    ThrowUsageErrorException(String.Format(
                        "The specified excluded value '{0}' is not a defined enumeration value of type '{1}'", value, enumerationType));
            }

            return allExcluded;
        }

        private IEnumerable GetValues()
        {
            ArrayList allExcluded = GetAllExcluded();

            foreach (object value in Enum.GetValues(enumerationType))
            {
                if (!allExcluded.Contains(value))
                {
                    yield return value;
                }
            }
        }
    }
}
