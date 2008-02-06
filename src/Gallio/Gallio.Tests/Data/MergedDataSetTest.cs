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
using Gallio.Data;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(MergedDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class MergedDataSetTest : BaseUnitTest
    {
        private delegate IEnumerable<IDataRow> MergeDelegate(IList<IDataProvider> providers, ICollection<DataBinding> bindings);

        [Test]
        public void DefaultStrategyIsConcatenation()
        {
            MergedDataSet dataSet = new MergedDataSet();
            Assert.AreSame(ConcatenationMergeStrategy.Instance, dataSet.Strategy);
        }

        [Test, ExpectedArgumentNullException]
        public void StrategySetterThrowsIfValueIsNull()
        {
            MergedDataSet dataSet = new MergedDataSet();
            dataSet.Strategy = null;
        }

        [Test]
        public void StrategySetThenGet()
        {
            MergedDataSet dataSet = new MergedDataSet();
            dataSet.Strategy = UnionMergeStrategy.Instance;

            Assert.AreSame(UnionMergeStrategy.Instance, dataSet.Strategy);
        }

        [Test]
        public void AddingDataSetsUpdatesTheColumnCountAndIsDynamicAndDataSetsCollection()
        {
            MergedDataSet dataSet = new MergedDataSet();

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

                Assert.AreEqual(3, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns, dataSetWithThreeColumns }, dataSet.DataSets);
                Assert.IsTrue(dataSet.IsDynamic);
            }
        }

        [Test, ExpectedArgumentNullException]
        public void AddingDataSetWithNullArgumentThrows()
        {
            MergedDataSet dataSet = new MergedDataSet();
            dataSet.AddDataSet(null);
        }

        [Test, ExpectedArgumentException]
        public void AddingDataSetThatIsAlreadyAddedThrows()
        {
            MergedDataSet dataSet = new MergedDataSet();

            IDataSet dataSetToAdd = Mocks.Stub<IDataSet>();
            dataSet.AddDataSet(dataSetToAdd);

            dataSet.AddDataSet(dataSetToAdd);
        }

        [Test]
        public void CanBindReturnsTrueOnlyIfAllDataSetsCanSatisfyTheBinding()
        {
            MergedDataSet dataSet = new MergedDataSet();
            DataBinding binding = new SimpleDataBinding(typeof(int));

            IDataSet dataSetWithTwoColumns = Mocks.CreateMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);
                SetupResult.For(dataSetWithTwoColumns.IsDynamic).Return(false);
                Expect.Call(dataSetWithTwoColumns.CanBind(binding)).Repeat.Twice().Return(true);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
                SetupResult.For(dataSetWithThreeColumns.IsDynamic).Return(true);
                Expect.Call(dataSetWithThreeColumns.CanBind(binding)).Return(false);
            }

            using (Mocks.Playback())
            {
                Assert.IsTrue(dataSet.CanBind(binding), "Can always bind if there are no data sets.");

                dataSet.AddDataSet(dataSetWithTwoColumns);
                Assert.IsTrue(dataSet.CanBind(binding), "Can bind because only data set can bind.");

                dataSet.AddDataSet(dataSetWithThreeColumns);
                Assert.IsFalse(dataSet.CanBind(binding), "Cannot bind because one of the data sets cannot bind.");
            }
        }

        [Test]
        public void GetRowsDelegatesToTheStrategy()
        {
            MergedDataSet dataSet = new MergedDataSet();

            IDataSet provider = Mocks.Stub<IDataSet>();
            dataSet.AddDataSet(provider);

            IMergeStrategy strategy = Mocks.CreateMock<IMergeStrategy>();
            dataSet.Strategy = strategy;

            IEnumerable<IDataRow> results = Mocks.Stub<IEnumerable<IDataRow>>();
            DataBinding[] bindings = new DataBinding[0];

            using (Mocks.Record())
            {
                Expect.Call(strategy.Merge(null, null)).IgnoreArguments().Do((MergeDelegate)delegate(IList<IDataProvider> mergeProviders, ICollection<DataBinding> mergeBindings)
                {
                    CollectionAssert.AreElementsEqual(new IDataProvider[] { provider }, mergeProviders);
                    Assert.AreSame(bindings, mergeBindings);
                    return results;
                });
            }

            using (Mocks.Playback())
            {
                Assert.AreSame(results, dataSet.GetRows(bindings));
            }
        }
    }
}
