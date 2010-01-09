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
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ItemSequenceDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class ItemSequenceDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenRowEnumerationIsNull()
        {
            new ItemSequenceDataSet(null, 0);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsWhenColumnCountIsNegative()
        {
            new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, -1);
        }

        [Test]
        public void ColumnCountIsSameAsSpecifiedInConstructor()
        {
            ItemSequenceDataSet dataSet = new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 5);
            Assert.AreEqual(5, dataSet.ColumnCount);
        }

        [Test]
        public void RowEnumerationIsSameAsSpecifiedInConstructorWhenIncludingDynamicRows()
        {
            List<IDataItem> items = new List<IDataItem>();
            ItemSequenceDataSet dataSet = new ItemSequenceDataSet(items, 5);
            Assert.AreSame(items, dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));
        }

        [Test]
        public void RowEnumerationIsFilteredWhenNotIncludingDynamicRows()
        {
            List<IDataItem> items = new List<IDataItem>();
            items.Add(new ScalarDataItem<int>(42, null, true));
            items.Add(new ScalarDataItem<int>(53, null, false));

            ItemSequenceDataSet dataSet = new ItemSequenceDataSet(items, 5);

            List<IDataItem> result = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, false));
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(items[1], result[0]);
        }

        [Test]
        [Row(false, null, null)]
        [Row(false, null, 3)]
        [Row(false, null, -1)]
        [Row(false, "abc", null)]
        [Row(false, "abc", 3)]
        [Row(false, "abc", -1)]
        [Row(true, null, 0)]
        [Row(true, "abc", 0)]
        [Row(true, null, 2)]
        [Row(true, "abc", 2)]
        public void CanBindReturnsTrueOnlyIfTheBindingIndexIsBetweenZeroAndColumnCount(bool expectedResult, string path, object index)
        {
            ItemSequenceDataSet dataSet = new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 3);
            Assert.AreEqual(expectedResult, dataSet.CanBind(new DataBinding((int?)index, path)));
        }
    }
}