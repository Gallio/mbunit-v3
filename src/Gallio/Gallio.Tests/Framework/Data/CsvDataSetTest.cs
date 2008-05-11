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
using System.IO;
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(CsvDataSet))]
    public class CsvDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfDocumentReaderProviderIsNull()
        {
            new CsvDataSet(null, false);
        }

        [Test]
        public void DefaultFieldDelimiterIsComma()
        {
            Func<TextReader> documentReaderProvider = delegate { return new StringReader(""); };
            CsvDataSet dataSet = new CsvDataSet(documentReaderProvider, false);

            Assert.AreEqual(',', dataSet.FieldDelimiter);
        }

        [Test]
        public void DefaultCommentPrefixIsPound()
        {
            Func<TextReader> documentReaderProvider = delegate { return new StringReader(""); };
            CsvDataSet dataSet = new CsvDataSet(documentReaderProvider, false);

            Assert.AreEqual('#', dataSet.CommentPrefix);
        }

        [Test]
        public void DefaultHasHeaderIsFalse()
        {
            Func<TextReader> documentReaderProvider = delegate { return new StringReader(""); };
            CsvDataSet dataSet = new CsvDataSet(documentReaderProvider, false);

            Assert.IsFalse(dataSet.HasHeader);
        }

        [Test]
        [Row("", ',', '#', false, 0, "column",
            new string[] { },
            Description = "Empty document.")]
        [Row("", ',', '#', true, 0, "column",
            new string[] { },
            Description = "Empty document with header.")]
        [Row("abc,def\n#comment\n123,456", ',', '#', false, 0, "column",
            new string[] { "abc", "123" },
            Description = "Binding by index 0.")]
        [Row("abc,def\n#comment\n123,456", ',', '#', false, 1, "column",
            new string[] { "def", "456" },
            Description = "Binding by index 1.")]
        [Row("abc,def\n#comment\n123,456", ',', '#', false, 2, "column",
            new string[] { },
            Description = "Binding by index failure: index too high.",
            ExpectedException = typeof(DataBindingException))]
        [Row("abc,def\n#comment\n123,456", ',', '#', false, -1, "column",
            new string[] { },
            Description = "Binding by index failure: index too low.",
            ExpectedException = typeof(DataBindingException))]
        [Row("abc,def\n#comment\n123,456", ',', '#', false, null, null,
            new string[] { },
            Description = "Binding failure: no path or index provided.",
            ExpectedException = typeof(DataBindingException))]
        [Row("first,second\nabc,def\n#comment\n123,456", ',', '#', true, null, "first",
            new string[] { "abc", "123" },
            Description = "Binding by path with header.")]
        [Row("first,second\nabc,def\n#comment\n123,456", ',', '#', true, null, "second",
            new string[] { "def", "456" },
            Description = "Binding by path with header.")]
        [Row("first,second\nabc,def\n#comment\n123,456", ',', '#', true, null, "sEcoNd",
            new string[] { "def", "456" },
            Description = "Binding by path with header, case insensitively.")]
        [Row("first,second\nabc,def\n#comment\n123,456", ',', '#', true, 1, "third",
            new string[] { "def", "456" },
            Description = "Binding by path with header fallback to index.")]
        public void BindValues(string document, char fieldDelimiter, char commentPrefix, bool hasHeader,
            int? bindingIndex, string bindingPath, string[] expectedValues)
        {
            Func<TextReader> documentReaderProvider = delegate { return new StringReader(document); };
            CsvDataSet dataSet = new CsvDataSet(documentReaderProvider, false);

            dataSet.FieldDelimiter = fieldDelimiter;
            Assert.AreEqual(fieldDelimiter, dataSet.FieldDelimiter);

            dataSet.CommentPrefix = commentPrefix;
            Assert.AreEqual(commentPrefix, dataSet.CommentPrefix);

            dataSet.HasHeader = hasHeader;
            Assert.AreEqual(hasHeader, dataSet.HasHeader);

            DataBinding binding = new SimpleDataBinding(bindingIndex, bindingPath);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(new DataBinding[] { binding }, true));

            string[] actualValues = GenericUtils.ConvertAllToArray<IDataRow, string>(rows, delegate(IDataRow row)
            {
                return (string) row.GetValue(binding);
            });

            InterimAssert.With("Expected", expectedValues, "Actual", actualValues, delegate
            {
                ArrayAssert.AreEqual(expectedValues, actualValues);
            });
        }

        [Test]
        public void GetRowsReturnsNothingIfIsDynamicAndNotIncludingDynamicRows()
        {
            CsvDataSet dataSet = new CsvDataSet(delegate { return new StringReader(""); }, true);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, false));
            Assert.AreEqual(0, rows.Count);
        }
    }
}
