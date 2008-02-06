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
using Gallio.Collections;
using Gallio.Data;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(RowSequenceDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class RowSequenceDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenRowEnumerationIsNull()
        {
            new RowSequenceDataSet(null, 0, false);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsWhenColumnCountIsNegative()
        {
            new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, -1, false);
        }

        [Test]
        public void IsDynamicIsSameAsSpecifiedInConstructor()
        {
            RowSequenceDataSet dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 0, true);
            Assert.IsTrue(dataSet.IsDynamic);

            dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 0, false);
            Assert.IsFalse(dataSet.IsDynamic);
        }

        [Test]
        public void ColumnCountIsSameAsSpecifiedInConstructor()
        {
            RowSequenceDataSet dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 5, true);
            Assert.AreEqual(5, dataSet.ColumnCount);
        }

        [Test]
        public void RowEnumerationIsSameAsSpecifiedInConstructor()
        {
            List<IDataRow> rows = new List<IDataRow>();
            RowSequenceDataSet dataSet = new RowSequenceDataSet(rows, 5, true);
            Assert.AreSame(rows, dataSet.GetRows(EmptyArray<DataBinding>.Instance));
        }

        [RowTest]
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
            RowSequenceDataSet dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 3, true);
            Assert.AreEqual(expectedResult, dataSet.CanBind(new SimpleDataBinding(typeof(string), path, (int?)index)));
        }
    }
}