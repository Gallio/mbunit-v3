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

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(BaseDataRow))]
    public class BaseDataRowTest
    {
        [Test]
        public void GetMetadataReturnsAnEmptyArrayIfConstructorArgumentWasNull()
        {
            BaseDataRow row = new StubDataRow(null);
            Assert.AreEqual(0, new List<KeyValuePair<string, string>>(row.GetMetadata()).Count);
        }

        [Test]
        public void GetMetadataReturnsSameEnumerationAsWasSpecifiedInConstructor()
        {
            List<KeyValuePair<string, string>> metadata = new List<KeyValuePair<string, string>>();
            BaseDataRow row = new StubDataRow(metadata);
            Assert.AreSame(metadata, row.GetMetadata());
        }

        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfBindingIsNull()
        {
            BaseDataRow row = new StubDataRow(EmptyArray<KeyValuePair<string, string>>.Instance);
            row.GetValue(null);
        }

        private class StubDataRow : BaseDataRow
        {
            public StubDataRow(IEnumerable<KeyValuePair<string, string>> metadata)
                : base(metadata)
            {
            }

            protected override object GetValueInternal(DataBinding binding)
            {
                throw new NotImplementedException();
            }
        }
    }
}