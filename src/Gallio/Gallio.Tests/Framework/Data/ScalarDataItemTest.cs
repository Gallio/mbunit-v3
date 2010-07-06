// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Collections;
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(ScalarDataItem<>))]
    [DependsOn(typeof(BaseDataItemTest))]
    public class ScalarDataItemTest
    {
        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicReturnsSameValueAsWasSpecifiedInTheConstructor(bool isDynamic)
        {
            ScalarDataItem<object> item = new ScalarDataItem<object>(null, null, isDynamic);
            Assert.AreEqual(isDynamic, item.IsDynamic);
        }

        [Test]
        public void HasNoMetadataIfNullSpecifiedInConstructor()
        {
            ScalarDataItem<object> item = new ScalarDataItem<object>(null, null, false);
            PropertyBag metadata = DataItemUtils.GetMetadata(item);
            Assert.Count(0, metadata);
        }

        [Test]
        public void ContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string, string>("Foo", "Bar"));
            ScalarDataItem<object> item = new ScalarDataItem<object>("abc", metadataPairs, false);

            PropertyBag map = DataItemUtils.GetMetadata(item);
            Assert.Count(1, map);
            Assert.AreEqual("Bar", map.GetValue("Foo"));
        }

        [Test]
        [Row(null, null, ExpectedException=typeof(DataBindingException))]
        [Row(null, 1, ExpectedException=typeof(DataBindingException))]
        [Row(null, -1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", null, ExpectedException=typeof(DataBindingException))]
        [Row("abc", 1, ExpectedException=typeof(DataBindingException))]
        [Row("abc", -1, ExpectedException=typeof(DataBindingException))]
        [Row(null, 0)]
        [Row("abc", 0)]
        public void GetValueReturnsValueOnlyIfTheBindingIndexIsZero(string path, object index)
        {
            ScalarDataItem<object> item = new ScalarDataItem<object>(42, null, false);
            object value = item.GetValue(new DataBinding((int?)index, path));

            Assert.AreEqual(42, value);
        }

        [Test]
        public void CanGetDescriptiveDataBindingsFromItem()
        {
            ScalarDataItem<string> item = new ScalarDataItem<string>("abc", null, false);

            Assert.AreElementsEqual(new[]
            {
                new DataBinding(0, null)
            }, item.GetBindingsForInformalDescription());
        }
    }
}