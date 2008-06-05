// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// Specifies the kind of factory member referenced by <see cref="FactoryDataSet"/>.
    /// </para>
    /// <para>
    /// Each kind of factory expresses its contents by returning enumerations of data sets,
    /// data rows, object arrays and single objects.
    /// </para>
    /// <para>
    /// The kind of a factory may be automatically determined or it may be explicitly
    /// specified in cases where it may be ambiguous.
    /// </para>
    /// </summary>
    public enum FactoryKind
    {
        /// <summary>
        /// <para>
        /// Automatically determines the type of factory based on the type of element
        /// returned by the enumeration.
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item>If the element is a <see cref="IDataSet" /> then it is processed in the
        /// same manner as <see cref="FactoryKind.DataSet" />.</item>
        /// <item>If the element is a <see cref="IDataRow" /> (such as <see cref="ScalarDataRow{T}" />,
        /// <see cref="ObjectArray" /> or <see cref="FactoryKind" />) then it is 
        /// processed in the same manner as <see cref="Object" />.</item>
        /// <item>If the element is an array then it is processed in the same manner as
        /// <see cref="Data.DataRow" />.</item>
        /// <item>Otherwise the element is process in the same manner as <see cref="Gallio.Framework" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Refer to the other factory kinds for usage examples.
        /// </para>
        /// </summary>
        Auto,

        /// <summary>
        /// <para>
        /// Specifies that the factory returns an enumeration <see cref="IDataSet" />s
        /// whose rows are to be consumed.
        /// </para>
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// public IEnumerable<IDataSet> MyFactory
        /// {
        ///     get
        ///     {
        ///         yield return new MyCustomDataSet(1);
        ///         yield return new MyCustomDataSet(2);
        ///     }
        /// }
        /// 
        /// [Test, Factory("MyFactory", Kind=FactoryKind.DataSetEnumeration)]
        /// public void MyTest(object value)
        /// {
        ///     // test logic...
        /// }
        /// ]]></code>
        /// </example>
        DataSet,

        /// <summary>
        /// <para>
        /// Specifies that the factory returns an enumeration of <see cref="IDataRow" />s
        /// (such as <see cref="ScalarDataRow{T}" />, <see cref="Gallio.Framework" /> or <see cref="Gallio" />).
        /// </para>
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// public IEnumerable<IDataRow> MyFactory
        /// {
        ///     get
        ///     {
        ///         yield return new DataRow(1);
        ///         yield return new DataRow(42)
        ///             .WithMetadata(MetadataKeys.ExpectedException, typeof(CustomException).FullName);
        ///     }
        /// }
        /// 
        /// [Test, Factory("MyFactory", Kind=FactoryKind.DataRowEnumeration)]
        /// public void MyTest(int value)
        /// {
        ///     // test logic...
        /// }
        /// ]]></code>
        /// </example>
        DataRow,

        /// <summary>
        /// <para>
        /// Specifies that the factory returns an enumeration of object arrays
        /// that describe successive rows.
        /// </para>
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// public IEnumerable<object[]> MyFactory
        /// {
        ///     get
        ///     {
        ///         yield return new object[] { "abc", 123 };
        ///         yield return new object[] { "def", 456 };
        ///     }
        /// }
        /// 
        /// [Test, Factory("MyFactory", Kind=FactoryKind.ObjectArrayEnumeration)]
        /// public void MyTest(string value, int count)
        /// {
        ///     // test logic...
        /// }
        /// ]]></code>
        /// </example>
        ObjectArray,

        /// <summary>
        /// <para>
        /// Specifies that the factory returns an enumeration of single object values.
        /// </para>
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// public IEnumerable<string> MyFactory
        /// {
        ///     get
        ///     {
        ///         yield return "abc";
        ///         yield return "def";
        ///     }
        /// }
        /// 
        /// [Test, Factory("MyFactory", Kind=FactoryKind.ObjectEnumeration)]
        /// public void MyTest(string value)
        /// {
        ///     // test logic...
        /// }
        /// ]]></code>
        /// </example>
        Object
    }
}
