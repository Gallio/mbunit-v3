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
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ValueSequenceDataSet))]
    public class ValueSequenceDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenValueEnumerationIsNull()
        {
            new ValueSequenceDataSet(null, EmptyArray<KeyValuePair<string, string>>.Instance, false);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicIsSameAsSpecifiedInConstructor(bool isDynamic)
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(new object[] { "a" }, null, isDynamic);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            Assert.AreEqual(isDynamic, items[0].IsDynamic);
        }

        [Test]
        public void ColumnCountIsExactlyOne()
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(EmptyArray<object>.Instance, null, true);
            Assert.AreEqual(1, dataSet.ColumnCount);
        }

        [Test]
        public void ItemsContainEmptyMetadataWhenParameterWasNull()
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(new object[] { "a" }, null, false);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            PropertyBag map = DataItemUtils.GetMetadata(items[0]);
            Assert.AreEqual(0, map.Count);
        }

        [Test]
        public void ItemsContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string, string>("Foo", "Bar"));

            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(new object[] { "a" }, metadataPairs, false);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            PropertyBag map = DataItemUtils.GetMetadata(items[0]);
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("Bar", map.GetValue("Foo"));
        }

        [Test]
        public void ItemsAreScalarDataItemsThatContainValuesAtBindingIndexZero()
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(new object[] { "a", "b" }, null, false);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            Assert.AreEqual(2, items.Count);

            Assert.AreEqual("a", items[0].GetValue(new DataBinding(0, null)));
            Assert.AreEqual("b", items[1].GetValue(new DataBinding(0, null)));

            Assert.IsInstanceOfType(typeof(ScalarDataItem<object>), items[0]);
            Assert.IsInstanceOfType(typeof(ScalarDataItem<object>), items[1]);
        }

        [Test]
        public void ItemsEnumerationIsEmptyIfDynamicAndNotIncludingDynamicRows()
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(new object[] { "a" }, null, true);
            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, false));

            Assert.AreEqual(0, items.Count);
        }

        [Test]
        [Row(false, null, null)]
        [Row(false, null, 1)]
        [Row(false, null, -1)]
        [Row(false, "abc", null)]
        [Row(false, "abc", 1)]
        [Row(false, "abc", -1)]
        [Row(true, null, 0)]
        [Row(true, "abc", 0)]
        public void CanBindReturnsTrueOnlyIfTheBindingIndexIsZero(bool expectedResult, string path, object index)
        {
            ValueSequenceDataSet dataSet = new ValueSequenceDataSet(EmptyArray<object>.Instance, null, true);
            Assert.AreEqual(expectedResult, dataSet.CanBind(new DataBinding((int?) index, path)));
        }
    }
}
