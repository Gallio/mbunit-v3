// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using System.IO;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// An CSV data set retrieves fields from a CSV document as strings.
    /// </para>
    /// </summary>
    /// <todo author="jeff">
    /// Support reading metadata using specially named columns in the document.
    /// </todo>
    public class CsvDataSet : BaseDataSet
    {
        private readonly Func<TextReader> documentReaderProvider;
        private readonly bool isDynamic;

        private char fieldDelimiter = ',';
        private char commentPrefix = '#';
        private bool hasHeader;

        /// <summary>
        /// Creates a CSV data set.
        /// </summary>
        /// <param name="documentReaderProvider">A delegate that provides the text reader for reading the CSV document on demand</param>
        /// <param name="isDynamic">True if the data set should be considered dynamic</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="documentReaderProvider"/> is null</exception>
        public CsvDataSet(Func<TextReader> documentReaderProvider, bool isDynamic)
        {
            if (documentReaderProvider == null)
                throw new ArgumentNullException("documentReaderProvider");

            this.documentReaderProvider = documentReaderProvider;
            this.isDynamic = isDynamic;
        }

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
        public override int ColumnCount
        {
            // TODO: compute from the document
            get { return 0; }
        }

        /// <inheritdoc />
        protected override bool CanBindInternal(DataBinding binding)
        {
            // TODO: compute from the document
            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsInternal(ICollection<DataBinding> bindings, bool includeDynamicRows)
        {
            if (!isDynamic || includeDynamicRows)
            {
                using (CsvReader reader = new CsvReader(documentReaderProvider()))
                {
                    reader.FieldDelimiter = fieldDelimiter;
                    reader.CommentPrefix = commentPrefix;

                    string[] header = null;
                    if (hasHeader)
                    {
                        header = reader.ReadRecord();
                        if (header == null)
                            yield break;
                    }

                    for (; ; )
                    {
                        string[] record = reader.ReadRecord();
                        if (record == null)
                            yield break;

                        yield return new CsvDataRow(record, header, isDynamic);
                    }
                }
            }
        }

        private sealed class CsvDataRow : BaseDataRow
        {
            private readonly string[] record;
            private readonly string[] header;

            public CsvDataRow(string[] record, string[] header, bool isDynamic)
                : base(null, isDynamic)
            {
                this.record = record;
                this.header = header;
            }

            protected override object GetValueInternal(DataBinding binding)
            {
                int index = GetIndex(binding);
                if (index >= 0 && index < record.Length)
                    return record[index];

                throw new DataBindingException("Binding path or index could not be mapped to a CSV column.");
            }

            private int GetIndex(DataBinding binding)
            {
                if (header != null && binding.Path != null)
                {
                    int index = Array.IndexOf(header, binding.Path);
                    if (index >= 0)
                        return index;
                }

                return binding.Index.GetValueOrDefault(-1);
            }
        }
    }
}
