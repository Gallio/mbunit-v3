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

extern alias MbUnit2;

using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Data;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(ListDataRow<>))]
    [DependsOn(typeof(BaseDataRowTest))]
    public class ListDataRowTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfListIsNull()
        {
            new ListDataRow<object>(null, EmptyArray<KeyValuePair<string, string>>.Instance);
        }

        [Test]
        public void HasNoMetadataIfNullSpecifiedInConstructor()
        {
            ListDataRow<object> row = new ListDataRow<object>(EmptyArray<object>.Instance, null);
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>(row.GetMetadata());
            Assert.AreEqual(0, metadata.Count);
        }

        [Test]
        public void ContainSameMetadataAsSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            ListDataRow<object> row = new ListDataRow<object>(EmptyArray<object>.Instance, metadata);

            Assert.AreSame(metadata, row.GetMetadata());
        }

        [RowTest]
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
            ListDataRow<object> row = new ListDataRow<object>(values, null);
            object value = row.GetValue(new SimpleDataBinding(typeof(string), path, (int?)index));

            Assert.AreEqual(values[(int)index], value);
        }
    }
}