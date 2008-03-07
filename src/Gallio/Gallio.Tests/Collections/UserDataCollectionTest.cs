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
