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
using Gallio.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(DataSource))]
    [DependsOn(typeof(MergedDataSetTest))]
    public class DataSourceTest : BaseTestWithMocks
    {
        private delegate bool CanBindDelegate(DataBinding binding);
        private delegate IEnumerable<IDataItem> GetItemsDelegate(ICollection<DataBinding> bindings, bool includeDynamicItems);

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfNameIsNull()
        {
            new DataSource(null);
        }

        [Test]
        public void NameIsSameAsWasSpecifiedInTheConstructor()
        {
            DataSource source = new DataSource("theName");
            Assert.AreEqual("theName", source.Name);
        }

        [Test, ExpectedArgumentNullException]
        public void AddIndexAliasThrowsIfPathIsNull()
        {
            DataSource source = new DataSource("theName");
            source.AddIndexAlias(null, 2);
        }

        [Test]
        public void CanBindAppliesNoTranslationIfNoAliasesAreDefined()
        {
            IDataSet dataSet = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(2);

                Expect.Call(dataSet.CanBind(null)).IgnoreArguments().Do((CanBindDelegate)delegate(DataBinding binding)
                {
                    Assert.AreEqual("untranslatedPath", binding.Path);
                    Assert.AreEqual(5, binding.Index);
                    return false;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddDataSet(dataSet);

                Assert.IsFalse(source.CanBind(new DataBinding(5, "untranslatedPath")));
            }
        }

        [Test]
        public void CanBindAppliesIndexAliasTranslation()
        {
            IDataSet dataSet = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(2);

                Expect.Call(dataSet.CanBind(null)).IgnoreArguments().Do((CanBindDelegate)delegate(DataBinding binding)
                {
                    Assert.AreEqual("translatedPath", binding.Path);
                    Assert.AreEqual(2, binding.Index);
                    return true;
                });

                Expect.Call(dataSet.CanBind(null)).IgnoreArguments().Do((CanBindDelegate)delegate(DataBinding binding)
                {
                    Assert.AreEqual("untranslatedPath", binding.Path);
                    Assert.AreEqual(5, binding.Index);
                    return false;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddIndexAlias("translatedPath", 2);
                source.AddDataSet(dataSet);

                Assert.IsTrue(source.CanBind(new DataBinding(5, "translatedPath")));
                Assert.IsFalse(source.CanBind(new DataBinding(5, "untranslatedPath")));
            }
        }

        [Test]
        public void GetItemsAppliesNoTranslationIfNoAliasesAreDefined()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string, string>("Foo", "Bar"));

            IDataSet dataSet = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(2);

                Expect.Call(dataSet.GetItems(null, true)).IgnoreArguments().Do((GetItemsDelegate)delegate(ICollection<DataBinding> bindings,
                    bool includeDynamicItems)
                {
                    Assert.IsTrue(includeDynamicItems);
                    List<IDataItem> items = new List<IDataItem>();
                    items.Add(new ListDataItem<object>(new object[] { "abc", "def", "ghi" }, metadataPairs, false));

                    List<DataBinding> bindingList = new List<DataBinding>(bindings);

                    Assert.AreEqual("untranslatedPath", bindingList[0].Path);
                    Assert.AreEqual(1, bindingList[0].Index);

                    return items;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddDataSet(dataSet);

                DataBinding[] bindings = new DataBinding[] {
                    new DataBinding(1, "untranslatedPath")
                };

                List<IDataItem> items = new List<IDataItem>(source.GetItems(bindings, true));
                Assert.AreEqual(1, items.Count);

                PropertyBag map = DataItemUtils.GetMetadata(items[0]);
                Assert.AreEqual(1, map.Count);
                Assert.AreEqual("Bar", map.GetValue("Foo"));
                Assert.AreEqual("def", items[0].GetValue(bindings[0]));
            }
        }

        [Test]
        public void GetItemsAppliesIndexAliasTranslation()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string, string>("Foo", "Bar"));
            IDataSet dataSet = Mocks.StrictMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(3);

                Expect.Call(dataSet.GetItems(null, true)).IgnoreArguments().Do((GetItemsDelegate)delegate(ICollection<DataBinding> bindings,
                    bool includeDynamicItems)
                {
                    Assert.IsTrue(includeDynamicItems);

                    List<IDataItem> items = new List<IDataItem>();
                    items.Add(new ListDataItem<object>(new object[] { "abc", "def", "ghi" }, metadataPairs, true));

                    List<DataBinding> bindingList = new List<DataBinding>(bindings);

                    Assert.AreEqual("translatedPath", bindingList[0].Path);
                    Assert.AreEqual(2, bindingList[0].Index);

                    Assert.AreEqual("untranslatedPath", bindingList[1].Path);
                    Assert.AreEqual(1, bindingList[1].Index);

                    return items;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddIndexAlias("translatedPath", 2);
                source.AddDataSet(dataSet);

                DataBinding[] bindings = new DataBinding[] {
                    new DataBinding(5, "translatedPath"),
                    new DataBinding(1, "untranslatedPath")
                };

                List<IDataItem> items = new List<IDataItem>(source.GetItems(bindings, true));
                Assert.AreEqual(1, items.Count);

                PropertyBag map = DataItemUtils.GetMetadata(items[0]);
                Assert.AreEqual(1, map.Count);
                Assert.AreEqual("Bar", map.GetValue("Foo"));

                Assert.Throws<ArgumentNullException>(delegate { items[0].GetValue(null); });
                Assert.AreEqual("ghi", items[0].GetValue(bindings[0]));
                Assert.AreEqual("def", items[0].GetValue(bindings[1]));

                // Should throw ArgumentNullException when binding list is null.
                Assert.Throws<ArgumentNullException>(delegate
                {
                    items[0].GetValue(null);
                });
            }
        }

        [Test]
        public void CanGetDescriptiveDataBindingsFromItem()
        {
            DataSource dataSet = new DataSource("Source");
            dataSet.AddDataSet(new ItemSequenceDataSet(new[] { new DataRow("abc", "def") }, 2));
            dataSet.AddIndexAlias("xxx", 1);

            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(EmptyArray<DataBinding>.Instance, true));

            Assert.AreElementsEqual(new[]
            {
                new DataBinding(0, null),
                new DataBinding(1, "xxx")
            }, items[0].GetBindingsForInformalDescription());
        }
    }
}