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
using Gallio.Framework.Data;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(DataSource))]
    [DependsOn(typeof(MergedDataSetTest))]
    public class DataSourceTest : BaseUnitTest
    {
        private delegate bool CanBindDelegate(DataBinding binding);
        private delegate IEnumerable<IDataRow> GetRowsDelegate(ICollection<DataBinding> bindings, bool includeDynamicRows);

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
            IDataSet dataSet = Mocks.CreateMock<IDataSet>();

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

                Assert.IsFalse(source.CanBind(new SimpleDataBinding(5, "untranslatedPath")));
            }
        }

        [Test]
        public void CanBindAppliesIndexAliasTranslation()
        {
            IDataSet dataSet = Mocks.CreateMock<IDataSet>();

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

                Assert.IsTrue(source.CanBind(new SimpleDataBinding(5, "translatedPath")));
                Assert.IsFalse(source.CanBind(new SimpleDataBinding(5, "untranslatedPath")));
            }
        }

        [Test]
        public void GetRowsAppliesNoTranslationIfNoAliasesAreDefined()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            IDataSet dataSet = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(2);

                Expect.Call(dataSet.GetRows(null, true)).IgnoreArguments().Do((GetRowsDelegate)delegate(ICollection<DataBinding> bindings,
                    bool includeDynamicRows)
                {
                    Assert.IsTrue(includeDynamicRows);
                    List<IDataRow> rows = new List<IDataRow>();
                    rows.Add(new ListDataRow<object>(new object[] { "abc", "def", "ghi" }, metadata, false));

                    List<DataBinding> bindingList = new List<DataBinding>(bindings);

                    Assert.AreEqual("untranslatedPath", bindingList[0].Path);
                    Assert.AreEqual(1, bindingList[0].Index);

                    return rows;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddDataSet(dataSet);

                DataBinding[] bindings = new DataBinding[] {
                    new SimpleDataBinding(1, "untranslatedPath")
                };

                List<IDataRow> rows = new List<IDataRow>(source.GetRows(bindings, true));
                Assert.AreEqual(1, rows.Count);

                Assert.AreSame(metadata, rows[0].GetMetadata());
                Assert.AreEqual("def", rows[0].GetValue(bindings[0]));
            }
        }

        [Test]
        public void GetRowsAppliesIndexAliasTranslation()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            IDataSet dataSet = Mocks.CreateMock<IDataSet>();

            using (Mocks.Record())
            {
                SetupResult.For(dataSet.ColumnCount).Return(3);

                Expect.Call(dataSet.GetRows(null, true)).IgnoreArguments().Do((GetRowsDelegate)delegate(ICollection<DataBinding> bindings,
                    bool includeDynamicRows)
                {
                    Assert.IsTrue(includeDynamicRows);

                    List<IDataRow> rows = new List<IDataRow>();
                    rows.Add(new ListDataRow<object>(new object[] { "abc", "def", "ghi" }, metadata, true));

                    List<DataBinding> bindingList = new List<DataBinding>(bindings);

                    Assert.AreEqual("translatedPath", bindingList[0].Path);
                    Assert.AreEqual(2, bindingList[0].Index);

                    Assert.AreEqual("untranslatedPath", bindingList[1].Path);
                    Assert.AreEqual(1, bindingList[1].Index);

                    return rows;
                });
            }

            using (Mocks.Playback())
            {
                DataSource source = new DataSource("theName");
                source.AddIndexAlias("translatedPath", 2);
                source.AddDataSet(dataSet);

                DataBinding[] bindings = new DataBinding[] {
                    new SimpleDataBinding(5, "translatedPath"),
                    new SimpleDataBinding(1, "untranslatedPath")
                };

                List<IDataRow> rows = new List<IDataRow>(source.GetRows(bindings, true));
                Assert.AreEqual(1, rows.Count);

                Assert.AreSame(metadata, rows[0].GetMetadata());

                InterimAssert.Throws<ArgumentNullException>(delegate { rows[0].GetValue(null); });
                Assert.AreEqual("ghi", rows[0].GetValue(bindings[0]));
                Assert.AreEqual("def", rows[0].GetValue(bindings[1]));

                // Should throw ArgumentNullException when binding list is null.
                MbUnit.Framework.InterimAssert.Throws<ArgumentNullException>(delegate
                {
                    rows[0].GetValue(null);
                });
            }
        }
    }
}