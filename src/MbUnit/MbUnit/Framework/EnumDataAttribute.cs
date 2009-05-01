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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of enumeration values as a data source.
    /// </para>
    /// <para>
    /// Each value from the specified enumeration type is used as input for the
    /// data-driven test method.
    /// </para>
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
    ///     public void MyTestMethod([EnumData(typeof(Planet))] Planet planet)
    ///     {
    ///         // This test will run 8 times with all possible values of
    ///         // the specified enumeration type.
    ///     }
    /// }]]></code>
    /// </example>
    /// </summary>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class EnumDataAttribute : DataAttribute
    {
        private readonly Type enumerationType;

        /// <summary>
        /// <para>
        /// Adds a column of enumeration values.
        /// </para>
        /// </summary>
        /// <param name="enumerationType">The type of the enumeration.</param>
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
            dataSource.AddDataSet(new ValueSequenceDataSet(Enum.GetValues(enumerationType), GetMetadata(), false));
        }
    }
}
