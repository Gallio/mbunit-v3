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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A CSV data set retrieves fields from a CSV document as strings.
    /// </para>
    /// <para>
    /// If the CSV document has a header, then it is interpreted as the names of the
    /// columns.  Columns with names in brackets, such as "[ExpectedException]",
    /// are interpreted as containing metadata values associated with the named key.
    /// </para>
    /// </summary>
    public class CsvDataSet : BaseDataSet
    {
        private readonly Func<TextReader> documentReaderProvider;
        private readonly bool isDynamic;

        private char fieldDelimiter = ',';
        private char commentPrefix = '#';
        private string dataLocationName;
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
        /// Gets the name of the location that is providing the data, or null if none.
        /// </para>
        /// <para>
        /// The data location name and line number are exposed as
        /// <see cref="MetadataKeys.DataLocation" /> metadata when provided.
        /// </para>
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public string DataLocationName
        {
            get { return dataLocationName; }
            set { dataLocationName = value; }
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
        protected override bool CanBindImpl(DataBinding binding)
        {
            // TODO: compute from the document
            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<IDataRow> GetRowsImpl(ICollection<DataBinding> bindings, bool includeDynamicRows)
        {
            if (!isDynamic || includeDynamicRows)
            {
                using (CsvReader reader = new CsvReader(documentReaderProvider()))
                {
                    reader.FieldDelimiter = fieldDelimiter;
                    reader.CommentPrefix = commentPrefix;

                    string[] header = null;
                    Dictionary<string, int> metadataColumns = null;
                    if (hasHeader)
                    {
                        header = reader.ReadRecord();
                        if (header == null)
                            yield break;

                        metadataColumns = new Dictionary<string, int>();
                        for (int i = 0; i < header.Length; i++)
                        {
                            string columnName = header[i];
                            if (columnName.StartsWith("[") && columnName.EndsWith("]"))
                                metadataColumns[columnName.Substring(1, columnName.Length - 2)] = i;
                        }
                    }

                    for (; ; )
                    {
                        string[] record = reader.ReadRecord();
                        if (record == null)
                            yield break;
                        
                        CodeLocation dataLocation = dataLocationName != null
                            ? new CodeLocation(dataLocationName, reader.PreviousRecordLineNumber, 0)
                            : CodeLocation.Unknown;
                        yield return new CsvDataRow(record, header,
                            GetMetadata(dataLocation, record, metadataColumns), isDynamic);
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetMetadata(CodeLocation dataLocation, string[] record, Dictionary<string, int> metadataColumns)
        {
            if (dataLocation != CodeLocation.Unknown)
                yield return new KeyValuePair<string, string>(MetadataKeys.DataLocation, dataLocation.ToString());

            if (metadataColumns != null)
            {
                foreach (KeyValuePair<string, int> metadataColumn in metadataColumns)
                    yield return new KeyValuePair<string, string>(metadataColumn.Key, record[metadataColumn.Value]);
            }
        }

        private sealed class CsvDataRow : StoredDataRow
        {
            private readonly string[] record;
            private readonly string[] header;

            public CsvDataRow(string[] record, string[] header,
                IEnumerable<KeyValuePair<string, string>> metadataPairs, bool isDynamic)
                : base(metadataPairs, isDynamic)
            {
                this.record = record;
                this.header = header;
            }

            protected override object GetValueImpl(DataBinding binding)
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
                    int index = Array.FindIndex(header, delegate(string candidate)
                    {
                        return String.Compare(candidate, binding.Path, true, CultureInfo.InvariantCulture) == 0;
                    });

                    if (index >= 0)
                        return index;
                }

                return binding.Index.GetValueOrDefault(-1);
            }
        }
    }
}
