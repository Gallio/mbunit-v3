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
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(JoinedDataSet))]
    [DependsOn(typeof(BaseDataSetTest))]
    public class JoinedDataSetTest : BaseTestWithMocks
    {
        private delegate IEnumerable<IList<IDataItem>> JoinDelegate(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider,
            bool includeDynamicItems);

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

            IDataSet dataSetWithTwoColumns = Mocks.StrictMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(0, dataSet.ColumnCount);
                Assert.AreElementsEqual(new IDataSet[] { }, dataSet.DataSets);

                dataSet.AddDataSet(dataSetWithTwoColumns);

                Assert.AreEqual(2, dataSet.ColumnCount);
                Assert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns }, dataSet.DataSets);

                dataSet.AddDataSet(dataSetWithThreeColumns);

                Assert.AreEqual(5, dataSet.ColumnCount);
                Assert.AreElementsEqual(new IDataSet[] { dataSetWithTwoColumns, dataSetWithThreeColumns }, dataSet.DataSets);
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
            Assert.IsFalse(dataSet.CanBind(new DataBinding(0, null)),
                "Cannot bind because there are no data sets.");
        }

        [Test]
        public void CanBindResolvesExternalBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 3));
            IDataSet dataSet2 = new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 2);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(new DataBinding(null, null)),
                "Cannot bind because there is no path or index.");
            Assert.IsFalse(dataSet.CanBind(new DataBinding(5, null)),
                "Cannot bind because index 5 is beyond the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new DataBinding(4, null)),
                "Can bind because index 4 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new DataBinding(0, null)),
                "Can bind because index 0 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new DataBinding(null, "path")),
                "Can bind because path is supported by one of the data sets.");
        }

        [Test]
        public void CanBindResolvesScopedBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 3));
            IDataSet dataSet2 = new ItemSequenceDataSet(EmptyArray<IDataItem>.Instance, 2);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new DataBinding(null, null))),
                "Cannot bind because there is no path or index in the translated binding.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new DataBinding(3, null))),
                "Cannot bind because index 3 is beyond the range of columns in the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new DataBinding(2, null))),
                "Cannot bind because index 2 is beyond the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new DataBinding(1, null))),
                "Can bind because index 1 is within the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new DataBinding(null, "path"))),
                "Can bind because path is supported by one of the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new DataBinding(null, "path"))),
                "Cannot bind because path is supported by one of the scoped data set.");
        }

        [Test]
        public void GetItemsDelegatesToTheStrategy()
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
            dataSet1.AddDataSet(new ItemSequenceDataSet(new IDataItem[]
            {
                new ListDataItem<int>(new int[] { 1, 2, 3 }, metadata1, false),
                new ListDataItem<int>(new int[] { -1, -2, -3 }, metadata2, false)
            }, 3));
            dataSet.AddDataSet(dataSet1);

            IDataSet dataSet2 = new ItemSequenceDataSet(new IDataItem[]
            {
                new ListDataItem<int>(new int[] { 4, 5 }, metadata2, false),
                new ListDataItem<int>(new int[] { -4, -5 }, null, true)
            }, 2);
            dataSet.AddDataSet(dataSet2);

            List<IDataItem> dataSet1Items = new List<IDataItem>(dataSet1.GetItems(EmptyArray<DataBinding>.Instance, true));
            List<IDataItem> dataSet2Items = new List<IDataItem>(dataSet2.GetItems(EmptyArray<DataBinding>.Instance, true));

            List<IList<IDataItem>> results = new List<IList<IDataItem>>();
            results.Add(new IDataItem[] { dataSet1Items[0], dataSet2Items[0] });
            results.Add(new IDataItem[] { dataSet1Items[1], dataSet2Items[1] });

            IJoinStrategy strategy = Mocks.StrictMock<IJoinStrategy>();
            dataSet.Strategy = strategy;

            DataBinding pathBinding = new DataBinding(null, "path");
            DataBinding indexZeroBinding = new DataBinding(0, null);
            DataBinding indexOneBinding = new DataBinding(1, null);
            DataBinding indexThreeBinding = new DataBinding(3, null);

            DataBinding[] bindings = new DataBinding[]
            {
                new DataBinding(null, null), // unresolvable binding because no data sets can claim it
                pathBinding, // claimed by dataSet1
                indexZeroBinding, // claimed by dataSet1
                indexThreeBinding, // claimed by dataSet2
                dataSet.TranslateBinding(dataSet1, pathBinding), // scoped by dataSet1
                dataSet.TranslateBinding(dataSet2, indexOneBinding), // scoped by dataSet2
            };

            using (Mocks.Record())
            {
                Expect.Call(strategy.Join(null, null, true)).IgnoreArguments().Do((JoinDelegate)delegate(IList<IDataProvider> joinProviders, IList<ICollection<DataBinding>> joinBindingsPerProvider,
                    bool includeDynamicItems)
                {
                    Assert.IsTrue(includeDynamicItems);
                    Assert.AreElementsEqual(new IDataProvider[] { dataSet1, dataSet2 }, joinProviders);

                    Assert.AreEqual(2, joinBindingsPerProvider.Count);

                    Assert.AreElementsEqual(new DataBinding[] { pathBinding, indexZeroBinding, pathBinding }, joinBindingsPerProvider[0]);
                    Assert.AreElementsEqual(new DataBinding[] { indexZeroBinding, indexOneBinding }, joinBindingsPerProvider[1]);

                    return results;
                });
            }

            using (Mocks.Playback())
            {
                List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(bindings, true));
                Assert.AreEqual(items.Count, 2);

                Assert.Throws<ArgumentNullException>(delegate { items[0].GetValue(null); });

                Assert.Throws<DataBindingException>(delegate { items[0].GetValue(bindings[0]); });
                Assert.AreEqual(2, items[0].GetValue(bindings[1]));
                Assert.AreEqual(1, items[0].GetValue(bindings[2]));
                Assert.AreEqual(4, items[0].GetValue(bindings[3]));
                Assert.AreEqual(2, items[0].GetValue(bindings[4]));
                Assert.AreEqual(5, items[0].GetValue(bindings[5]));

                PropertyBag map = DataItemUtils.GetMetadata(items[0]);
                Assert.AreEqual(3, map.Count);
                Assert.AreEqual("123", map.GetValue("abc"));
                Assert.AreEqual("456", map.GetValue("def"));
                Assert.AreEqual("789", map.GetValue("ghi"));

                Assert.IsFalse(items[0].IsDynamic);

                Assert.Throws<DataBindingException>(delegate { items[1].GetValue(bindings[0]); });
                Assert.AreEqual(-2, items[1].GetValue(bindings[1]));
                Assert.AreEqual(-1, items[1].GetValue(bindings[2]));
                Assert.AreEqual(-4, items[1].GetValue(bindings[3]));
                Assert.AreEqual(-2, items[1].GetValue(bindings[4]));
                Assert.AreEqual(-5, items[1].GetValue(bindings[5]));

                map = DataItemUtils.GetMetadata(items[1]);
                Assert.AreEqual(1, map.Count);
                Assert.AreEqual("789", map.GetValue("ghi"));

                Assert.IsTrue(items[1].IsDynamic);
            }
        }

        [Test]
        public void TranslateBindingReplacesTheDataBindingIndexWhenPresentAsNeeded()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            IDataSet dataSetWithTwoColumns = Mocks.StrictMock<IDataSet>();
            IDataSet dataSetWithThreeColumns = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSetWithTwoColumns.ColumnCount).Return(2);

                SetupResult.For(dataSetWithThreeColumns.ColumnCount).Return(3);
            }

            using (Mocks.Playback())
            {
                dataSet.AddDataSet(dataSetWithTwoColumns);
                dataSet.AddDataSet(dataSetWithThreeColumns);

                DataBinding bindingWithNoIndex = new DataBinding(null, null);
                DataBinding bindingWithIndex = new DataBinding(1, null);

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
            dataSet.TranslateBinding(null, new DataBinding(0, null));
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
            dataSet.TranslateBinding(Mocks.Stub<IDataSet>(), new DataBinding(0, null));
        }

        [Test]
        public void TranslateBindingSupportsReplaceIndex()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            DataSource source = new DataSource("dummy");
            dataSet.AddDataSet(source);

            DataBinding dataBinding = dataSet.TranslateBinding(source, new DataBinding(null, "path"));
            Assert.IsNull(dataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);

            DataBinding changedDataBinding = dataBinding.ReplaceIndex(5);
            Assert.AreEqual(5, changedDataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);
        }

        [Test]
        public void CanGetDescriptiveDataBindingsFromItem()
        {
            DataSource source = new DataSource("Source");
            source.AddDataSet(new ItemSequenceDataSet(new[] { new DataRow("abc", "def") }, 2));
            source.AddIndexAlias("abc", 0);

            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.AddDataSet(new ItemSequenceDataSet(new[] { new DataRow("xyz") }, 1));
            dataSet.AddDataSet(source);

            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            Assert.AreElementsEqual(new[]
            {
                new DataBinding(0, null),
                new DataBinding(1, "abc"),
                new DataBinding(2, null)
            }, items[0].GetBindingsForInformalDescription());
        }
    }
}