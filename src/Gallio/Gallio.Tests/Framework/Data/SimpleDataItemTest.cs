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
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(SimpleDataItem))]
    [DependsOn(typeof(BaseDataItemTest))]
    public class SimpleDataItemTest
    {
        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicReturnsSameValueAsWasSpecifiedInTheConstructor(bool isDynamic)
        {
            SimpleDataItem item = new StubDataItem(null, isDynamic);
            Assert.AreEqual(isDynamic, item.IsDynamic);
        }

        [Test]
        public void GetMetadataReturnsAnEmptyArrayIfConstructorArgumentWasNull()
        {
            SimpleDataItem item = new StubDataItem(null, false);

            MetadataMap map = DataItemUtils.GetMetadata(item);
            Assert.AreEqual(0, map.Count);
        }

        [Test]
        public void GetMetadataReturnsSameEnumerationAsWasSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string,string>("Foo", "Bar"));
            BaseDataItem item = new StubDataItem(metadataPairs, false);

            MetadataMap map = DataItemUtils.GetMetadata(item);
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("Bar", map.GetValue("Foo"));
        }

        private class StubDataItem : SimpleDataItem
        {
            public StubDataItem(IEnumerable<KeyValuePair<string, string>> metadataPairs,
                bool isDynamic)
                : base(metadataPairs, isDynamic)
            {
            }

            protected override object GetValueImpl(DataBinding binding)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<DataBinding> GetBindingsForInformalDescription()
            {
                throw new NotImplementedException();
            }
        }
    }
}
