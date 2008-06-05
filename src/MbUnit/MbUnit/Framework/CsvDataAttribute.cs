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
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides data from Comma Separated Values contents.
    /// </para>
    /// <para>
    /// If the CSV document has a header, then it is interpreted as the names of the
    /// columns.  Columns with names in brackets, such as "[ExpectedException]",
    /// are interpreted as containing metadata values associated with the named key.
    /// </para>
    /// </summary>
    /// <seealso cref="CsvDataSet"/>
    public class CsvDataAttribute : ContentAttribute
    {
        private char fieldDelimiter = ',';
        private char commentPrefix = '#';
        private bool hasHeader;

        /// <summary>
        /// <para>
        /// Gets or sets the field delimiter character.
        /// </para>
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
        /// <para>
        /// Gets or sets a character that indicates that a line in the source represents a comment.
        /// May be set to '\0' (null) to disable comment handling.
        /// </para>
        /// <para>
        /// Comment lines are excluded from the record set.
        /// </para>
        /// </summary>
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
        /// The default value is 'false' which indicates that the file does not have a header.
        /// </value>
        public bool HasHeader
        {
            get { return hasHeader; }
            set { hasHeader = value; }
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(PatternEvaluationScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            CsvDataSet dataSet = new CsvDataSet(delegate { return OpenTextReader(codeElement); }, IsDynamic);
            dataSet.DataLocationName = GetDataLocationName();
            dataSet.FieldDelimiter = fieldDelimiter;
            dataSet.CommentPrefix = commentPrefix;
            dataSet.HasHeader = hasHeader;

            dataSource.AddDataSet(dataSet);
        }
    }
}
