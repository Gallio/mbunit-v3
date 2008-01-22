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

        [Test, ExpectedArgumentNullException]
        public void GetRowsThrowsIfBindingListIsNull()
        {
            RowSequenceDataSet dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 5, true);
            dataSet.GetRows(null);
        }

        [Test, ExpectedArgumentNullException]
        public void CanBindThrowsIfBindingIsNull()
        {
            RowSequenceDataSet dataSet = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 5, true);
            dataSet.CanBind(null);
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