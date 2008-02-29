// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ColumnSequenceDataSet))]
    public class ColumnSequenceDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenValueEnumerationIsNull()
        {
            new ColumnSequenceDataSet(null, EmptyArray<KeyValuePair<string, string>>.Instance, false);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicIsSameAsSpecifiedInConstructor(bool isDynamic)
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a" }, null, isDynamic);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, true));

            Assert.AreEqual(isDynamic, rows[0].IsDynamic);
        }

        [Test]
        public void ColumnCountIsExactlyOne()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, true);
            Assert.AreEqual(1, dataSet.ColumnCount);
        }

        [Test]
        public void RowsContainEmptyMetadataWhenParameterWasNull()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a" }, null, false);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, true));

            Assert.IsNotNull(rows[0].GetMetadata());
            Assert.IsFalse(rows[0].GetMetadata().GetEnumerator().MoveNext());
        }

        [Test]
        public void RowsContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a" }, metadata, false);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, true));

            Assert.AreSame(metadata, rows[0].GetMetadata());
        }

        [Test]
        public void RowsAreScalarDataRowsContainValuesAtBindingIndexZero()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a", "b" }, null, false);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, true));

            Assert.AreEqual(2, rows.Count);

            Assert.AreEqual("a", rows[0].GetValue(new SimpleDataBinding(0, null)));
            Assert.AreEqual("b", rows[1].GetValue(new SimpleDataBinding(0, null)));

            Assert.IsInstanceOfType(typeof(ScalarDataRow<object>), rows[0]);
            Assert.IsInstanceOfType(typeof(ScalarDataRow<object>), rows[1]);
        }

        [Test]
        public void RowsEnumerationIsEmptyIfDynamicAndNotIncludingDynamicRows()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a" }, null, true);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance, false));

            Assert.AreEqual(0, rows.Count);
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
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, true);
            Assert.AreEqual(expectedResult, dataSet.CanBind(new SimpleDataBinding((int?) index, path)));
        }
    }
}
