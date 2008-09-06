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


using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ListDataItem<>))]
    [DependsOn(typeof(BaseDataItemTest))]
    public class ListDataItemTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfListIsNull()
        {
            new ListDataItem<object>(null, EmptyArray<KeyValuePair<string, string>>.Instance, false);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicReturnsSameValueAsWasSpecifiedInTheConstructor(bool isDynamic)
        {
            ListDataItem<object> item = new ListDataItem<object>(EmptyArray<object>.Instance, null, isDynamic);
            Assert.AreEqual(isDynamic, item.IsDynamic);
        }

        [Test]
        public void HasNoMetadataIfNullSpecifiedInConstructor()
        {
            ListDataItem<object> item = new ListDataItem<object>(EmptyArray<object>.Instance, null, false);
            MetadataMap metadata = DataItemUtils.GetMetadata(item);
            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void ContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string, string>("Foo", "Bar"));

            ListDataItem<object> item = new ListDataItem<object>(EmptyArray<object>.Instance, metadataPairs, false);

            MetadataMap map = DataItemUtils.GetMetadata(item);
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("Bar", map.GetValue("Foo"));
        }

        [Test]
        [Row(null, null, ExpectedException=typeof(DataBindingException))]
        [Row(null, 3, ExpectedException=typeof(DataBindingException))]
        [Row(null, -1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", null, ExpectedException=typeof(DataBindingException))]
        [Row("abc", 3, ExpectedException=typeof(DataBindingException))]
        [Row("abc", -1, ExpectedException=typeof(DataBindingException))]
        [Row(null, 0)]
        [Row("abc", 0)]
        [Row(null, 1)]
        [Row(null, 2)]
        public void GetValueReturnsValueOnlyIfTheBindingIndexIsWithinTheListCount(string path, object index)
        {
            object[] values = new object[] { "abc", "def", 42 };
            ListDataItem<object> item = new ListDataItem<object>(values, null, false);
            object value = item.GetValue(new DataBinding((int?)index, path));

            Assert.AreEqual(values[(int)index], value);
        }

        [Test]
        public void CanGetDescriptiveDataBindingsFromItem()
        {
            ListDataItem<string> item = new ListDataItem<string>(new[] { "abc", "def" }, null, false);

            Assert.AreElementsEqual(new[]
            {
                new DataBinding(0, null),
                new DataBinding(1, null),
            }, item.GetBindingsForInformalDescription());
        }
    }
}