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

using System.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;
using System.Collections.Generic;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(FactoryDataSet))]
    public class FactoryDataSetTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfFactoryIsNull()
        {
            new FactoryDataSet(null, FactoryKind.Auto, 1);
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void ConstructorThrowsIfColumnCountIsNegative()
        {
            new FactoryDataSet(delegate { return null; }, FactoryKind.Auto, -1);
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void ConstructorThrowsIfFactoryKindIsUndefined()
        {
            new FactoryDataSet(delegate { return null; }, (FactoryKind) 100000, 1);
        }

        [Test]
        [Row(FactoryKind.DataSet)]
        [Row(FactoryKind.Auto)]
        [Row(FactoryKind.DataItem, ExpectedException=typeof(DataBindingException))]
        public void WithDataSet(FactoryKind factoryKind)
        {
            CheckFactory(new IDataSet[]
            {
                new ItemSequenceDataSet(new IDataItem[] {
                    new DataRow(1, "a").WithMetadata("Id", "1a"),
                    new DataRow(2, "b").WithMetadata("Id", "2b")
                }, 2),
                new ItemSequenceDataSet(new IDataItem[] {
                    new DataRow(3, "c").WithMetadata("Id", "3c")
                }, 2),
            }, factoryKind, 2, true);
        }

        [Test]
        [Row(FactoryKind.DataItem)]
        [Row(FactoryKind.Auto)]
        [Row(FactoryKind.DataSet, ExpectedException = typeof(DataBindingException))]
        public void WithDataItem(FactoryKind factoryKind)
        {
            CheckFactory(new IDataItem[]
            {
                new DataRow(1, "a").WithMetadata("Id", "1a"),
                new DataRow(2, "b").WithMetadata("Id", "2b"),
                new DataRow(3, "c").WithMetadata("Id", "3c")
            }, factoryKind, 2, true);
        }

        [Test]
        [Row(FactoryKind.ObjectArray)]
        [Row(FactoryKind.Auto)]
        [Row(FactoryKind.DataItem, ExpectedException = typeof(DataBindingException))]
        public void WithObjectArray(FactoryKind factoryKind)
        {
            CheckFactory(new object[][]
            {
                new object[] { 1, "a" },
                new object[] { 2, "b" },
                new object[] { 3, "c" }
            }, factoryKind, 2, false);
        }

        [Test]
        [Row(FactoryKind.Object)]
        [Row(FactoryKind.Auto)]
        [Row(FactoryKind.ObjectArray, ExpectedException = typeof(DataBindingException))]
        public void WithObject(FactoryKind factoryKind)
        {
            CheckFactory(new object[]
            {
                1,
                2,
                3
            }, factoryKind, 1, false);
        }

        private void CheckFactory(IEnumerable source, FactoryKind factoryKind, int columnCount, bool hasMetadata)
        {
            FactoryDataSet dataSet = new FactoryDataSet(delegate { return source; }, factoryKind, columnCount);
            DataBinding[] bindings = new DataBinding[]
            {
                new DataBinding(0, null),
                new DataBinding(1, null)
            };

            List<IDataItem> items = new List<IDataItem>(dataSet.GetItems(bindings, true));
            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(1, items[0].GetValue(bindings[0]));
            Assert.AreEqual(2, items[1].GetValue(bindings[0]));
            Assert.AreEqual(3, items[2].GetValue(bindings[0]));

            if (columnCount >= 2)
            {
                Assert.AreEqual("a", items[0].GetValue(bindings[1]));
                Assert.AreEqual("b", items[1].GetValue(bindings[1]));
                Assert.AreEqual("c", items[2].GetValue(bindings[1]));
            }

            if (hasMetadata)
            {
                Assert.AreEqual("1a", DataItemUtils.GetMetadata(items[0]).GetValue("Id"));
                Assert.AreEqual("2b", DataItemUtils.GetMetadata(items[1]).GetValue("Id"));
                Assert.AreEqual("3c", DataItemUtils.GetMetadata(items[2]).GetValue("Id"));
            }
        }
    }
}
