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

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides data from Comma Separated Values (CSV) contents.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the <a href="http://en.wikipedia.org/wiki/Comma-separated_values">CSV document</a> has a header, 
    /// then it is interpreted as the names of the columns.  Columns with names in brackets, such as "[ExpectedException]",
    /// are interpreted as containing metadata values associated with the named key.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This example reads data from an Embedded Resource called <c>Data.csv</c> within the
    /// same namespace as the test fixture.
    /// </para>
    /// <para>
    /// Data files:
    /// <code><![CDATA[
    /// Item, Quantity, UnitPrice, [Category]
    /// Bananas, 3, 0.85, Produce
    /// Cookies, 10, 0.10, Snacks
    /// # Comment: mmmm!
    /// Shortbread, 1, 2.25, Snacks
    /// ]]></code>
    /// </para>
    /// <para>
    /// A simple test.
    /// <code><![CDATA[
    /// public class AccountingTests
    /// {
    ///     [Test]
    ///     [CsvData(ResourcePath = "Data.csv")]
    ///     public void ShoppingCartTotalWithSingleItem(string item, decimal unitPrice, decimal quantity)
    ///     {
    ///         ShoppingCart shoppingCart = new ShoppingCart();
    ///         shoppingCart.Add(item, unitprice, quantity);
    ///         Assert.AreEqual(unitPrice * quantity, shoppingCart.TotalCost);
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <seealso cref="CsvDataSet"/>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class CsvDataAttribute : ContentAttribute
    {
        private char fieldDelimiter = ',';
        private char commentPrefix = '#';
        private bool hasHeader;

        /// <summary>
        /// Gets or sets the field delimiter character.
        /// </summary>
        /// <value>
        /// The default value is ',' (comma).
        /// </value>
        public char FieldDelimiter
        {
            get { return fieldDelimiter; }
            set { fieldDelimiter = value; }
        }

        /// <summary>
        /// Gets or sets a character that indicates that a line in the source represents a comment.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be set to '\0' (null) to disable comment handling.
        /// </para>
        /// <para>
        /// Comment lines are excluded from the record set.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value is '#' (pound).
        /// </value>
        public char CommentPrefix
        {
            get { return commentPrefix; }
            set { commentPrefix = value; }
        }

        /// <summary>
        /// Gets or sets whether the CSV document has a header that should be used to provide
        /// aliases for indexed columns.
        /// </summary>
        /// <value>
        /// The default value is <c>false</c> which indicates that the file does not have a header.
        /// </value>
        public bool HasHeader
        {
            get { return hasHeader; }
            set { hasHeader = value; }
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            var dataSet = new CsvDataSet(delegate { return OpenTextReader(codeElement); }, IsDynamic);
            dataSet.DataLocationName = GetDataLocationName();
            dataSet.FieldDelimiter = fieldDelimiter;
            dataSet.CommentPrefix = commentPrefix;
            dataSet.HasHeader = hasHeader;
            dataSource.AddDataSet(dataSet);
        }
    }
}
