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
using System.Text;
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(MergedDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class MergedDataSetTest : BaseUnitTest
    {
        private delegate IEnumerable<IDataItem> MergeDelegate(IList<IDataProvider> providers, ICollection<DataBinding> bindings,
            bool includeDynamicItems);

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
        public void AddingDataSetsUpdatesTheColumnCountAndDataSetsCollection()
        {
            MergedDataSet dataSet = new MergedDataSet();

            IDataSet dataSetWithTwoColumns = Mocks.CreateMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(0, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { }, dataSet.DataSets);

                dataSet.AddDataSet(dataSetWithTwoColumns);

                Assert.AreEqual(2, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns }, dataSet.DataSets);

                dataSet.AddDataSet(dataSetWithThreeColumns);

                Assert.AreEqual(3, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns, dataSetWithThreeColumns }, dataSet.DataSets);
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
            DataBinding binding = new DataBinding(0, null);

            IDataSet dataSetWithTwoColumns = Mocks.CreateMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);
                Expect.Call(dataSetWithTwoColumns.CanBind(binding)).Repeat.Twice().Return(true);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
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
        public void GetItemsDelegatesToTheStrategy()
        {
            MergedDataSet dataSet = new MergedDataSet();

            IDataSet provider = Mocks.Stub<IDataSet>();
            dataSet.AddDataSet(provider);

            IMergeStrategy strategy = Mocks.CreateMock<IMergeStrategy>();
            dataSet.Strategy = strategy;

            IEnumerable<IDataItem> results = Mocks.Stub<IEnumerable<IDataItem>>();
            DataBinding[] bindings = new DataBinding[0];

            using (Mocks.Record())
            {
                Expect.Call(strategy.Merge(null, null, false)).IgnoreArguments().Do((MergeDelegate)delegate(IList<IDataProvider> mergeProviders, ICollection<DataBinding> mergeBindings,
                    bool includeDynamicItems)
                {
                    Assert.IsTrue(includeDynamicItems);

                    CollectionAssert.AreElementsEqual(new IDataProvider[] { provider }, mergeProviders);
                    Assert.AreSame(bindings, mergeBindings);
                    return results;
                });
            }

            using (Mocks.Playback())
            {
                Assert.AreSame(results, dataSet.GetItems(bindings, true));
            }
        }
    }
}
