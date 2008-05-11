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
using Gallio.Framework.Data;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Defines names as aliases for the columns in an indexed data source
    /// such as those that have been populated by <see cref="RowAttribute" /> or <see cref="ColumnAttribute" />.
    /// The names may subsequently be used in data binding expressions in place
    /// of their corresponding column indices.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// [Test]
    /// [Header("username", "password")]
    /// [Row("jeff", "letmein")]
    /// [Row("liz", "password")]
    /// public void Test([BindObject] Credentials credentials)
    /// {
    ///     // ...
    /// }
    /// 
    /// public class Credentials
    /// {
    ///     public string UserName;
    ///     public string Password;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RowAttribute"/>
    /// <seealso cref="ColumnAttribute"/>
    [CLSCompliant(false)]
    public class HeaderAttribute : DataPatternAttribute
    {
        private readonly string[] columnNames;

        /// <summary>
        /// Defines names as aliases for the columns in an indexed data source.
        /// </summary>
        /// <param name="columnNames">The names of the columns in the data source in order by index</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="columnNames"/> or
        /// if one of the values it contains is null</exception>
        public HeaderAttribute(params string[] columnNames)
        {
            if (columnNames == null || Array.IndexOf(columnNames, null) != -1)
                throw new ArgumentNullException("columnNames");

            this.columnNames = columnNames;
        }

        /// <summary>
        /// Gets the array of column names.
        /// </summary>
        public string[] ColumnNames
        {
            get { return columnNames; }
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(DataSource dataSource, ICodeElementInfo codeElement)
        {
            for (int i = 0; i < columnNames.Length; i++)
                dataSource.AddIndexAlias(columnNames[i], i);
        }
    }
}