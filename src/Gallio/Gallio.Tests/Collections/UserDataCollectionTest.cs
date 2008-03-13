// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Collections;
using MbUnit.Framework;

namespace Gallio.Tests.Collections
{
    [TestFixture]
    [TestsOn(typeof(UserDataCollection))]
    public class UserDataCollectionTest
    {
        [Test]
        public void NonExistantValuesCannotBeRetrieved()
        {
            UserDataCollection collection = new UserDataCollection();
            Assert.IsFalse(collection.HasValue("key"));
            Assert.IsNull(collection.GetValue<object>("key"));
            Assert.AreEqual(0, collection.GetValue<int>("key"));
        }

        [Test]
        public void ExistingValuesCanBeRetrieved()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.SetValue("key", 123);
            Assert.IsTrue(collection.HasValue("key"));
            Assert.AreEqual(123, collection.GetValue<int>("key"));
        }

        [Test]
        public void SetValueRemovesValueWhenValueIsNull()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.SetValue("key", 123);
            Assert.IsTrue(collection.HasValue("key"));
            collection.SetValue("key", null);
            Assert.IsFalse(collection.HasValue("key"));
        }

        [Test, ExpectedArgumentNullException]
        public void SetValueThrowsIfKeyIsNull()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.SetValue(null, "abc");
        }

        [Test, ExpectedArgumentNullException]
        public void GetValueThrowsIfKeyIsNull()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.GetValue<object>(null);
        }

        [Test, ExpectedArgumentNullException]
        public void HasValueThrowsIfKeyIsNull()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.HasValue(null);
        }
    }
}
