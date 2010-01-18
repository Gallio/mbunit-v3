// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides text data from resource contents.
    /// </summary>
    /// <example>
    /// <para>
    /// This example reads data from an Embedded Resource called <c>Data.txt</c> within the
    /// same namespace as the test fixture.
    /// </para>
    /// <para>
    /// <code><![CDATA[
    /// public class AccountingTests
    /// {
    ///     [Test]
    ///     public void ShoppingCartTotalWithSingleItem([TextData(ResourcePath = "Data.txt")] string data)
    ///     {
    ///         // Code logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <seealso cref="BinaryDataAttribute"/>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class TextDataAttribute : ContentAttribute
    {
        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            using (var textReader = OpenTextReader(codeElement))
            {
                var text = textReader.ReadToEnd();
                var dataSet = new ValueSequenceDataSet(new[] { text }, null, false);
                dataSource.AddDataSet(dataSet);
            }
        }
    }
}
