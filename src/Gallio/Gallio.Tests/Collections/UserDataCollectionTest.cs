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
            Assert.IsFalse(collection.HasValue(new Key<int>("key")));
            int value;
            Assert.IsFalse(collection.TryGetValue(new Key<int>("key"), out value));
            Assert.AreEqual(0, value);
            InterimAssert.Throws<KeyNotFoundException>(delegate { collection.GetValue(new Key<int>("key"));});
            Assert.AreEqual(42, collection.GetValueOrDefault(new Key<int>("key"), 42));
        }

        [Test]
        public void NonExistantValuesCanBeRemovedWithoutSideEffects()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.RemoveValue(new Key<int>("key"));
            Assert.IsFalse(collection.HasValue(new Key<int>("key")));
        }

        [Test]
        public void ExistingValuesCanBeRetrieved()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.SetValue(new Key<int>("key"), 123);
            Assert.IsTrue(collection.HasValue(new Key<int>("key")));
            Assert.AreEqual(123, collection.GetValue(new Key<int>("key")));
            Assert.AreEqual(123, collection.GetValueOrDefault(new Key<int>("key"), 0));
            int value;
            Assert.IsTrue(collection.TryGetValue(new Key<int>("key"), out value));
            Assert.AreEqual(123, value);
        }

        [Test]
        public void ExistingValuesCanBeRemoved()
        {
            UserDataCollection collection = new UserDataCollection();
            collection.SetValue(new Key<int>("key"), 123);
            Assert.IsTrue(collection.HasValue(new Key<int>("key")));
            collection.RemoveValue(new Key<int>("key"));
            Assert.IsFalse(collection.HasValue(new Key<int>("key")));
        }
    }
}
