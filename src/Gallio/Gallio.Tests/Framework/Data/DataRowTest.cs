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
    [TestsOn(typeof(DataRow))]
    public class DataRowTest
    {
        [Test]
        public void ShouldBeDynamic()
        {
            DataRow row = new DataRow(null);
            Assert.IsTrue(row.IsDynamic);
        }

        [Test]
        public void ShouldAssumeASingleNullValueWasIntendedIfConstructorArgumentIsNull()
        {
            DataRow row = new DataRow(null);
            Assert.IsNull(row.GetValue(new DataBinding(0, null)));
        }

        [Test]
        public void ShouldAcceptMultipleValues()
        {
            DataRow row = new DataRow("abc", 123, 3.4);
            Assert.AreEqual("abc", row.GetValue(new DataBinding(0, null)));
            Assert.AreEqual(123, row.GetValue(new DataBinding(1, null)));
            Assert.AreEqual(3.4, row.GetValue(new DataBinding(2, null)));
        }

        [Test]
        public void ShouldProvideFluentInterfaceForBuildingMetadata()
        {
            MetadataMap extraMetadata = new MetadataMap();
            extraMetadata.SetValue("Author", "Lewis Carroll");
            extraMetadata.SetValue("Title", "The Jabberwocky");

            DataRow row = new DataRow("abc")
                .WithMetadata("Description", "Frumious")
                .WithMetadata("Name", "Bandersnatch")
                .WithMetadata(extraMetadata);
            Assert.AreEqual("abc", row.GetValue(new DataBinding(0, null)));

            MetadataMap map = DataItemUtils.GetMetadata(row);
            Assert.AreEqual(4, map.Count);
            Assert.AreEqual("Frumious", map.GetValue("Description"));
            Assert.AreEqual("Bandersnatch", map.GetValue("Name"));
            Assert.AreEqual("Lewis Carroll", map.GetValue("Author"));
            Assert.AreEqual("The Jabberwocky", map.GetValue("Title"));
        }

        [Test, ExpectedArgumentNullException]
        public void WithMetadataShouldThrowIfKeyIsNull()
        {
            new DataRow("abc").WithMetadata(null, "123");
        }

        [Test, ExpectedArgumentNullException]
        public void WithMetadataShouldThrowIfValueIsNull()
        {
            new DataRow("abc").WithMetadata("123", null);
        }

        [Test, ExpectedArgumentNullException]
        public void GetValueShouldThrowIfBindingIsNull()
        {
            new DataRow("abc").GetValue(null);
        }
    }
}
