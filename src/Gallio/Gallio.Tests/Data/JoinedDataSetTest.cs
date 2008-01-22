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

using Gallio.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(JoinedDataSet))]
    public class JoinedDataSetTest : BaseUnitTest
    {
        [Test]
        public void DefaultStrategyIsCombinatorial()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            Assert.AreSame(CombinatorialJoinStrategy.Instance, dataSet.Strategy);
        }

        [Test, ExpectedArgumentNullException]
        public void StrategySetterThrowsIfValueIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.Strategy = null;
        }

        [Test]
        public void StrategySetThenGet()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.Strategy = PairwiseJoinStrategy.Instance;

            Assert.AreSame(PairwiseJoinStrategy.Instance, dataSet.Strategy);
        }

        [Test]
        public void AddingDataSetsUpdatesTheColumnCountAndIsDynamicAndDataSetsCollection()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            IDataSet dataSetWithTwoColumns = Mocks.CreateMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);
                SetupResult.For(dataSetWithTwoColumns.IsDynamic).Return(false);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
                SetupResult.For(dataSetWithThreeColumns.IsDynamic).Return(true);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(0, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { }, dataSet.DataSets);
                Assert.IsFalse(dataSet.IsDynamic);

                dataSet.AddDataSet(dataSetWithTwoColumns);

                Assert.AreEqual(2, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns }, dataSet.DataSets);
                Assert.IsFalse(dataSet.IsDynamic);

                dataSet.AddDataSet(dataSetWithThreeColumns);

                Assert.AreEqual(5, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns, dataSetWithThreeColumns }, dataSet.DataSets);
                Assert.IsTrue(dataSet.IsDynamic);
            }
        }

        [Test, ExpectedArgumentNullException]
        public void AddingDataSetWithNullArgumentThrows()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.AddDataSet(null);
        }

        [Test, ExpectedArgumentException]
        public void AddingDataSetThatIsAlreadyAddedThrows()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            IDataSet dataSetToAdd = Mocks.Stub<IDataSet>();
            dataSet.AddDataSet(dataSetToAdd);

            dataSet.AddDataSet(dataSetToAdd);
        }

        [Test, ExpectedArgumentNullException]
        public void CanBindThrowsIfArgumentIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.CanBind(null);
        }

        [Test, ExpectedArgumentNullException]
        public void GetRowsThrowsIfArgumentIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.GetRows(null);
        }

        [Test]
        public void TranslateBindingReplacesTheDataBindingIndexWhenPresentAsNeeded()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            IDataSet dataSetWithTwoColumns = Mocks.CreateMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);
                SetupResult.For(dataSetWithTwoColumns.IsDynamic).Return(false);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
                SetupResult.For(dataSetWithThreeColumns.IsDynamic).Return(true);
            }

            using (Mocks.Playback())
            {
                dataSet.AddDataSet(dataSetWithTwoColumns);
                dataSet.AddDataSet(dataSetWithThreeColumns);

                DataBinding bindingWithNoIndex = new SimpleDataBinding(typeof(string));
                DataBinding bindingWithIndex = new SimpleDataBinding(typeof(string), null, 1);

                Assert.AreEqual(null, dataSet.TranslateBinding(dataSetWithTwoColumns, bindingWithNoIndex).Index,
                    "No binding index in original so none should be present when translated.");
                Assert.AreEqual(1, dataSet.TranslateBinding(dataSetWithTwoColumns, bindingWithIndex).Index,
                    "Offset of data set is 0 so index should be unchanged.");

                Assert.AreEqual(null, dataSet.TranslateBinding(dataSetWithThreeColumns, bindingWithNoIndex).Index,
                    "No binding index in original so none should be present when translated.");
                Assert.AreEqual(3, dataSet.TranslateBinding(dataSetWithThreeColumns, bindingWithIndex).Index,
                    "Offset of data set is 2 so index should be incremented by 2.");
            }
        }

        [Test, ExpectedArgumentNullException]
        public void TranslateBindingThrowsIfDataSetIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.TranslateBinding(null, new SimpleDataBinding(typeof(string)));
        }

        [Test, ExpectedArgumentNullException]
        public void TranslateBindingThrowsIfDataBindingIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.TranslateBinding(Mocks.Stub<IDataSet>(), null);
        }

        [Test, ExpectedArgumentException]
        public void TranslateBindingThrowsIfDataSetNotAMember()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.TranslateBinding(Mocks.Stub<IDataSet>(), new SimpleDataBinding(typeof(string)));
        }
    }
}