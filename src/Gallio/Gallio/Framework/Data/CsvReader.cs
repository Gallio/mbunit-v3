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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// Reads data from a Comma Separated Values document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The document format simply consists of sequential lines of delimited field values.
    /// <list type="bullet">
    /// <item>The standard delimiter is ',' (comma) but it may be overridden using <see cref="FieldDelimiter" />.</item>
    /// <item>Empty lines are considered to be records with one empty field.</item>
    /// <item>Lines that begin with a special <see cref="CommentPrefix" /> are excluded from the record set.</item>
    /// <item>Field values may be quoted using '"' (quote) characters.  The quotes will be omitted from the record set.
    /// Quotes may be escaped by doubling them within a quoted field.</item>
    /// <item>Unbalanced quotes are tolerated but may produce unexpected results.</item>
    /// <item>Excess whitespace is trimmed unless quoted.</item>
    /// <item>The document may contain a header consiting of field names but the reader does not interpret it in any
    /// special way.  It will simply be returned to the client as an ordinary record.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class CsvReader : IDisposable
    {
        private readonly TextReader documentReader;
        private readonly List<string> fieldBuffer;

        private char fieldDelimiter = ',';
        private char commentPrefix = '#';
        private int lineNumber;

        /// <summary>
        /// Creates a CSV reader. 
        /// </summary>
        /// <param name="documentReader">The text reader from which to read the contents of the document.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="documentReader"/> is null.</exception>
        public CsvReader(TextReader documentReader)
        {
            if (documentReader == null)
                throw new ArgumentNullException("documentReader");

            this.documentReader = documentReader;
            fieldBuffer = new List<string>();
        }

        /// <summary>
        /// Closes the reader.
        /// </summary>
        public void Close()
        {
            documentReader.Close();
        }

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
        /// May be set to '\0' (null) to disable comment handling.
        /// </summary>
        /// <remarks>
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
        /// Reads the list of fields that belong to the next record in the document.
        /// Returns null at the end of the document.
        /// </summary>
        /// <returns>The record contents as an array of field values, or null if at the end of the document.</returns>
        /// <exception cref="IOException">Thrown if an I/O error occurs.</exception>
        public string[] ReadRecord()
        {
            for (; ; )
            {
                string line = documentReader.ReadLine();
                if (line == null)
                    return null;

                lineNumber += 1;
                if (line.Length != 0 && commentPrefix != '\0' && line[0] == commentPrefix)
                    continue;

                try
                {
                    ParseLineIntoFieldBuffer(line);
                    return fieldBuffer.ToArray();
                }
                finally
                {
                    fieldBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the 1-based line number of the previously returned record,
        /// or 0 if no lines have been read yet.
        /// </summary>
        public int PreviousRecordLineNumber
        {
            get { return lineNumber; }
        }

        private void ParseLineIntoFieldBuffer(string line)
        {
            var fieldValue = new StringBuilder();
            int length = line.Length;

            bool inQuotes = false;
            int lastPreservedCharPosition = 0;

            for (int i = 0; i < length; )
            {
                char c = line[i++];

                if (inQuotes)
                {
                    if (c == '"' && (i == length || line[i] != '"'))
                    {
                        inQuotes = false;
                    }
                    else
                    {
                        fieldValue.Append(c);
                        lastPreservedCharPosition = fieldValue.Length;
                    }
                }
                else
                {
                    if (c == fieldDelimiter)
                    {
                        fieldValue.Length = lastPreservedCharPosition;
                        fieldBuffer.Add(fieldValue.ToString());

                        lastPreservedCharPosition = 0;
                        fieldValue.Length = 0;
                    }
                    else if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        if (lastPreservedCharPosition != 0)
                            fieldValue.Append(c);
                    }
                    else
                    {
                        fieldValue.Append(c);
                        lastPreservedCharPosition = fieldValue.Length;
                    }
                }
            }

            fieldValue.Length = lastPreservedCharPosition;
            fieldBuffer.Add(fieldValue.ToString());
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
