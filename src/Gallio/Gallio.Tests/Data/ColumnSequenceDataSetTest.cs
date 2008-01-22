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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Collections;
using Gallio.Data;
using Gallio.Model;
using MbUnit2::MbUnit.Framework;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Data
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
        public void IsDynamicIsSameAsSpecifiedInConstructor()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, true);
            Assert.IsTrue(dataSet.IsDynamic);

            dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, false);
            Assert.IsFalse(dataSet.IsDynamic);
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
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance));

            Assert.IsNotNull(rows[0].GetMetadata());
            Assert.IsFalse(rows[0].GetMetadata().GetEnumerator().MoveNext());
        }

        [Test, ExpectedArgumentNullException]
        public void GetRowsThrowsIfBindingListIsNull()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, true);
            dataSet.GetRows(null);
        }

        [Test]
        public void RowsContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a" }, metadata, false);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance));

            Assert.AreSame(metadata, rows[0].GetMetadata());
        }

        [Test]
        public void RowsAreScalarDataRowsContainValuesAtBindingIndexZero()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(new object[] { "a", "b" }, null, false);
            List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(EmptyArray<DataBinding>.Instance));

            Assert.AreEqual(2, rows.Count);

            Assert.AreEqual("a", rows[0].GetValue(new SimpleDataBinding(typeof(string), null, 0)));
            Assert.AreEqual("b", rows[1].GetValue(new SimpleDataBinding(typeof(string), null, 0)));

            Assert.IsInstanceOfType(typeof(ScalarDataRow<object>), rows[0]);
            Assert.IsInstanceOfType(typeof(ScalarDataRow<object>), rows[1]);
        }

        [Test, ExpectedArgumentNullException]
        public void CanBindThrowsIfBindingIsNull()
        {
            ColumnSequenceDataSet dataSet = new ColumnSequenceDataSet(EmptyArray<object>.Instance, null, true);
            dataSet.CanBind(null);
        }

        [RowTest]
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
            Assert.AreEqual(expectedResult, dataSet.CanBind(new SimpleDataBinding(typeof(string), path, (int?) index)));
        }
    }
}
