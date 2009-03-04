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
using Gallio;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

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
            Func<IEnumerable> factory = CreateFactory(scope);
            FactoryDataSet dataSet = new FactoryDataSet(factory, kind, columnCount);
            dataSource.AddDataSet(dataSet);
        }

        private Func<IEnumerable> CreateFactory(IPatternScope scope)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                BindingFlags.FlattenHierarchy;
            ITypeInfo factoryOwner;
            if (type != null)
            {
                factoryOwner = Reflector.Wrap(type);
            }
            else
            {
                factoryOwner = ReflectionUtils.GetType(scope.TestBuilder.CodeElement);
                if (factoryOwner == null)
                    throw new PatternUsageErrorException(
                        "Cannot infer the declaring type of the factory member.  Provide the declaring type to the [Factory] attribute constructor instead.");
                bindingFlags |= BindingFlags.Instance;
            }

            IMethodInfo factoryMethod = factoryOwner.GetMethod(memberName, bindingFlags);
            if (factoryMethod != null)
            {
                return delegate
                {
                    object fixtureInstance = GetFixtureInstance(factoryMethod.IsStatic);
                    MethodInfo method = type != null ? factoryMethod.Resolve(true) : GetFixtureType().GetMethod(memberName, bindingFlags);
                    if (method == null)
                        throw new TestFailedException(String.Format("Could not find factory method '{0}' on fixture.", memberName));
                    return (IEnumerable) method.Invoke(fixtureInstance, null);
                };
            }

            IPropertyInfo factoryProperty = factoryOwner.GetProperty(memberName, bindingFlags);
            if (factoryProperty != null && factoryProperty.GetMethod != null)
            {
                return delegate
                {
                    object fixtureInstance = GetFixtureInstance(factoryProperty.GetMethod.IsStatic);
                    PropertyInfo property = type != null ? factoryProperty.Resolve(true) : GetFixtureType().GetProperty(memberName, bindingFlags);
                    if (property == null)
                        throw new TestFailedException(String.Format("Could not find factory property '{0}' on fixture.", memberName));
                    return (IEnumerable)property.GetValue(fixtureInstance, null);
                };
            }

            IFieldInfo factoryField = factoryOwner.GetField(memberName, bindingFlags);
            if (factoryField != null)
            {
                return delegate
                {
                    object fixtureInstance = GetFixtureInstance(factoryField.IsStatic);
                    FieldInfo field = type != null ? factoryField.Resolve(true) : GetFixtureType().GetField(memberName, bindingFlags);
                    if (field == null)
                        throw new TestFailedException(String.Format("Could not find factory field '{0}' on fixture.", memberName));
                    return (IEnumerable)field.GetValue(fixtureInstance);
                };
            }

            throw new PatternUsageErrorException(String.Format("Could not find factory method, property or field named '{0}' on type '{1}'.",
                memberName, factoryOwner));
        }

        private Type GetFixtureType()
        {
            return GetCurrentTestInstanceState().FixtureType;
        }

        private object GetFixtureInstance(bool isStatic)
        {
            if (isStatic)
                return null;

            return GetCurrentTestInstanceState().FixtureInstance;
        }

        private PatternTestInstanceState GetCurrentTestInstanceState()
        {
            PatternTestInstanceState state = PatternTestInstanceState.FromContext(TestContext.CurrentContext);
            if (state == null || state.FixtureInstance == null)
                throw new InvalidOperationException(String.Format("Cannot invoke factory '{0}' because it is non-static and there is no fixture instance available for this test.", memberName));

            return state;
        }
    }
}
