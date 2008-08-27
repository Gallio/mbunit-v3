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
using Gallio.Model;
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

            DataBinding binding = new DataBinding(bindingIndex, bindingPath);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(new DataBinding[] { binding }, true));

            string[] actualValues = GenericUtils.ConvertAllToArray<IDataItem, string>(items, delegate(IDataItem item)
            {
                return (string) item.GetValue(binding);
            });

            Assert.AreEqual(expectedValues, actualValues);
        }

        [Test]
        public void ProducesMetadata()
        {
            string document = "value,[Metadata]\n123,abc\n456,def";
            Func<TextReader> documentReaderProvider = delegate { return new StringReader(document); };
            CsvDataSet dataSet = new CsvDataSet(documentReaderProvider, false);

            dataSet.HasHeader = true;
            dataSet.DataLocationName = "<inline>";
            Assert.AreEqual("<inline>", dataSet.DataLocationName);

            DataBinding binding = new DataBinding(0, null);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(new DataBinding[] { binding }, true));

            Assert.AreEqual("123", items[0].GetValue(binding));
            MetadataMap map = DataItemUtils.GetMetadata(items[0]);
            Assert.AreEqual("<inline>(2)", map.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual("abc", map.GetValue("Metadata"));

            Assert.AreEqual("456", items[1].GetValue(binding));
            map = new MetadataMap();
            items[1].PopulateMetadata(map);
            Assert.AreEqual("<inline>(3)", map.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual("def", map.GetValue("Metadata"));
        }

        [Test]
        public void GetItemsReturnsNothingIfIsDynamicAndNotIncludingDynamicRows()
        {
            CsvDataSet dataSet = new CsvDataSet(delegate { return new StringReader(""); }, true);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, false));
            Assert.AreEqual(0, items.Count);
        }
    }
}
