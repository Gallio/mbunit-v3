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
using System.Collections;
using System.Reflection;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies a factory member that will provide values for a data-driven test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The factory may be an instance or static member of the fixture class or a
    /// static member of some other class.
    /// </para>
    /// <para>
    /// Refer to <see cref="FactoryDataSet" /> and <see cref="FactoryKind" />
    /// for more information about how the factory data set works and the kinds of
    /// factories that are supported.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class FactoryAttribute : DataPatternAttribute
    {
        private readonly Type type;
        private readonly string memberName;
        private FactoryKind kind = FactoryKind.Auto;
        private int columnCount;

        /// <summary>
        /// <para>
        /// Specifies the name of a method, property or field of the fixture
        /// class that will provide values for a data-driven test.  The factory
        /// member must return an enumeration of values (<seealso cref="FactoryKind" />).
        /// </para>
        /// <para>
        /// The factory member may be non-static if it will be used within the
        /// scope of an initialized fixture.  Typically this is the case for factory
        /// members referenced by non-static test methods.
        /// </para>
        /// <seealso cref="FactoryAttribute" /> for more information about factories.
        /// </summary>
        /// <param name="memberName">The factory member name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberName"/> is null</exception>
        public FactoryAttribute(string memberName)
        {
            if (memberName == null)
                throw new ArgumentNullException("memberName");

            this.memberName = memberName;
        }

        /// <summary>
        /// <para>
        /// Specifies the declaring type and name of a static method, property or field
        /// that will provide values for a data-driven test.  The factory
        /// member must return an enumeration of values (<seealso cref="FactoryKind" />).
        /// </para>
        /// <seealso cref="FactoryAttribute" /> for more information about factories.
        /// </summary>
        /// <param name="type">The declaring type of the factory</param>
        /// <param name="memberName">The factory member name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/>
        /// or <paramref name="memberName"/> is null</exception>
        public FactoryAttribute(Type type, string memberName)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (memberName == null)
                throw new ArgumentNullException("memberName");

            this.type = type;
            this.memberName = memberName;
        }

        /// <summary>
        /// Gets the declaring type of the factory, or null if it is assumed to be the fixture class.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the factory member name, never null.
        /// </summary>
        public string MemberName
        {
            get { return memberName; }
        }

        /// <summary>
        /// Gets or sets the kind of the factory.
        /// Defaults to <see cref="FactoryKind.Auto" />.
        /// </summary>
        /// <value>The kind of the factory.</value>
        public FactoryKind Kind
        {
            get { return kind; }
            set { kind = value; }
        }

        /// <summary>
        /// Gets or sets the number of columns produced by the factory, or 0 if unknown.
        /// Defaults to 0.
        /// </summary>
        /// <value>The number of columns</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is
        /// less than zero</exception>
        public int ColumnCount
        {
            get { return columnCount; }
            set
            {
                if (columnCount < 0)
                    throw new ArgumentOutOfRangeException("value", value, "Column count must be non-negative.");
                columnCount = value;
            }
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            var invoker = new FixtureMemberInvoker<IEnumerable>(type, scope, memberName);
            var dataSet = new FactoryDataSet(() => invoker.Invoke(), kind, columnCount);
            dataSource.AddDataSet(dataSet);
        }
    }
}
