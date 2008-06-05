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
using Gallio.Framework.Data;
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(StoredDataRow))]
    [DependsOn(typeof(BaseDataRowTest))]
    public class StoredDataRowTest
    {
        [Test]
        [Row(true)]
        [Row(false)]
        public void IsDynamicReturnsSameValueAsWasSpecifiedInTheConstructor(bool isDynamic)
        {
            StoredDataRow row = new StubDataRow(null, isDynamic);
            Assert.AreEqual(isDynamic, row.IsDynamic);
        }

        [Test]
        public void GetMetadataReturnsAnEmptyArrayIfConstructorArgumentWasNull()
        {
            StoredDataRow row = new StubDataRow(null, false);

            MetadataMap map = row.GetMetadata();
            Assert.AreEqual(0, map.Count);
        }

        [Test]
        public void GetMetadataReturnsSameEnumerationAsWasSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadataPairs = new List<KeyValuePair<string, string>>();
            metadataPairs.Add(new KeyValuePair<string,string>("Foo", "Bar"));
            BaseDataRow row = new StubDataRow(metadataPairs, false);

            MetadataMap map = row.GetMetadata();
            Assert.AreEqual(1, map.Count);
            Assert.AreEqual("Bar", map.GetValue("Foo"));
        }

        private class StubDataRow : StoredDataRow
        {
            public StubDataRow(IEnumerable<KeyValuePair<string, string>> metadataPairs,
                bool isDynamic)
                : base(metadataPairs, isDynamic)
            {
            }

            protected override object GetValueImpl(DataBinding binding)
            {
                throw new NotImplementedException();
            }
        }
    }
}
