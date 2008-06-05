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
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(JoinedDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class JoinedDataSetTest : BaseUnitTest
    {
        private delegate IEnumerable<IList<IDataRow>> JoinDelegate(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicRows);

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
        public void AddingDataSetsUpdatesTheColumnCountAndDataSetsCollection()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

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

                Assert.AreEqual(5, dataSet.ColumnCount);
                CollectionAssert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns, dataSetWithThreeColumns }, dataSet.DataSets);
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

        [Test]
        public void CanBindReturnsFalseIfThereAreNoDataSets()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(0, null)),
                "Cannot bind because there are no data sets.");
        }

        [Test]
        public void CanBindResolvesExternalBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 3));
            IDataSet dataSet2 = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 2);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(null, null)),
                "Cannot bind because there is no path or index.");
            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(5, null)),
                "Cannot bind because index 5 is beyond the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(4, null)),
                "Can bind because index 4 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(0, null)),
                "Can bind because index 0 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(null, "path")),
                "Can bind because path is supported by one of the data sets.");
        }

        [Test]
        public void CanBindResolvesScopedBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 3));
            IDataSet dataSet2 = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 2);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(null, null))),
                "Cannot bind because there is no path or index in the translated binding.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(3, null))),
                "Cannot bind because index 3 is beyond the range of columns in the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(2, null))),
                "Cannot bind because index 2 is beyond the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(1, null))),
                "Can bind because index 1 is within the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(null, "path"))),
                "Can bind because path is supported by one of the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(null, "path"))),
                "Cannot bind because path is supported by one of the scoped data set.");
        }

        [Test]
        public void GetRowsDelegatesToTheStrategy()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            IList<KeyValuePair<string, string>> metadata1 = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("abc", "123"),
                new KeyValuePair<string, string>("def", "456")
            };
            IList<KeyValuePair<string, string>> metadata2 = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("ghi", "789"),
            };

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new RowSequenceDataSet(new IDataRow[]
            {
                new ListDataRow<int>(new int[] { 1, 2, 3 }, metadata1, false),
                new ListDataRow<int>(new int[] { -1, -2, -3 }, metadata2, false)
            }, 3));
            dataSet.AddDataSet(dataSet1);

            IDataSet dataSet2 = new RowSequenceDataSet(new IDataRow[]
            {
                new ListDataRow<int>(new int[] { 4, 5 }, metadata2, false),
                new ListDataRow<int>(new int[] { -4, -5 }, null, true)
            }, 2);
            dataSet.AddDataSet(dataSet2);

            List<IDataRow> dataSet1Rows = new List<IDataRow>(dataSet1.GetRows(EmptyArray<DataBinding>.Instance, true));
            List<IDataRow> dataSet2Rows = new List<IDataRow>(dataSet2.GetRows(EmptyArray<DataBinding>.Instance, true));

            List<IList<IDataRow>> results = new List<IList<IDataRow>>();
            results.Add(new IDataRow[] { dataSet1Rows[0], dataSet2Rows[0] });
            results.Add(new IDataRow[] { dataSet1Rows[1], dataSet2Rows[1] });

            IJoinStrategy strategy = Mocks.CreateMock<IJoinStrategy>();
            dataSet.Strategy = strategy;

            DataBinding pathBinding = new SimpleDataBinding(null, "path");
            DataBinding indexZeroBinding = new SimpleDataBinding(0, null);
            DataBinding indexOneBinding = new SimpleDataBinding(1, null);
            DataBinding indexThreeBinding = new SimpleDataBinding(3, null);

            DataBinding[] bindings = new DataBinding[]
            {
                new SimpleDataBinding(null, null), // unresolvable binding because no data sets can claim it
                pathBinding, // claimed by dataSet1
                indexZeroBinding, // claimed by dataSet1
                indexThreeBinding, // claimed by dataSet2
                dataSet.TranslateBinding(dataSet1, pathBinding), // scoped by dataSet1
                dataSet.TranslateBinding(dataSet2, indexOneBinding), // scoped by dataSet2
            };

            using (Mocks.Record())
            {
                Expect.Call(strategy.Join(null, null, true)).IgnoreArguments().Do((JoinDelegate)delegate(IList<IDataProvider> joinProviders, IList<ICollection<DataBinding>> joinBindingsPerProvider,
                    bool includeDynamicRows)
                {
                    Assert.IsTrue(includeDynamicRows);
                    CollectionAssert.AreElementsEqual(new IDataProvider[] { dataSet1, dataSet2 }, joinProviders);

                    Assert.AreEqual(2, joinBindingsPerProvider.Count);

                    CollectionAssert.AreElementsEqual(new DataBinding[] { pathBinding, indexZeroBinding, pathBinding }, joinBindingsPerProvider[0]);
                    CollectionAssert.AreElementsEqual(new DataBinding[] { indexZeroBinding, indexOneBinding }, joinBindingsPerProvider[1]);

                    return results;
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(bindings, true));
                Assert.AreEqual(rows.Count, 2);

                InterimAssert.Throws<ArgumentNullException>(delegate { rows[0].GetValue(null); });

                InterimAssert.Throws<DataBindingException>(delegate { rows[0].GetValue(bindings[0]); });
                Assert.AreEqual(2, rows[0].GetValue(bindings[1]));
                Assert.AreEqual(1, rows[0].GetValue(bindings[2]));
                Assert.AreEqual(4, rows[0].GetValue(bindings[3]));
                Assert.AreEqual(2, rows[0].GetValue(bindings[4]));
                Assert.AreEqual(5, rows[0].GetValue(bindings[5]));

                MetadataMap map = rows[0].GetMetadata();
                Assert.AreEqual(3, map.Count);
                Assert.AreEqual("123", map.GetValue("abc"));
                Assert.AreEqual("456", map.GetValue("def"));
                Assert.AreEqual("789", map.GetValue("ghi"));

                Assert.IsFalse(rows[0].IsDynamic);

                InterimAssert.Throws<DataBindingException>(delegate { rows[1].GetValue(bindings[0]); });
                Assert.AreEqual(-2, rows[1].GetValue(bindings[1]));
                Assert.AreEqual(-1, rows[1].GetValue(bindings[2]));
                Assert.AreEqual(-4, rows[1].GetValue(bindings[3]));
                Assert.AreEqual(-2, rows[1].GetValue(bindings[4]));
                Assert.AreEqual(-5, rows[1].GetValue(bindings[5]));

                map = rows[1].GetMetadata();
                Assert.AreEqual(1, map.Count);
                Assert.AreEqual("789", map.GetValue("ghi"));

                Assert.IsTrue(rows[1].IsDynamic);
            }
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

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
            }

            using (Mocks.Playback())
            {
                dataSet.AddDataSet(dataSetWithTwoColumns);
                dataSet.AddDataSet(dataSetWithThreeColumns);

                DataBinding bindingWithNoIndex = new SimpleDataBinding(null, null);
                DataBinding bindingWithIndex = new SimpleDataBinding(1, null);

                AssertTranslateReplacedIndex(dataSet, dataSetWithTwoColumns, bindingWithNoIndex, null,
                    "No binding index in original so none should be present when translated.");
                AssertTranslateReplacedIndex(dataSet, dataSetWithTwoColumns, bindingWithIndex, 1,
                    "Offset of data set is 0 so index should be unchanged.");

                AssertTranslateReplacedIndex(dataSet, dataSetWithThreeColumns, bindingWithNoIndex, null,
                    "No binding index in original so none should be present when translated.");
                AssertTranslateReplacedIndex(dataSet, dataSetWithThreeColumns, bindingWithIndex, 3,
                    "Offset of data set is 2 so index should be incremented by 2.");
            }
        }

        private void AssertTranslateReplacedIndex(JoinedDataSet joinedDataSet, IDataSet innerDataSet, DataBinding binding, int? expectedIndex, string message)
        {
            DataBinding translatedBinding = joinedDataSet.TranslateBinding(innerDataSet, binding);
            Assert.AreEqual(expectedIndex, translatedBinding.Index, message);
            Assert.AreEqual(binding.Path, translatedBinding.Path, "Path should be preserved.");
        }

        [Test, ExpectedArgumentNullException]
        public void TranslateBindingThrowsIfDataSetIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.TranslateBinding(null, new SimpleDataBinding(0, null));
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
            dataSet.TranslateBinding(Mocks.Stub<IDataSet>(), new SimpleDataBinding(0, null));
        }

        [Test]
        public void TranslateBindingSupportsReplaceIndex()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            DataSource source = new DataSource("dummy");
            dataSet.AddDataSet(source);

            DataBinding dataBinding = dataSet.TranslateBinding(source, new SimpleDataBinding(null, "path"));
            Assert.IsNull(dataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);

            DataBinding changedDataBinding = dataBinding.ReplaceIndex(5);
            Assert.AreEqual(5, changedDataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);
        }
    }
}