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
using Gallio.Collections;
using Gallio.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(JoinedDataSet))]
    public class JoinedDataSetTest : BaseUnitTest
    {
        private delegate IEnumerable<IList<IDataRow>> JoinDelegate(IList<IDataProvider> providers, IList<ICollection<DataBinding>> bindingsPerProvider);

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

        [Test]
        public void CanBindReturnsFalseIfThereAreNoDataSets()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(typeof(string))),
                "Cannot bind because there are no data sets.");
        }

        [Test]
        public void CanBindResolvesExternalBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 3, false));
            IDataSet dataSet2 = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 2, false);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(typeof(string))),
                "Cannot bind because there is no path or index.");
            Assert.IsFalse(dataSet.CanBind(new SimpleDataBinding(typeof(string), null, 5)),
                "Cannot bind because index 5 is beyond the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(typeof(string), null, 4)),
                "Can bind because index 4 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(typeof(string), null, 0)),
                "Can bind because index 0 is within the range of columns in the joined data set.");
            Assert.IsTrue(dataSet.CanBind(new SimpleDataBinding(typeof(string), "path", null)),
                "Can bind because path is supported by one of the data sets.");
        }

        [Test]
        public void CanBindResolvesScopedBindings()
        {
            JoinedDataSet dataSet = new JoinedDataSet();

            DataSource dataSet1 = new DataSource("");
            dataSet1.AddIndexAlias("path", 1);
            dataSet1.AddDataSet(new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 3, false));
            IDataSet dataSet2 = new RowSequenceDataSet(EmptyArray<IDataRow>.Instance, 2, false);

            dataSet.AddDataSet(dataSet1);
            dataSet.AddDataSet(dataSet2);

            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(typeof(string)))),
                "Cannot bind because there is no path or index in the translated binding.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(typeof(string), null, 3))),
                "Cannot bind because index 3 is beyond the range of columns in the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(typeof(string), null, 2))),
                "Cannot bind because index 2 is beyond the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(typeof(string), null, 1))),
                "Can bind because index 1 is within the range of columns in the scoped data set.");
            Assert.IsTrue(dataSet.CanBind(dataSet.TranslateBinding(dataSet1, new SimpleDataBinding(typeof(string), "path", null))),
                "Can bind because path is supported by one of the scoped data set.");
            Assert.IsFalse(dataSet.CanBind(dataSet.TranslateBinding(dataSet2, new SimpleDataBinding(typeof(string), "path", null))),
                "Cannot bind because path is supported by one of the scoped data set.");
        }

        [Test, ExpectedArgumentNullException]
        public void CanBindThrowsIfArgumentIsNull()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            dataSet.CanBind(null);
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
                new ListDataRow<int>(new int[] { 1, 2, 3 }, metadata1),
                new ListDataRow<int>(new int[] { -1, -2, -3 }, metadata2)
            }, 3, false));
            dataSet.AddDataSet(dataSet1);

            IDataSet dataSet2 = new RowSequenceDataSet(new IDataRow[]
            {
                new ListDataRow<int>(new int[] { 4, 5 }, metadata2),
                new ListDataRow<int>(new int[] { -4, -5 }, null)
            }, 2, false);
            dataSet.AddDataSet(dataSet2);

            List<IDataRow> dataSet1Rows = new List<IDataRow>(dataSet1.GetRows(EmptyArray<DataBinding>.Instance));
            List<IDataRow> dataSet2Rows = new List<IDataRow>(dataSet2.GetRows(EmptyArray<DataBinding>.Instance));

            List<IList<IDataRow>> results = new List<IList<IDataRow>>();
            results.Add(new IDataRow[] { dataSet1Rows[0], dataSet2Rows[0] });
            results.Add(new IDataRow[] { dataSet1Rows[1], dataSet2Rows[1] });

            IJoinStrategy strategy = Mocks.CreateMock<IJoinStrategy>();
            dataSet.Strategy = strategy;

            DataBinding pathBinding = new SimpleDataBinding(typeof(string), "path", null);
            DataBinding indexZeroBinding = new SimpleDataBinding(typeof(string), null, 0);
            DataBinding indexOneBinding = new SimpleDataBinding(typeof(string), null, 1);
            DataBinding indexThreeBinding = new SimpleDataBinding(typeof(string), null, 3);

            DataBinding[] bindings = new DataBinding[]
            {
                new SimpleDataBinding(typeof(string)), // unresolvable binding because no data sets can claim it
                pathBinding, // claimed by dataSet1
                indexZeroBinding, // claimed by dataSet1
                indexThreeBinding, // claimed by dataSet2
                dataSet.TranslateBinding(dataSet1, pathBinding), // scoped by dataSet1
                dataSet.TranslateBinding(dataSet2, indexOneBinding), // scoped by dataSet2
            };

            using (Mocks.Record())
            {
                Expect.Call(strategy.Join(null, null)).IgnoreArguments().Do((JoinDelegate)delegate(IList<IDataProvider> joinProviders, IList<ICollection<DataBinding>> joinBindingsPerProvider)
                {
                    CollectionAssert.AreElementsEqual(new IDataProvider[] { dataSet1, dataSet2 }, joinProviders);

                    Assert.AreEqual(2, joinBindingsPerProvider.Count);

                    CollectionAssert.AreElementsEqual(new DataBinding[] { pathBinding, indexZeroBinding, pathBinding }, joinBindingsPerProvider[0]);
                    CollectionAssert.AreElementsEqual(new DataBinding[] { indexZeroBinding, indexOneBinding }, joinBindingsPerProvider[1]);

                    return results;
                });
            }

            using (Mocks.Playback())
            {
                List<IDataRow> rows = new List<IDataRow>(dataSet.GetRows(bindings));
                Assert.AreEqual(rows.Count, 2);

                InterimAssert.Throws<ArgumentNullException>(delegate { rows[0].GetValue(null); });

                InterimAssert.Throws<DataBindingException>(delegate { rows[0].GetValue(bindings[0]); });
                Assert.AreEqual(2, rows[0].GetValue(bindings[1]));
                Assert.AreEqual(1, rows[0].GetValue(bindings[2]));
                Assert.AreEqual(4, rows[0].GetValue(bindings[3]));
                Assert.AreEqual(2, rows[0].GetValue(bindings[4]));
                Assert.AreEqual(5, rows[0].GetValue(bindings[5]));

                CollectionAssert.AreElementsEqual(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("abc", "123"),
                    new KeyValuePair<string, string>("def", "456"),
                    new KeyValuePair<string, string>("ghi", "789")
                }, rows[0].GetMetadata());

                InterimAssert.Throws<DataBindingException>(delegate { rows[1].GetValue(bindings[0]); });
                Assert.AreEqual(-2, rows[1].GetValue(bindings[1]));
                Assert.AreEqual(-1, rows[1].GetValue(bindings[2]));
                Assert.AreEqual(-4, rows[1].GetValue(bindings[3]));
                Assert.AreEqual(-2, rows[1].GetValue(bindings[4]));
                Assert.AreEqual(-5, rows[1].GetValue(bindings[5]));

                CollectionAssert.AreElementsEqual(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("ghi", "789")
                }, rows[1].GetMetadata());
            }
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
            Assert.AreEqual(binding.Path, translatedBinding.Path, "Path should be preserved always.");
            Assert.AreEqual(binding.ValueType, translatedBinding.ValueType, "Value type should be preserved always.");
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

        [Test]
        public void TranslateBindingSupportsReplaceIndex()
        {
            JoinedDataSet dataSet = new JoinedDataSet();
            DataSource source = new DataSource("dummy");
            dataSet.AddDataSet(source);

            DataBinding dataBinding = dataSet.TranslateBinding(source, new SimpleDataBinding(typeof(string), "path", null));
            Assert.IsNull(dataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);
            Assert.AreEqual(typeof(string), dataBinding.ValueType);

            DataBinding changedDataBinding = dataBinding.ReplaceIndex(5);
            Assert.AreEqual(5, changedDataBinding.Index);
            Assert.AreEqual("path", dataBinding.Path);
            Assert.AreEqual(typeof(string), dataBinding.ValueType);
        }
    }
}